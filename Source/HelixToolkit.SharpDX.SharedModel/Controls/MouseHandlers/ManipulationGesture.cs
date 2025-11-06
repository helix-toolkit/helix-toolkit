#if false
#elif WINUI
#elif WPF
using System.Windows.Input;
#elif AVALONIA
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// Defines a touch input gesture that can be used to invoke a command.
/// </summary>
public partial class ManipulationGesture : InputGesture
{
    public ManipulationAction ManipulationAction { get; }

    public int FingerCount { get; }

    public ManipulationGesture(ManipulationAction manipulationAction)
    {
        this.ManipulationAction = manipulationAction;
        this.FingerCount = manipulationAction.FingerCount();
    }
}
