namespace TreniniApp.Services;

public interface INavigationService
{
    Task PushAsync<TPage>()
        where TPage : Page;
    Task PopAsync();
    Task PopToRootAsync();
    Task PushModalAsync<TPage>()
        where TPage : Page;
    Task PopModalAsync();
}
