using HelixToolkit.SharpDX.Model.Scene;
using NUnit.Framework;
using System.Numerics;
using System.Reflection;
using HelixToolkit.Maths;

namespace HelixToolkit.SharpDX.Tests;

[TestFixture]
[Apartment(ApartmentState.STA)]
public class CrossSectionMeshNodeTests
{
    private readonly ViewportCore viewport = new(IntPtr.Zero);

    [OneTimeTearDown]
    public void Dispose()
    {
        viewport.Dispose();
    }

    private static CrossSectionMeshNode GetNode()
    {
        var meshBuilder = new MeshBuilder();
        meshBuilder.AddBox(new Vector3(0f), 1, 1, 1);
        return new CrossSectionMeshNode()
        {
            Geometry = meshBuilder.ToMeshGeometry3D(),
        };
    }

    [Test]
    public void HitTestShouldReturnOnePointOnFrontOfCubeWithNoCuttingPlanes()
    {
        var ray = new Ray(new Vector3(2f, 0f, 0f), new Vector3(-1, 0, 0));
        var hits = new List<HitTestResult>();
        var node = CrossSectionMeshNodeTests.GetNode();
        node.HitTest(new HitTestContext(viewport.RenderContext, ref ray), ref hits);
        Assert.AreEqual(1, hits.Count);
        Assert.AreEqual(new Vector3(0.5f, 0, 0), hits[0].PointHit);
    }

    [TestCaseSource(nameof(GetPlanes))]
    public void HitTestShouldReturnOnePointOnBackOfCubeWithCuttingPlaneInXZero(Action<CrossSectionMeshNode, Plane> setupPlane)
    {
        var plane = PlaneHelper.Create(new Vector3(0f), new Vector3(-1, 0, 0));
        var node = CrossSectionMeshNodeTests.GetNode();
        setupPlane(node, plane);
        var ray = new Ray(new Vector3(2f, 0f, 0f), new Vector3(-1, 0, 0));
        var hits = new List<HitTestResult>();
        node.HitTest(new HitTestContext(viewport.RenderContext, ref ray), ref hits);
        Assert.AreEqual(1, hits.Count);
        Assert.AreEqual(new Vector3(-0.5f, 0, 0), hits[0].PointHit);
    }

    /// <summary>
    /// Get all planes in the CrossSectionMeshNode with reflection so that if more planes are added tests will fail
    /// </summary>
    private static IEnumerable<object[]> GetPlanes()
    {
        var properties = typeof(CrossSectionMeshNode)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public);

        var enables = properties
            .Where(p => p.Name.StartsWith("EnablePlane", StringComparison.InvariantCulture))
            .ToArray();
        var planes = properties
            .Where(p => p.Name.StartsWith("Plane", StringComparison.InvariantCulture))
            .ToArray();
        for (int i = 0; i < enables.Length; i++)
        {
            var planeProperty = planes[i];
            var enableProperty = enables[i];

            void Action(CrossSectionMeshNode model, Plane plane)
            {
                planeProperty.SetValue(model, plane);
                enableProperty.SetValue(model, true);
            }

            yield return new object[] { (Action<CrossSectionMeshNode, Plane>)Action };
        }
    }
}
