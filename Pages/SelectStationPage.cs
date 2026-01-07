using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
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
            RowDefinitions = Rows.Define(GridLength.Auto, GridLength.Star),
            Children =
            {
                // SearchBar in alto
                new Microsoft.Maui.Controls.SearchBar { Placeholder = "Search station..." }
                    .Bind(
                        Microsoft.Maui.Controls.SearchBar.TextProperty,
                        static (SelectStationViewModel vm) => vm.SearchText,
                        mode: BindingMode.TwoWay
                    )
                    .Row(0),
                // CollectionView con caricamento infinito
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
                        };
                    }),
                    RemainingItemsThreshold = 10
                }
                    .Bind(
                        CollectionView.ItemsSourceProperty,
                        static (SelectStationViewModel vm) => vm.FilteredStations
                    )
                    .Bind(
                        CollectionView.SelectedItemProperty,
                        static (SelectStationViewModel vm) => vm.SelectedStation,
                        mode: BindingMode.TwoWay
                    )
                    .Invoke(cv =>
                        cv.RemainingItemsThresholdReached += async (s, e) =>
                        {
                            if (BindingContext is SelectStationViewModel vm)
                                await vm.LoadStationsAsync();
                        }
                    )
                    .Row(1)
            }
        };
    }
}
