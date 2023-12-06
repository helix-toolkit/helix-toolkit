using DependencyPropertyGenerator;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;

namespace MouseDragDemo;

[DependencyProperty<bool>("CanTranslateX", DefaultValue = true, OnChanged = nameof(OnChildrenChanged))]
[DependencyProperty<bool>("CanTranslateY", DefaultValue = true, OnChanged = nameof(OnChildrenChanged))]
[DependencyProperty<bool>("CanTranslateZ", DefaultValue = true, OnChanged = nameof(OnChildrenChanged))]
[DependencyProperty<MeshGeometryModel3D>("Content")]
public partial class ResizeManipulator3D : GroupElement3D //, IHitable, INotifyPropertyChanged
{
    private UITranslateManipulator3D translateXL, translateYL, translateZL, translateXR, translateYR, translateZR;
    private LineGeometryModel3D? selectionBounds;

    /// <summary>
    /// 
    /// </summary>
    public ResizeManipulator3D()
    {
        var red = PhongMaterials.Red;
        red.ReflectiveColor = Color.Black;
        //red.SpecularShininess = 0f;
        this.translateXR = new UITranslateManipulator3D { Direction = new Vector3(+1, 0, 0), IsThrowingShadow = false, Material = red, };
        this.translateYR = new UITranslateManipulator3D { Direction = new Vector3(0, +1, 0), IsThrowingShadow = false, Material = PhongMaterials.Green };
        this.translateZR = new UITranslateManipulator3D { Direction = new Vector3(0, 0, +1), IsThrowingShadow = false, Material = PhongMaterials.Blue };
        this.translateXL = new UITranslateManipulator3D { Direction = new Vector3(-1, 0, 0), IsThrowingShadow = false, Material = red };
        this.translateYL = new UITranslateManipulator3D { Direction = new Vector3(0, -1, 0), IsThrowingShadow = false, Material = PhongMaterials.Green };
        this.translateZL = new UITranslateManipulator3D { Direction = new Vector3(0, 0, -1), IsThrowingShadow = false, Material = PhongMaterials.Blue };
        //this.rotateZ = new UIRotateManipulator3D { Axis = Vector3.UnitZ, InnerDiameter = 2, OuterDiameter = 2.15, Length = 0.05 };

        this.CanTranslateX = true;
        this.CanTranslateY = false;
        this.CanTranslateZ = false;
        this.IsRendering = false;

        this.OnChildrenChanged();
        // this.OnContentChanged();                       
    }

    /// <summary>
    /// The on children changed.
    /// </summary>
    protected virtual void OnChildrenChanged()
    {
        this.translateXL.Length = 0.5;
        this.translateYL.Length = 0.5;
        this.translateZL.Length = 0.5;
        this.translateXR.Length = 0.5;
        this.translateYR.Length = 0.5;
        this.translateZR.Length = 0.5;

        this.Children.Clear();

        if (this.CanTranslateX)
        {
            this.Children.Add(this.translateXL);
            this.Children.Add(this.translateXR);
        }

        if (this.CanTranslateY)
        {
            this.Children.Add(this.translateYL);
            this.Children.Add(this.translateYR);
        }

        if (this.CanTranslateZ)
        {
            this.Children.Add(this.translateZL);
            this.Children.Add(this.translateZR);
        }


        {
            var g = new LineBuilder();
            g.AddLine(new Vector3(0, 0, 0), new Vector3(1, 0, 0));
            g.AddLine(new Vector3(1, 0, 0), new Vector3(1, 1, 0));
            g.AddLine(new Vector3(1, 1, 0), new Vector3(0, 1, 0));
            g.AddLine(new Vector3(0, 1, 0), new Vector3(0, 0, 0));
            this.selectionBounds = new LineGeometryModel3D()
            {
                Thickness = 3,
                Smoothness = 2,
                Color = System.Windows.Media.Colors.Red,
                IsThrowingShadow = false,
                Geometry = g.ToLineGeometry3D(),
            };
            this.Children.Add(this.selectionBounds);
        }
    }
}
