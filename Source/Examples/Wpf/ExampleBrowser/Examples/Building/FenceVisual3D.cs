using HelixToolkit;
using HelixToolkit.Wpf;
using PropertyTools.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BrowsableAttribute = System.ComponentModel.BrowsableAttribute;
using System.Numerics;

namespace Building;

public sealed class FenceVisual3D : UIElement3D
{
    public static readonly DependencyProperty DiameterProperty = DependencyPropertyEx.Register<double, FenceVisual3D>("Diameter", 0.05, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty HeightProperty = DependencyPropertyEx.Register<double, FenceVisual3D>("Height", 1, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty PositionsProperty = DependencyPropertyEx.Register<Point3DCollection, FenceVisual3D>("Positions", new Point3DCollection(), (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty MeshSizeProperty = DependencyPropertyEx.Register<double, FenceVisual3D>("MeshSize", 0.06, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty PoleDistanceProperty = DependencyPropertyEx.Register<double, FenceVisual3D>("PoleDistance", 2, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty FenceTextureProperty = DependencyPropertyEx.Register<Brush?, FenceVisual3D>("FenceTexture", null, (s, e) => s.FenceTextureChanged());
    public static readonly DependencyProperty PoleTextureProperty = DependencyPropertyEx.Register<Brush?, FenceVisual3D>("PoleTexture", null, (s, e) => s.PoleTextureChanged());

    private readonly GeometryModel3D postsModel = new();
    private readonly GeometryModel3D fenceModel = new();

    public FenceVisual3D()
    {
        this.AppearanceChanged();
        var group = new Model3DGroup();
        group.Children.Add(this.postsModel);
        group.Children.Add(this.fenceModel);
        this.Visual3DModel = group;
    }

    [Category("Fence properties")]
    [Slidable(0.01, 0.2)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double Diameter
    {
        get { return (double)this.GetValue(DiameterProperty); }
        set { this.SetValue(DiameterProperty, value); }
    }

    [Slidable(0, 2)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double Height
    {
        get { return (double)this.GetValue(HeightProperty); }
        set { this.SetValue(HeightProperty, value); }
    }

    public Point3DCollection Positions
    {
        get { return (Point3DCollection)this.GetValue(PositionsProperty); }
        set { this.SetValue(PositionsProperty, value); }
    }

    [Slidable(0.01, 0.2)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double MeshSize
    {
        get { return (double)this.GetValue(MeshSizeProperty); }
        set { this.SetValue(MeshSizeProperty, value); }
    }

    [Slidable(0.2, 10)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double PoleDistance
    {
        get { return (double)this.GetValue(PoleDistanceProperty); }
        set { this.SetValue(PoleDistanceProperty, value); }
    }

    public Brush? FenceTexture
    {
        get { return (Brush?)this.GetValue(FenceTextureProperty); }
        set { this.SetValue(FenceTextureProperty, value); }
    }

    public Brush? PoleTexture
    {
        get { return (Brush?)this.GetValue(PoleTextureProperty); }
        set { this.SetValue(PoleTextureProperty, value); }
    }

    private void FenceTextureChanged()
    {
        this.fenceModel.Material = this.fenceModel.BackMaterial = this.FenceTexture != null ?
            new DiffuseMaterial(this.FenceTexture) { AmbientColor = Colors.White } : null;
    }

    private void PoleTextureChanged()
    {
        this.postsModel.Material = this.postsModel.BackMaterial = this.PoleTexture != null ? MaterialHelper.CreateMaterial(this.PoleTexture) : null;
    }

    private void AppearanceChanged()
    {
        var builder = new MeshBuilder(false, false);
        foreach (var p1 in DistributePoles(this.Positions, (float)this.PoleDistance))
        {
            var p2 = p1 + new Vector3D(0, 0, this.Height);
            builder.AddCylinder(p1.ToVector3(), p2.ToVector3(), (float)this.Diameter, 36);
        }

        this.postsModel.Geometry = builder.ToMesh().ToWndMeshGeometry3D();

        var fenceBuilder = new MeshBuilder(false, true);
        var w0 = 0d;
        for (int i = 0; i + 1 < this.Positions.Count; i++)
        {
            var p0 = this.Positions[i];
            var p1 = this.Positions[i + 1];
            var p2 = this.Positions[i + 1] + new Vector3D(0, 0, this.Height);
            var p3 = this.Positions[i] + new Vector3D(0, 0, this.Height);
            var h = this.Height / this.MeshSize;
            var dw = p0.DistanceTo(p1) / this.MeshSize;
            fenceBuilder.AddQuad(
                p0.ToVector3(),
                p1.ToVector3(),
                p2.ToVector3(),
                p3.ToVector3(),
                new Vector2((float)w0, (float)h),
                new Vector2((float)(w0 + dw), (float)h),
                new Vector2((float)(w0 + dw), 0),
                new Vector2((float)w0, 0));
            w0 += dw;
        }

        this.fenceModel.Geometry = fenceBuilder.ToMesh().ToWndMeshGeometry3D();
    }

    private static IEnumerable<Point3D> DistributePoles(IList<Point3D> positions, double distance)
    {
        var l0 = 0d;
        var x = distance;
        for (int i = 0; i + 1 < positions.Count; i++)
        {
            var p0 = positions[i];
            var p1 = positions[i + 1];
            var d = p0.DistanceTo(p1);
            while (x >= l0 && x < l0 + d)
            {
                var f = (x - l0) / d;
                yield return p0 + ((p1 - p0) * f);
                x += distance;
            }

            l0 += d;
            if (i + 1 == positions.Count)
            {
                yield return p1;
            }
        }
    }
}
