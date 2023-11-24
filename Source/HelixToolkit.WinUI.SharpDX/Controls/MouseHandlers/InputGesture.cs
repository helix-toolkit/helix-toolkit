namespace HelixToolkit.WinUI.SharpDX;

/// <summary>
/// Defines an input gesture that can be used to invoke a command.
/// </summary>
public abstract class InputGesture
{
    public abstract bool Matches(object targetElement, RoutedEventArgs inputEventArgs);
}
