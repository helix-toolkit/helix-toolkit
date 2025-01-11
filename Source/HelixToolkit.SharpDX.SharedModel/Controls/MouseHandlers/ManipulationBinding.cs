using System.Windows.Input;
#if false
#elif WINUI
#elif WPF
using System.ComponentModel;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
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

#if false
#elif WINUI
#elif WPF
    [TypeConverter(typeof(ManipulationGestureConverter))]
#else
#error Unknown framework
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
