using System.Collections;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TreniniApp.Constants;
using TreniniApp.Models;
using TreniniApp.Pages;
using TreniniApp.Services;
using TreniniApp.Utils;

namespace TreniniApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IWebScrapingService _webScrapingService;
    private readonly IStationService _stationService;
    private readonly IDispatcher _dispatcher;
    private readonly INavigationService _navigationService;

    public ObservableCollection<TrainRow> TrainRows { get; } = [];

    [ObservableProperty]
    private string? _searchText;

    private readonly string _selectedStation = Preferences.Get(
        StationConstant.SelectedStationKey,
        StationConstant.DefaultStationId
    );

    [ObservableProperty]
    private string? _stationName;

    public MainViewModel(
        IDispatcher dispatcher,
        IWebScrapingService webScrapingService,
        IStationService stationService,
        INavigationService navigationService
    )
    {
        _webScrapingService = webScrapingService;
        _stationService = stationService;
        _dispatcher = dispatcher;
        _navigationService = navigationService;

        BindingBase.EnableCollectionSynchronization(TrainRows, null, ObservableCollectionCallback);
    }

    public async Task LoadTrainDataAsync(string placeId)
    {
        var trainRows = (await _webScrapingService.GetTrainsAsync(placeId)).OrderBy(static x =>
            x.Time
        );

        foreach (var trainRow in trainRows)
        {
            TrainRows.Add(trainRow);
        }
    }

    readonly WeakEventManager _pullToRefreshEventManager = new();

    [ObservableProperty]
    bool _isListRefreshing;

    public event EventHandler<string> PullToRefreshFailed
    {
        add => _pullToRefreshEventManager.AddEventHandler(value);
        remove => _pullToRefreshEventManager.RemoveEventHandler(value);
    }

    [RelayCommand]
    async Task SelectStation()
    {
        // var selectStationPage = Application
        //     .Current?.Windows[0]?
        //     .Page?.Handler?.MauiContext?.Services.GetService<SelectStationPage>();

        // if (selectStationPage is null)
        //     return;

        // var station = await .Current(selectStationPage);

        // if (station is null)
        //     return;

        // Preferences.Set(SelectedStationKey, station.Code);
        // _selectedStation = station.Code;

        // var selectStationPage = await DipendencyInjectionUtil.ResolveAsync<SelectStationPage>();
        await _navigationService.PushModalAsync<SelectStationPage>();
    }

    [RelayCommand]
    async Task Refresh()
    {
        TrainRows.Clear();

        try
        {
            IsListRefreshing = true;
            await LoadTrainDataAsync(_selectedStation);
        }
        catch (HttpRequestException e)
        {
            OnPullToRefreshFailed(e.ToString());
        }
        catch (Exception e)
        {
            OnPullToRefreshFailed(e.ToString());
        }
        finally
        {
            IsListRefreshing = false;
        }
    }

    //Ensure Observable Collection is thread-safe https://codetraveler.io/2019/09/11/using-observablecollection-in-a-multi-threaded-xamarin-forms-application/
    void ObservableCollectionCallback(
        IEnumerable collection,
        object context,
        Action accessMethod,
        bool writeAccess
    )
    {
        _dispatcher.Dispatch(accessMethod);
    }

    void OnPullToRefreshFailed(string message) =>
        _pullToRefreshEventManager.HandleEvent(this, message, nameof(PullToRefreshFailed));

    static void InsertIntoSortedCollection<T>(
        ObservableCollection<T> collection,
        Comparison<T> comparison,
        T modelToInsert
    )
    {
        if (collection.Count is 0)
        {
            collection.Add(modelToInsert);
        }
        else
        {
            int index = 0;
            foreach (var model in collection)
            {
                if (comparison(model, modelToInsert) >= 0)
                {
                    collection.Insert(index, modelToInsert);
                    return;
                }

                index++;
            }

            collection.Insert(index, modelToInsert);
        }
    }

    public override async Task OnAppearingAsync()
    {
        var stationName =
            await _stationService.GetStationNameByIdAsync($"{_selectedStation}") ?? "Uknown";

        StationName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
            stationName.ToLower()
        );
    }
}
