// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshBuilderTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class MeshBuilderTests
    {
        [Test]
        public void A_B_C()
        {
            var mb = new MeshBuilder();
            Assert.NotNull(mb);
        }

        [Test]
        public void AddTriangle_Normals()
        {
            var mb = new MeshBuilder();
            var p0 = new Point3D(0, 0, 0);
            var p1 = new Point3D(1, 0, 0);
            var p2 = new Point3D(1, 1, 0);
            mb.AddTriangle(p0, p1, p2);

            Assert.IsTrue(mb.HasNormals);
            Assert.AreEqual(3, mb.Normals.Count);

            foreach (Point3D normal in mb.Normals)
            {
                Assert.AreEqual(new Point3D(0, 0, 1), normal);
            }
        }

        [Test]
        public void AddQuad_Normals()
        {
            var mb = new MeshBuilder();
            var p0 = new Point3D(0, 0, 0);
            var p1 = new Point3D(1, 0, 0);
            var p2 = new Point3D(1, 1, 0);
            var p3 = new Point3D(0, 1, 0);
            mb.AddQuad(p0, p1, p2, p3);

            Assert.IsTrue(mb.HasNormals);
            Assert.AreEqual(4, mb.Normals.Count);

            foreach (Point3D normal in mb.Normals)
            {
                Assert.AreEqual(new Point3D(0, 0, 1), normal);
            }
        }

        [Test]
        public void AddInvalidPolygon()
        {
            var meshBuilder = new MeshBuilder(false, false);
            Assert.AreEqual(0, meshBuilder.Positions.Count);
            Assert.AreEqual(0, meshBuilder.TriangleIndices.Count);
            var p = new List<Point>
                {
                    new Point(0, 0 ),
                    new Point(1, 1 ),
                };

            meshBuilder.AddPolygon(p, new Vector3D(0, 1, 0), new Vector3D(1, 0, 0), new Point3D(0, 0, 0));
            Assert.AreEqual(0, meshBuilder.Positions.Count);
            Assert.AreEqual(0, meshBuilder.TriangleIndices.Count);

            var p3 = new List<Point3D>
                {
                    new Point3D(0, 0, 0),
                    new Point3D(1, 1, 0),
                };
            meshBuilder.AddPolygon(p3);
            Assert.AreEqual(0, meshBuilder.Positions.Count);
            Assert.AreEqual(0, meshBuilder.TriangleIndices.Count);
        }

        [Test]
        public void AddValidPolygon()
        {
            var meshBuilder = new MeshBuilder(false, false);
            Assert.AreEqual(0, meshBuilder.Positions.Count);
            Assert.AreEqual(0, meshBuilder.TriangleIndices.Count);
            var p = new List<Point>
                {
                    new Point(0, 0 ),
                    new Point(1, 1 ),
                    new Point(2, 2 ),
                };

            meshBuilder.AddPolygon(p, new Vector3D(0, 1, 0), new Vector3D(1, 0, 0), new Point3D(0, 0, 0));
            Assert.AreEqual(3, meshBuilder.Positions.Count);
            Assert.AreEqual(3, meshBuilder.TriangleIndices.Count);

            var p3 = new List<Point3D>
                {
                    new Point3D(0, 0, 0),
                    new Point3D(1, 1, 0),
                    new Point3D(2, 2, 0),
                };
            meshBuilder.AddPolygon(p3);
            Assert.AreEqual(6, meshBuilder.Positions.Count);
            Assert.AreEqual(6, meshBuilder.TriangleIndices.Count);



        }

    }
}