using HelixToolkit;
using HelixToolkit.Wpf;
using PropertyTools.DataAnnotations;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BrowsableAttribute = System.ComponentModel.BrowsableAttribute;

namespace Building;

public sealed class ChimneyVisual3D : UIElement3D
{
    public static readonly DependencyProperty PositionProperty = DependencyPropertyEx.Register<Point3D, ChimneyVisual3D>("Position", new Point3D(0, 0, 0), (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty TopPositionProperty = DependencyPropertyEx.Register<Point3D, ChimneyVisual3D>("TopPosition", new Point3D(0, 0, 0), (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty BaseDiameterProperty = DependencyPropertyEx.Register<double, ChimneyVisual3D>("BaseDiameter", 6, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty TopDiameterProperty = DependencyPropertyEx.Register<double, ChimneyVisual3D>("TopDiameter", 3, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty HeightProperty = DependencyPropertyEx.Register<double, ChimneyVisual3D>("Height", 80, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty BandsProperty = DependencyPropertyEx.Register<int, ChimneyVisual3D>("Bands", 8, (s, e) => s.AppearanceChanged());

    private readonly GeometryModel3D redbands = new GeometryModel3D();
    private readonly GeometryModel3D whitebands = new GeometryModel3D();

    public ChimneyVisual3D()
    {
        this.redbands.Material = MaterialHelper.CreateMaterial(Brushes.Red, ambient: 10);
        this.whitebands.Material = MaterialHelper.CreateMaterial(Brushes.White, ambient: 10);
        this.AppearanceChanged();
        var group = new Model3DGroup();
        group.Children.Add(this.redbands);
        group.Children.Add(this.whitebands);
        this.Visual3DModel = group;
    }

    public Point3D Position
    {
        get { return (Point3D)this.GetValue(PositionProperty); }
        set { this.SetValue(PositionProperty, value); }
    }

    public Point3D TopPosition
    {
        get { return (Point3D)this.GetValue(TopPositionProperty); }
        set { this.SetValue(TopPositionProperty, value); }
    }

    [Category("Chimney properties")]
    [Slidable(0, 20)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double BaseDiameter
    {
        get { return (double)this.GetValue(BaseDiameterProperty); }
        set { this.SetValue(BaseDiameterProperty, value); }
    }

    [Slidable(0, 20)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double TopDiameter
    {
        get { return (double)this.GetValue(TopDiameterProperty); }
        set { this.SetValue(TopDiameterProperty, value); }
    }

    [Slidable(0, 100)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double Height
    {
        get { return (double)this.GetValue(HeightProperty); }
        set { this.SetValue(HeightProperty, value); }
    }

    [Slidable(1, 20)]
    [FormatString("0.00")]
    [Browsable(true)]
    public int Bands
    {
        get { return (int)this.GetValue(BandsProperty); }
        set { this.SetValue(BandsProperty, value); }
    }

    private void AppearanceChanged()
    {
        var redbuilder = new MeshBuilder(false, false);
        var whitebuilder = new MeshBuilder(false, false);
        for (int i = 0; i < this.Bands; i++)
        {
            var f0 = (double)i / this.Bands;
            var f1 = (double)(i + 1) / this.Bands;
            var y0 = this.Height * f0;
            var y1 = this.Height * f1;
            var d0 = (this.BaseDiameter * (1 - f0)) + (this.TopDiameter * f0);
            var d1 = (this.BaseDiameter * (1 - f1)) + (this.TopDiameter * f1);
            var builder = (this.Bands - i) % 2 == 1 ? redbuilder : whitebuilder;
            builder.AddCone((this.Position + new Vector3D(0, 0, y0)).ToVector(), new Vector3D(0, 0, 1).ToVector(), (float)(d0 / 2), (float)(d1 / 2), (float)(y1 - y0), i == 0, i == this.Bands - 1, 20);
        }

        this.TopPosition = this.Position + new Vector3D(0, 0, this.Height);

        this.redbands.Geometry = redbuilder.ToMesh().ToMeshGeometry3D(true);
        this.whitebands.Geometry = whitebuilder.ToMesh().ToMeshGeometry3D(true);
    }
}
