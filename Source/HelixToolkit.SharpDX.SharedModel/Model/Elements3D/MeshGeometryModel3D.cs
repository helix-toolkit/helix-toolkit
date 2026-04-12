using HelixToolkit.SharpDX.Model.Scene;
using SharpDX.Direct3D11;

#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Model;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Model;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Model;
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
/// 
/// </summary>
public class MeshGeometryModel3D : MaterialGeometryModel3D
{
    #region Dependency Properties        
    /// <summary>
    /// The front counter clockwise property
    /// </summary>
    public static readonly DependencyProperty FrontCounterClockwiseProperty =
        HelixProperty.Register<MeshGeometryModel3D, bool>("FrontCounterClockwise",
            true, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MeshNode node })
            {
                node.FrontCCW = (bool)e.NewValue!;
            }
        });

    /// <summary>
    /// The cull mode property
    /// </summary>
    public static readonly DependencyProperty CullModeProperty =
        HelixProperty.Register<MeshGeometryModel3D, CullMode>("CullMode",
            CullMode.None, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MeshNode node })
            {
                node.CullMode = (CullMode)e.NewValue!;
            }
        });

    /// <summary>
    /// The invert normal property
    /// </summary>
    public static readonly DependencyProperty InvertNormalProperty =
        HelixProperty.Register<MeshGeometryModel3D, bool>("InvertNormal",
            false, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MeshNode node })
            {
                node.InvertNormal = (bool)e.NewValue!;
            }
        });

    /// <summary>
    /// The render wireframe property
    /// </summary>
    public static readonly DependencyProperty RenderWireframeProperty =
        HelixProperty.Register<MeshGeometryModel3D, bool>("RenderWireframe",
            false, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MeshNode node })
            {
                node.RenderWireframe = (bool)e.NewValue!;
            }
        });

    /// <summary>
    /// The wireframe color property
    /// </summary>
    public static readonly DependencyProperty WireframeColorProperty =
        HelixProperty.Register<MeshGeometryModel3D, UIColor>("WireframeColor",
            UIColors.SkyBlue, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MeshNode node })
            {
                node.WireframeColor = ((UIColor)e.NewValue!).ToColor4();
            }
        });

    /// <summary>
    /// Gets or sets a value indicating whether [render overlapping wireframe].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [render wireframe]; otherwise, <c>false</c>.
    /// </value>
    public bool RenderWireframe
    {
        get
        {
            return (bool)GetValue(RenderWireframeProperty)!;
        }
        set
        {
            SetValue(RenderWireframeProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the color of the wireframe.
    /// </summary>
    /// <value>
    /// The color of the wireframe.
    /// </value>
    public UIColor WireframeColor
    {
        get
        {
            return (UIColor)GetValue(WireframeColorProperty)!;
        }
        set
        {
            SetValue(WireframeColorProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [front counter clockwise].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [front counter clockwise]; otherwise, <c>false</c>.
    /// </value>
    public bool FrontCounterClockwise
    {
        set
        {
            SetValue(FrontCounterClockwiseProperty, value);
        }
        get
        {
            return (bool)GetValue(FrontCounterClockwiseProperty)!;
        }
    }

    /// <summary>
    /// Gets or sets the cull mode.
    /// </summary>
    /// <value>
    /// The cull mode.
    /// </value>
    public CullMode CullMode
    {
        set
        {
            SetValue(CullModeProperty, value);
        }
        get
        {
            return (CullMode)GetValue(CullModeProperty)!;
        }
    }

    /// <summary>
    /// Invert the surface normal during rendering
    /// </summary>
    public bool InvertNormal
    {
        set
        {
            SetValue(InvertNormalProperty, value);
        }
        get
        {
            return (bool)GetValue(InvertNormalProperty)!;
        }
    }

    #endregion

    /// <summary>
    /// Called when [create scene node].
    /// </summary>
    /// <returns></returns>
    protected override SceneNode OnCreateSceneNode()
    {
        return new MeshNode();
    }

    /// <summary>
    /// Assigns the default values to core.
    /// </summary>
    /// <param name="node">The node.</param>
    protected override void AssignDefaultValuesToSceneNode(SceneNode node)
    {
        if (node is MeshNode c)
        {
            c.InvertNormal = this.InvertNormal;
            c.WireframeColor = this.WireframeColor.ToColor4();
            c.RenderWireframe = this.RenderWireframe;
        }

        base.AssignDefaultValuesToSceneNode(node);
    }
}
