using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using TreniniApp.Pages;
using TreniniApp.Services;
using TreniniApp.ViewModels;

namespace TreniniApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp
            .CreateBuilder()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit(options =>
            {
                options.SetShouldSuppressExceptionsInConverters(false);
                options.SetShouldSuppressExceptionsInBehaviors(false);
                options.SetShouldSuppressExceptionsInAnimations(false);
            })
            .UseMauiCommunityToolkitMarkup()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // App
        builder.Services.AddSingleton<App>();

        // Navigation
        builder.Services.AddSingleton<NavigationPage>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // Services
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton(Browser.Default);
        builder.Services.AddTransient<IWebScrapingService, WebScrapingService>();
        builder.Services.AddTransient<IStationService, StationService>();

        // Pages + View Models
        builder.Services.AddTransient<MainPage, MainViewModel>();
        builder.Services.AddTransient<SelectStationPage, SelectStationViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
