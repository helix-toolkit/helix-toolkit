using HelixToolkit.Geometry;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Windows;

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
        p.Points.Add(new Vector3(0, 0, 0));
        p.Points.Add(new Vector3(1, 0, 0));
        p.Points.Add(new Vector3(1, 1, 0.76f));
        p.Points.Add(new Vector3(0, 1, 0.76f));
        Assert.That(p.IsPlanar());
    }

    [Test]
    public void IsPlanar_NotPlanarPolygon_ReturnsFalse()
    {
        var p = new Polygon3D();
        p.Points.Add(new Vector3(0, 0, 0));
        p.Points.Add(new Vector3(1, 0, 0));
        p.Points.Add(new Vector3(1, 1, 0));
        p.Points.Add(new Vector3(0, 1, 0.3f));
        Assert.That(p.IsPlanar(), Is.False);
    }

    [Test]
    public void GetNormal_PlanarPolygon_ReturnsCorrectResult()
    {
        var p = new Polygon3D();
        p.Points.Add(new Vector3(0, 0, 0));
        p.Points.Add(new Vector3(1, 0, 0));
        p.Points.Add(new Vector3(1, 1, 0));
        p.Points.Add(new Vector3(0, 1, 0));
        var n = p.GetNormal();
        Assert.That(n, Is.EqualTo(new Vector3(0, 0, 1)));
    }

    [Test]
    public void Flatten_PlanarPolygon_ReturnsCorrectResult()
    {
        var p = new Polygon3D();
        p.Points.Add(new Vector3(0, 0, 4));
        p.Points.Add(new Vector3(1, 0, 4));
        p.Points.Add(new Vector3(1, 1, 4));
        p.Points.Add(new Vector3(0, 1, 4));
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
        p.Points.Add(new Vector3(0, 0, 4));
        p.Points.Add(new Vector3(1, 0, 4));
        p.Points.Add(new Vector3(1, 1, 4.01f));
        p.Points.Add(new Vector3(0, 1, 4.01f));
        var p2 = p.Flatten();
        Assert.AreEqual(4, p2.Points.Count);
        var tri = p2.Triangulate();
        Assert.That(tri, Is.Not.Null);
        Assert.AreEqual(6, tri.Count);
    }

    [Test]
    public void Polygon_GetNormal_NotThrows()
    {
        var points = new List<Vector3>()
            {
                new(-1.39943f, 0.328622f, 0.97968f),
                new(-1.39969f, 0.328622f, 0.99105f),
                new(-1.39963f, 0.328631f, 0.99105f),
                new(-1.39954f, 0.328631f, 0.98726f),
                new(-1.39937f, 0.328631f, 0.97968f),
            };

        var polygon = new Polygon3D(points);
        var normal = polygon.GetNormal();
    }
}
