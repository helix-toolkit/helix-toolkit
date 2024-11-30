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
/// Specifies constants that define actions performed by manipulation.
/// </summary>
#if WPF
[TypeConverter(typeof(ManipulationActionConverter))]
#endif
public enum ManipulationAction
{
    None,

    Pan,

    Pinch,

    TwoFingerPan,

    ThreeFingerPan,
}
