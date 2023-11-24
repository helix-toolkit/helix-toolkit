using HelixToolkit;
using HelixToolkit.Wpf;
using PropertyTools.DataAnnotations;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BrowsableAttribute = System.ComponentModel.BrowsableAttribute;

namespace Building;

public sealed class HouseVisual3D : UIElement3D
{
    public static readonly DependencyProperty RoofAngleProperty = DependencyPropertyEx.Register<double, HouseVisual3D>("RoofAngle", 15, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty RoofThicknessProperty = DependencyPropertyEx.Register<double, HouseVisual3D>("RoofThickness", 0.2, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty FloorThicknessProperty = DependencyPropertyEx.Register<double, HouseVisual3D>("FloorThickness", 0.2, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty StoryHeightProperty = DependencyPropertyEx.Register<double, HouseVisual3D>("StoryHeight", 2.5, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty WidthProperty = DependencyPropertyEx.Register<double, HouseVisual3D>("Width", 10, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty LengthProperty = DependencyPropertyEx.Register<double, HouseVisual3D>("Length", 20, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty StoriesProperty = DependencyPropertyEx.Register<int, HouseVisual3D>("Stories", 1, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty ExplodedRoofProperty = DependencyPropertyEx.Register<bool, HouseVisual3D>("ExplodedRoof", false, (s, e) => s.ExplodedRoofChanged());

    private readonly GeometryModel3D roof = new();
    private readonly GeometryModel3D walls = new();

    public HouseVisual3D()
    {
        this.roof.Material = MaterialHelper.CreateMaterial(Brushes.Brown, ambient: 10);
        this.walls.Material = MaterialHelper.CreateMaterial(Brushes.White, ambient: 10);
        this.AppearanceChanged();
        var model = new Model3DGroup();
        model.Children.Add(this.roof);
        model.Children.Add(this.walls);
        this.Visual3DModel = model;
    }

    [Category("House properties")]
    [Slidable(0, 60)]
    [Browsable(true)]
    public double Width
    {
        get { return (double)this.GetValue(WidthProperty); }
        set { this.SetValue(WidthProperty, value); }
    }

    [Slidable(0, 60)]
    [Browsable(true)]
    public double Length
    {
        get { return (double)this.GetValue(LengthProperty); }
        set { this.SetValue(LengthProperty, value); }
    }

    [Slidable(0, 4)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double StoryHeight
    {
        get { return (double)this.GetValue(StoryHeightProperty); }
        set { this.SetValue(StoryHeightProperty, value); }
    }

    [Slidable(1, 8)]
    [Browsable(true)]
    public int Stories
    {
        get { return (int)this.GetValue(StoriesProperty); }
        set { this.SetValue(StoriesProperty, value); }
    }

    [Slidable(0, 60)]
    [Browsable(true)]
    public double RoofAngle
    {
        get { return (double)this.GetValue(RoofAngleProperty); }
        set { this.SetValue(RoofAngleProperty, value); }
    }

    [Slidable(0, 2)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double RoofThickness
    {
        get { return (double)this.GetValue(RoofThicknessProperty); }
        set { this.SetValue(RoofThicknessProperty, value); }
    }

    [Slidable(0, 1)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double FloorThickness
    {
        get { return (double)this.GetValue(FloorThicknessProperty); }
        set { this.SetValue(FloorThicknessProperty, value); }
    }

    [Browsable(true)]
    public bool ExplodedRoof
    {
        get { return (bool)this.GetValue(ExplodedRoofProperty); }
        set { this.SetValue(ExplodedRoofProperty, value); }
    }
    private void ExplodedRoofChanged()
    {
        if (ExplodedRoof)
            roof.Transform = new TranslateTransform3D(0, 0, 30);
        else
            roof.Transform = Transform3D.Identity;
    }

    private void AppearanceChanged()
    {
        var y0 = 0d;
        var wallBuilder = new MeshBuilder(false, false);
        for (int i = 0; i < this.Stories; i++)
        {
            if (i > 0 && this.FloorThickness > 0)
            {
                wallBuilder.AddBox(new Point3D(0, 0, y0 + this.FloorThickness / 2).ToVector(), (float)(this.Length + 0.2), (float)(this.Width + 0.2), (float)this.FloorThickness);
                y0 += this.FloorThickness;
            }

            wallBuilder.AddBox(new Point3D(0, 0, y0 + this.StoryHeight / 2).ToVector(), (float)this.Length, (float)this.Width, (float)this.StoryHeight);
            y0 += this.StoryHeight;
        }

        var theta = Math.PI / 180 * this.RoofAngle;
        var roofBuilder = new MeshBuilder(false, false);
        var y1 = y0 + Math.Tan(theta) * this.Width / 2;
        var p0 = new Point(0, y1);
        var p1 = new Point(this.Width / 2 + 0.2 * Math.Cos(theta), y0 - 0.2 * Math.Sin(theta));
        var p2 = new Point(p1.X + this.RoofThickness * Math.Sin(theta), p1.Y + this.RoofThickness * Math.Cos(theta));
        var p3 = new Point(0, y1 + this.RoofThickness / Math.Cos(theta));
        var p4 = new Point(-p2.X, p2.Y);
        var p5 = new Point(-p1.X, p1.Y);
        var roofSection = new[] { p0.ToVector(), p1.ToVector(), p1.ToVector(), p2.ToVector(), p2.ToVector(), p3.ToVector(), p3.ToVector(), p4.ToVector(), p4.ToVector(), p5.ToVector(), p5.ToVector(), p0.ToVector() };
        roofBuilder.AddExtrudedSegments(roofSection, new Vector3D(0, -1, 0).ToVector(), new Point3D(-this.Length / 2, 0, 0).ToVector(), new Point3D(this.Length / 2, 0, 0).ToVector());
        var cap = new[] { p0.ToVector(), p1.ToVector(), p2.ToVector(), p3.ToVector(), p4.ToVector(), p5.ToVector() };
        roofBuilder.AddPolygon(cap, new Vector3D(0, -1, 0).ToVector(), new Vector3D(0, 0, 1).ToVector(), new Point3D(-this.Length / 2, 0, 0).ToVector());
        roofBuilder.AddPolygon(cap, new Vector3D(0, 1, 0).ToVector(), new Vector3D(0, 0, 1).ToVector(), new Point3D(this.Length / 2, 0, 0).ToVector());
        var p6 = new Point(this.Width / 2, y0);
        var p7 = new Point(-this.Width / 2, y0);
        wallBuilder.AddPolygon(new[] { p0.ToVector(), p6.ToVector(), p7.ToVector() }, new Vector3D(0, -1, 0).ToVector(), new Vector3D(0, 0, 1).ToVector(), new Point3D(-this.Length / 2, 0, 0).ToVector());
        wallBuilder.AddPolygon(new[] { p0.ToVector(), p6.ToVector(), p7.ToVector() }, new Vector3D(0, 1, 0).ToVector(), new Vector3D(0, 0, 1).ToVector(), new Point3D(this.Length / 2, 0, 0).ToVector());
        this.walls.Geometry = wallBuilder.ToMesh().ToMeshGeometry3D(true);
        this.roof.Geometry = roofBuilder.ToMesh().ToMeshGeometry3D(true);
    }
}
