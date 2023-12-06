using System.Windows.Media.Media3D;
using System.Windows;
using DependencyPropertyGenerator;

namespace Overlay;

/// <summary>
/// The overlay.
/// </summary>
[AttachedDependencyProperty<Point3D, Overlay>("Position3D")]
public partial class Overlay : DependencyObject
{
    // Using a DependencyProperty as the backing store for Position3D.  This enables animation, styling, binding, etc...
}
