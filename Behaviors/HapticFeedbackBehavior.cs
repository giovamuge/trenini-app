using CommunityToolkit.Maui.Behaviors;
using TreniniApp.Services;
using TreniniApp.Utils;

namespace TreniniApp.Behaviors;

/// <summary>
/// Behavior that adds haptic feedback to any tappable control.
/// </summary>
public class HapticFeedbackBehavior : BaseBehavior<View>
{
    private TapGestureRecognizer? _tapGestureRecognizer;

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);

        // Add tap gesture recognizer to trigger haptic feedback
        _tapGestureRecognizer = new TapGestureRecognizer();
        _tapGestureRecognizer.Tapped += OnTapped;
        bindable.GestureRecognizers.Add(_tapGestureRecognizer);
    }

    protected override void OnDetachingFrom(View bindable)
    {
        if (_tapGestureRecognizer != null)
        {
            _tapGestureRecognizer.Tapped -= OnTapped;
            bindable.GestureRecognizers.Remove(_tapGestureRecognizer);
            _tapGestureRecognizer = null;
        }

        base.OnDetachingFrom(bindable);
    }

    private void OnTapped(object? sender, EventArgs e)
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
    }
}
