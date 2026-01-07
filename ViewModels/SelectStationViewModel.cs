using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TreniniApp.Constants;
using TreniniApp.Models;
using TreniniApp.Services;

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
        private Station? selectedStation;

        [ObservableProperty]
        private string? searchText;

        public ObservableCollection<Station> FilteredStations { get; } = [];

        public void OnStationSelected(Station station)
        {
            if (station == null)
            {
                return;
            }

            SelectedStation = station;

            // Save the selected station as default
            Preferences.Set(StationConstant.SelectedStationKey, station.Value);

            // Close the modal
            _ = _navigationService.PopModalAsync();
        }

        public Task OnSearchChangedAsync(string? newTextValue)
        {
            SearchText = newTextValue;
            return Task.CompletedTask;
        }

        private List<Station> _allStations = [];
        private int _pageSize = 50;
        private int _currentPage = 0;
        private bool _isLoading = false;
        private bool _allLoaded = false;
        private CancellationTokenSource? _searchCts;

        partial void OnSearchTextChanged(string? value)
        {
            // Cancel previous search if any
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            // Debouncing: wait 300ms before searching
            _ = PerformDebouncedSearchAsync(token);
        }

        private async Task PerformDebouncedSearchAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(300, token);

                // Run on the main thread to update UI-bound collections
                await _dispatcher.DispatchAsync(FilterStationsAsync);
            }
            catch (TaskCanceledException)
            {
                // Search canceled, ignore
            }
        }

        private async Task FilterStationsAsync()
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

                var searchText = SearchText;
                var filtered = string.IsNullOrWhiteSpace(searchText)
                    ? _allStations
                    :
                    [
                        .. _allStations.Where(s =>
                            s.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                        )
                    ];

                var nextItems = filtered.Skip(_currentPage * _pageSize).Take(_pageSize).ToList();

                foreach (var station in nextItems)
                {
                    FilteredStations.Add(station);
                }

                _currentPage++;
                _allLoaded = nextItems.Count < _pageSize;

                // Seleziona la stazione salvata
                SelectedStation = FilteredStations.FirstOrDefault(static x =>
                    x.Value
                    == Preferences.Get(
                        StationConstant.SelectedStationKey,
                        StationConstant.DefaultStationId
                    )
                );
            }
            finally
            {
                _isLoading = false;
            }
        }

        [RelayCommand]
        public async Task LoadStationsAsync()
        {
            try
            {
                await FilterStationsAsync();
            }
            catch (Exception ex)
            {
                await _dispatcher.DispatchAsync(async () =>
                {
                    var page = Application.Current?.Windows?[0]?.Page;
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
                    {
                        continue;
                    }

                    FilteredStations.Add(station);
                }

                _currentPage++;
                _allLoaded = nextItems.Count < _pageSize;
            }
            catch (Exception ex)
            {
                await _dispatcher.DispatchAsync(async () =>
                {
                    var page = Application.Current?.Windows?[0]?.Page;
                    if (page == null)
                    {
                        return;
                    }

                    await page.DisplayAlertAsync(
                        "Error",
                        $"Failed to load more stations: {ex.Message}",
                        "OK"
                    );
                });
            }
            finally
            {
                _isLoading = false;
            }
        }

        [RelayCommand]
        public Task Cancel() => _navigationService.PopModalAsync();

        public override Task OnAppearingAsync() => LoadStationsAsync();
    }
}
