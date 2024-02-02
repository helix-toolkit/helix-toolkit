using HelixToolkit.SharpDX.Model.Scene;
using NUnit.Framework;
using System.Numerics;
using HelixToolkit.Maths;
using HelixToolkit.Geometry;

namespace HelixToolkit.SharpDX.Tests;

[TestFixture]
[Apartment(ApartmentState.STA)]
public class SceneNodeTests
{
    private static SceneNode GetNode()
    {
        var meshBuilder = new MeshBuilder();
        meshBuilder.AddBox(new Vector3(0f), 1, 1, 1);
        return new MeshNode()
        {
            Geometry = meshBuilder.ToMeshGeometry3D(),
        };
    }

    [Test]
    public void HitTestShouldReturnOnePointOnFrontOfCubeWithNoCuttingPlanes()
    {
        using var viewport = new ViewportCore(IntPtr.Zero);
        var ray = new Ray(new Vector3(2f, 0f, 0f), new Vector3(-1, 0, 0));
        var hits = new List<HitTestResult>();
        var sceneNode = SceneNodeTests.GetNode();
        sceneNode.HitTest(new HitTestContext(viewport.RenderContext, ref ray), ref hits);
        Assert.AreEqual(1, hits.Count);
        Assert.AreEqual(new Vector3(0.5f, 0, 0), hits[0].PointHit);
    }
}
