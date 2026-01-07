using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using TreniniApp.Extensions;
using TreniniApp.Models;
using TreniniApp.ViewModels;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace TreniniApp.Pages;

public partial class SelectStationPage : BaseContentPage<SelectStationViewModel>
{
    public SelectStationPage(SelectStationViewModel viewModel)
        : base(viewModel, "Select Station")
    {
        On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.PageSheet);

        ToolbarItems.Add(
            new ToolbarItem { Text = "Cancel" }.Bind(
                MenuItem.CommandProperty,
                static (SelectStationViewModel vm) => vm.CancelCommand
            )
        );

        Content = new Grid
        {
            RowDefinitions = Rows.Define(GridLength.Auto, GridLength.Auto, GridLength.Star),
            ColumnDefinitions = Columns.Define(GridLength.Star, GridLength.Auto),
            Children =
            {
                new Label
                {
                    Text = "Select a station",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = new Thickness(10)
                }.Row(0),
                new Button
                {
                    Text = "Close",
                    HorizontalOptions = LayoutOptions.End,
                    Margin = new Thickness(5, 10)
                }
                    .Bind(
                        Button.CommandProperty,
                        static (SelectStationViewModel vm) => vm.CancelCommand
                    )
                    .WithHapticFeedback()
                    .Column(1)
                    .Row(0),
                // SearchBar at the top
                new Microsoft.Maui.Controls.SearchBar { Placeholder = "Search station..." }
                    .Bind(
                        Microsoft.Maui.Controls.SearchBar.TextProperty,
                        static (SelectStationViewModel vm) => vm.SearchText,
                        mode: BindingMode.TwoWay
                    )
                    .Invoke(sb =>
                        sb.TextChanged += async (s, e) =>
                        {
                            if (BindingContext is SelectStationViewModel vm)
                            {
                                await vm.OnSearchChangedAsync(e.NewTextValue);
                            }
                        }
                    )
                    .Row(1)
                    .ColumnSpan(2),
                // CollectionView with infinite scrolling
                new CollectionView
                {
                    SelectionMode = SelectionMode.Single,
                    ItemTemplate = new DataTemplate(() =>
                    {
                        return new StackLayout
                        {
                            Padding = new Thickness(10),
                            Children =
                            {
                                new Label().Bind(
                                    Label.TextProperty,
                                    static (Station s) => s.Name,
                                    convert: static value =>
                                    {
                                        return !string.IsNullOrWhiteSpace(value)
                                            ? System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                                                value.ToLower()
                                            )
                                            : string.Empty;
                                    }
                                )
                            }
                        }.WithHapticFeedback();
                    }),
                    RemainingItemsThreshold = 10
                }
                    .Bind(
                        ItemsView.ItemsSourceProperty,
                        static (SelectStationViewModel vm) => vm.FilteredStations
                    )
                    .Bind(
                        SelectableItemsView.SelectedItemProperty,
                        static (SelectStationViewModel vm) => vm.SelectedStation,
                        mode: BindingMode.TwoWay
                    )
                    .Invoke(cv =>
                    {
                        cv.SelectionChanged += (s, e) =>
                        {
                            if (
                                e.CurrentSelection?.FirstOrDefault() is Station station
                                && BindingContext is SelectStationViewModel vm
                            )
                            {
                                vm.OnStationSelected(station);
                            }
                        };
                        cv.RemainingItemsThresholdReached += async (s, e) =>
                        {
                            if (BindingContext is SelectStationViewModel vm)
                            {
                                await vm.LoadMoreStationsAsync();
                            }
                        };
                    })
                    .Row(2)
                    .ColumnSpan(2)
            }
        };
    }
}
