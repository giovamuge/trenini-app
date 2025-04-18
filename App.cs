using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace TreniniApp;

public class App : Microsoft.Maui.Controls.Application
{
    public App() { }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var navigationPage = new Microsoft.Maui.Controls.NavigationPage(new MainPage());
        navigationPage.On<iOS>().SetPrefersLargeTitles(true);
        return new Window(navigationPage);
    }
}
