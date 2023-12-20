using NUnit.Framework;
using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.Helpers;

[TestFixture]
public class ContourHelperTests
{
    private static Point3D origin = new();
    private static Vector3D normal = new(0, 0, 1);

    private Point3D[] newPositions = Array.Empty<Point3D>();
    private Vector3D[] newNormals = Array.Empty<Vector3D>();
    private Point[] newTextureCoordinates = Array.Empty<Point>();
    private int[] triangleIndices = Array.Empty<int>();

    [Test]
    public void OnePointAboveAndTwoPointsBelow()
    {
        this.Contour(MeshFromTriangle(new Point3D(2, 0, 1), new Point3D(2, 0, -1), new Point3D(0, 0, -1)));
        Assert.Multiple(() =>
        {
            Assert.That(newPositions, Has.Length.EqualTo(2));
            Assert.That(newPositions[0], Is.EqualTo(new Point3D(2, 0, 0)));
            Assert.That(newPositions[1], Is.EqualTo(new Point3D(1, 0, 0)));
            Assert.That(triangleIndices, Has.Length.EqualTo(3));
            Assert.That(triangleIndices, Is.EquivalentTo(new[] { 0, 3, 4 }));
        });
    }

    [Test]
    public void OnePointBelowAndTwoPointsAbove()
    {
        this.Contour(MeshFromTriangle(new Point3D(2, 0, -1), new Point3D(2, 0, 1), new Point3D(0, 0, 1)));
        Assert.Multiple(() =>
        {
            Assert.That(newPositions, Has.Length.EqualTo(2));
            Assert.That(newPositions[0], Is.EqualTo(new Point3D(1, 0, 0)));
            Assert.That(newPositions[1], Is.EqualTo(new Point3D(2, 0, 0)));
            Assert.That(triangleIndices, Has.Length.EqualTo(6));
            Assert.That(triangleIndices, Is.EquivalentTo(new[] { 1, 2, 3, 3, 4, 1 }));
        });
    }

    [Test]
    [Ignore("#55")]
    public void OnePointOnCuttingPlaneAndOthersOnOppositeSides()
    {
        this.Contour(MeshFromTriangle(new Point3D(0, 0, 0), new Point3D(1, 0, -1), new Point3D(1, 0, 1)));
        Assert.Multiple(() =>
        {
            Assert.That(newPositions, Has.Length.EqualTo(1));
            Assert.That(newPositions[0], Is.EqualTo(new Point3D(1, 0, 0)));
            Assert.That(triangleIndices, Is.EquivalentTo(new[] { 0, 3, 2 }));
        });
    }

    [Test]
    public void TwoPointsOnCuttingPlaneAndOneOnPositiveSide()
    {
        this.Contour(MeshFromTriangle(new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(1, 0, 1)));
        Assert.Multiple(() =>
        {
            Assert.That(newPositions, Is.Empty);
            Assert.That(triangleIndices, Has.Length.EqualTo(3));
            Assert.That(triangleIndices, Is.EquivalentTo(new[] { 0, 1, 2 }));
        });
    }

    [Test]
    public void TwoPointsOnCuttingPlaneAndOneOnNegativeSide()
    {
        this.Contour(MeshFromTriangle(new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(1, 0, -1)));
        Assert.Multiple(() =>
        {
            Assert.That(newPositions, Is.Empty);
            Assert.That(triangleIndices, Is.Empty);
        });
    }

    private void Contour(MeshGeometry3D mesh)
    {
        var ch = new ContourHelper(origin.ToVector3(), normal.ToVector3(), mesh.ToWndMeshGeometry3D());
        ch.ContourFacet(0, 1, 2, out var newPositions, out var newNormals, out var newTextureCoordinates, out var triangleIndices);
        this.newPositions = newPositions is null ? Array.Empty<Point3D>() : Array.ConvertAll(newPositions, t => t.ToWndPoint3D());
        this.newNormals = newNormals is null ? Array.Empty<Vector3D>() : Array.ConvertAll(newNormals, t => t.ToWndVector3D());
        this.newTextureCoordinates = newTextureCoordinates is null ? Array.Empty<Point>() : Array.ConvertAll(newTextureCoordinates, t => t.ToWndPoint());
        this.triangleIndices = triangleIndices ?? Array.Empty<int>();
    }

    private static MeshGeometry3D MeshFromTriangle(Point3D p1, Point3D p2, Point3D p3)
    {
        var mb = new MeshBuilder(false, false);
        mb.AddTriangle(p1.ToVector3(), p2.ToVector3(), p3.ToVector3());
        return mb.ToMesh().ToMeshGeometry3D();
    }
}
