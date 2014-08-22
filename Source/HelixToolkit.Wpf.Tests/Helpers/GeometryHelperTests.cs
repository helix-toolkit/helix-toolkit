// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeometryHelperTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    using NUnit.Framework;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class GeometryHelperTests
    {
        [Test]
        public void TriangleInsideRectTest()
        {
            var point1 = new Point(0, 0);
            var point2 = new Point(2, 0);
            var point3 = new Point(0, 3);
            var rect = new Rect(new Point(0, 0), new Size(2, 2));

            Assert.IsFalse(GeometryHelper.TriangleInsideRect(point1, point2, point3, rect));
            rect.Size = new Size(2, 4);
            Assert.IsTrue(GeometryHelper.TriangleInsideRect(point1, point2, point3, rect));
            rect.Size = new Size(2, 3);
            Assert.IsTrue(GeometryHelper.TriangleInsideRect(point1, point2, point3, rect));
            rect.Location = new Point(0.5, 1);
            rect.Size = new Size(0.25, 0.25);
            Assert.IsFalse(GeometryHelper.TriangleInsideRect(point1, point2, point3, rect));
            rect.Location = new Point(-1, -1);
            rect.Size = new Size(5, 5);
            Assert.IsTrue(GeometryHelper.TriangleInsideRect(point1, point2, point3, rect));
        }

        [Test]
        public void PointInTriangleTest()
        {
            var point1 = new Point(0, 0);
            var point2 = new Point(2, 0);
            var point3 = new Point(0, 3);
            var point4 = new Point(3, 0);

            var p = new Point(1, 1);

            Assert.IsTrue(GeometryHelper.PointInTriangle(p, point1, point2, point3));
            p = new Point(2, 3);
            Assert.IsFalse(GeometryHelper.PointInTriangle(p, point1, point2, point3));
            p = new Point(1, 0);
            Assert.IsFalse(GeometryHelper.PointInTriangle(p, point1, point2, point3));
            Assert.IsFalse(GeometryHelper.PointInTriangle(point1, point1, point2, point3));
            Assert.IsFalse(GeometryHelper.PointInTriangle(p, point1, point2, point4));
            Assert.IsFalse(GeometryHelper.PointInTriangle(point3, point1, point2, point4));
        }

        [Test]
        public void IntersectTest()
        {
            var a1 = new Point(0, 0);
            var a2 = new Point(1, 0);
            var b1 = new Point(1, 1);
            var b2 = new Point(2, 1);

            Assert.IsFalse(GeometryHelper.Intersect(a1, a2, b1, b2));
            b2 = new Point(0, 0);
            Assert.IsTrue(GeometryHelper.Intersect(a1, a2, b1, b2));
            b2 = new Point(-1, 0);
            Assert.IsFalse(GeometryHelper.Intersect(a1, a2, b1, b2));
        }
    }
}
