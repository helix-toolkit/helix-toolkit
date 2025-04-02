using HelixToolkit.Tests;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace HelixToolkit.Maths.Tests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class Vector4HelperTests
    {
        [TestCase(1, 100)]
        [TestCase(3, 100)]
        [TestCase(5, 100)]
        [TestCase(7, 100)]
        [TestCase(11, 100)]
        [TestCase(1001, 100)]
        [TestCase(10001, 10)]
        [TestCase(101233, 10)]
        public void Transform(int size, int iteration)
        {
            var data = new Vector4[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = Utils.GetRandomVector4();
            }
            var result = new Vector4[size];
            for (int iter = 0; iter < iteration; ++iter)
            {
                var transform = Matrix4x4.CreateFromAxisAngle(Vector3.Normalize(Utils.GetRandomVector3()), Utils.GetRandomAngleRadian())
                    * Matrix4x4.CreateTranslation(Utils.GetRandomVector3());
                data.Transform(ref transform, result);
                for (int i = 0; i < size; i++)
                {
                    Vector4Helper.Transform(ref data[i], ref transform, out var expected);
                    ClassicAssert.AreEqual(expected.X, result[i].X, 1e-5);
                    ClassicAssert.AreEqual(expected.Y, result[i].Y, 1e-5);
                    ClassicAssert.AreEqual(expected.Z, result[i].Z, 1e-5);
                    ClassicAssert.AreEqual(expected.W, result[i].W, 1e-5);
                }
            }
        }
    }
}