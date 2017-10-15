// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeshBuilderTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
{
    using System.Diagnostics.CodeAnalysis;
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
    }
}