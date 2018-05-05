// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Polygon3DTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests.Geometry
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media.Media3D;

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

        [Test]
        public void Polygon_GetNormal_NotThrows()
        {
            var points = new List<Point3D>()
            {
                new Point3D(-1.39943, 0.328622, 0.97968),
                new Point3D(-1.39969, 0.328622, 0.99105),
                new Point3D(-1.39963, 0.328631, 0.99105),
                new Point3D(-1.39954, 0.328631, 0.98726),
                new Point3D(-1.39937, 0.328631, 0.97968),
            };

            var polygon = new Polygon3D(points);
            Vector3D normal = polygon.GetNormal();
        }
    }
}
