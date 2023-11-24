using NUnit.Framework;
using System.Windows;

namespace HelixToolkit.Wpf.Tests.Geometry;

[TestFixture]
public class LineSegmentTests
{
    [Test]
    public void IntersectsWith()
    {
        var line1 = new LineSegment(new Point(0, 0), new Point(1, 0));
        var line2 = new LineSegment(new Point(1, 1), new Point(2, 1));
        Assert.That(line1.IntersectsWith(line2), Is.False, "collinear");
        var line3 = new LineSegment(new Point(1, 1), new Point(0, 0));
        Assert.That(line1.IntersectsWith(line3), "intersecting");
        var line4 = new LineSegment(new Point(1, 1), new Point(-1, 0));
        Assert.That(line1.IntersectsWith(line4), Is.False, "intersection is outside segment");
    }

    [Test]
    public void IntersectTest()
    {
        var a1 = new Point(0, 0);
        var a2 = new Point(1, 0);
        var b1 = new Point(1, 1);
        var b2 = new Point(2, 1);

        Assert.That(LineSegment.AreLineSegmentsIntersecting(a1, a2, b1, b2), Is.False);
        b2 = new Point(0, 0);
        Assert.That(LineSegment.AreLineSegmentsIntersecting(a1, a2, b1, b2));
        b2 = new Point(-1, 0);
        Assert.That(LineSegment.AreLineSegmentsIntersecting(a1, a2, b1, b2), Is.False);
    }
}
