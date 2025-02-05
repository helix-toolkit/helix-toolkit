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
/// Specifies constants that define actions performed by manipulation.
/// </summary>
#if false
#elif WINUI
#elif WPF
[TypeConverter(typeof(ManipulationActionConverter))]
#else
#error Unknown framework
#endif
public enum ManipulationAction
{
    None,

    Pan,

    Pinch,

    TwoFingerPan,

    ThreeFingerPan,
}
