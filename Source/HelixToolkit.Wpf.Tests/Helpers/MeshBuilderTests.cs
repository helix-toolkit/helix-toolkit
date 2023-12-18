using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

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
        var p0 = new Vector3(0, 0, 0);
        var p1 = new Vector3(1, 0, 0);
        var p2 = new Vector3(1, 1, 0);
        mb.AddTriangle(p0, p1, p2);

        Assert.That(mb.HasNormals);
        Assert.That(mb.Normals, Has.Count.EqualTo(3));

        foreach (var normal in mb.Normals)
        {
            Assert.That(normal, Is.EqualTo(new Vector3(0, 0, 1)));
        }
    }

    [Test]
    public void AddQuad_Normals()
    {
        var mb = new MeshBuilder();
        var p0 = new Vector3(0, 0, 0);
        var p1 = new Vector3(1, 0, 0);
        var p2 = new Vector3(1, 1, 0);
        var p3 = new Vector3(0, 1, 0);
        mb.AddQuad(p0, p1, p2, p3);

        Assert.Multiple(() =>
        {
            Assert.That(mb.HasNormals);
            Assert.That(mb.Normals, Is.Not.Null);
            Assert.AreEqual(4, mb.Normals!.Count);

            foreach (var normal in mb.Normals)
            {
                Assert.AreEqual(new Vector3(0, 0, 1), normal);
            }
        });
    }

    [Test]
    public void AddInvalidPolygon()
    {
        var meshBuilder = new MeshBuilder(false, false);
        Assert.AreEqual(0, meshBuilder.Positions.Count);
        Assert.AreEqual(0, meshBuilder.TriangleIndices.Count);
        var p = new List<Vector2>
                {
                    new(0, 0),
                    new(1, 1),
                };

        meshBuilder.AddPolygon(p, new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 0));
        Assert.AreEqual(0, meshBuilder.Positions.Count);
        Assert.AreEqual(0, meshBuilder.TriangleIndices.Count);

        var p3 = new List<Vector3>
                {
                    new(0, 0, 0),
                    new(1, 1, 0),
                };
        meshBuilder.AddPolygon(p3);
        Assert.AreEqual(0, meshBuilder.Positions.Count);
        Assert.AreEqual(0, meshBuilder.TriangleIndices.Count);
    }

    [Test]
    public void AddValidPolygon()
    {
        var meshBuilder = new MeshBuilder(false, false);
        Assert.AreEqual(0, meshBuilder.Positions.Count);
        Assert.AreEqual(0, meshBuilder.TriangleIndices.Count);
        var p = new List<Vector2>
                {
                    new(0, 0),
                    new(1, 1),
                    new(2, 2),
                };

        meshBuilder.AddPolygon(p, new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 0));
        Assert.AreEqual(3, meshBuilder.Positions.Count);
        Assert.AreEqual(3, meshBuilder.TriangleIndices.Count);

        var p3 = new List<Vector3>
                {
                    new(0, 0, 0),
                    new(1, 1, 0),
                    new(2, 2, 0),
                };
        meshBuilder.AddPolygon(p3);
        Assert.AreEqual(6, meshBuilder.Positions.Count);
        Assert.AreEqual(6, meshBuilder.TriangleIndices.Count);
    }
}
