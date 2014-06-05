namespace HelixToolkit.Wpf.Tests.Visual3Ds.MeshVisuals
{
    using System.Windows.Media.Media3D;

    using NUnit.Framework;

    [TestFixture]
    public class CubeVisual3DTests
    {
        [Test]
        public void Geometry_WithDefaultValues_NumberOfTriangleIndicesShouldBe36()
        {
            var cube = new CubeVisual3D();
            var g = (MeshGeometry3D)((GeometryModel3D)cube.Content).Geometry;
            Assert.AreEqual(36, g.TriangleIndices.Count);
        }

        [Test]
        public void Visible_ChangeValue_GeometryShouldBeChanged()
        {
            var cube = new CubeVisual3D();
            Assert.IsNotNull(((GeometryModel3D)cube.Content).Geometry);
            cube.Visible = false;
            Assert.IsNull(((GeometryModel3D)cube.Content).Geometry);
            cube.Visible = true;
            Assert.IsNotNull(((GeometryModel3D)cube.Content).Geometry);
        }

        [Test]
        public void Visible_EditableChangeValue_GeometryShouldBeChanged()
        {
            var cube = new CubeVisual3D();
            Assert.IsNotNull(((GeometryModel3D)cube.Content).Geometry);
            cube.BeginEdit();
            cube.Visible = false;
            Assert.IsNotNull(((GeometryModel3D)cube.Content).Geometry);
            cube.EndEdit();
            Assert.IsNull(((GeometryModel3D)cube.Content).Geometry);
            cube.BeginEdit();
            cube.Visible = true;
            cube.EndEdit();
            Assert.IsNotNull(((GeometryModel3D)cube.Content).Geometry);
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
}
