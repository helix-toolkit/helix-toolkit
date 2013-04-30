namespace HelixToolkit.Wpf.Tests.Geometry
{
    using System.Collections.Generic;
    using System.Windows;

    using NUnit.Framework;

    [TestFixture]
    public class CuttingEarsTriangulatorTests
    {
        [Test]
        public void Triangulate_Discussion440914_ShouldBeValid()
        {
            var polygon = new[]
                              {
                                  new Point(0, 0),
                                  new Point(0, 1.894),
                                  new Point(-2.536, 1.42),
                                  new Point(-5.072, 1.42),
                                  new Point(-5.072, 2.84),
                                  new Point(-10.144, 2.84),
                                  new Point(-10.144, 0)
                              };
            var result = CuttingEarsTriangulator.Triangulate(polygon);
            Assert.IsNotNull(result);
            Assert.AreEqual(5 * 3, result.Count);
        }
    }
}