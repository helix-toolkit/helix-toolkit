using System.Windows.Input;
#if WINUI
#else
using System.ComponentModel;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// Binds a <see cref="ManipulationGesture"/> to an <see cref="ICommand"/> implementation.
/// </summary>
public class ManipulationBinding : InputBinding
{
    /// <summary>
    /// Gets the finger count.
    /// </summary>
    public int FingerCount => (this.Gesture as ManipulationGesture)?.FingerCount ?? 0;

#if WPF
    [TypeConverter(typeof(ManipulationGestureConverter))]
#endif
    public override InputGesture? Gesture
    {
        get
        {
            return base.Gesture;
        }

        set
        {
            var oldGesture = this.Gesture;

            if (value is ManipulationGesture newGesture)
            {
                if (oldGesture != newGesture)
                {
                    base.Gesture = newGesture;
                }
            }
            else
            {
                throw new ArgumentException($"Invalid {nameof(ManipulationGesture)}", nameof(value));
            }
        }
    }

    public ManipulationBinding()
    {
    }

    public ManipulationBinding(ICommand command, ManipulationGesture gesture)
      : base(command, gesture)
    {
    }
}
