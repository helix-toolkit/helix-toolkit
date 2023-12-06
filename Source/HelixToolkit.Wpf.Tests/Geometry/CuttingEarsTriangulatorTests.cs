using NUnit.Framework;
using System.Windows;

namespace HelixToolkit.Wpf.Tests.Geometry;

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
        var result = CuttingEarsTriangulator.Triangulate(polygon.Select(t => t.ToVector()).ToList());
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(5 * 3, result.Count);
        Assert.AreEqual(new[] { 0, 1, 2, 3, 4, 5, 6, 0, 2, 3, 5, 6, 6, 2, 3 }, result);
    }

    [Test]
    public void Triangulate_Discussion440914b_ShouldBeValid()
    {
        var polygon = new[]
                          {
                                  new Point(0, 0),
                    new Point(0, 2.97),
                    new Point(-2.389999999999997, 2.97),
                    new Point(-2.389999999999997, 1.4849999999999999),
                    new Point(-4.7799999999999976, 1.4849999999999999),
                    new Point(-4.7799999999999976, 2.97),
                    new Point(-9.5599999999999987, 2.97),
                    new Point(-9.5599999999999987, 0)
                              };
        var result = CuttingEarsTriangulator.Triangulate(polygon.Select(t => t.ToVector()).ToList());
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(6 * 3, result.Count);
        Assert.AreEqual(new[] { 0, 1, 2, 4, 5, 6, 0, 2, 3, 4, 6, 7, 7, 0, 3, 3, 4, 7 }, result);
    }
}
