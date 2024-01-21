using HelixToolkit;
using HelixToolkit.Wpf;
using PropertyTools.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Numerics;
using BrowsableAttribute = System.ComponentModel.BrowsableAttribute;

namespace Building;

public sealed class SiloVisual3D : UIElement3D
{
    public static readonly DependencyProperty DiameterProperty = DependencyPropertyEx.Register<double, SiloVisual3D>("Diameter", 40, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty HeightProperty = DependencyPropertyEx.Register<double, SiloVisual3D>("Height", 20, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty DomeHeightProperty = DependencyPropertyEx.Register<double, SiloVisual3D>("DomeHeight", 8, (s, e) => s.AppearanceChanged());
    public static readonly DependencyProperty DomeDiameterProperty = DependencyPropertyEx.Register<double, SiloVisual3D>("DomeDiameter", 38, (s, e) => s.AppearanceChanged());

    private readonly GeometryModel3D walls = new();
    private readonly GeometryModel3D railing = new();
    private readonly GeometryModel3D stairs = new();

    public SiloVisual3D()
    {
        this.walls.Material = MaterialHelper.CreateMaterial(Brushes.White, ambient: 10);
        this.walls.BackMaterial = MaterialHelper.CreateMaterial(Brushes.Green);
        this.railing.Material = MaterialHelper.CreateMaterial(Brushes.Silver, ambient: 10);
        this.stairs.Material = MaterialHelper.CreateMaterial(Brushes.Brown, ambient: 10);
        this.AppearanceChanged();
        var group = new Model3DGroup();
        group.Children.Add(this.walls);
        group.Children.Add(this.railing);
        group.Children.Add(this.stairs);
        this.Visual3DModel = group;
    }

    [Category("Silo/tank properties")]
    [Slidable(0, 100)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double Diameter
    {
        get { return (double)this.GetValue(DiameterProperty); }
        set { this.SetValue(DiameterProperty, value); }
    }

    [Slidable(0, 100)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double Height
    {
        get { return (double)this.GetValue(HeightProperty); }
        set { this.SetValue(HeightProperty, value); }
    }

    [Slidable(0, 20)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double DomeHeight
    {
        get { return (double)this.GetValue(DomeHeightProperty); }
        set { this.SetValue(DomeHeightProperty, value); }
    }

    [Slidable(0, 50)]
    [FormatString("0.00")]
    [Browsable(true)]
    public double DomeDiameter
    {
        get { return (double)this.GetValue(DomeDiameterProperty); }
        set { this.SetValue(DomeDiameterProperty, value); }
    }

    private void AppearanceChanged()
    {
        var builder = new MeshBuilder(false, false);
        var p0 = new Point(0, 0);
        var p1 = new Point(this.Diameter / 2, 0);
        var p2 = new Point(this.Diameter / 2, this.Height);
        var p3 = new Point(this.DomeDiameter / 2, this.Height);

        var section = new List<Point> { p0, p1, p1, p2, p2, p3 };
        var sectionIndices = new List<int> { 0, 1, 2, 3, 4, 5 };
        int n = 40;
        for (int i = n; i >= 0; i--)
        {
            double x = (double)i / n;
            double y = x * x;
            if (i < n)
            {
                sectionIndices.Add(section.Count - 1);
                sectionIndices.Add(section.Count);
            }

            section.Add(new Point(x * this.DomeDiameter / 2, this.Height + (this.DomeHeight * (1 - y))));
        }

        builder.AddSurfaceOfRevolution(new Vector3(0, 0, 0), new Vector3(0, 0, 1), section.ToVector2Collection()!, sectionIndices, 80);
        this.walls.Geometry = builder.ToMesh().ToWndMeshGeometry3D(true);

        var treadDepth = 0.3;
        var riseHeight = 0.15;
        var thickness = 0.05;
        var width = 1;
        var steps = (int)(this.Height / riseHeight);
        var r = (this.Diameter * 0.5) + (width * 0.5);
        var rp = (this.Diameter * 0.5) + (width * 0.95);
        var stairBuilder = new MeshBuilder(false, false);
        var railBases = new List<Point3D>();
        for (int i = 0; i < steps; i++)
        {
            var theta = treadDepth * i / r;
            var p = new Point3D(Math.Cos(theta) * r, Math.Sin(theta) * r, (riseHeight * i) + (thickness / 2));
            var x = new Vector3D(Math.Cos(theta), Math.Sin(theta), 0);
            var z = new Vector3D(0, 0, 1);
            var y = Vector3D.CrossProduct(z, x);
            stairBuilder.AddBox(p.ToVector3(), x.ToVector3(), y.ToVector3(), width, (float)treadDepth, (float)thickness);
            railBases.Add(new Point3D(Math.Cos(theta) * rp, Math.Sin(theta) * rp, (riseHeight * i) + thickness));
        }

        var lastTheta = treadDepth * steps / r;

        // Railing along stairs
        var railingHeight = 0.8;
        var railingDiameter = 0.05;
        var railings = 3;
        var railingBuilder = new MeshBuilder(false, false);

        // Top railing
        var railingPostDistance = 0.5;
        int topRailingPosts = (int)(this.Diameter * Math.PI / railingPostDistance);
        var tr = (this.Diameter / 2) - railingDiameter;
        for (int i = 0; i < topRailingPosts; i++)
        {
            var theta = lastTheta + (2 * Math.PI * i / topRailingPosts);
            railBases.Add(new Point3D(Math.Cos(theta) * tr, Math.Sin(theta) * tr, this.Height));
        }

        BuildRailing(railingBuilder, railBases, railingHeight, railingDiameter, railings);

        this.stairs.Geometry = stairBuilder.ToMesh().ToWndMeshGeometry3D();
        this.railing.Geometry = railingBuilder.ToMesh().ToWndMeshGeometry3D();
    }

    private static void BuildRailing(
        MeshBuilder railingBuilder,
        List<Point3D> bases,
        double height,
        double diameter,
        int railings)
    {
        foreach (var point in bases)
        {
            railingBuilder.AddCylinder(point.ToVector3(), (point.ToVector3() + new Vector3(0, 0, (float)height)), (float)diameter, 10);
        }

        for (int i = 1; i <= railings; i++)
        {
            var h = height * i / railings;
            var path = bases.Select(p => (p.ToVector3() + new Vector3(0, 0, (float)h))).ToArray();
            railingBuilder.AddTube(path, (float)diameter, 10, false);
        }
    }
}
