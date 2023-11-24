using NUnit.Framework;
using System.Reflection;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.Geometry;

[TestFixture]
public class BoundingSphereTests
{
    [Test]
    public void RayInterSection_OutsideSpherePointingToCenter_TwoIntersections()
    {
        var sphere = new BoundingSphere(new Point3D(0.2, 0.3, 0), 1.0);
        var p = new Point3D(12, 23, 32);
        var ray = new Ray3D(p, sphere.Center - p);

        Assert.That(sphere.RayIntersection(ray, out Point3D[]? result));
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(sphere.Radius, sphere.Center.DistanceTo(result[0]), 1e-6, "Point 1 is not on sphere.");
        Assert.AreEqual(0, ray.GetNearest(result[0]).DistanceTo(result[0]), 1e-6, "Point 1" + result[0] + " is not on ray.");
        Assert.AreEqual(sphere.Radius, sphere.Center.DistanceTo(result[1]), 1e-6, "Point 2 is not on sphere.");
        Assert.AreEqual(0, ray.GetNearest(result[1]).DistanceTo(result[1]), 1e-6, "Point 2 " + result[0] + " is not on ray.");
        Assert.That(ray.Origin.DistanceTo(result[0]), Is.LessThan(ray.Origin.DistanceTo(result[1])), "The points should be sorted by distance from ray origin.");
    }

    [Test]
    public void RayInterSection_InsideSpherePointingOut_OneIntersection()
    {
        var sphere = new BoundingSphere(new Point3D(0.2, 0.3, 0), 2.0);
        var ray = new Ray3D(sphere.Center + new Vector3D(0.1, 0.08, 0.03), new Vector3D(1, 0.99, 0.87));
        Assert.That(sphere.RayIntersection(ray, out Point3D[]? result));
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(1, result.Length, "Number of intersections should be 1");
        Assert.AreEqual(sphere.Radius, sphere.Center.DistanceTo(result[0]), 1e-6, "Point is not on sphere.");
        Assert.AreEqual(0, ray.GetNearest(result[0]).DistanceTo(result[0]), 1e-6, "Point is not on ray.");
    }

    [Test]
    public void RayInterSection_OnSpherePointingOut_OneIntersection()
    {
        var sphere = new BoundingSphere(new Point3D(0.2, 0.3, 0), 2.0);
        var ray = new Ray3D(sphere.Center + new Vector3D(0, 0, sphere.Radius), new Vector3D(1, 0.99, 0.87));
        Assert.That(sphere.RayIntersection(ray, out Point3D[]? result));
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(1, result.Length, "Number of intersections should be 1");
        Assert.AreEqual(sphere.Radius, sphere.Center.DistanceTo(result[0]), 1e-6, "Point is not on sphere.");
        Assert.AreEqual(0, ray.GetNearest(result[0]).DistanceTo(result[0]), 1e-6, "Point is not on ray.");
    }

    [Test]
    public void RayInterSection_OnSpherePointingIn_OneIntersection()
    {
        var sphere = new BoundingSphere(new Point3D(0.2, 0.3, 0), 2.0);
        var ray = new Ray3D(sphere.Center + new Vector3D(0, 0, sphere.Radius), new Vector3D(0.01, 0.09, -0.87));
        Assert.That(sphere.RayIntersection(ray, out Point3D[]? result));
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(2, result.Length, "Number of intersections should be 1");
        Assert.AreEqual(sphere.Radius, sphere.Center.DistanceTo(result[0]), 1e-6, "Point 1 is not on sphere.");
        Assert.AreEqual(0, ray.GetNearest(result[0]).DistanceTo(result[0]), 1e-6, "Point 1 is not on ray.");
        Assert.AreEqual(sphere.Radius, sphere.Center.DistanceTo(result[1]), 1e-6, "Point 2 is not on sphere.");
        Assert.AreEqual(0, ray.GetNearest(result[1]).DistanceTo(result[1]), 1e-6, "Point 2 is not on ray.");
    }

    [Test]
    public void RayInterSection_OutsideSpherePointingAway_NoIntersection()
    {
        var sphere = new BoundingSphere(new Point3D(0.2, 0.3, 0), 1.0);
        var ray = new Ray3D(sphere.Center + new Vector3D(0.2, 0.3, sphere.Radius), new Vector3D(1, 0.1, 0.1));
        Assert.That(sphere.RayIntersection(ray, out Point3D[]? result), Is.False);
    }

    [Test]
    public void RayInterSection_OutsideAndTangentToSphere_OneIntersection()
    {
        var sphere = new BoundingSphere(new Point3D(0.2, 0.3, 0), 1.0);
        var p = new Point3D(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + 50);
        var p2 = new Point3D(sphere.Center.X + sphere.Radius, sphere.Center.Y, sphere.Center.Z);

        // ray tangent to sphere
        var ray = new Ray3D(p, p2 - p);
        Assert.That(sphere.RayIntersection(ray, out Point3D[]? result), "No intersection");
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(1, result.Length, "One intersection");
        Assert.AreEqual(sphere.Radius, result[0].DistanceTo(sphere.Center), 1e-8);
        Assert.AreEqual(0, ray.GetNearest(result[0]).DistanceTo(result[0]), 1e-6, "Point 1 is not on ray.");
    }
}
