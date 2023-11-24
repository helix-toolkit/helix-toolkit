using NUnit.Framework;
using System.Windows;
using System.Windows.Media;

namespace HelixToolkit.Wpf.Tests.Geometry;

[TestFixture]
public class PolygonTriangulationTests
{
    [Test]
    public void TestBaseCaseTriangle()
    {
        List<Point> triangleInPositiveOrientation = new()
        {
            new Point(0, 0), new Point(1, 0), new Point(0, 1)
        };

        Int32Collection indices = Converter.ToInt32Collection(SweepLinePolygonTriangulator.Triangulate(triangleInPositiveOrientation.Select(t => t.ToVector()).ToList())) ?? new();
        Assert.That(indices.Count == 3);
        Assert.That(indices[0] == 0);
        Assert.That(indices[1] == 1);
        Assert.That(indices[2] == 2);


        List<Point> triangleInNegativeOrientation = new List<Point>()
                {
                    new Point(0, 0), new Point(0, 1),  new Point(1, 0)
                };

        indices = Converter.ToInt32Collection(SweepLinePolygonTriangulator.Triangulate(triangleInNegativeOrientation.Select(t => t.ToVector()).ToList())) ?? new();
        Assert.That(indices.Count == 3);
        Assert.That(indices[0] == 0);
        Assert.That(indices[1] == 2);
        Assert.That(indices[2] == 1);
    }
}
