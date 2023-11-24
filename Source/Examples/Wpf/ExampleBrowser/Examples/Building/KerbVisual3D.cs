using HelixToolkit;
using HelixToolkit.Wpf;
using PropertyTools.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BrowsableAttribute = System.ComponentModel.BrowsableAttribute;

namespace Building;

public sealed class KerbVisual3D : UIElement3D
{
    public static readonly DependencyProperty WidthProperty = DependencyPropertyEx.Register<double, KerbVisual3D>("Width", 0.2, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty HeightProperty = DependencyPropertyEx.Register<double, KerbVisual3D>("Height", 0.1, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty PositionsProperty = DependencyPropertyEx.Register<Point3DCollection, KerbVisual3D>("Positions", new Point3DCollection(), (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty LengthProperty = DependencyPropertyEx.Register<double, KerbVisual3D>("Length", 1, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty TextureProperty = DependencyPropertyEx.Register<Brush?, KerbVisual3D>("Texture", null, (s, e) => s.TextureChanged());

    private readonly GeometryModel3D kerbModel = new();

    public KerbVisual3D()
    {
        this.AppearanceChanged();
        this.Visual3DModel = this.kerbModel;
    }

    [Category("Kerb properties")]
    [Slidable(0.1, 1)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double Width
    {
        get { return (double)this.GetValue(WidthProperty); }
        set { this.SetValue(WidthProperty, value); }
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

    [Slidable(0.1, 10)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double Length
    {
        get { return (double)this.GetValue(LengthProperty); }
        set { this.SetValue(LengthProperty, value); }
    }

    public Brush Texture
    {
        get { return (Brush)this.GetValue(TextureProperty); }
        set { this.SetValue(TextureProperty, value); }
    }

    private void TextureChanged()
    {
        this.kerbModel.Material = this.Texture != null ? new DiffuseMaterial(this.Texture) : null;
        this.kerbModel.BackMaterial = this.Texture != null ? Materials.Gray : null;
    }

    private void AppearanceChanged()
    {
        var builder = new MeshBuilder(false, true);

        // hard code a kerb section
        var section = new PointCollection();
        int m = 41;
        double n = 4;
        double a = this.Width / 2;
        double b = this.Height;
        for (int i = 0; i < m; i++)
        {
            double t = Math.PI * i / (m - 1);
            section.Add(new Point(
                a * Math.Sign(Math.Cos(t)) * Math.Pow(Math.Abs(Math.Cos(t)), 2 / n),
                -b * Math.Sign(Math.Sin(t)) * Math.Pow(Math.Abs(Math.Sin(t)), 2 / n)));
        }

        // calculate the texture coordinates
        var values = new List<float> { 0 };
        for (int i = 1; i < this.Positions.Count; i++)
        {
            var d = this.Positions[i - 1].DistanceTo(this.Positions[i]);
            values.Add((float)(values[^1] + (d / this.Length)));
        }

        // create the extruded geometry
        builder.AddTube(this.Positions.ToCollection()!, null, values, null, section.ToCollection(), new Vector3D(1, 0, 0).ToVector(), false, false);

        this.kerbModel.Geometry = builder.ToMesh().ToMeshGeometry3D();
    }
}
