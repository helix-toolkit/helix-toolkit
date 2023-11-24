using System.ComponentModel;

namespace HelixToolkit.Wpf.SharpDX;

/// <summary>
/// Specifies constants that define actions performed by manipulation.
/// </summary>
[TypeConverter(typeof(ManipulationActionConverter))]
public enum ManipulationAction
{
    None,

    Pan,

    Pinch,

    TwoFingerPan,

    ThreeFingerPan,
}
