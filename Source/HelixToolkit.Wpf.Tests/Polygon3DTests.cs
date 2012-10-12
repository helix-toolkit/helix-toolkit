// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Polygon3DTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkitTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class Polygon3DTests
    {
        [Test]
        public void IsPlanar_PlanarPolygon_ReturnsTrue()
        {
            var p = new Polygon3D();
            p.Points.Add(new Point3D(0, 0, 0));
            p.Points.Add(new Point3D(1, 0, 0));
            p.Points.Add(new Point3D(1, 1, 0.76));
            p.Points.Add(new Point3D(0, 1, 0.76));
            Assert.IsTrue(p.IsPlanar());
        }

        [Test]
        public void IsPlanar_NotPlanarPolygon_ReturnsFalse()
        {
            var p = new Polygon3D();
            p.Points.Add(new Point3D(0, 0, 0));
            p.Points.Add(new Point3D(1, 0, 0));
            p.Points.Add(new Point3D(1, 1, 0));
            p.Points.Add(new Point3D(0, 1, 0.3));
            Assert.IsFalse(p.IsPlanar());
        }

        [Test]
        public void GetNormal_PlanarPolygon_ReturnsCorrectResult()
        {
            var p = new Polygon3D();
            p.Points.Add(new Point3D(0, 0, 0));
            p.Points.Add(new Point3D(1, 0, 0));
            p.Points.Add(new Point3D(1, 1, 0));
            p.Points.Add(new Point3D(0, 1, 0));
            Assert.AreEqual(new Vector3D(0, 0, 1), p.GetNormal());
        }

        [Test]
        public void Flatten_PlanarPolygon_ReturnsCorrectResult()
        {
            var p = new Polygon3D();
            p.Points.Add(new Point3D(0, 0, 4));
            p.Points.Add(new Point3D(1, 0, 4));
            p.Points.Add(new Point3D(1, 1, 4));
            p.Points.Add(new Point3D(0, 1, 4));
            var p2 = p.Flatten();
            Assert.AreEqual(p2.Points.Count, 4);
            Assert.AreEqual(new Point(0, 0), p2.Points[0]);
            Assert.AreEqual(new Point(1, 0), p2.Points[1]);
            Assert.AreEqual(new Point(1, 1), p2.Points[2]);
            Assert.AreEqual(new Point(0, 1), p2.Points[3]);
            var tri = p2.Triangulate();
            Assert.AreEqual(6, tri.Count);
        }

        [Test]
        public void Flatten_PlanarPolygon2_ReturnsCorrectResult()
        {
            var p = new Polygon3D();
            p.Points.Add(new Point3D(0, 0, 4));
            p.Points.Add(new Point3D(1, 0, 4));
            p.Points.Add(new Point3D(1, 1, 4.01));
            p.Points.Add(new Point3D(0, 1, 4.01));
            var p2 = p.Flatten();
            Assert.AreEqual(p2.Points.Count, 4);
            var tri = p2.Triangulate();
            Assert.AreEqual(6, tri.Count);
        }
    }
}