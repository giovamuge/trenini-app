using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TreniniApp.Models;
using TreniniApp.Services;

namespace TreniniApp.ViewModels
{
    public partial class SelectStationViewModel(IStationService stationService) : BaseViewModel
    {
        private readonly IStationService _stationService = stationService;

        [ObservableProperty]
        private Station? selectedStation;

        [ObservableProperty]
        private bool isModalVisible;

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
            IsModalVisible = true;
        }

        [RelayCommand(CanExecute = nameof(CanSelectStation))]
        public void SelectStation()
        {
            // Handle station selection logic here
            IsModalVisible = false;
        }

        private bool CanSelectStation() => SelectedStation != null;

        [RelayCommand]
        public void CloseModal()
        {
            IsModalVisible = false;
        }
    }
}
