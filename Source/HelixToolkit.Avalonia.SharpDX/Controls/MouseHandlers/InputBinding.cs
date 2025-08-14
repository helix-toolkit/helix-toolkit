using Avalonia;
using System.Windows.Input;

namespace HelixToolkit.Avalonia.SharpDX;

/// <summary>
/// Binds a <see cref="InputGesture"/> to an <see cref="ICommand"/> implementation.
/// </summary>
public class InputBinding : AvaloniaObject
{
    /// <summary>
    /// Dependency Property for Command property
    /// </summary>
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<InputBinding, ICommand?>("Command");

    /// <summary>
    /// Dependency Property for Command Parameter
    /// </summary>
    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<InputBinding, object?>("CommandParameter");

    /// <summary>
    /// Dependency property for command target
    /// </summary>
    public static readonly StyledProperty<object?> CommandTargetProperty =
        // UIElement
        AvaloniaProperty.Register<InputBinding, object?>("CommandTarget");

    private InputGesture? gesture;

    /// <summary>
    /// Command Object associated
    /// </summary>
    public ICommand? Command
    {
        get => (ICommand?)this.GetValue(CommandProperty);
        set => this.SetValue(CommandProperty, value);
    }

    /// <summary>
    /// A parameter for the command.
    /// </summary>
    public object? CommandParameter
    {
        get => this.GetValue(CommandParameterProperty);
        set => this.SetValue(CommandParameterProperty, value);
    }

    /// <summary>
    /// Where the command should be raised.
    /// </summary>
    public object? CommandTarget
    {
        // UIElement
        get => (object?)this.GetValue(CommandTargetProperty);
        set => this.SetValue(CommandTargetProperty, value);
    }

    public virtual InputGesture? Gesture
    {
        get => this.gesture;
        set => this.gesture = value;
    }

    protected InputBinding()
    {
    }

    public InputBinding(ICommand command, InputGesture gesture)
    {
        this.Command = command;
        this.gesture = gesture;
    }
}
