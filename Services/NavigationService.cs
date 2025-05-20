using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using MauiControls = Microsoft.Maui.Controls;

namespace TreniniApp.Services;

public class NavigationService(MauiControls.NavigationPage navigationPage) : INavigationService
{
    private MauiControls.NavigationPage _navigationPage = navigationPage;

    public void SetNavigationPage(MauiControls.NavigationPage navigationPage)
    {
        _navigationPage = navigationPage;
    }

    public Task PushAsync(MauiControls.Page page) =>
        EnsureModalPageIsNotInStack(page, _navigationPage.PushAsync(page));

    public Task PopAsync() => _navigationPage.PopAsync();

    public Task PopToRootAsync() => _navigationPage.PopToRootAsync();

    public Task PushModalAsync(MauiControls.Page page)
    {
        _navigationPage.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);
        return EnsureModalPageIsNotInStack(page, _navigationPage.Navigation.PushModalAsync(page));
    }

    public Task PopModalAsync() => _navigationPage.Navigation.PopModalAsync();

    private Task EnsureModalPageIsNotInStack(MauiControls.Page page, Task task)
    {
        // Check if the page is already in the modal stack
        if (_navigationPage.Navigation.ModalStack.Contains(page))
        {
            return Task.CompletedTask; // or throw an exception if you prefer
        }

        return task;
    }
}
