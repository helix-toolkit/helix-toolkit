// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LwoReaderTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkitTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class LwoReaderTests
    {
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

        [Test, Ignore]
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