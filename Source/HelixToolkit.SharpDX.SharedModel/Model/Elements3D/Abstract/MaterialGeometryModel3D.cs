using HelixToolkit.SharpDX.Model.Scene;

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
/// <seealso cref="GeometryModel3D" />
public abstract class MaterialGeometryModel3D : GeometryModel3D
{
    #region Dependency Properties
    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty MaterialProperty =
        HelixProperty.Register<MaterialGeometryModel3D, Material?>("Material",
            null, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MaterialGeometryNode core })
            {
                core.Material = (Material?)e.NewValue;
            }
        });

    /// <summary>
    /// Specifiy if model material is transparent. 
    /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
    /// </summary>
    public static readonly DependencyProperty IsTransparentProperty =
        HelixProperty.Register<MaterialGeometryModel3D, bool>("IsTransparent",
            false, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MaterialGeometryNode core })
            {
                core.IsTransparent = (bool)e.NewValue!;
            }
        });

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
            return (bool)GetValue(IsTransparentProperty)!;
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
