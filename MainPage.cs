using CommunityToolkit.Maui.Markup;

namespace TreniniApp;

public partial class MainPage : ContentPage
{
    private ListView trainScheduleList;
    private IEnumerable<object> trainSchedules;

    public MainPage()
    {
        Title = "Train Schedule";
        Shell.SetNavBarIsVisible(this, true);

        trainSchedules =
        [
            new
            {
                Train = "Train 101",
                Time = "08:30 AM",
                Destination = "Rome"
            },
            new
            {
                Train = "Train 202",
                Time = "09:15 AM",
                Destination = "Milan"
            },
            new
            {
                Train = "Train 303",
                Time = "10:00 AM",
                Destination = "Venice"
            },
            new
            {
                Train = "Train 404",
                Time = "11:45 AM",
                Destination = "Naples"
            },
            new
            {
                Train = "Train 505",
                Time = "01:30 PM",
                Destination = "Turin"
            }
        ];

        var searchBar = new SearchBar
        {
            Placeholder = "Search trains...",
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = 10
        };

        searchBar.TextChanged += OnSearchTextChanged;

        var view = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            },
            Padding = 10
        };

        view.Add(
            new Label { FontAttributes = FontAttributes.Bold }
                .Bind(Label.TextProperty, "Train")
                .Column(0)
        );
        view.Add(
            new Label { HorizontalTextAlignment = TextAlignment.Center }
                .Bind(Label.TextProperty, "Time")
                .Column(1)
        );
        view.Add(
            new Label { HorizontalTextAlignment = TextAlignment.End }
                .Bind(Label.TextProperty, "Destination")
                .Column(2)
        );

        trainScheduleList = new ListView
        {
            ItemsSource = trainSchedules,
            ItemTemplate = new DataTemplate(() =>
            {
                return new ViewCell { View = view };
            }),
            SeparatorColor = Colors.Gray,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        Content = new VerticalStackLayout
        {
            Padding = 10,
            Children = { searchBar, trainScheduleList }
        };
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.ToLower() ?? string.Empty;

        trainScheduleList.ItemsSource = trainSchedules.Where(schedule =>
            schedule
                .GetType()
                .GetProperty("Train")
                ?.GetValue(schedule)
                ?.ToString()
                .ToLower()
                .Contains(searchText) == true
            || schedule
                .GetType()
                .GetProperty("Destination")
                ?.GetValue(schedule)
                ?.ToString()
                .ToLower()
                .Contains(searchText) == true
        );
    }
}
