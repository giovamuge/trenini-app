using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using TreniniApp.Pages;
using TreniniApp.Services;

namespace TreniniApp;

public class App(IServiceProvider service) : Microsoft.Maui.Controls.Application
{
    readonly IServiceProvider _service = service;

    // protected override Window CreateWindow(IActivationState? activationState) => new(_mainPage);

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Ensure that the MainPage by service provider is not null
        var mainPage =
            _service.GetRequiredService<MainPage>()
            ?? throw new InvalidOperationException(
                "MainPage is not registered in the service provider."
            );

        var navigationPage = new Microsoft.Maui.Controls.NavigationPage(mainPage);
        // Ensure that MainPage is set as the root of the NavigationPage
        navigationPage.On<iOS>().SetPrefersLargeTitles(true);
        return new Window(navigationPage);
    }
}
