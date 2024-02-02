using HelixToolkit.Geometry;
using HelixToolkit.Maths;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX.Tests.Controls;
using NUnit.Framework;
using System.IO;
using System.Numerics;

namespace HelixToolkit.Wpf.SharpDX.Tests.Elements3D;

[TestFixture]
[Apartment(ApartmentState.STA)]
class MeshGeometryModel3DTests
{
    [SetUp]
    public void SetUp()
    {
        string dir = Path.GetDirectoryName(typeof(MeshGeometryModel3DTests).Assembly.Location) ?? "";
        dir = Path.Combine(dir!, string.Concat(Enumerable.Repeat("..\\", 5)));
        Directory.SetCurrentDirectory(dir);
    }

    /// <summary>
    /// Test for pull request #54.
    /// Fix IndexOutOfRange exception in CreateDefaultVertexArray.
    /// </summary>
    [Test]
    public void CreateDefaultVertexArrayForTriangle()
    {
        var reader = new ObjReader();
        var objects = reader.Read(@"Models\obj\Triangle.obj");

        Assert.That(objects, Is.Not.Null);
        Assert.AreEqual(1, objects.Count);

        var geometry = objects[0].Geometry;
        var model = new MeshGeometryModel3D { Geometry = geometry };

        var canvas = new CanvasMock();
        model.SceneNode.Attach(canvas.RenderHost.EffectsManager);

        Assert.AreEqual(true, model.IsAttached);
    }

    private MeshGeometryModel3D GetGeometryModel3D()
    {
        var meshBuilder = new MeshBuilder();
        meshBuilder.AddBox(new Vector3(0f), 1, 1, 1);
        return new MeshGeometryModel3D()
        {
            Geometry = meshBuilder.ToMeshGeometry3D(),
        };
    }

    [Test]
    public void HitTestShouldReturnOnePointOnFrontOfCubeWithNoCuttingPlanes()
    {
        var viewport = new Viewport3DX();
        var ray = new Ray(new Vector3(2f, 0f, 0f), new Vector3(-1, 0, 0));
        var hits = new List<HitTestResult>();
        var geometryModel3D = GetGeometryModel3D();
        geometryModel3D.HitTest(new HitTestContext(viewport.RenderContext, ref ray), ref hits);
        Assert.AreEqual(1, hits.Count);
        Assert.AreEqual(new Vector3(0.5f, 0, 0), hits[0].PointHit);
    }
}
