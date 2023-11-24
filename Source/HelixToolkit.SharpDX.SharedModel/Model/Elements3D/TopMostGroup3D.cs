using HelixToolkit.SharpDX.Model.Scene;

#if WINUI
using HelixToolkit.WinUI.SharpDX.Model;
#else
using HelixToolkit.Wpf.SharpDX.Model;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// Provides a way to render child elements always on top of other elements
/// This is rendered at the same level of screen spaced group items.
/// Child items do not support post effects.
/// </summary>
public class TopMostGroup3D : GroupModel3D
{
    /// <summary>
    /// Gets or sets a value indicating whether [enable top most mode].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable top most mode]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableTopMost
    {
        get
        {
            return (bool)GetValue(EnableTopMostProperty);
        }
        set
        {
            SetValue(EnableTopMostProperty, value);
        }
    }

    // Using a DependencyProperty as the backing store for EnableTopMost.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty EnableTopMostProperty =
        DependencyProperty.Register("EnableTopMost", typeof(bool), typeof(TopMostGroup3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: TopMostGroupNode node })
            {
                node.EnableTopMost = (bool)e.NewValue;
            }
        }));

    protected override SceneNode OnCreateSceneNode()
    {
        return new TopMostGroupNode();
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode node)
    {
        if (node is TopMostGroupNode n)
        {
            n.EnableTopMost = EnableTopMost;
        }

        base.AssignDefaultValuesToSceneNode(node);
    }
}
