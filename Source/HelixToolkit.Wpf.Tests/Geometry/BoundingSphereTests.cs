using HelixToolkit.Maths;
using NUnit.Framework;
using System.Numerics;
using System.Reflection;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.Geometry;

[TestFixture]
public class BoundingSphereTests
{
    [Test]
    public void RayInterSection_OutsideSpherePointingToCenter_TwoIntersections()
    {
        var sphere = new BoundingSphere(new Vector3(0.2f, 0.3f, 0f), 1.0f);
        var p = new Vector3(12, 23, 32);
        var ray = new Ray(p, sphere.Center - p);
        Assert.That(sphere.Intersects(ray, out Vector3[]? result));
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(sphere.Radius, Vector3.Distance(sphere.Center, result[0]), 1e-5, "Point 1 is not on sphere.");
        Assert.AreEqual(0, Vector3.Distance(ray.GetNearest(result[0]), result[0]), 1e-6, "Point 1" + result[0] + " is not on ray.");
        Assert.AreEqual(sphere.Radius, Vector3.Distance(sphere.Center, result[1]), 1e-5, "Point 2 is not on sphere.");
        Assert.AreEqual(0, Vector3.Distance(ray.GetNearest(result[1]), result[1]), 1e-6, "Point 2 " + result[1] + " is not on ray.");
        Assert.That(Vector3.Distance(ray.Position, result[0]), Is.LessThan(Vector3.Distance(ray.Position, result[1])), "The points should be sorted by distance from ray origin.");
    }

    [Test]
    public void RayInterSection_InsideSpherePointingOut_OneIntersection()
    {
        var sphere = new BoundingSphere(new Vector3(0.2f, 0.3f, 0f), 2.0f);
        var ray = new Ray(sphere.Center + new Vector3(0.1f, 0.08f, 0.03f), new Vector3(1, 0.99f, 0.87f));
        Assert.That(sphere.Intersects(ray, out Vector3[]? result));
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(1, result.Length, "Number of intersections should be 1");
        Assert.AreEqual(sphere.Radius, Vector3.Distance(sphere.Center, result[0]), 1e-6, "Point is not on sphere.");
        Assert.AreEqual(0, ray.ToRay3D().GetNearest(result[0].ToWndPoint3D()).DistanceTo(result[0].ToWndPoint3D()), 1e-6, "Point is not on ray.");
    }

    [Test]
    public void RayInterSection_OnSpherePointingOut_OneIntersection()
    {
        var sphere = new BoundingSphere(new Vector3(0.2f, 0.3f, 0), 2.0f);
        var ray = new Ray(sphere.Center + new Vector3(0f, 0f, sphere.Radius), new Vector3(1f, 0.99f, 0.87f));
        Assert.That(sphere.Intersects(ray, out Vector3[]? result));
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(1, result.Length, "Number of intersections should be 1");
        Assert.AreEqual(sphere.Radius, Vector3.Distance(sphere.Center, result[0]), 1e-6, "Point is not on sphere.");
        Assert.AreEqual(0, ray.ToRay3D().GetNearest(result[0].ToWndPoint3D()).DistanceTo(result[0].ToWndPoint3D()), 1e-6, "Point is not on ray.");
    }

    [Test]
    public void RayInterSection_OnSpherePointingIn_OneIntersection()
    {
        var sphere = new BoundingSphere(new Vector3(0.2f, 0.3f, 0f), 2.0f);
        var ray = new Ray(sphere.Center + new Vector3(0f, 0f, sphere.Radius), new Vector3(0.01f, 0.09f, -0.87f));
        Assert.That(sphere.Intersects(ray, out Vector3[]? result));
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(2, result.Length, "Number of intersections should be 1");
        Assert.AreEqual(sphere.Radius, Vector3.Distance(sphere.Center, result[0]), 1e-6, "Point 1 is not on sphere.");
        Assert.AreEqual(0, ray.ToRay3D().GetNearest(result[0].ToWndPoint3D()).DistanceTo(result[0].ToWndPoint3D()), 1e-6, "Point 1 is not on ray.");
        Assert.AreEqual(sphere.Radius, Vector3.Distance(sphere.Center, result[1]), 1e-6, "Point 2 is not on sphere.");
        Assert.AreEqual(0, ray.ToRay3D().GetNearest(result[1].ToWndPoint3D()).DistanceTo(result[1].ToWndPoint3D()), 1e-6, "Point 2 is not on ray.");
    }

    [Test]
    public void RayInterSection_OutsideSpherePointingAway_NoIntersection()
    {
        var sphere = new BoundingSphere(new Vector3(0.2f, 0.3f, 0f), 1.0f);
        var ray = new Ray(sphere.Center + new Vector3(0.2f, 0.3f, sphere.Radius), new Vector3(1f, 0.1f, 0.1f));
        Assert.That(sphere.Intersects(ray, out Vector3[]? result), Is.False);
    }

    [Test]
    public void RayInterSection_OutsideAndTangentToSphere_OneIntersection()
    {
        var sphere = new BoundingSphere(new Vector3(0.2f, 0.3f, 0f), 1.0f);
        var p = new Vector3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + 50);
        var p2 = new Vector3(sphere.Center.X + sphere.Radius, sphere.Center.Y, sphere.Center.Z);

        // ray tangent to sphere
        var ray = new Ray(p, p2 - p);
        Assert.That(sphere.Intersects(ray, out Vector3[]? result), "No intersection");
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(1, result.Length, "One intersection");
        Assert.AreEqual(sphere.Radius, Vector3.Distance(result[0], sphere.Center), 1e-8);
        Assert.AreEqual(0, Vector3.Distance(ray.GetNearest(result[0]), result[0]), 1e-5, "Point 1 is not on ray.");
    }
}
