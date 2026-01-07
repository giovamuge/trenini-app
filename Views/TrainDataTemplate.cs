using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using TreniniApp.Extensions;
using TreniniApp.Models;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace TreniniApp.Views;

public class TrainDataTemplate : DataTemplate
{
    public TrainDataTemplate()
        : base(CreateGrid) { }

    static View CreateGrid() =>
        new Border
        {
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            Content = new Grid
            {
                RowSpacing = 3,
                ColumnSpacing = 5,
                Padding = new Thickness(16, 11), // padding iOS standard

                RowDefinitions = Rows.Define(
                    (Row.Up, GridLength.Auto),
                    (Row.Down, GridLength.Auto),
                    (Row.Border, GridLength.Auto)
                ),
                ColumnDefinitions = Columns.Define(
                    (Col.Time, GridLength.Auto),
                    (Col.Information, GridLength.Star),
                    (Col.Track, GridLength.Auto)
                ),

                BackgroundColor = Colors.Transparent,

                Children =
                {
                    new Label()
                        .Row(Row.Up)
                        .Column(Col.Time)
                        .TextColor(Colors.Black)
                        .Font(size: 17, bold: true) // Apple title font
                        .Bind(
                            Label.TextProperty,
                            static (TrainRow m) => m.Time,
                            mode: BindingMode.OneTime
                        ),
                    new Label()
                        .Row(Row.Down)
                        .Column(Col.Time)
                        .TextColor(Color.FromArgb("#FF6B6B")) // rosso leggero
                        .Set(Label.HorizontalTextAlignmentProperty, TextAlignment.End)
                        .Font(size: 13)
                        .Bind(
                            Label.TextProperty,
                            static (TrainRow m) => m.Delay,
                            mode: BindingMode.OneTime
                        ),
                    new Label()
                        .Row(Row.Up)
                        .Column(Col.Information)
                        .TextColor(Colors.Black)
                        .Font(size: 17, bold: true)
                        .Set(Label.LineBreakModeProperty, LineBreakMode.TailTruncation)
                        .Bind(
                            Label.TextProperty,
                            static (TrainRow m) => m.Destination,
                            mode: BindingMode.OneTime,
                            convert: static value =>
                            {
                                return !string.IsNullOrWhiteSpace(value)
                                    ? System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                                        value.ToLower()
                                    )
                                    : string.Empty;
                            }
                        ),
                    new Label()
                        .Row(Row.Down)
                        .Column(Col.Information)
                        .TextColor(Colors.Gray)
                        .Font(size: 13)
                        .Bind(
                            Label.TextProperty,
                            static (TrainRow m) => m.Train,
                            mode: BindingMode.OneTime
                        ),
                    new Label()
                        .Row(Row.Down)
                        .Column(Col.Track)
                        .TextColor(Colors.Gray)
                        .Font(size: 13)
                        .Bind(
                            Label.TextProperty,
                            static (TrainRow m) => m.Track,
                            mode: BindingMode.OneTime
                        ),
                    // Apple style: subtitle separator at the bottom, solo se non è l'ultimo elemento
                    // new BoxView
                    // {
                    //     HeightRequest = 0.5,
                    //     BackgroundColor = Color.FromArgb("#C6C6C8"),
                    //     Margin = new Thickness(16, 0, 0, 0), // inset a sinistra iOS
                    //     HorizontalOptions = LayoutOptions.Fill,
                    //     VerticalOptions = LayoutOptions.End,
                    // }
                    //     .Row(Row.Border)
                    //     .ColumnSpan(3)
                    //     .Bind(
                    //         BoxView.IsVisibleProperty,
                    //         static (TrainRow m) => m.Position,
                    //         mode: BindingMode.OneTime,
                    //         convert: static (object? pos) => pos is Position p && p != Position.Last
                    //     )
                }
            }
        };

    // }.Bind(
    //     Border.StrokeShapeProperty,
    //     static (TrainRow m) => m.Position,
    //     mode: BindingMode.OneTime,
    //     convert: static (object? pos) =>
    //     {
    //         if (pos is not Position p)
    //             return new RoundRectangle { CornerRadius = new CornerRadius(0) };
    //         return p == Position.First && p == Position.Last
    //             ? new RoundRectangle { CornerRadius = new CornerRadius(10) } // iOS usa 10pt
    //             : p == Position.First
    //                 ? new RoundRectangle { CornerRadius = new CornerRadius(10, 10, 0, 0) }
    //                 : p == Position.Last
    //                     ? new RoundRectangle { CornerRadius = new CornerRadius(0, 0, 10, 10) }
    //                     : new RoundRectangle { CornerRadius = new CornerRadius(0) };
    //     }
    // );

    enum Row
    {
        Up,
        Down,
        Border
    }

    enum Col
    {
        Time,
        Information,
        Track
    }
}
