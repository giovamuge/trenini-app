using System.Collections;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using TreniniApp.Pages;
using TreniniApp.ViewModels;
using TreniniApp.Views;

namespace TreniniApp.Pages;

public partial class MainPage : BaseContentPage<MainViewModel>
{
    private readonly IDispatcher _dispatcher;

    public MainPage(MainViewModel viewModel, IDispatcher dispatcher)
        : base(viewModel, "Train Schedule")
    {
        _dispatcher = dispatcher;

        BindingContext.PullToRefreshFailed += HandlePullToRefreshFailed;
        BackgroundColor = Colors.WhiteSmoke;
        Padding = new Thickness(10, 5);

        ToolbarItems.Add(
            new ToolbarItem { Order = ToolbarItemOrder.Primary, Priority = 0 }
                .Bind(
                    ToolbarItem.CommandProperty,
                    static (MainViewModel vm) => vm.SelectStationCommand
                )
                .Bind(ToolbarItem.TextProperty, static (MainViewModel vm) => vm.StationName)
        );

        Content = new RefreshView
        {
            Content = new CollectionView
            {
                Shadow = new Shadow
                {
                    Brush = Brush.Black,
                    Opacity = 0.08f,
                    Offset = new Point(0, 2),
                    Radius = 8
                },
                BackgroundColor = Colors.Transparent,
                Margin = new Thickness(0),
                ItemTemplate = new TrainDataTemplate(),
                // Apple style: no separators, no uneven rows, no background
                SelectionMode = SelectionMode.None
            }.Bind(CollectionView.ItemsSourceProperty, static (MainViewModel m) => m.TrainRows)
        }
            .Bind(
                RefreshView.IsRefreshingProperty,
                getter: static (MainViewModel vm) => vm.IsListRefreshing,
                setter: static (vm, isRefreshing) => vm.IsListRefreshing = isRefreshing
            )
            .Bind(
                RefreshView.CommandProperty,
                getter: static (MainViewModel vm) => vm.RefreshCommand,
                mode: BindingMode.OneTime
            );
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (
            HasCollectionViewContent(Content, out var refreshView, out var collectionView)
            && IsNullOrEmpty(collectionView?.ItemsSource)
            && refreshView is not null
        )
        {
            refreshView.IsRefreshing = true;
        }

        static bool HasCollectionViewContent(
            in View content,
            out RefreshView? refreshView,
            out CollectionView? collectionView
        )
        {
            refreshView = null;
            collectionView = null;

            if (
                content is RefreshView refresh
                // && refresh.Content is Border borderView
                && refresh.Content is CollectionView view
            )
            {
                refreshView = refresh;
                collectionView = view;
                return true;
            }

            return false;
        }

        static bool IsNullOrEmpty(in IEnumerable? enumerable)
        {
            if (enumerable is null)
                return false;

            var enumerator =
                enumerable.GetEnumerator()
                ?? throw new InvalidOperationException("Enumerator not found");
            using var disposable = (IDisposable)enumerator;
            return !enumerator.MoveNext();
        }
    }

    void HandlePullToRefreshFailed(object? sender, string message) =>
        _dispatcher.DispatchAsync(() => DisplayAlert("Refresh Failed", message, "OK"));
}
