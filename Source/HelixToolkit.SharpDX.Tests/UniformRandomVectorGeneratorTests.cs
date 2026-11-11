using System.Numerics;
using HelixToolkit.SharpDX.Utilities;
using NUnit.Framework;

namespace HelixToolkit.SharpDX.Tests;

[TestFixture]
public class UniformRandomVectorGeneratorTests
{
    [Test]
    public void EqualBoundsReturnTheConfiguredVector()
    {
        var expected = new Vector3(1, 10, 100);
        var generator = new UniformRandomVectorGenerator
        {
            MinVector = expected,
            MaxVector = expected
        };

        Assert.That(generator.RandomVector3, Is.EqualTo(expected));
    }

    [Test]
    public void ComponentsStayWithinTheirOwnBounds()
    {
        var minimum = new Vector3(1, 10, 100);
        var maximum = new Vector3(2, 20, 200);
        var generator = new UniformRandomVectorGenerator
        {
            MinVector = minimum,
            MaxVector = maximum
        };

        for (var iteration = 0; iteration < 100; iteration++)
        {
            var value = generator.RandomVector3;
            Assert.That(value.X, Is.InRange(minimum.X, maximum.X));
            Assert.That(value.Y, Is.InRange(minimum.Y, maximum.Y));
            Assert.That(value.Z, Is.InRange(minimum.Z, maximum.Z));
        }
    }
}
