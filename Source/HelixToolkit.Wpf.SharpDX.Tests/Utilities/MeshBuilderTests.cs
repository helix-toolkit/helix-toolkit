// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshBuilderTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using NUnit.Framework;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Tests.Utilities
{
    [TestFixture]
    class MeshBuilderTests
    {
        [Test]
        public void ComputeNormals()
        {
            var builder = new MeshBuilder(false);
            builder.AddPolygon(new List<Vector3>
                {
                    new Vector3(0f, 0f, 0f),
                    new Vector3(7f, 0f, 0f),
                    new Vector3(7f, 0f, 7f),
                });

            Assert.IsNull(builder.Normals);
            Assert.IsFalse(builder.HasNormals);

            builder.ComputeNormalsAndTangents(MeshFaces.Default);

            Assert.IsTrue(builder.HasNormals);
            Assert.AreEqual(3, builder.Normals.Count);
        }

        [Test]
        public void AddTriangle_Normals()
        {
            var mb = new MeshBuilder();
            var p0 = new Vector3(0, 0, 0);
            var p1 = new Vector3(1, 0, 0);
            var p2 = new Vector3(1, 1, 0);
            mb.AddTriangle(p0, p1, p2);

            Assert.IsTrue(mb.HasNormals);
            Assert.AreEqual(3, mb.Normals.Count);

            foreach (Vector3 normal in mb.Normals)
            {
                Assert.AreEqual(new Vector3(0, 0, 1), normal);
            }
        }

        [Test]
        public void AddQuad_Normals()
        {
            var mb = new MeshBuilder();
            var p0 = new Vector3(0, 0, 0);
            var p1 = new Vector3(1, 0, 0);
            var p2 = new Vector3(1, 1, 0);
            var p3 = new Vector3(0, 1, 0);
            mb.AddQuad(p0, p1, p2, p3);

            Assert.IsTrue(mb.HasNormals);
            Assert.AreEqual(4, mb.Normals.Count);

            foreach (Vector3 normal in mb.Normals)
            {
                Assert.AreEqual(new Vector3(0, 0, 1), normal);
            }
        }
    }
}