using HelixToolkit.Geometry;
using HelixToolkit.Maths;
using HelixToolkit.SharpDX;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Numerics;
using System.Reflection;

namespace HelixToolkit.Wpf.SharpDX.Tests.Elements3D;

[TestFixture]
[Apartment(ApartmentState.STA)]
class CrossSectionMeshGeometryModel3DTests
{
    private readonly Viewport3DX viewport = new();

    private CrossSectionMeshGeometryModel3D GetGeometryModel3D()
    {
        var meshBuilder = new MeshBuilder();
        meshBuilder.AddBox(new Vector3(0f), 1, 1, 1);
        return new CrossSectionMeshGeometryModel3D()
        {
            Geometry = meshBuilder.ToMeshGeometry3D(),
        };
    }

    [Test]
    public void HitTestShouldReturnOnePointOnFrontOfCubeWithNoCuttingPlanes()
    {
        var ray = new Ray(new Vector3(2f, 0f, 0f), new Vector3(-1, 0, 0));
        var hits = new List<HitTestResult>();
        var geometryModel3D = GetGeometryModel3D();
        var hitContext = new HitTestContext(viewport.RenderContext, ref ray);
        geometryModel3D.HitTest(hitContext, ref hits);
        ClassicAssert.AreEqual(1, hits.Count);
        ClassicAssert.AreEqual(new Vector3(0.5f, 0, 0), hits[0].PointHit);
    }

    [TestCaseSource(nameof(GetPlanes))]
    public void HitTestShouldReturnOnePointOnBackOfCubeWithCuttingPlaneInXZero(Action<CrossSectionMeshGeometryModel3D, Plane> setupPlane)
    {
        var plane = PlaneHelper.Create(new Vector3(0f), new Vector3(-1, 0, 0));
        var geometryModel3D = GetGeometryModel3D();
        setupPlane(geometryModel3D, plane);
        var ray = new Ray(new Vector3(2f, 0f, 0f), new Vector3(-1, 0, 0));
        var hits = new List<HitTestResult>();
        var hitContext = new HitTestContext(viewport.RenderContext, ref ray);
        geometryModel3D.HitTest(hitContext, ref hits);
        ClassicAssert.AreEqual(1, hits.Count);
        ClassicAssert.AreEqual(new Vector3(-0.5f, 0, 0), hits[0].PointHit);
    }

    /// <summary>
    /// Get all planes in the CrossSectionMeshGeometryModel3D with reflection so that if more planes are added tests will fail
    /// </summary>
    public static IEnumerable<object[]> GetPlanes()
    {
        var properties = typeof(CrossSectionMeshGeometryModel3D)
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

            void Action(CrossSectionMeshGeometryModel3D model, Plane plane)
            {
                planeProperty.SetValue(model, plane);
                enableProperty.SetValue(model, true);
            }

            yield return new object[] { (Action<CrossSectionMeshGeometryModel3D, Plane>)Action };
        }
    }
}
