// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LwoReaderTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;
    using NUnit.Framework;
    using System.IO;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class LwoReaderTests
    {
        [SetUp]
        public void SetUp()
        {
            var dir = Path.GetDirectoryName(typeof(LwoReaderTests).Assembly.Location);
            Directory.SetCurrentDirectory(dir);
        }
        [Test]
        public void Read_Teddy_ValidModel()
        {
            var r = new LwoReader();
            var model = r.Read(@"Models\lwo\teddy.lwo");
            Assert.AreEqual(3, model.Children.Count);
            var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
            var m1 = (MeshGeometry3D)((GeometryModel3D)model.Children[1]).Geometry;
            var m2 = (MeshGeometry3D)((GeometryModel3D)model.Children[2]).Geometry;
            Assert.AreEqual(1848, m0.TriangleIndices.Count / 3);
            Assert.AreEqual(96, m1.TriangleIndices.Count / 3);
            Assert.AreEqual(168, m2.TriangleIndices.Count / 3);
        }

        [Test, Ignore("")]
        public void Read_Apple_ValidModel()
        {
            // LWO2 not yet supported
            var r = new LwoReader();
            var model = r.Read(@"Models\lwo\apple.lwo");
            Assert.AreEqual(1, model.Children.Count);
            var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
            Assert.AreEqual(0, m0.TriangleIndices.Count / 3);
        }
    }
}