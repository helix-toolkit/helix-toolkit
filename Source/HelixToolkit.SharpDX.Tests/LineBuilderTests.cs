using NUnit.Framework;
using System.Numerics;
using HelixToolkit.Maths;

namespace HelixToolkit.SharpDX.Tests;

[TestFixture]
public class LineBuilderTests
{
    private const float Tolerance = 1e-4f;

    [Test]
    public void GetRayToLineDistanceShouldNotClampRayWhenNearestPointIsLineEnd()
    {
        // A short segment far along the ray, beside it: the nearest point on the segment is its end at x = 1.
        var ray = new Ray(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f));

        var distance = LineBuilder.GetRayToLineDistance(ray, new Vector3(1f, 0f, 100f), new Vector3(2f, 0f, 100f), out var sp, out var tp, out var sc, out var tc);

        Assert.That(distance, Is.EqualTo(1f).Within(Tolerance));
        Assert.That(sc, Is.EqualTo(100f).Within(Tolerance));
        Assert.That(tc, Is.EqualTo(0f).Within(Tolerance));
        AssertEqual(new Vector3(0f, 0f, 100f), sp);
        AssertEqual(new Vector3(1f, 0f, 100f), tp);
    }

    [Test]
    public void GetRayToLineDistanceShouldNotClampRayWhenNearestPointIsOtherLineEnd()
    {
        var ray = new Ray(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f));

        var distance = LineBuilder.GetRayToLineDistance(ray, new Vector3(-2f, 0f, 100f), new Vector3(-1f, 0f, 100f), out var sp, out var tp, out var sc, out var tc);

        Assert.That(distance, Is.EqualTo(1f).Within(Tolerance));
        Assert.That(sc, Is.EqualTo(100f).Within(Tolerance));
        Assert.That(tc, Is.EqualTo(1f).Within(Tolerance));
        AssertEqual(new Vector3(0f, 0f, 100f), sp);
        AssertEqual(new Vector3(-1f, 0f, 100f), tp);
    }

    [Test]
    public void GetRayToLineDistanceShouldFindCrossingInsideLine()
    {
        var ray = new Ray(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f));

        var distance = LineBuilder.GetRayToLineDistance(ray, new Vector3(-1f, 0f, 50f), new Vector3(1f, 0f, 50f), out var sp, out _, out var sc, out var tc);

        Assert.That(distance, Is.EqualTo(0f).Within(Tolerance));
        Assert.That(sc, Is.EqualTo(50f).Within(Tolerance));
        Assert.That(tc, Is.EqualTo(0.5f).Within(Tolerance));
        AssertEqual(new Vector3(0f, 0f, 50f), sp);
    }

    [Test]
    public void GetRayToLineDistanceShouldClampToRayStartWhenLineIsBehindIt()
    {
        var ray = new Ray(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f));

        LineBuilder.GetRayToLineDistance(ray, new Vector3(-1f, 0f, -10f), new Vector3(1f, 0f, -10f), out var sp, out var tp, out var sc, out _);

        Assert.That(sc, Is.EqualTo(0f).Within(Tolerance));
        AssertEqual(new Vector3(0f, 0f, 0f), sp);
        AssertEqual(new Vector3(0f, 0f, -10f), tp);
    }

    [Test]
    public void GetLineToLineDistanceShouldStillClampSegmentEnd()
    {
        // The same geometry as the first test, but s is a segment of length 1: its end is the nearest point.
        var distance = LineBuilder.GetLineToLineDistance(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(1f, 0f, 100f), new Vector3(2f, 0f, 100f),
            out var sp, out var tp, out var sc, out _);

        Assert.That(sc, Is.EqualTo(1f).Within(Tolerance));
        AssertEqual(new Vector3(0f, 0f, 1f), sp);
        AssertEqual(new Vector3(1f, 0f, 100f), tp);
        Assert.That(distance, Is.EqualTo(Vector3.Distance(sp, tp)).Within(Tolerance));
    }

    private static void AssertEqual(Vector3 expected, Vector3 actual)
    {
        Assert.That(Vector3.Distance(expected, actual), Is.LessThan(Tolerance), $"expected {expected}, actual {actual}");
    }
}
