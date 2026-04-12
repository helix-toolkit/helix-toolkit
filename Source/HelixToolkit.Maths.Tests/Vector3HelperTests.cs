using HelixToolkit.Tests;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace HelixToolkit.Maths.Tests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class Vector3HelperTests
    {
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(7)]
        [TestCase(11)]
        [TestCase(1001)]
        [TestCase(10001)]
        [TestCase(101233)]
        public void MinMaxTest(int size)
        {
            var data = new Vector3[size];
            var expectedMin = new Vector3(float.MaxValue);
            var expectedMax = new Vector3(float.MinValue);
            for (int i = 0; i < size; i++)
            {
                data[i] = Utils.GetRandomVector3();
                expectedMin = Vector3.Min(expectedMin, data[i]);
                expectedMax = Vector3.Max(expectedMax, data[i]);
            }
            data.MinMax(out var min, out var max);
            ClassicAssert.AreEqual(expectedMin, min);
            ClassicAssert.AreEqual(expectedMax, max);
        }

        [TestCase(1, 100)]
        [TestCase(3, 100)]
        [TestCase(5, 100)]
        [TestCase(7, 100)]
        [TestCase(11, 100)]
        [TestCase(1001, 100)]
        [TestCase(10001, 10)]
        [TestCase(101233, 10)]
        public void TransformCoordinate(int size, int iteration)
        {
            var data = new Vector3[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = Utils.GetRandomVector3();
            }
            var result = new Vector3[size];
            for (int iter = 0; iter < iteration; ++iter)
            {
                var transform = Matrix4x4.CreateFromAxisAngle(Vector3.Normalize(Utils.GetRandomVector3()), Utils.GetRandomAngleRadian()) 
                    * Matrix4x4.CreateTranslation(Utils.GetRandomVector3());
                data.TransformCoordinate(ref transform, result);
                for (int i = 0; i < size; i++)
                {
                    Vector3Helper.TransformCoordinate(ref data[i], ref transform, out var expected);
                    ClassicAssert.AreEqual(expected.X, result[i].X, 1e-5);
                    ClassicAssert.AreEqual(expected.Y, result[i].Y, 1e-5);
                    ClassicAssert.AreEqual(expected.Z, result[i].Z, 1e-5);
                }
            }
        }

        [TestCase(1, 100)]
        [TestCase(3, 100)]
        [TestCase(5, 100)]
        [TestCase(7, 100)]
        [TestCase(11, 100)]
        [TestCase(1001, 100)]
        [TestCase(10001, 10)]
        [TestCase(101233, 10)]
        public void TransformNormal(int size, int iteration)
        {
            var data = new Vector3[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = Utils.GetRandomVector3();
            }
            var result = new Vector3[size];
            for (int iter = 0; iter < iteration; ++iter)
            {
                var transform = Matrix4x4.CreateFromAxisAngle(Vector3.Normalize(Utils.GetRandomVector3()), Utils.GetRandomAngleRadian())
                    * Matrix4x4.CreateTranslation(Utils.GetRandomVector3());
                data.TransformNormal(ref transform, result);
                for (int i = 0; i < size; i++)
                {
                    Vector3Helper.TransformNormal(ref data[i], ref transform, out var expected);
                    ClassicAssert.AreEqual(expected.X, result[i].X, 1e-5);
                    ClassicAssert.AreEqual(expected.Y, result[i].Y, 1e-5);
                    ClassicAssert.AreEqual(expected.Z, result[i].Z, 1e-5);
                }
            }
        }

        [TestCase(1, 100)]
        [TestCase(3, 100)]
        [TestCase(5, 100)]
        [TestCase(7, 100)]
        [TestCase(11, 100)]
        [TestCase(1001, 100)]
        [TestCase(10001, 10)]
        [TestCase(101233, 10)]
        public void GetCentroidTest(int size, int iteration)
        {
            var data = new Vector3[size];

            for (int iter = 0; iter < iteration; ++iter)
            {
                for (int i = 0; i < size; i++)
                {
                    data[i] = Utils.GetRandomVector3();
                }
                var centroid = data.GetCentroid();
                var expected = Vector3.Zero;
                for (int i = 0; i < size; i++)
                {
                    expected += data[i];
                }
                expected /= size;
                ClassicAssert.AreEqual(expected.X, centroid.X, 1e-5);
                ClassicAssert.AreEqual(expected.Y, centroid.Y, 1e-5);
                ClassicAssert.AreEqual(expected.Z, centroid.Z, 1e-5);
            }
        }
    }
}