using System.Numerics;
using NUnit.Framework;

namespace HelixToolkit.Maths.Tests;

[TestFixture]
[NonParallelizable]
public class Vector4ClampTests
{
    [TestCase(false, -2, 3)]
    [TestCase(false, 5, 5)]
    [TestCase(false, 10, 7)]
    [TestCase(true, -2, 3)]
    [TestCase(true, 5, 5)]
    [TestCase(true, 10, 7)]
    public void ClampUsesEachComponentsBounds(bool enableSimd, float w, float expectedW)
    {
        var previous = MathSettings.EnableSIMD;
        try
        {
            MathSettings.EnableSIMD = enableSimd;
            var value = new Vector4(-1, 10, 4, w);
            var minimum = new Vector4(0, 1, 2, 3);
            var maximum = new Vector4(4, 5, 6, 7);

            var result = value.Clamp(minimum, maximum);

            Assert.That(result, Is.EqualTo(new Vector4(0, 5, 4, expectedW)));
        }
        finally
        {
            MathSettings.EnableSIMD = previous;
        }
    }
}
