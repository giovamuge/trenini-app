using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TreniniApp.Constants;
using TreniniApp.Models;
using TreniniApp.Services;
using TreniniApp.Utils;

namespace TreniniApp.ViewModels
{
    public partial class SelectStationViewModel(
        IDispatcher dispatcher,
        IStationService stationService,
        INavigationService navigationService
    ) : BaseViewModel
    {
        private readonly IStationService _stationService = stationService;
        private readonly INavigationService _navigationService = navigationService;
        private readonly IDispatcher _dispatcher = dispatcher;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SelectStationCommand))]
        private Station? selectedStation;

        public ObservableCollection<Station> FilteredStations { get; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSearching))]
        private string? searchText;

        // ✅ Proprietà calcolata che si aggiorna automaticamente
        public bool IsSearching => !string.IsNullOrWhiteSpace(SearchText);

        private List<Station> _allStations = [];
        private int _pageSize = 50;
        private int _currentPage = 0;
        private bool _isLoading = false;
        private bool _allLoaded = false;
        private CancellationTokenSource? _searchCts;

        // ✅ Chiamato automaticamente quando SearchText cambia
        partial void OnSearchTextChanged(string? value)
        {
            // Cancella la ricerca precedente
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();

            // Debouncing: aspetta 300ms prima di cercare
            Task.Run(
                async () =>
                {
                    try
                    {
                        await Task.Delay(300, _searchCts.Token);
                        await LoadStationsAsync();
                    }
                    catch (TaskCanceledException)
                    {
                        // Ricerca cancellata, ignora
                    }
                },
                _searchCts.Token
            );
        }

        [RelayCommand]
        public async Task LoadStationsAsync()
        {
            if (_isLoading)
            {
                return;
            }

            _isLoading = true;

            try
            {
                FilteredStations.Clear();
                _currentPage = 0;
                _allLoaded = false;

                if (_allStations.Count == 0)
                {
                    var stations = await _stationService.GetAllStationsAsync();
                    _allStations = [.. stations];
                }

                var filtered = string.IsNullOrWhiteSpace(SearchText)
                    ? _allStations
                    :
                    [
                        .. _allStations.Where(s =>
                            s.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                        )
                    ];

                var nextItems = filtered.Skip(_currentPage * _pageSize).Take(_pageSize).ToList();

                foreach (var station in nextItems)
                {
                    if (FilteredStations.Contains(station))
                        continue;

                    FilteredStations.Add(station);
                }

                _currentPage++;
                _allLoaded = nextItems.Count < _pageSize;

                SelectedStation = FilteredStations.FirstOrDefault(static x =>
                    x.Value
                    == Preferences.Get(
                        StationConstant.SelectedStationKey,
                        StationConstant.DefaultStationId
                    )
                );
            }
            catch (Exception ex)
            {
                await _dispatcher.DispatchAsync(async () =>
                {
                    var page = App.Current?.Windows?[0]?.Page;
                    if (page != null)
                    {
                        await page.DisplayAlertAsync(
                            "Error",
                            $"Failed to load stations: {ex.Message}",
                            "OK"
                        );
                    }
                });
            }
            finally
            {
                _isLoading = false;
            }
        }

        [RelayCommand]
        public async Task LoadMoreStationsAsync()
        {
            if (_isLoading || _allLoaded)
            {
                return;
            }

            _isLoading = true;

            try
            {
                var filtered = string.IsNullOrWhiteSpace(SearchText)
                    ? _allStations
                    :
                    [
                        .. _allStations.Where(s =>
                            s.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                        )
                    ];

                var nextItems = filtered.Skip(_currentPage * _pageSize).Take(_pageSize).ToList();

                foreach (var station in nextItems)
                {
                    if (FilteredStations.Contains(station))
                        continue;

                    FilteredStations.Add(station);
                }

                _currentPage++;
                _allLoaded = nextItems.Count < _pageSize;
            }
            catch (Exception ex)
            {
                await _dispatcher.DispatchAsync(async () =>
                {
                    var page = App.Current?.Windows?[0]?.Page;
                    if (page != null)
                    {
                        await page.DisplayAlertAsync(
                            "Error",
                            $"Failed to load more stations: {ex.Message}",
                            "OK"
                        );
                    }
                });
            }
            finally
            {
                _isLoading = false;
            }
        }

        private bool CanSelectStation() => SelectedStation != null;

        [RelayCommand(CanExecute = nameof(CanSelectStation))]
        public void SelectStation(Station station)
        {
            if (station == null)
                return;

            SelectedStation = station;

            Preferences.Set(
                StationConstant.SelectedStationKey,
                SelectedStation?.Value ?? StationConstant.DefaultStationId
            );

            _navigationService.PopAsync();
        }

        [RelayCommand]
        public void Cancel()
        {
            _navigationService.PopAsync();
        }

        public override async Task OnAppearingAsync()
        {
            await LoadStationsAsync();
        }
    }
}
