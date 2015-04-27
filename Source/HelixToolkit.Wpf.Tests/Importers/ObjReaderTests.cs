// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjReaderTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Media;

namespace HelixToolkit.Wpf.Tests
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
        private ObjReader _objReader;

        [SetUp]
        public void SetUp() 
        {
            _objReader = new ObjReader();
        }

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

        [Test]
        public void CanParseFaceWithRelativeIndices() 
        {
            var model = _objReader.Read(@"Models\obj\face_relative_vertices.obj");

            Assert.AreEqual(1, model.Children.Count);
            var mesh = model.Children[0].GetMesh();
            mesh.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
        }

        [Test]
        public void CanParseFaceWithAbsoluteNormals() 
        {
            var model = _objReader.Read(@"Models\obj\simple_triangle_with_normals.obj");

            Assert.AreEqual(1, model.Children.Count);
            var mesh = model.Children[0].GetMesh();
            mesh.Normals.AssertContains(new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d });
        }

        [Test]
        public void CanParseFaceWithRelativeNormals() 
        {
            var model = _objReader.Read(@"Models\obj\face_relative_vertex_normals.obj");

            Assert.AreEqual(1, model.Children.Count);
            var mesh = model.Children[0].GetMesh();
            mesh.Normals.AssertContains(new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d });
        }

        [Test]
        public void CanParseFaceWithAbsoluteTextureCoords() 
        {
            var model = _objReader.Read(@"Models\obj\simple_triangle_with_texture.obj");

            Assert.AreEqual(1, model.Children.Count);
            var mesh = model.Children[0].GetMesh();
            mesh.TextureCoordinates.AssertContains(new[] {0d, 0d}, new[] {0d, 0d}, new[] {0d, 0d});
        }

        [Test]
        public void CanParseFaceWithRelativeTextureCoords() 
        {
            var model = _objReader.Read(@"Models\obj\face_relative_texture_vertices.obj");

            Assert.AreEqual(1, model.Children.Count);
            var mesh = model.Children[0].GetMesh();
            mesh.TextureCoordinates.AssertContains(new[] {0d, 0d}, new[] {0d, 0d}, new[] {0d, 0d});
        }

        [Test]
        public void CanReadWrappedLines() {
            var expectedNumberOfGeometries = 14;
            var objReader = new ObjReader();

            var model = objReader.Read(@"Models\obj\wrap_long_lines.obj");

            Assert.AreEqual(expectedNumberOfGeometries, model.Children.Count);
        }

        [Test]
        public void CanReadLinesWrappedMultipleTimes() {
            var expectedNumberOfGeometries = 14;
            var objReader = new ObjReader();

            var model = objReader.Read(@"Models\obj\wrap_lines_multiple_times.obj");

            Assert.AreEqual(expectedNumberOfGeometries, model.Children.Count);
        }
    }

    public static class TestExtensions {
        public static void AssertContains(this PointCollection collection, params double[][] points) {
            Assert.AreEqual(points.Length, collection.Count, "Expected to find {0} points in collection", points.Length);
            foreach (var point in points)
                Assert.IsTrue(collection.Contains(new Point(point[0],point[1])), "Expected collection to contain point [{0},{1}]", point[0], point[1]);
        }

        public static void AssertContains(this Vector3DCollection collection, params double[][] points) {
            Assert.AreEqual(points.Length, collection.Count, "Expected to find {0} points in collection", points.Length);
            foreach (var point in points)
                Assert.IsTrue(collection.Contains(new Vector3D(point[0],point[1],point[2])), "Expected collection to contain point [{0},{1},{2}]", point[0], point[1], point[2]);
        }

        public static void AssertContains(this Point3DCollection collection, params double[][] points) {
            Assert.AreEqual(points.Length, collection.Count, "Expected to find {0} points in collection", points.Length);
            foreach (var point in points)
                Assert.IsTrue(collection.Contains(new Point3D(point[0],point[1],point[2])), "Expected collection to contain point [{0},{1},{2}]", point[0], point[1], point[2]);
        }

        public static MeshGeometry3D GetMesh(this Model3D model) 
        {
            var geometryModel = (GeometryModel3D) model;
            return (MeshGeometry3D) geometryModel.Geometry;
        }
    }		  
}
