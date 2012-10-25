// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StlReaderTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkitTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class StlReaderTests
    {
        //// test models from http://orion.math.iastate.edu/burkardt/data/stl/stl.html

        [Test]
        public void Read_Bottle_ValidModel()
        {
            var r = new StLReader();
            var model = r.Read(@"Models\stl\bottle.stl");
            Assert.AreEqual(1, model.Children.Count);
            var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
            Assert.AreEqual(1240, m0.TriangleIndices.Count / 3);
        }

        [Test]
        public void Read_Teapot_ValidModel()
        {
            var r = new StLReader();
            var model = r.Read(@"Models\stl\teapot.stl");
            Assert.AreEqual(1, model.Children.Count);
            var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
            Assert.AreEqual(2016, m0.TriangleIndices.Count / 3);
        }

        [Test]
        public void Read_Magnolia_ValidModel()
        {
            var r = new StLReader();
            var model = r.Read(@"Models\stl\magnolia.stl");
            Assert.AreEqual(1, model.Children.Count);
            var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
            Assert.AreEqual(1247, m0.TriangleIndices.Count / 3);
        }

        [Test]
        public void Read_Sphere_ValidModel()
        {
            var r = new StLReader();
            var model = r.Read(@"Models\stl\sphere.stl");
            Assert.AreEqual(1, model.Children.Count);
            var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
            Assert.AreEqual(228, m0.TriangleIndices.Count / 3);
        }

        [Test]
        public void Read_Cube_ValidModel()
        {
            // reader does not yet support quads
            var r = new StLReader();
            var model = r.Read(@"Models\stl\cube.stl");
            Assert.AreEqual(1, model.Children.Count);
            var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;

            // Expects 6 quad faces => 12 triangles
            Assert.AreEqual(12, m0.TriangleIndices.Count / 3);
        }

        [Test]
        public void Read_BottleFromStream_ValidModel()
        {
            Model3DGroup model;
            using (var s = File.OpenRead(@"Models\stl\bottle.stl"))
            {
                var r = new StLReader();
                model = r.Read(s);
            }

            Assert.AreEqual(1, model.Children.Count);
            var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
            Assert.AreEqual(1240, m0.TriangleIndices.Count / 3);
        }
    }
}