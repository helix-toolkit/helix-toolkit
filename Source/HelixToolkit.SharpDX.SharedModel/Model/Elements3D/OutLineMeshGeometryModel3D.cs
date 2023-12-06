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

public class OutLineMeshGeometryModel3D : MeshGeometryModel3D
{
    public static readonly DependencyProperty EnableOutlineProperty = DependencyProperty.Register("EnableOutline", typeof(bool), typeof(OutLineMeshGeometryModel3D),
        new PropertyMetadata(true, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MeshOutlineNode node })
            {
                node.EnableOutline = (bool)e.NewValue;
            }
        }));

    public bool EnableOutline
    {
        set
        {
            SetValue(EnableOutlineProperty, value);
        }
        get
        {
            return (bool)GetValue(EnableOutlineProperty);
        }
    }

    public static readonly DependencyProperty OutlineColorProperty = DependencyProperty.Register("OutlineColor", typeof(UIColor), typeof(OutLineMeshGeometryModel3D),
        new PropertyMetadata(UIColors.White,
        (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MeshOutlineNode node })
            {
                node.OutlineColor = ((UIColor)e.NewValue).ToColor4();
            }
        }));

    public UIColor OutlineColor
    {
        set
        {
            SetValue(OutlineColorProperty, value);
        }
        get
        {
            return (UIColor)GetValue(OutlineColorProperty);
        }
    }

    public static readonly DependencyProperty IsDrawGeometryProperty = DependencyProperty.Register("IsDrawGeometry", typeof(bool), typeof(OutLineMeshGeometryModel3D),
        new PropertyMetadata(true, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MeshOutlineNode node })
            {
                node.IsDrawGeometry = (bool)e.NewValue;
            }
        }));

    public bool IsDrawGeometry
    {
        set
        {
            SetValue(IsDrawGeometryProperty, value);
        }
        get
        {
            return (bool)GetValue(IsDrawGeometryProperty);
        }
    }

    public static readonly DependencyProperty OutlineFadingFactorProperty = DependencyProperty.Register("OutlineFadingFactor", typeof(double), typeof(OutLineMeshGeometryModel3D),
        new PropertyMetadata(1.5, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: MeshOutlineNode node })
            {
                node.OutlineFadingFactor = (float)(double)e.NewValue;
            }
        }));

    public double OutlineFadingFactor
    {
        set
        {
            SetValue(OutlineFadingFactorProperty, value);
        }
        get
        {
            return (double)GetValue(OutlineFadingFactorProperty);
        }
    }

    protected override SceneNode OnCreateSceneNode()
    {
        return new MeshOutlineNode();
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode core)
    {
        if (core is MeshOutlineNode c)
        {
            c.OutlineColor = this.OutlineColor.ToColor4();
            c.EnableOutline = this.EnableOutline;
            c.OutlineFadingFactor = (float)this.OutlineFadingFactor;
            c.IsDrawGeometry = this.IsDrawGeometry;
        }

        base.AssignDefaultValuesToSceneNode(core);
    }
}
