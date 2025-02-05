using HelixToolkit.SharpDX.Model.Scene;

#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Model;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Model;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public class PointMaterialGeometryModel3D : GeometryModel3D
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty MaterialProperty =
        DependencyProperty.Register("Material", typeof(Material), typeof(PointMaterialGeometryModel3D), new PropertyMetadata(null, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: PointNode node })
            {
                node.Material = e.NewValue as Material;
            }
        }));

    /// <summary>
    /// 
    /// </summary>
    public Material Material
    {
        get
        {
            return (Material)this.GetValue(MaterialProperty);
        }
        set
        {
            this.SetValue(MaterialProperty, value);
        }
    }

    /// <summary>
    /// The hit test thickness property
    /// </summary>
    public static readonly DependencyProperty HitTestThicknessProperty =
        DependencyProperty.Register("HitTestThickness", typeof(double), typeof(PointMaterialGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: PointNode node })
            {
                node.HitTestThickness = (double)e.NewValue;
            }
        }));

    /// <summary>
    /// Used only for point/line hit test
    /// </summary>
    public double HitTestThickness
    {
        get
        {
            return (double)this.GetValue(HitTestThicknessProperty);
        }
        set
        {
            this.SetValue(HitTestThicknessProperty, value);
        }
    }
    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override SceneNode OnCreateSceneNode()
    {
        return new PointNode();
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode node)
    {
        base.AssignDefaultValuesToSceneNode(node);
        if (node is PointNode p)
        {
            p.Material = Material;
            p.HitTestThickness = HitTestThickness;
        }
    }
}
