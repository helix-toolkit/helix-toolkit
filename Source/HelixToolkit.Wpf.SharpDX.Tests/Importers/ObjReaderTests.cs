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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using NUnit.Framework;
using SharpDX;

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
            var objects = _objReader.Read(@"Models\obj\cornell_box.obj");
            
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
        public void CanParseSimpleTriangle() 
        {
            var model = _objReader.Read(@"Models\obj\simple_triangle.obj");

            Assert.AreEqual(1, model.Count);
            model[0].Geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
        }

        [Test]
        public void CanParseLineContinuations() 
        {
            var model = _objReader.Read(@"Models\obj\line_continuation_single.obj");

            Assert.AreEqual(1, model.Count);
            model[0].Geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
        }

        [Test]
        public void CanParseLineContinuationsWithMultipleBreaks() 
        {
            var model = _objReader.Read(@"Models\obj\line_continuation_multiple_breaks.obj");

            Assert.AreEqual(1, model.Count);
            model[0].Geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
        }

        [Test]
        public void CanParseLineContinuationsWithEmptyContinuations() 
        {
            var model = _objReader.Read(@"Models\obj\line_continuation_empty_continuation.obj");

            Assert.AreEqual(1, model.Count);
            model[0].Geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
        }

        [Test]
        public void CanParseLineContinuationsWithEmptyLineInMiddle() 
        {
            var model = _objReader.Read(@"Models\obj\line_continuation_empty_line.obj");

            Assert.AreEqual(1, model.Count);
            model[0].Geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
        }

        [Test]
        public void CanParseLineContinuationsInComments() 
        {
            var model = _objReader.Read(@"Models\obj\line_continuation_comment.obj");

            Assert.AreEqual(1, model.Count);
            model[0].Geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
        }

        [Test]
        public void BuildModel_SmoothingOff_Valid()
        {
            string content = @"
s off

v 0 0 0
v 1 0 0
v 0 1 0
v 1 1 0

vt 0.0 0.0
vt 0.5 0.0
vt 0.0 0.5
vt 1.0 1.0
vt 0.0 1.0
vt 1.0 0.0

f 1/1 2/2 3/3
f 4/4 3/5 2/6
";

            var expectedPositions = new Vector3[]
            {
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f)
            };

            var expectedTextureCoordinates = new Vector2[]
            {
                new Vector2(0.0f, 1.0f),
                new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, 0.5f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 1.0f)
            };

            var buffer = Encoding.UTF8.GetBytes(content);

            using (var stream = new MemoryStream(buffer, false))
            {
                var model = _objReader.Read(stream);
                var mesh = (MeshGeometry3D)model[0].Geometry;

                Assert.AreEqual(6, mesh.Indices.Count);
                Assert.AreEqual(6, mesh.Positions.Count);
                Assert.AreEqual(6, mesh.TextureCoordinates.Count);

                CollectionAssert.AreEqual(expectedPositions, mesh.Positions);
                CollectionAssert.AreEqual(expectedTextureCoordinates, mesh.TextureCoordinates);
            }
        }

        [Test]
        public void BuildModel_SmoothingOn_Valid()
        {
            string content = @"
s 1

v 0 0 0
v 1 0 0
v 0 1 0
v 1 1 0

vt 0.0 0.0
vt 0.5 0.0
vt 0.0 0.5
vt 1.0 1.0
vt 0.0 1.0
vt 1.0 0.0

f 1/1 2/2 3/3
f 4/4 3/5 2/6
";

            var expectedPositions = new Vector3[]
            {
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f)
            };

            var expectedTextureCoordinates = new Vector2[]
            {
                new Vector2(0.0f, 1.0f),
                new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, 0.5f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 1.0f)
            };

            var buffer = Encoding.UTF8.GetBytes(content);

            using (var stream = new MemoryStream(buffer, false))
            {
                var model = _objReader.Read(stream);
                var mesh = (MeshGeometry3D)model[0].Geometry;

                Assert.AreEqual(6, mesh.Indices.Count);
                Assert.AreEqual(6, mesh.Positions.Count);
                Assert.AreEqual(6, mesh.TextureCoordinates.Count);

                CollectionAssert.AreEqual(expectedPositions, mesh.Positions);
                CollectionAssert.AreEqual(expectedTextureCoordinates, mesh.TextureCoordinates);
            }
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
