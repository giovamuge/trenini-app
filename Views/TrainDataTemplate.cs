using CommunityToolkit.Maui.Markup;
using TreniniApp.Extensions;
using TreniniApp.Models;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace TreniniApp.Views;

public class TrainDataTemplate : DataTemplate
{
    public TrainDataTemplate()
        : base(CreateGrid) { }

    static Grid CreateGrid()
    {
        return new Grid
        {
            RowSpacing = 1,
            Padding = new Thickness(0, 10),
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            RowDefinitions = Rows.Define((Row.Up, 1), (Row.Down, 1)),
            ColumnDefinitions = Columns.Define(
                (Col.Time, 1),
                (Col.Information, GridLength.Star),
                (Col.Track, 1)
            ),

            Children =
            {
                new Label()
                    .Row(Row.Up)
                    .Column(Col.Time)
                    .Font(size: 18)
                    .TextColor(Colors.Black)
                    .Paddings(10, 0, 10, 0)
                    .Bind(
                        Label.TextProperty,
                        static (TrainRow m) => m.Time,
                        mode: BindingMode.OneTime
                    ),
                new Label()
                    .Row(Row.Down)
                    .Column(Col.Time)
                    .Font(size: 18)
                    .TextColor(Colors.Black)
                    .Paddings(10, 0, 10, 0)
                    .Bind(
                        Label.TextProperty,
                        static (TrainRow m) => m.Delay,
                        mode: BindingMode.OneTime
                    ),
                new Label()
                    .Row(Row.Up)
                    .Column(Col.Information)
                    .Font(size: 16)
                    .TextColor(Colors.Black)
                    .Set(Label.LineBreakModeProperty, LineBreakMode.TailTruncation)
                    .Bind(
                        Label.TextProperty,
                        static (TrainRow m) => m.Destination,
                        mode: BindingMode.OneTime
                    ),
                new Label()
                    .Row(Row.Down)
                    .Column(Col.Information)
                    .Font(size: 16)
                    .TextColor(Colors.Black)
                    .Bind(
                        Label.TextProperty,
                        static (TrainRow m) => m.Train,
                        mode: BindingMode.OneTime
                    ),
                new Label()
                    .Row(Row.Down)
                    .Column(Col.Track)
                    .Font(size: 16)
                    .TextColor(Colors.Gray)
                    .Bind(
                        Label.TextProperty,
                        static (TrainRow m) => m.Track,
                        mode: BindingMode.OneTime
                    )
            }
        };
    }

    enum Row
    {
        Up,
        Down
    }

    enum Col
    {
        Time,
        Information,
        Track
    }
}
