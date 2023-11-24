/*
using System;
using System.Linq;
using System.Windows.Media.Media3D;

namespace HalfEdgeMesh.Tests;

[TestFixture]
public class HalfEdgeMeshTests
{
    [Test]
    public void ToString_TriangulatedQuadMesh()
    {
        var mesh = CreateTriangulatedQuadMesh();
        Console.WriteLine(mesh);
    }

    [Test]
    public void ToString_UnitCube()
    {
        var mesh = CreateUnitCube();
        Console.WriteLine(mesh);
    }

    [Test]
    public void AddFace_Triangle()
    {
        var vertices = new[] { new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(1, 1, 0), new Point3D(0, 1, 0) };
        var mesh = new HalfEdgeMesh(vertices);
        mesh.AddFace(0, 1, 2);
        Assert.AreEqual(1, mesh.Faces.Count);
        Assert.AreEqual(3, mesh.Edges.Count);
        Assert.AreEqual(4, mesh.Vertices.Count);
    }

    [Test]
    public void AddFace_Quad()
    {
        var vertices = new[] { new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(1, 1, 0), new Point3D(0, 1, 0) };
        var mesh = new HalfEdgeMesh(vertices);
        mesh.AddFace(0, 1, 2, 3);
        Assert.AreEqual(1, mesh.Faces.Count);
        Assert.AreEqual(4, mesh.Edges.Count);
        Assert.AreEqual(4, mesh.Vertices.Count);
    }

    [Test, ExpectedException]
    public void AddFace_NonManifold_ShouldThrowException()
    {
        var vertices = new[] { new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(1, 1, 0), new Point3D(0, 1, 0) };
        var mesh = new HalfEdgeMesh(vertices);
        mesh.AddFace(0, 1, 2);
        mesh.AddFace(0, 1, 2);
    }

    [Test, Ignore]
    public void VertexOutGoingEdges_QuadMeshVertex2_ShouldReturnTwoEdges()
    {
        var mesh = CreateTriangulatedQuadMesh();
        var oe = mesh.Vertices[2].OutgoingEdges.ToList();
        Assert.AreEqual(2, oe.Count);
        Assert.AreEqual(2, oe[0].Index);
        Assert.AreEqual(3, oe[1].Index);
    }

    [Test]
    public void VertexIncomingEdges_TriangulatedQuadMeshVertex2_ShouldReturnTwoEdges()
    {
        var mesh = CreateTriangulatedQuadMesh();
        var oe = mesh.Vertices[2].IncomingEdges.ToList();
        Assert.AreEqual(2, oe.Count);
        Assert.AreEqual(1, oe[0].Index);
        Assert.AreEqual(5, oe[1].Index);
    }

    private static HalfEdgeMesh CreateTriangulatedQuadMesh()
    {
        var vertices = new[] { new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(1, 1, 0), new Point3D(0, 1, 0) };
        var triangleIndices = new[] { 0, 1, 2, 2, 3, 0 };
        var mesh = new HalfEdgeMesh(vertices, triangleIndices);
        return mesh;
    }

    private static HalfEdgeMesh CreateUnitCube()
    {
        var vertices = new[]
            {
                    new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(1, 1, 0), new Point3D(0, 1, 0),
                    new Point3D(0, 0, 1), new Point3D(1, 0, 1), new Point3D(1, 1, 1), new Point3D(0, 1, 1)
                };
        var mesh = new HalfEdgeMesh(vertices);
        mesh.AddFace(3, 2, 1, 0);
        mesh.AddFace(4, 5, 6, 7);
        mesh.AddFace(0, 1, 5, 4);
        mesh.AddFace(1, 2, 6, 5);
        mesh.AddFace(2, 3, 7, 6);
        mesh.AddFace(3, 0, 4, 7);
        return mesh;
    }
}
*/
