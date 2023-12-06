using NUnit.Framework;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.Geometry;

[TestFixture]
public class Ray3DTests
{
    [Test]
    public void GetNearest_PointOnOrigin()
    {
        var ray = new Ray3D(new Point3D(0, 0, 0), new Vector3D(1, 2, 3));
        var p0 = new Point3D(0, 0, 0);
        var p = ray.GetNearest(p0);
        Assert.AreEqual(0, p0.DistanceTo(p), 1e-12);
    }

    [Test]
    public void GetNearest_PointOnRay()
    {
        var ray = new Ray3D(new Point3D(0, 0, 0), new Vector3D(1, 2, 3));
        var p0 = new Point3D(0.1, 0.2, 0.3);
        var p = ray.GetNearest(p0);
        Assert.AreEqual(0, p0.DistanceTo(p), 1e-12);
    }

    [Test]
    public void GetNearest_PointOffRay()
    {
        var ray = new Ray3D(new Point3D(0, 0, 0), new Vector3D(1, 0, 0));
        var p0 = new Point3D(0.1, 2, 3);
        var pe = new Point3D(0.1, 0, 0);
        var p = ray.GetNearest(p0);
        Assert.AreEqual(0, pe.DistanceTo(p), 1e-12);
    }

    [Test]
    public void PlaneIntersection_PlaneInOriginOfRay()
    {
        var ray = new Ray3D(new Point3D(0, 0, 0), new Vector3D(1, 2, 3));
        Point3D p;
        Assert.That(ray.PlaneIntersection(ray.Origin, ray.Direction, out p));
        Assert.AreEqual(0, ray.Origin.DistanceTo(p), 1e-12);
    }

    [Test]
    public void PlaneIntersection_RayThroughPlaneOrigin()
    {
        var ray = new Ray3D(new Point3D(1, 2, 3), new Vector3D(-1, -2, -3));
        Point3D p;
        Assert.That(ray.PlaneIntersection(new Point3D(0, 0, 0), new Vector3D(1, 1, 1), out p));
        var pe = new Point3D(0, 0, 0);
        Assert.AreEqual(0, pe.DistanceTo(p), 1e-12);
    }

    [Test]
    public void PlaneIntersection_RayInPlane()
    {
        var ray = new Ray3D(new Point3D(0, 0, 0), new Vector3D(1, 0, 0));
        Point3D p;
        Assert.That(ray.PlaneIntersection(ray.Origin, ray.Direction.FindAnyPerpendicular(), out p), Is.False);
    }
}
