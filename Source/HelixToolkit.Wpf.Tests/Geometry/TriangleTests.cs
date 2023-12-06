using NUnit.Framework;
using System.Windows;

namespace HelixToolkit.Wpf.Tests.Geometry;

[TestFixture]
public class TriangleTests
{
    [Test]
    public void IsCompletelyInside()
    {
        var triangle = new Triangle(new Point(0, 0), new Point(2, 0), new Point(0, 3));

        Assert.That(triangle.IsCompletelyInside(new Rect(0, 0, 2, 2)), Is.False, "partly inside");
        Assert.That(triangle.IsCompletelyInside(new Rect(0, 0, 2, 4)), "completely inside");
        Assert.That(triangle.IsCompletelyInside(new Rect(0, 0, 2, 3)), "completely inside");
        Assert.That(triangle.IsCompletelyInside(new Rect(0, 0, 0.25, 0.25)), Is.False, "not inside");
        Assert.That(triangle.IsCompletelyInside(new Rect(-1, -1, 5, 5)), "completely inside");
    }

    [Test]
    public void IsPointInside()
    {
        var triangle = new Triangle(new Point(0, 0), new Point(2, 0), new Point(0, 3));

        Assert.That(triangle.IsPointInside(new Point(1, 1)), "inside");
        Assert.That(triangle.IsPointInside(new Point(2, 3)), Is.False, "outside");
        Assert.That(triangle.IsPointInside(new Point(1, 0)), Is.False, "on edge");
        Assert.That(triangle.IsPointInside(new Point(0, 0)), Is.False, "on vertex");
    }

    [Test]
    public void IsPointInsideDegenerateTriangle()
    {
        // collinear points
        var triangle = new Triangle(new Point(0, 0), new Point(2, 0), new Point(3, 0));
        Assert.That(triangle.IsPointInside(new Point(1, 0)), Is.False, "on edge");
        Assert.That(triangle.IsPointInside(new Point(0, 3)), Is.False, "outside");
    }

    [Test]
    public void IsRectInside()
    {
        var triangle = new Triangle(new Point(0, 0), new Point(2, 0), new Point(0, 3));
        Assert.That(triangle.IsRectCompletelyInside(new Rect(1e-10, 1e-10, 1, 1)), "completely inside");
        Assert.That(triangle.IsRectCompletelyInside(new Rect(0, 0, 1, 1)), Is.False, "touching rectangle");
        Assert.That(triangle.IsRectCompletelyInside(new Rect(0, 0, 2, 2)), Is.False, "partly outside");
        Assert.That(triangle.IsRectCompletelyInside(new Rect(2, 2, 1, 1)), Is.False, "completely outside");
    }
}
