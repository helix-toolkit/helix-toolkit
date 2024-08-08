using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests;

public static class Model3DTestExtensions
{
    public static void AssertHasVertices(this Model3D model, params double[][] vertices)
    {
        var geometryModel = (GeometryModel3D)model;
        var geometry = (MeshGeometry3D)geometryModel.Geometry;
        ClassicAssert.AreEqual(vertices.Length, geometry.Positions.Count, "Expected to find {0} vertices in model", vertices.Length);
        foreach (var vertex in vertices)
            Assert.That(geometry.Positions, Does.Contain(new Point3D(vertex[0], vertex[1], vertex[2])), $"Expected geometry to contain vertex [{vertex[0]},{vertex[1]},{vertex[2]}]");
    }
}
