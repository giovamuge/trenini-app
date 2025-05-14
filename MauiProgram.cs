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
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // .UseMauiCommunityToolkit(options =>
        // {
        //     options.SetShouldSuppressExceptionsInConverters(false);
        //     options.SetShouldSuppressExceptionsInBehaviors(false);
        //     options.SetShouldSuppressExceptionsInAnimations(false);
        // })

        //         builder.ConfigureMauiHandlers(handlers =>
        //         {
        // #if IOS || MACCATALYST
        //             handlers.AddHandler<Shell, ShellWithLargeTitles>();
        // #endif
        //         });

        // App
        builder.Services.AddSingleton<App>();
        // builder.Services.AddSingleton<AppShell>();

        // Services
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton(Browser.Default);
        builder.Services.AddTransient<IWebScrapingService, WebScrapingService>();

        // Pages + View Models
        builder.Services.AddTransientWithShellRoute<MainPage, MainViewModel>(
            $"//{nameof(MainPage)}"
        );
        // builder.Services.AddTransient<MainPage, MainViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
