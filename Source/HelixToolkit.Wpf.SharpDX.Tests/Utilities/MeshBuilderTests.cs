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
    }
}