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
        Padding = new Thickness(0, 10);

        // Content =
        // new VerticalStackLayout
        // {
        //     Padding = 10,
        //     Children =
        // {
        // new SearchBar { Placeholder = "Search trains..." }.Bind(
        //     SearchBar.TextProperty,
        //     static (MainViewModel m) => m.SearchText,
        //     mode: BindingMode.TwoWay
        // ),
        // .Invoke(searchBar =>
        //     searchBar.TextChanged += (s, e) =>
        //     {
        //         ((MainViewModel)BindingContext).FilterTrains();
        //     }
        // ),

        Content = new RefreshView
        {
            Content = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(16) },
                Background = Colors.White,
                StrokeThickness = 0,
                Margin = new Thickness(16, 0, 16, 0),
                Content = new CollectionView
                {
                    // SeparatorColor = Color.FromArgb("#E5E5EA"),
                    BackgroundColor = Colors.Transparent,
                    // HasUnevenRows = true,
                    ItemTemplate = new TrainDataTemplate()
                }.Bind(CollectionView.ItemsSourceProperty, static (MainViewModel m) => m.TrainRows)
            }
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
                && refresh.Content is Border borderView
                && borderView.Content is CollectionView view
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
