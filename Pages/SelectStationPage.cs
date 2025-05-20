using CommunityToolkit.Maui.Markup;
using TreniniApp.Models;
using TreniniApp.ViewModels;

namespace TreniniApp.Pages;

public partial class SelectStationPage : BaseContentPage<SelectStationViewModel>
{
    public SelectStationPage(SelectStationViewModel viewModel)
        : base(viewModel, "Stations")
    {
        Content = new VerticalStackLayout
        {
            Children =
            {
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
                                new Label().Bind(Label.TextProperty, static (Station s) => s.Name)
                            }
                        };
                    })
                }
                    .Bind(
                        CollectionView.ItemsSourceProperty,
                        static (SelectStationViewModel vm) => vm.Stations
                    )
                    .Bind(
                        CollectionView.SelectedItemProperty,
                        static (SelectStationViewModel vm) => vm.SelectedStation,
                        mode: BindingMode.TwoWay
                    )
            }
        };
    }
}
