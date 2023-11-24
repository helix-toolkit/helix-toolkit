using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.Helpers;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class MeshBuilderTests
{
    [Test]
    public void A_B_C()
    {
        var mb = new MeshBuilder();
        Assert.That(mb, Is.Not.Null);
    }

    [Test]
    public void AddTriangle_Normals()
    {
        var mb = new MeshBuilder();
        var p0 = new Point3D(0, 0, 0);
        var p1 = new Point3D(1, 0, 0);
        var p2 = new Point3D(1, 1, 0);
        mb.AddTriangle(p0.ToVector(), p1.ToVector(), p2.ToVector());

        Assert.That(mb.HasNormals);
        Assert.That(mb.Normals, Has.Count.EqualTo(3));

        foreach (Point3D normal in mb.Normals.Select(t => t.ToWndPoint()))
        {
            Assert.That(normal, Is.EqualTo(new Point3D(0, 0, 1)));
        }
    }

    [Test]
    public void AddQuad_Normals()
    {
        var mb = new MeshBuilder();
        var p0 = new Point3D(0, 0, 0);
        var p1 = new Point3D(1, 0, 0);
        var p2 = new Point3D(1, 1, 0);
        var p3 = new Point3D(0, 1, 0);
        mb.AddQuad(p0.ToVector(), p1.ToVector(), p2.ToVector(), p3.ToVector());

        Assert.Multiple(() =>
        {
            Assert.That(mb.HasNormals);
            Assert.That(mb.Normals, Is.Not.Null);
            Assert.AreEqual(4, mb.Normals!.Count);

            foreach (Point3D normal in mb.Normals.Select(t => t.ToWndPoint()))
            {
                Assert.AreEqual(new Point3D(0, 0, 1), normal);
            }
        });
    }

    [Test]
    public void AddInvalidPolygon()
    {
        var meshBuilder = new MeshBuilder(false, false);
        Assert.AreEqual(0, meshBuilder.Positions.Count);
        Assert.AreEqual(0, meshBuilder.TriangleIndices.Count);
        var p = new List<Point>
                {
                    new Point(0, 0 ),
                    new Point(1, 1 ),
                };

        meshBuilder.AddPolygon(p.Select(t => t.ToVector()).ToList(), new Vector3D(0, 1, 0).ToVector(), new Vector3D(1, 0, 0).ToVector(), new Point3D(0, 0, 0).ToVector());
        Assert.AreEqual(0, meshBuilder.Positions.Count);
        Assert.AreEqual(0, meshBuilder.TriangleIndices.Count);

        var p3 = new List<Point3D>
                {
                    new Point3D(0, 0, 0),
                    new Point3D(1, 1, 0),
                };
        meshBuilder.AddPolygon(p3.Select(t => t.ToVector()).ToList());
        Assert.AreEqual(0, meshBuilder.Positions.Count);
        Assert.AreEqual(0, meshBuilder.TriangleIndices.Count);
    }

    [Test]
    public void AddValidPolygon()
    {
        var meshBuilder = new MeshBuilder(false, false);
        Assert.AreEqual(0, meshBuilder.Positions.Count);
        Assert.AreEqual(0, meshBuilder.TriangleIndices.Count);
        var p = new List<Point>
                {
                    new Point(0, 0 ),
                    new Point(1, 1 ),
                    new Point(2, 2 ),
                };

        meshBuilder.AddPolygon(p.Select(t => t.ToVector()).ToList(), new Vector3D(0, 1, 0).ToVector(), new Vector3D(1, 0, 0).ToVector(), new Point3D(0, 0, 0).ToVector());
        Assert.AreEqual(3, meshBuilder.Positions.Count);
        Assert.AreEqual(3, meshBuilder.TriangleIndices.Count);

        var p3 = new List<Point3D>
                {
                    new Point3D(0, 0, 0),
                    new Point3D(1, 1, 0),
                    new Point3D(2, 2, 0),
                };
        meshBuilder.AddPolygon(p3.Select(t => t.ToVector()).ToList());
        Assert.AreEqual(6, meshBuilder.Positions.Count);
        Assert.AreEqual(6, meshBuilder.TriangleIndices.Count);
    }
}
