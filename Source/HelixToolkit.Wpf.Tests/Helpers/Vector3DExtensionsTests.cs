// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vector3DExtensionsTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkitTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;
    using NUnit.Framework;

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
            Assert.AreEqual(new Vector3D(0, 0, -1), p);
            Assert.AreEqual(new Vector3D(1, 0, 0), v); // check that the input vector is unchanged
        }

        [Test]
        public void FindAnyPerpendicular_GivenSmallVector_ReturnsUnitZ()
        {
            var v = new Vector3D(1e-100, 0, 0);
            var p = v.FindAnyPerpendicular();
            Assert.AreEqual(new Vector3D(0, 0, -1), p);
            Assert.AreEqual(new Vector3D(1e-100, 0, 0), v); // check that the input vector is unchanged
        }

        [Test]
        public void FindAnyPerpendicular_GivenLargeVector_ReturnsUnitZ()
        {
            var v = new Vector3D(1e-100, 1e100, 0);
            var p = v.FindAnyPerpendicular();
            Assert.AreEqual(new Vector3D(0, 0, 1), p);
            Assert.AreEqual(new Vector3D(1e-100, 1e100, 0), v); // check that the input vector is unchanged
        }

        [Test]
        public void FindAnyPerpendicular_GivenNullVector_ReturnsUndefinedVector()
        {
            var v = new Vector3D(0, 0, 0);
            var p = v.FindAnyPerpendicular();
            Assert.IsTrue(p.IsUndefined());
            Assert.AreEqual(new Vector3D(0, 0, 0), v); // check that the input vector is unchanged
        }

        [Test]
        public void IsUndefined_GivenNormalizedNullVector_ReturnsTrue()
        {
            var v = new Vector3D(0, 0, 0);
            v.Normalize();
            Assert.IsTrue(v.IsUndefined());
        }

        [Test]
        public void ToPoint3D_GivenAVector_ReturnsCorrectPoint()
        {
            var v = new Vector3D(1, 2, 3);
            Assert.AreEqual(new Point3D(1, 2, 3), v.ToPoint3D());
        }
    }
}