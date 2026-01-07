namespace TreniniApp.Services;

/// <summary>
/// Implementation of haptic feedback service using .NET MAUI HapticFeedback API.
/// </summary>
public class HapticFeedbackService : IHapticFeedbackService
{
    public void PerformClick()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch
        {
            // Haptic feedback might not be available on all devices
        }
    }

    public void PerformLongPress()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        }
        catch
        {
            // Haptic feedback might not be available on all devices
        }
    }
}
