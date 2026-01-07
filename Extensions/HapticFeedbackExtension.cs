using TreniniApp.Behaviors;

namespace TreniniApp.Extensions;

/// <summary>
/// Extension methods for adding haptic feedback to UI elements.
/// </summary>
public static class HapticFeedbackExtension
{
    /// <summary>
    /// Adds haptic feedback behavior to any View.
    /// </summary>
    public static T WithHapticFeedback<T>(this T view)
        where T : View
    {
        view.Behaviors.Add(new HapticFeedbackBehavior());
        return view;
    }
}
