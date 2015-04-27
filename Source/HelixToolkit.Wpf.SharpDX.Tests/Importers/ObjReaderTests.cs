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
        public void CanReadWrappedLines() {
            var expectedNumberOfGeometries = 14;
            var objReader = new ObjReader();

            var model = objReader.Read(@"Models\obj\wrap_long_lines.obj");

            Assert.AreEqual(expectedNumberOfGeometries, model.Count);
        }
    }
}