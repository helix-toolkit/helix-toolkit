using HelixToolkit.Tests;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace HelixToolkit.Geometry.Tests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class NormalizationTests
    {
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(7)]
        [TestCase(11)]
        [TestCase(101)]
        [TestCase(1001)]
        [TestCase(10003)]
        [TestCase(101233)]
        public void NormalizeInPlaceTest(int size)
        {          
            var positions = new Vector3Collection(size);
            
            for (int i = 0; i < size; i++)
            {
                positions.Add(Utils.GetRandomVector3());
            }
            var cloned = new Vector3Collection(positions);

            MeshGeometryHelper.NormalizeInPlace(positions);

            for (int i = 0; i < size; i++)
            {
                ClassicAssert.AreEqual(1, positions[i].Length(), 4e-4);
                var expected = Vector3.Normalize(cloned[i]);
                ClassicAssert.AreEqual(expected.X, positions[i].X, 4e-4);
                ClassicAssert.AreEqual(expected.Y, positions[i].Y, 4e-4);
                ClassicAssert.AreEqual(expected.Z, positions[i].Z, 4e-4);
            }
        }
    }
}