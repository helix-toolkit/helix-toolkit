// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StudioReaderTests.cs" company="Helix 3D Toolkit">
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
    public class StudioReaderTests
    {
        [Test]
        public void Read_Skeleton_ValidModel()
        {
            var r = new StudioReader();
            var model = r.Read(@"Models\3ds\skeleton.3ds");
            Assert.AreEqual(1, model.Children.Count);
            var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
            Assert.AreEqual(4595, m0.TriangleIndices.Count / 3);
        }
    }
}