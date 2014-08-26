// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriangleTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
{
    using System.Windows;

    using NUnit.Framework;

    [TestFixture]
    public class TriangleTests
    {
        [Test]
        public void IsCompletelyInside()
        {
            var triangle = new Triangle(new Point(0, 0), new Point(2, 0), new Point(0, 3));

            Assert.IsFalse(triangle.IsCompletelyInside(new Rect(0, 0, 2, 2)), "partly inside");
            Assert.IsTrue(triangle.IsCompletelyInside(new Rect(0, 0, 2, 4)), "completely inside");
            Assert.IsTrue(triangle.IsCompletelyInside(new Rect(0, 0, 2, 3)), "completely inside");
            Assert.IsFalse(triangle.IsCompletelyInside(new Rect(0, 0, 0.25, 0.25)), "not inside");
            Assert.IsTrue(triangle.IsCompletelyInside(new Rect(-1, -1, 5, 5)), "completely inside");
        }

        [Test]
        public void IsPointInside()
        {
            var triangle = new Triangle(new Point(0, 0), new Point(2, 0), new Point(0, 3));

            Assert.IsTrue(triangle.IsPointInside(new Point(1, 1)), "inside");
            Assert.IsFalse(triangle.IsPointInside(new Point(2, 3)), "outside");
            Assert.IsFalse(triangle.IsPointInside(new Point(1, 0)), "on edge");
            Assert.IsFalse(triangle.IsPointInside(new Point(0, 0)), "on vertex");
        }

        [Test]
        public void IsPointInsideDegenerateTriangle()
        {
            // collinear points
            var triangle = new Triangle(new Point(0, 0), new Point(2, 0), new Point(3, 0));
            Assert.IsFalse(triangle.IsPointInside(new Point(1, 0)), "on edge");
            Assert.IsFalse(triangle.IsPointInside(new Point(0, 3)), "outside");
        }

        [Test]
        public void IsRectInside()
        {
            var triangle = new Triangle(new Point(0, 0), new Point(2, 0), new Point(0, 3));
            Assert.IsTrue(triangle.IsRectCompletelyInside(new Rect(1e-10, 1e-10, 1, 1)), "completely inside");
            Assert.IsFalse(triangle.IsRectCompletelyInside(new Rect(0, 0, 1, 1)), "touching rectangle");
            Assert.IsFalse(triangle.IsRectCompletelyInside(new Rect(0, 0, 2, 2)), "partly outside");
            Assert.IsFalse(triangle.IsRectCompletelyInside(new Rect(2, 2, 1, 1)), "completely outside");
        }
    }
}
