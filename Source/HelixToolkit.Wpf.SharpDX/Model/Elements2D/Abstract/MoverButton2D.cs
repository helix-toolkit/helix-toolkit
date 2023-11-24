using System.Windows;

namespace HelixToolkit.Wpf.SharpDX.Elements2D;

/// <summary>
/// Use to apply style for mover button from Generic.xaml/>
/// </summary>
/// <seealso cref="HelixToolkit.Wpf.SharpDX.Elements2D.Button2D" />
public sealed class MoverButton2D : Button2D
{
    static MoverButton2D()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(MoverButton2D), new FrameworkPropertyMetadata(typeof(MoverButton2D)));
    }
}
