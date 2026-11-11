using System.Numerics;
using NUnit.Framework;

namespace HelixToolkit.Maths.Tests;

[TestFixture]
public class OrientedBoundingBoxLineTests
{
    [TestCase(-2, 0, 0, 2, 0, 0, ContainmentType.Intersects)]
    [TestCase(2, 0, 0, -2, 0, 0, ContainmentType.Intersects)]
    [TestCase(0, -2, 0, 0, 2, 0, ContainmentType.Intersects)]
    [TestCase(0, 0, -2, 0, 0, 2, ContainmentType.Intersects)]
    [TestCase(-2, 2, 0, 2, 2, 0, ContainmentType.Disjoint)]
    [TestCase(-3, 0, 0, -2, 0, 0, ContainmentType.Disjoint)]
    [TestCase(0, 0, 0, 0.5f, 0.5f, 0.5f, ContainmentType.Contains)]
    [TestCase(0, 0, 0, 2, 0, 0, ContainmentType.Intersects)]
    public void ContainsLineUsesBothEndpoints(float x1, float y1, float z1, float x2, float y2, float z2, ContainmentType expected)
    {
        var box = new OrientedBoundingBox(new Vector3(-1), new Vector3(1));
        var first = new Vector3(x1, y1, z1);
        var second = new Vector3(x2, y2, z2);

        Assert.That(box.ContainsLine(ref first, ref second), Is.EqualTo(expected));
    }

    [Test]
    public void ContainsLineTransformsBothEndpointsIntoBoxSpace()
    {
        var box = new OrientedBoundingBox(new Vector3(-1), new Vector3(1));
        box.Transformation = Matrix4x4.CreateRotationZ(0.5f) * Matrix4x4.CreateTranslation(4, 5, 6);
        var first = Vector3.Transform(new Vector3(-2, 0, 0), box.Transformation);
        var second = Vector3.Transform(new Vector3(2, 0, 0), box.Transformation);

        Assert.That(box.ContainsLine(ref first, ref second), Is.EqualTo(ContainmentType.Intersects));
    }
}
