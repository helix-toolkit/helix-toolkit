namespace HelixToolkit.Wpf.Tests.Geometry
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    namespace HelixToolkit.WPF.Tests
    {
        [TestFixture]
        public class PolygonTriangulationTests
        {
            [Test]
            public void TestBaseCaseTriangle()
            {
                List<Point> triangleInPositiveOrientation = new List<Point>()
                {
                    new Point(0, 0), new Point(1, 0), new Point(0, 1)
                };

                Int32Collection indices = SweepLinePolygonTriangulator.Triangulate(triangleInPositiveOrientation);
                Assert.That(indices.Count == 3);
                Assert.That(indices[0] == 0);
                Assert.That(indices[1] == 1);
                Assert.That(indices[2] == 2);


                List<Point> triangleInNegativeOrientation = new List<Point>()
                {
                    new Point(0, 0), new Point(0, 1),  new Point(1, 0)
                };

                indices = SweepLinePolygonTriangulator.Triangulate(triangleInNegativeOrientation);
                Assert.That(indices.Count == 3);
                Assert.That(indices[0] == 0);
                Assert.That(indices[1] == 2);
                Assert.That(indices[2] == 1);
            }
        }
    }
}