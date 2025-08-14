using HelixToolkit.SharpDX.Model.Scene2D;
#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Extensions;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Extensions;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Extensions;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX.Elements2D;
#else
#error Unknown framework
#endif

public class StackPanel2D : Panel2D
{
    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    /// <value>
    /// The orientation.
    /// </value>
    public UIOrientation Orientation
    {
        get
        {
            return (UIOrientation)GetValue(OrientationProperty)!;
        }
        set
        {
            SetValue(OrientationProperty, value);
        }
    }

    /// <summary>
    /// The orientation property
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        HelixProperty.Register<StackPanel2D, UIOrientation>("Orientation",
            UIOrientation.Horizontal,
            (d, e) =>
            {
                if (d is Element2D { SceneNode: StackPanelNode2D node })
                {
                    node.Orientation = ((UIOrientation)e.NewValue!).ToD2DOrientation();
                }
            });

    protected override SceneNode2D OnCreateSceneNode()
    {
        return new StackPanelNode2D();
    }
}
