using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using TreniniApp.Extensions;

namespace TreniniApp.Services;

public class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    private static INavigation Navigation
    {
        get
        {
            // Assumes MainPage is a NavigationPage
            if (Application.Current?.Windows[0]?.Page is not NavigationPage navigationPage)
            {
                throw new InvalidOperationException("MainPage is not a NavigationPage");
            }

            return navigationPage.Navigation;
        }
    }

    public Task PushAsync<TPage>()
        where TPage : Page
    {
        var page = _serviceProvider.GetService<TPage>();
        return Navigation.PushAsync(page);
    }

    public Task PopAsync() => Navigation.PopAsync();

    public Task PopToRootAsync() => Navigation.PopToRootAsync();

    public Task PushModalAsync<TPage>()
        where TPage : Page
    {
        var page = _serviceProvider.GetService<TPage>();
        return Navigation.PushModalAsync(page);
    }

    public Task PopModalAsync() => Navigation.PopModalAsync();

    private Task EnsureModalPageIsNotInStack(Page page, Task task)
    {
        // Check if the page is already in the modal stack
        if (Navigation.ModalStack.Contains(page))
        {
            return Task.CompletedTask; // or throw an exception if you prefer
        }

        return task;
    }
}
