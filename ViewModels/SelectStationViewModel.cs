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
    public partial class SelectStationViewModel : BaseViewModel
    {
        private readonly IStationService _stationService;
        private readonly INavigationService _navigationService;
        private readonly IDispatcher _dispatcher;

        public SelectStationViewModel(
            IDispatcher dispatcher,
            IStationService stationService,
            INavigationService navigationService
        )
        {
            _stationService = stationService;
            _navigationService = navigationService;
            _dispatcher = dispatcher;
        }

        [ObservableProperty]
        private Station? selectedStation;

        public ObservableCollection<Station> Stations { get; } = [];

        [RelayCommand]
        public async Task LoadStationsAsync()
        {
            Stations.Clear();
            var stations = await _stationService.GetAllStationsAsync();
            foreach (var station in stations)
            {
                Stations.Add(station);
            }

            SelectedStation = Stations.FirstOrDefault(static x =>
                x.Value
                == Preferences.Get(
                    StationConstant.SelectedStationKey,
                    StationConstant.DefaultStationId
                )
            );
        }

        private bool CanSelectStation() => SelectedStation != null;

        [RelayCommand(CanExecute = nameof(CanSelectStation))]
        public void SelectStation(Station station)
        {
            if (station == null)
                return;

            SelectedStation = station;

            // Handle station selection logic here
            Preferences.Set(
                StationConstant.SelectedStationKey,
                SelectedStation?.Value ?? StationConstant.DefaultStationId
            );

            // Assuming you have a method to navigate back or close the modal
            _navigationService.PopAsync();
        }

        [RelayCommand]
        public void Cancel()
        {
            // Handle cancel logic here
            _navigationService.PopAsync();
        }

        public override async Task OnAppearingAsync()
        {
            await LoadStationsAsync();
        }
    }
}
