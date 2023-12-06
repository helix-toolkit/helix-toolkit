using System.Windows.Input;
using Windows.Foundation.Metadata;

namespace HelixToolkit.WinUI.SharpDX;

/// <summary>
/// Binds a <see cref="ManipulationGesture"/> to an <see cref="ICommand"/> implementation.
/// </summary>
public class ManipulationBinding : DependencyObject
{
    /// <summary>
    /// Dependency Property for Command property
    /// </summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("Command", typeof(ViewportCommand), typeof(ManipulationBinding), new PropertyMetadata(null));

    /// <summary>
    /// Dependency Property for Command Parameter
    /// </summary>
    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register("CommandParameter", typeof(object), typeof(ManipulationBinding), new PropertyMetadata(null));

    /// <summary>
    /// Dependency property for command target
    /// </summary>
    public static readonly DependencyProperty CommandTargetProperty =
        DependencyProperty.Register("CommandTarget", typeof(UIElement), typeof(ManipulationBinding), new PropertyMetadata(null));

    public static readonly DependencyProperty GestureProperty =
        DependencyProperty.Register("Gesture", typeof(ManipulationGesture), typeof(ManipulationBinding), new PropertyMetadata(null));


    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <remarks>
    /// Makes it possible to assign a string to this property in XAML via <see cref="CreateFromStringAttribute"/>.
    /// Another way would be the use of <see cref="ViewportCommandExtension"/>, but it 
    /// requires Min Target Platform Version "Windows 10 Fall Creators Update (introduced v10.0.16299.0)".
    /// </remarks>
    public ViewportCommand? Command
    {
        get => (ViewportCommand?)this.GetValue(CommandProperty);
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
    public UIElement? CommandTarget
    {
        get => (UIElement?)this.GetValue(CommandTargetProperty);
        set => this.SetValue(CommandTargetProperty, value);
    }

    /// <summary>
    /// Gets the finger count.
    /// </summary>
    public int FingerCount => this.Gesture?.FingerCount ?? 0;

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <remarks>
    /// Makes it possible to assign a string to this property in XAML via <see cref="CreateFromStringAttribute"/>.
    /// </remarks>

    public ManipulationGesture? Gesture
    {
        get
        {
            return (ManipulationGesture?)GetValue(GestureProperty);
        }
        set
        {
            SetValue(GestureProperty, value);
        }
    }
}
