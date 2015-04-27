// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjReaderTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Test for pull request #52.
//   Fix ArgumentOutOfRangeException in MeshBuilder.ComputeNormals.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using NUnit.Framework;

namespace HelixToolkit.Wpf.SharpDX.Tests.Importers
{
    [TestFixture]
    class ObjReaderTests
    {
        private ObjReader _objReader;

        [SetUp]
        public void SetUp()
        {
            _objReader = new ObjReader();
        }

        /// <summary>
        /// Test for pull request #52.
        /// Fix ArgumentOutOfRangeException in MeshBuilder.ComputeNormals.
        /// </summary>
        [Test]
        public void LoadModelWithoutNormals()
        {
            var reader = new ObjReader();
            var objects = reader.Read(@"Models\obj\cornell_box.obj");
            
            Assert.IsNotNull(objects);
            Assert.AreEqual(9, objects.Count);

            var floorGeometry = objects[0].Geometry as MeshGeometry3D;

            Assert.IsNotNull(floorGeometry);
            Assert.AreEqual(4, floorGeometry.Positions.Count);
            Assert.AreEqual(4, floorGeometry.Normals.Count);
        }

        [Test]
        public void CanParseFaceWithRelativeIndices() 
        {
            var model = _objReader.Read(@"Models\obj\face_relative_vertices.obj");

            Assert.AreEqual(1, model.Count);
            var geometry = (MeshGeometry3D)model[0].Geometry;
            geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
        }

        [Test]
        public void CanParseFaceWithAbsoluteNormals() 
        {
            var model = _objReader.Read(@"Models\obj\simple_triangle_with_normals.obj");

            Assert.AreEqual(1, model.Count);
            var geometry = (MeshGeometry3D)model[0].Geometry;
            geometry.Normals.AssertContains(new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d });
        }

        [Test]
        public void CanParseFaceWithRelativeNormals() 
        {
            var model = _objReader.Read(@"Models\obj\face_relative_vertex_normals.obj");

            Assert.AreEqual(1, model.Count);
            var geometry = (MeshGeometry3D)model[0].Geometry;
            geometry.Normals.AssertContains(new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d });
        }

        [Test]
        public void CanParseFaceWithAbsoluteTextureCoords() 
        {
            var model = _objReader.Read(@"Models\obj\simple_triangle_with_texture.obj");

            Assert.AreEqual(1, model.Count);
            var geometry = (MeshGeometry3D)model[0].Geometry;
            geometry.TextureCoordinates.AssertContains(new[] { 0d, 0d }, new[] { 0d, 0d }, new[] { 0d, 0d });
        }

        [Test]
        public void CanParseFaceWithRelativeTextureCoords() 
        {
            var model = _objReader.Read(@"Models\obj\face_relative_texture_vertices.obj");

            Assert.AreEqual(1, model.Count);
            var geometry = (MeshGeometry3D)model[0].Geometry;
            geometry.TextureCoordinates.AssertContains(new[] { 0d, 0d }, new[] { 0d, 0d }, new[] { 0d, 0d });
        }

        [Test]
        public void CanReadWrappedLines() {
            var expectedNumberOfGeometries = 14;
            var objReader = new ObjReader();

            var model = objReader.Read(@"Models\obj\wrap_long_lines.obj");

            Assert.AreEqual(expectedNumberOfGeometries, model.Count);
        }
    }

    public static class TestExtensions 
    {
        public static void AssertContains(this Core.Vector2Collection collection, params double[][] points) 
        {
            Assert.AreEqual(points.Length, collection.Count, "Expected to find {0} points in collection", points.Length);
            foreach (var point in points)
                Assert.IsTrue(collection.Contains(point), "Expected collection to contain point [{0},{1}]", point[0], point[1]);
        }

        public static void AssertContains(this Core.Vector3Collection collection, params double[][] points) 
        {
            Assert.AreEqual(points.Length, collection.Count, "Expected to find {0} points in collection", points.Length);
            foreach (var point in points)
                Assert.IsTrue(collection.Contains(point), "Expected collection to contain point [{0},{1},{2}]", point[0], point[1], point[2]);
        }

        public static bool Contains(this Core.Vector3Collection vectors, double[] expectedVector)
        {
            return vectors.Any(vector => Math.Abs((float) expectedVector[0] - vector.X) < float.Epsilon &&
                                         Math.Abs((float) expectedVector[1] - vector.Y) < float.Epsilon &&
                                         Math.Abs((float) expectedVector[2] - vector.Z) < float.Epsilon);
        }

        public static bool Contains(this Core.Vector2Collection vectors, double[] expectedVector)
        {
            return vectors.Any(vector => Math.Abs((float) expectedVector[0] - vector.X) < float.Epsilon &&
                                         Math.Abs((float) expectedVector[1] - vector.Y) < float.Epsilon);
        }
     }		     
}
