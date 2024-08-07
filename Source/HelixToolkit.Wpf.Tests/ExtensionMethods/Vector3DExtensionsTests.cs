using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.ExtensionMethods;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class Vector3DExtensionsTests
{
    [Test]
    public void FindAnyPerpendicular_GivenUnitX_ReturnsUnitZ()
    {
        var v = new Vector3D(1, 0, 0);
        var p = v.FindAnyPerpendicular();
        ClassicAssert.AreEqual(new Vector3D(0, 0, -1), p);
        ClassicAssert.AreEqual(new Vector3D(1, 0, 0), v); // check that the input vector is unchanged
    }

    [Test]
    public void FindAnyPerpendicular_GivenSmallVector_ReturnsUnitZ()
    {
        var v = new Vector3D(1e-100, 0, 0);
        var p = v.FindAnyPerpendicular();
        ClassicAssert.AreEqual(new Vector3D(0, 0, -1), p);
        ClassicAssert.AreEqual(new Vector3D(1e-100, 0, 0), v); // check that the input vector is unchanged
    }

    [Test]
    public void FindAnyPerpendicular_GivenLargeVector_ReturnsUnitZ()
    {
        var v = new Vector3D(1e-100, 1e100, 0);
        var p = v.FindAnyPerpendicular();
        ClassicAssert.AreEqual(new Vector3D(0, 0, 1), p);
        ClassicAssert.AreEqual(new Vector3D(1e-100, 1e100, 0), v); // check that the input vector is unchanged
    }

    [Test]
    public void FindAnyPerpendicular_GivenNullVector_ReturnsUndefinedVector()
    {
        var v = new Vector3D(0, 0, 0);
        var p = v.FindAnyPerpendicular();
        Assert.That(p.IsUndefined());
        ClassicAssert.AreEqual(new Vector3D(0, 0, 0), v); // check that the input vector is unchanged
    }

    [Test]
    public void IsUndefined_GivenNormalizedNullVector_ReturnsTrue()
    {
        var v = new Vector3D(0, 0, 0);
        v.Normalize();
        Assert.That(v.IsUndefined());
    }

    [Test]
    public void ToPoint3D_GivenAVector_ReturnsCorrectPoint()
    {
        var v = new Vector3D(1, 2, 3);
        ClassicAssert.AreEqual(new Point3D(1, 2, 3), v.ToWndPoint3D());
    }
}
