// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineSegmentTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
{
    using System.Windows;

    using NUnit.Framework;

    [TestFixture]
    public class LineSegmentTests
    {
        [Test]
        public void IntersectsWith()
        {
            var line1 = new LineSegment(new Point(0, 0), new Point(1, 0));
            var line2 = new LineSegment(new Point(1, 1), new Point(2, 1));
            Assert.IsFalse(line1.IntersectsWith(line2), "collinear");
            var line3 = new LineSegment(new Point(1, 1), new Point(0, 0));
            Assert.IsTrue(line1.IntersectsWith(line3), "intersecting");
            var line4 = new LineSegment(new Point(1, 1), new Point(-1, 0));
            Assert.IsFalse(line1.IntersectsWith(line4), "intersection is outside segment");
        }

        [Test]
        public void IntersectTest()
        {
            var a1 = new Point(0, 0);
            var a2 = new Point(1, 0);
            var b1 = new Point(1, 1);
            var b2 = new Point(2, 1);

            Assert.IsFalse(LineSegment.AreLineSegmentsIntersecting(a1, a2, b1, b2));
            b2 = new Point(0, 0);
            Assert.IsTrue(LineSegment.AreLineSegmentsIntersecting(a1, a2, b1, b2));
            b2 = new Point(-1, 0);
            Assert.IsFalse(LineSegment.AreLineSegmentsIntersecting(a1, a2, b1, b2));
        }
    }
}