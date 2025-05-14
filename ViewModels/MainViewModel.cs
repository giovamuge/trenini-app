using System.Collections;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TreniniApp.Models;
using TreniniApp.Services;

namespace TreniniApp.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly IWebScrapingService _webScrapingService;
    private readonly IDispatcher _dispatcher;

    public ObservableCollection<TrainRow> TrainRows { get; } = [];

    [ObservableProperty]
    private string? _searchText;

    public MainViewModel(IDispatcher dispatcher, IWebScrapingService webScrapingService)
    {
        _webScrapingService = webScrapingService;
        _dispatcher = dispatcher;
        BindingBase.EnableCollectionSynchronization(TrainRows, null, ObservableCollectionCallback);
    }

    public async Task LoadTrainDataAsync(int placeId)
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
    async Task Refresh()
    {
        TrainRows.Clear();

        try
        {
            IsListRefreshing = true;
            await LoadTrainDataAsync(2416);
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
}
