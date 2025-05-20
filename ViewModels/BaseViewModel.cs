using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TreniniApp.ViewModels;

public abstract class BaseViewModel : ObservableObject
{
    /// <summary>
    /// Called when the associated View appears. Override to load data asynchronously.
    /// </summary>
    public virtual Task OnAppearingAsync()
    {
        // Override in derived ViewModel to load data
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the associated View disappears. Override to cleanup or save state.
    /// </summary>
    public virtual Task OnDisappearingAsync()
    {
        // Override in derived ViewModel to cleanup
        return Task.CompletedTask;
    }
}
