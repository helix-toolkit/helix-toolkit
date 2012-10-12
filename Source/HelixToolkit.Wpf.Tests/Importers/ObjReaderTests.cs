// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjReaderTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkitTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class ObjReaderTests
    {
        [Test, Ignore]
        public void Read_Bunny_ValidModel()
        {
            var r = new ObjReader();
            var model = r.Read(@"Models\obj\bunny.obj");
            Assert.AreEqual(2, model.Children.Count);
            var gm1 = model.Children[1] as GeometryModel3D;
            Assert.NotNull(gm1);
            var mg1 = gm1.Geometry as MeshGeometry3D;
            Assert.NotNull(mg1);
            Assert.AreEqual(69451, mg1.TriangleIndices.Count / 3);
        }

        [Test]
        public void Read_CornellBox_ValidModel()
        {
            var r = new ObjReader();
            var model = r.Read(@"Models\obj\cornell_box.obj");
            Assert.AreEqual(9, model.Children.Count);
            var gm1 = model.Children[1] as GeometryModel3D;
            Assert.NotNull(gm1);
            var mg1 = gm1.Geometry as MeshGeometry3D;
            Assert.NotNull(mg1);
            //// Assert.AreEqual(69451, mg1.TriangleIndices.Count / 3);
        }

        [Test, Ignore]
        public void Read_Ducky_ValidModel()
        {
            var r = new ObjReader();
            var model = r.Read(@"Models\obj\ducky.obj");
            Assert.AreEqual(4, model.Children.Count);
            var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
            var m1 = (MeshGeometry3D)((GeometryModel3D)model.Children[1]).Geometry;
            var m2 = (MeshGeometry3D)((GeometryModel3D)model.Children[2]).Geometry;
            var m3 = (MeshGeometry3D)((GeometryModel3D)model.Children[3]).Geometry;
            Assert.AreEqual(5632, m0.TriangleIndices.Count / 3);
            Assert.AreEqual(4800, m1.TriangleIndices.Count / 3);
            Assert.AreEqual(3024, m2.TriangleIndices.Count / 3);
            Assert.AreEqual(672, m3.TriangleIndices.Count / 3);
        }

        [Test]
        public void Read_Test_ValidModel()
        {
            var r = new ObjReader();
            var model = r.Read(@"Models\obj\test.obj");
            Assert.AreEqual(2, model.Children.Count);
            var gm1 = model.Children[0] as GeometryModel3D;
            Assert.NotNull(gm1);
            var mg1 = gm1.Geometry as MeshGeometry3D;
            Assert.NotNull(mg1);
            Assert.AreEqual(12, mg1.TriangleIndices.Count / 3);
        }

        [Test]
        public void Read_SmoothingOff_ValidModel()
        {
            var r = new ObjReader();
            var model = r.Read(@"Models\obj\SmoothingOff.obj");
            Assert.AreEqual(1, model.Children.Count);
            var gm1 = model.Children[0] as GeometryModel3D;
            Assert.NotNull(gm1);
            var mg1 = gm1.Geometry as MeshGeometry3D;
            Assert.NotNull(mg1);
            Assert.AreEqual(4, mg1.TriangleIndices.Count / 3);
        }

        [Test]
        public void Read_SmoothingGroup0_ValidModel()
        {
            var r = new ObjReader();
            var model = r.Read(@"Models\obj\SmoothingGroup0.obj");
            Assert.AreEqual(1, model.Children.Count);
            var gm1 = model.Children[0] as GeometryModel3D;
            Assert.NotNull(gm1);
            var mg1 = gm1.Geometry as MeshGeometry3D;
            Assert.NotNull(mg1);
            Assert.AreEqual(4, mg1.TriangleIndices.Count / 3);
        }

        [Test]
        public void Read_SmoothingGroup1_ValidModel()
        {
            var r = new ObjReader();
            var model = r.Read(@"Models\obj\SmoothingGroup1.obj");
            Assert.AreEqual(1, model.Children.Count);
            var gm1 = model.Children[0] as GeometryModel3D;
            Assert.NotNull(gm1);
            var mg1 = gm1.Geometry as MeshGeometry3D;
            Assert.NotNull(mg1);
            Assert.AreEqual(4, mg1.TriangleIndices.Count / 3);
        }
    }
}