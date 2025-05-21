using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace TreniniApp.Services;

public class NavigationService(NavigationPage navigationPage) : INavigationService
{
    private NavigationPage _navigationPage = navigationPage;

    public void SetNavigationPage(NavigationPage navigationPage)
    {
        _navigationPage = navigationPage;
    }

    public Task PushAsync(Page page) =>
        EnsureModalPageIsNotInStack(page, _navigationPage.PushAsync(page));

    public Task PopAsync() => _navigationPage.PopAsync();

    public Task PopToRootAsync() => _navigationPage.PopToRootAsync();

    public Task PushModalAsync(Page page) =>
        EnsureModalPageIsNotInStack(page, _navigationPage.Navigation.PushModalAsync(page));

    public Task PopModalAsync() => _navigationPage.Navigation.PopModalAsync();

    private Task EnsureModalPageIsNotInStack(Page page, Task task)
    {
        // Check if the page is already in the modal stack
        if (_navigationPage.Navigation.ModalStack.Contains(page))
        {
            return Task.CompletedTask; // or throw an exception if you prefer
        }

        return task;
    }
}
