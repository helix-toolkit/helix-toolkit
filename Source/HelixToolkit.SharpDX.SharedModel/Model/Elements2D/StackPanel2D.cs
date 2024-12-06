using HelixToolkit.SharpDX.Model.Scene2D;
#if WINUI
using HelixToolkit.WinUI.SharpDX.Extensions;
#else
using HelixToolkit.Wpf.SharpDX.Extensions;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX.Elements2D;
#else
namespace HelixToolkit.Wpf.SharpDX.Elements2D;
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
            return (UIOrientation)GetValue(OrientationProperty);
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
        DependencyProperty.Register("Orientation", typeof(UIOrientation), typeof(StackPanel2D), new PropertyMetadata(UIOrientation.Horizontal,
            (d, e) =>
            {
                if (d is Element2D { SceneNode: StackPanelNode2D node })
                {
                    node.Orientation = ((UIOrientation)e.NewValue).ToD2DOrientation();
                }
            }));

    protected override SceneNode2D OnCreateSceneNode()
    {
        return new StackPanelNode2D();
    }
}
