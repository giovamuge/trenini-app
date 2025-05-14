using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using TreniniApp.Pages;

namespace TreniniApp;

public class App : Microsoft.Maui.Controls.Application
{
    readonly MainPage _mainPage;

    public App(MainPage mainPage)
    {
        _mainPage = mainPage;
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
        var navigationPage = new Microsoft.Maui.Controls.NavigationPage(_mainPage);
        navigationPage.On<iOS>().SetPrefersLargeTitles(true);
        return new Window(navigationPage);
    }
}
