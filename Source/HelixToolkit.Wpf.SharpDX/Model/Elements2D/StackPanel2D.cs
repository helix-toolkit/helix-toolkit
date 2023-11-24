using HelixToolkit.SharpDX.Model.Scene2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using Orientation = System.Windows.Controls.Orientation;

namespace HelixToolkit.Wpf.SharpDX.Elements2D;

public class StackPanel2D : Panel2D
{
    /// <summary>
    /// Gets or sets the orientation.
    /// </summary>
    /// <value>
    /// The orientation.
    /// </value>
    public Orientation Orientation
    {
        get
        {
            return (Orientation)GetValue(OrientationProperty);
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
        DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackPanel2D), new PropertyMetadata(Orientation.Horizontal,
            (d, e) =>
            {
                if (d is Element2D { SceneNode: StackPanelNode2D node })
                {
                    node.Orientation = ((Orientation)e.NewValue).ToD2DOrientation();
                }
            }));

    protected override SceneNode2D OnCreateSceneNode()
    {
        return new StackPanelNode2D();
    }
}
