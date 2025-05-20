using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using TreniniApp.Pages;
using TreniniApp.Services;

namespace TreniniApp;

public class App : Microsoft.Maui.Controls.Application
{
    readonly MainPage _mainPage;
    readonly INavigationService _navigationService;
    readonly Microsoft.Maui.Controls.NavigationPage _navigationPage;

    public App(
        MainPage mainPage,
        INavigationService navigationService,
        Microsoft.Maui.Controls.NavigationPage navigationPage
    )
    {
        _mainPage = mainPage;
        _navigationService = navigationService;
        _navigationPage = navigationPage;
        // Resources = new ResourceDictionary()
        // {
        // 	new Style<Shell>(
        // 		(Shell.NavBarHasShadowProperty, true),
        // 		(Shell.TitleColorProperty, ColorConstants.NavigationBarTextColor),
        // 		(Shell.DisabledColorProperty, ColorConstants.NavigationBarTextColor),
        // 		(Shell.UnselectedColorProperty, ColorConstants.NavigationBarTextColor),
        // 		(Shell.ForegroundColorProperty, ColorConstants.NavigationBarTextColor),
        // 		(Shell.BackgroundColorProperty, ColorConstants.NavigationBarBackgroundColor)).ApplyToDerivedTypes(true),

        // 	new Style<NavigationPage>(
        // 		(NavigationPage.BarTextColorProperty, ColorConstants.NavigationBarTextColor),
        // 		(NavigationPage.BarBackgroundColorProperty, ColorConstants.NavigationBarBackgroundColor)).ApplyToDerivedTypes(true)
        // };

        // Resources = new ResourceDictionary
        // {
        //     new Style<Microsoft.Maui.Controls.NavigationPage>(
        //         (Microsoft.Maui.Controls.NavigationPage.BarTextColorProperty, Colors.White),
        //         (Microsoft.Maui.Controls.NavigationPage.BarBackgroundColorProperty, Colors.DarkBlue)
        //     ).ApplyToDerivedTypes(true)
        // };
    }

    // protected override Window CreateWindow(IActivationState? activationState) => new(_mainPage);

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Ensure that MainPage is set as the root of the NavigationPage
        _navigationPage.On<iOS>().SetPrefersLargeTitles(true);

        if (_navigationPage.CurrentPage == null)
        {
            _navigationPage.PushAsync(_mainPage); // fallback, but it's better to use the constructor
        }

        // Update the NavigationPage in the service if necessary
        if (_navigationService is NavigationService navService)
        {
            navService.SetNavigationPage(_navigationPage);
        }

        return new Window(_navigationPage);
    }
}
