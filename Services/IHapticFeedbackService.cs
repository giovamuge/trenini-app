namespace TreniniApp.Services;

/// <summary>
/// Service for providing haptic feedback to user interactions.
/// </summary>
public interface IHapticFeedbackService
{
    /// <summary>
    /// Performs a click haptic feedback.
    /// </summary>
    void PerformClick();

    /// <summary>
    /// Performs a long press haptic feedback.
    /// </summary>
    void PerformLongPress();
}
