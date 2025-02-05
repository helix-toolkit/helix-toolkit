using HelixToolkit.SharpDX;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public class OctreeLineGeometryModel3D : CompositeModel3D
{
    public static readonly DependencyProperty OctreeProperty
        = DependencyProperty.Register("Octree", typeof(IOctreeBasic), typeof(OctreeLineGeometryModel3D),
            new PropertyMetadata(null, (s, e) =>
            {
                if (s is not OctreeLineGeometryModel3D d)
                {
                    return;
                }

                if (e.OldValue != null)
                {
                    if (e.OldValue is IOctreeBasic o)
                    {
                        o.Hit -= d.OctreeLineGeometryModel3D_OnHit;
                    }
                }
                if (e.NewValue != null)
                {
                    if (e.NewValue is IOctreeBasic o)
                    {
                        o.Hit += d.OctreeLineGeometryModel3D_OnHit;
                    }
                }
                d.CreateOctreeLines();
            }));

    public static readonly DependencyProperty LineColorProperty
        = DependencyProperty.Register("LineColor", typeof(UIColor), typeof(OctreeLineGeometryModel3D), new PropertyMetadata(UIColors.Green));

    public static readonly DependencyProperty HitLineColorProperty
        = DependencyProperty.Register("HitLineColor", typeof(UIColor), typeof(OctreeLineGeometryModel3D), new PropertyMetadata(UIColors.Red));

    public IOctreeBasic Octree
    {
        set
        {
            SetValue(OctreeProperty, value);
        }
        get
        {
            return (IOctreeBasic)GetValue(OctreeProperty);
        }
    }

    public UIColor LineColor
    {
        set
        {
            SetValue(LineColorProperty, value);
        }
        get
        {
            return (UIColor)GetValue(LineColorProperty);
        }
    }
    public UIColor HitLineColor
    {
        set
        {
            SetValue(HitLineColorProperty, value);
        }
        get
        {
            return (UIColor)GetValue(HitLineColorProperty);
        }
    }

    private readonly LineGeometryModel3D OctreeVisual = new();
    private readonly LineGeometryModel3D HitVisual = new();

    public OctreeLineGeometryModel3D()
    {
        IsHitTestVisible = OctreeVisual.IsHitTestVisible = HitVisual.IsHitTestVisible = false;
        Children.Add(OctreeVisual);
        Children.Add(HitVisual);
        OctreeVisual.Color = LineColor;
        HitVisual.Color = HitLineColor;
        OctreeVisual.Thickness = 0;
        OctreeVisual.FillMode = global::SharpDX.Direct3D11.FillMode.Wireframe;
        HitVisual.Thickness = 1.5;
        this.SceneNode.VisibleChanged += OctreeLineGeometryModel3D_OnVisibleChanged;
    }

    private void OctreeLineGeometryModel3D_OnVisibleChanged(object? sender, BoolArgs e)
    {
        CreateOctreeLines();
    }

    private void CreateOctreeLines()
    {
        if (Octree != null && Visibility == UIVisibility.Visible && IsRendering)
        {
            OctreeVisual.Geometry = Octree.CreateOctreeLineModel();
            OctreeVisual.Color = LineColor;
        }
        else
        {
            OctreeVisual.Geometry = null;
        }
    }

    private void OctreeLineGeometryModel3D_OnHit(object? sender, EventArgs args)
    {
        if (sender is IOctreeBasic node && node.HitPathBoundingBoxes.Count > 0 && Visibility == UIVisibility.Visible && IsRendering)
        {
            HitVisual.Geometry = node.HitPathBoundingBoxes.CreatePathLines();
            HitVisual.Color = HitLineColor;
        }
        else
        {
            HitVisual.Geometry = null;
        }
    }
}
