using HelixToolkit.SharpDX;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HelixToolkit.Wpf.SharpDX.Tests.Importers;

public static class TestExtensions
{
    public static void AssertContains(this Vector2Collection collection, params double[][] points)
    {
        ClassicAssert.AreEqual(points.Length, collection.Count, "Expected to find {0} points in collection", points.Length);
        foreach (var point in points)
            Assert.That(collection.Contains(point), $"Expected collection to contain point [{point[0]},{point[1]}]");
    }

    public static void AssertContains(this Vector3Collection collection, params double[][] points)
    {
        ClassicAssert.AreEqual(points.Length, collection.Count, "Expected to find {0} points in collection", points.Length);
        foreach (var point in points)
            Assert.That(collection.Contains(point), $"Expected collection to contain point [{point[0]},{point[1]},{point[2]}]");
    }

    public static bool Contains(this Vector3Collection vectors, double[] expectedVector)
    {
        return vectors.Any(vector => Math.Abs((float)expectedVector[0] - vector.X) < float.Epsilon &&
                                     Math.Abs((float)expectedVector[1] - vector.Y) < float.Epsilon &&
                                     Math.Abs((float)expectedVector[2] - vector.Z) < float.Epsilon);
    }

    public static bool Contains(this Vector2Collection vectors, double[] expectedVector)
    {
        return vectors.Any(vector => Math.Abs((float)expectedVector[0] - vector.X) < float.Epsilon &&
                                     Math.Abs((float)expectedVector[1] - vector.Y) < float.Epsilon);
    }
}
