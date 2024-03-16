using HelixToolkit.Geometry;
using NUnit.Framework;
using System.Numerics;
using System.Windows.Media;

namespace HelixToolkit.Wpf.Tests.Geometry;

[TestFixture]
public class PolygonTriangulationTests
{
    [Test]
    public void TestBaseCaseTriangle()
    {
        List<Vector2> triangleInPositiveOrientation = new()
        {
            new(0, 0), new(1, 0), new(0, 1)
        };

        Int32Collection indices = ConverterExtensions.ToWndInt32Collection(SweepLinePolygonTriangulator.Triangulate(triangleInPositiveOrientation)) ?? new();
        Assert.That(indices.Count == 3);
        Assert.That(indices[0] == 0);
        Assert.That(indices[1] == 1);
        Assert.That(indices[2] == 2);


        List<Vector2> triangleInNegativeOrientation = new()
                {
                    new(0, 0), new(0, 1),  new(1, 0)
                };

        indices = ConverterExtensions.ToWndInt32Collection(SweepLinePolygonTriangulator.Triangulate(triangleInNegativeOrientation)) ?? new();
        Assert.That(indices.Count == 3);
        Assert.That(indices[0] == 0);
        Assert.That(indices[1] == 2);
        Assert.That(indices[2] == 1);
    }
}
