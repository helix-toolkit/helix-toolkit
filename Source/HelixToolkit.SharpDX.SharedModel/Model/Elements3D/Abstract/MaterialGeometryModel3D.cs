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
/// 
/// </summary>
/// <seealso cref="GeometryModel3D" />
public abstract class MaterialGeometryModel3D : GeometryModel3D
{
    #region Dependency Properties
    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty MaterialProperty =
        DependencyProperty.Register("Material", typeof(Material), typeof(MaterialGeometryModel3D), new PropertyMetadata(null, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MaterialGeometryNode core })
            {
                core.Material = (Material?)e.NewValue;
            }
        }));

    /// <summary>
    /// Specifiy if model material is transparent. 
    /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
    /// </summary>
    public static readonly DependencyProperty IsTransparentProperty =
        DependencyProperty.Register("IsTransparent", typeof(bool), typeof(MaterialGeometryModel3D), new PropertyMetadata(false, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MaterialGeometryNode core })
            {
                core.IsTransparent = (bool)e.NewValue;
            }
        }));

    /// <summary>
    /// 
    /// </summary>
    public Material? Material
    {
        get
        {
            return (Material?)this.GetValue(MaterialProperty);
        }
        set
        {
            this.SetValue(MaterialProperty, value);
        }
    }

    /// <summary>
    /// Specifiy if model material is transparent. 
    /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
    /// </summary>
    public bool IsTransparent
    {
        get
        {
            return (bool)GetValue(IsTransparentProperty);
        }
        set
        {
            SetValue(IsTransparentProperty, value);
        }
    }
    #endregion

    /// <summary>
    /// Assigns the default values to scene node.
    /// </summary>
    /// <param name="node">The node.</param>
    protected override void AssignDefaultValuesToSceneNode(SceneNode node)
    {
        if (node is MaterialGeometryNode n)
        {
            n.Material = this.Material;
        }
        base.AssignDefaultValuesToSceneNode(node);
    }
}
