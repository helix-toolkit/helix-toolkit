// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjReaderTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Test for pull request #52.
//   Fix ArgumentOutOfRangeException in MeshBuilder.ComputeNormals.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
            var objects = _objReader.Read(@"Models\obj\cornell_box.obj");
            
            Assert.IsNotNull(objects);
            Assert.AreEqual(9, objects.Count);

            var floorGeometry = objects[0].Geometry as MeshGeometry3D;

            Assert.IsNotNull(floorGeometry);
            Assert.AreEqual(4, floorGeometry.Positions.Count);
            Assert.AreEqual(4, floorGeometry.Normals.Count);
        }

        [Test]
        public void CanParseLineContinuations() 
        {
            var expectedNumberOfGeometries = 1;

            var model = _objReader.Read(@"Models\obj\line_continuation_single.obj");

            Assert.AreEqual(expectedNumberOfGeometries, model.Count);
        }

        [Test]
        public void CanParseLineContinuationsWithMultipleBreaks() 
        {
            var expectedNumberOfGeometries = 1;

            var model = _objReader.Read(@"Models\obj\line_continuation_multiple_breaks.obj");

            Assert.AreEqual(expectedNumberOfGeometries, model.Count);
        }

        [Test]
        public void CanParseLineContinuationsWithEmptyContinuations() 
        {
            var expectedNumberOfGeometries = 1;

            var model = _objReader.Read(@"Models\obj\line_continuation_empty_continuation.obj");

            Assert.AreEqual(expectedNumberOfGeometries, model.Count);
        }

        [Test]
        public void CanParseLineContinuationsWithEmptyLineInMiddle() 
        {
            var expectedNumberOfGeometries = 1;

            var model = _objReader.Read(@"Models\obj\line_continuation_empty_line.obj");

            Assert.AreEqual(expectedNumberOfGeometries, model.Count);
        }

        [Test]
        public void CanParseLineContinuationsInComments() 
        {
            var expectedNumberOfGeometries = 1;

            var model = _objReader.Read(@"Models\obj\line_continuation_comment.obj");

            Assert.AreEqual(expectedNumberOfGeometries, model.Count);
        }
    }
}