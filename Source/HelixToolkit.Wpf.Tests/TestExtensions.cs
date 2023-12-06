using NUnit.Framework;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace HelixToolkit.Wpf.Tests;

public static class TestExtensions
{
    public static void AssertContains(this PointCollection collection, params double[][] points)
    {
        Assert.AreEqual(points.Length, collection.Count, "Expected to find {0} points in collection", points.Length);
        foreach (var point in points)
            Assert.That(collection, Does.Contain(new Point(point[0], point[1])), "Expected collection to contain point [{0},{1}]", point[0], point[1]);
    }

    public static void AssertContains(this Vector3DCollection collection, params double[][] points)
    {
        Assert.AreEqual(points.Length, collection.Count, "Expected to find {0} points in collection", points.Length);
        foreach (var point in points)
            Assert.That(collection, Does.Contain(new Vector3D(point[0], point[1], point[2])), "Expected collection to contain point [{0},{1},{2}]", point[0], point[1], point[2]);
    }

    public static void AssertContains(this Point3DCollection collection, params double[][] points)
    {
        Assert.AreEqual(points.Length, collection.Count, "Expected to find {0} points in collection", points.Length);
        foreach (var point in points)
            Assert.That(collection, Does.Contain(new Point3D(point[0], point[1], point[2])), "Expected collection to contain point [{0},{1},{2}]", point[0], point[1], point[2]);
    }

    public static MeshGeometry3D GetMesh(this Model3D model)
    {
        var geometryModel = (GeometryModel3D)model;
        return (MeshGeometry3D)geometryModel.Geometry;
    }
}
