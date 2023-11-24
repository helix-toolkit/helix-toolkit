using NUnit.Framework;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.Visual3Ds.MeshVisuals;

[TestFixture]
public class CubeVisual3DTests
{
    [Test]
    public void Geometry_WithDefaultValues_NumberOfTriangleIndicesShouldBe36()
    {
        var cube = new CubeVisual3D();
        var g = (MeshGeometry3D)((GeometryModel3D)cube.Content).Geometry;
        Assert.That(g.TriangleIndices, Has.Count.EqualTo(36));
    }

    [Test]
    public void Visible_ChangeValue_GeometryShouldBeChanged()
    {
        var cube = new CubeVisual3D();
        Assert.That(((GeometryModel3D)cube.Content).Geometry, Is.Not.Null);
        cube.Visible = false;
        Assert.That(((GeometryModel3D)cube.Content).Geometry, Is.Null);
        cube.Visible = true;
        Assert.That(((GeometryModel3D)cube.Content).Geometry, Is.Not.Null);
    }

    [Test]
    public void Visible_EditableChangeValue_GeometryShouldBeChanged()
    {
        var cube = new CubeVisual3D();
        Assert.That(((GeometryModel3D)cube.Content).Geometry, Is.Not.Null);
        cube.BeginEdit();
        cube.Visible = false;
        Assert.That(((GeometryModel3D)cube.Content).Geometry, Is.Not.Null);
        cube.EndEdit();
        Assert.That(((GeometryModel3D)cube.Content).Geometry, Is.Null);
        cube.BeginEdit();
        cube.Visible = true;
        cube.EndEdit();
        Assert.That(((GeometryModel3D)cube.Content).Geometry, Is.Not.Null);
    }

    [Test]
    public void IEditable_ChangeTwoProperties_TesselateIsCalledOnce()
    {
        // Tessellate is called for default values in the constructor
        var cube = new CubeVisual3D();

        cube.BeginEdit();
        cube.SideLength = 2;
        cube.Fill = GradientBrushes.Rainbow;

        // Tessellate is called here
        cube.EndEdit();

        // verified manually....
    }
}
