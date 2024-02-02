using HelixToolkit.Geometry;
using NUnit.Framework;
using System.Numerics;

namespace HelixToolkit.Wpf.SharpDX.Tests.Utilities;

[TestFixture]
class MeshBuilderTests
{
    [Test]
    public void ComputeNormals()
    {
        var builder = new MeshBuilder(false);
        builder.AddPolygon(new List<System.Numerics.Vector3>
                {
                    new Vector3(0f, 0f, 0f),
                    new Vector3(7f, 0f, 0f),
                    new Vector3(7f, 0f, 7f),
                });

        Assert.Multiple(() =>
        {
            Assert.That(builder.Normals, Is.Null);
            Assert.That(builder.HasNormals, Is.False);
        });

        builder.ComputeNormalsAndTangents(MeshFaces.Default);

        Assert.That(builder.HasNormals);
        Assert.AreEqual(3, builder.Normals!.Count);
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
        Assert.AreEqual(3, mb.Normals!.Count);

        foreach (System.Numerics.Vector3 normal in mb.Normals)
        {
            Assert.AreEqual(new System.Numerics.Vector3(0, 0, 1), normal);
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

        Assert.That(mb.HasNormals);
        Assert.AreEqual(4, mb.Normals!.Count);

        foreach (System.Numerics.Vector3 normal in mb.Normals)
        {
            Assert.AreEqual(new System.Numerics.Vector3(0, 0, 1), normal);
        }
    }
}
