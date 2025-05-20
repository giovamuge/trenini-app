namespace TreniniApp.Services;

public interface INavigationService
{
    Task PushAsync(Page page);
    Task PopAsync();
    Task PopToRootAsync();
    Task PushModalAsync(Page page);
    Task PopModalAsync();
}
