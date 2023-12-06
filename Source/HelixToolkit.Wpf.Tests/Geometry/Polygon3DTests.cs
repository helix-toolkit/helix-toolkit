using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.Geometry;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class Polygon3DTests
{
    [Test]
    public void IsPlanar_PlanarPolygon_ReturnsTrue()
    {
        var p = new Polygon3D();
        p.Points.Add(new Point3D(0, 0, 0).ToVector());
        p.Points.Add(new Point3D(1, 0, 0).ToVector());
        p.Points.Add(new Point3D(1, 1, 0.76).ToVector());
        p.Points.Add(new Point3D(0, 1, 0.76).ToVector());
        Assert.That(p.IsPlanar());
    }

    [Test]
    public void IsPlanar_NotPlanarPolygon_ReturnsFalse()
    {
        var p = new Polygon3D();
        p.Points.Add(new Point3D(0, 0, 0).ToVector());
        p.Points.Add(new Point3D(1, 0, 0).ToVector());
        p.Points.Add(new Point3D(1, 1, 0).ToVector());
        p.Points.Add(new Point3D(0, 1, 0.3).ToVector());
        Assert.That(p.IsPlanar(), Is.False);
    }

    [Test]
    public void GetNormal_PlanarPolygon_ReturnsCorrectResult()
    {
        var p = new Polygon3D();
        p.Points.Add(new Point3D(0, 0, 0).ToVector());
        p.Points.Add(new Point3D(1, 0, 0).ToVector());
        p.Points.Add(new Point3D(1, 1, 0).ToVector());
        p.Points.Add(new Point3D(0, 1, 0).ToVector());
        var n = p.GetNormal().ToWndVector();
        Assert.That(n, Is.EqualTo(new Vector3D(0, 0, 1)));
    }

    [Test]
    public void Flatten_PlanarPolygon_ReturnsCorrectResult()
    {
        var p = new Polygon3D();
        p.Points.Add(new Point3D(0, 0, 4).ToVector());
        p.Points.Add(new Point3D(1, 0, 4).ToVector());
        p.Points.Add(new Point3D(1, 1, 4).ToVector());
        p.Points.Add(new Point3D(0, 1, 4).ToVector());
        var p2 = p.Flatten();
        Assert.AreEqual(4, p2.Points.Count);
        Assert.AreEqual(new Point(0, 0), p2.Points[0].ToWndPoint());
        Assert.AreEqual(new Point(1, 0), p2.Points[1].ToWndPoint());
        Assert.AreEqual(new Point(1, 1), p2.Points[2].ToWndPoint());
        Assert.AreEqual(new Point(0, 1), p2.Points[3].ToWndPoint());
        var tri = p2.Triangulate();
        Assert.That(tri, Is.Not.Null);
        Assert.AreEqual(6, tri.Count);
    }

    [Test]
    public void Flatten_PlanarPolygon2_ReturnsCorrectResult()
    {
        var p = new Polygon3D();
        p.Points.Add(new Point3D(0, 0, 4).ToVector());
        p.Points.Add(new Point3D(1, 0, 4).ToVector());
        p.Points.Add(new Point3D(1, 1, 4.01).ToVector());
        p.Points.Add(new Point3D(0, 1, 4.01).ToVector());
        var p2 = p.Flatten();
        Assert.AreEqual(4, p2.Points.Count);
        var tri = p2.Triangulate();
        Assert.That(tri, Is.Not.Null);
        Assert.AreEqual(6, tri.Count);
    }

    [Test]
    public void Polygon_GetNormal_NotThrows()
    {
        var points = new List<Point3D>()
            {
                new Point3D(-1.39943, 0.328622, 0.97968),
                new Point3D(-1.39969, 0.328622, 0.99105),
                new Point3D(-1.39963, 0.328631, 0.99105),
                new Point3D(-1.39954, 0.328631, 0.98726),
                new Point3D(-1.39937, 0.328631, 0.97968),
            };

        var polygon = new Polygon3D(points.Select(t => t.ToVector()).ToList());
        Vector3D normal = polygon.GetNormal().ToWndVector();
    }
}
