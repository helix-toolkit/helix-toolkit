using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HelixToolkit.Wpf.Tests.ExtensionMethods
{
    [TestFixture]
    public class ConverterExtensionsTests
    {
        [Test]
        public void ToFloatArray()
        {
            double[]? nullDoubles = null;
            ClassicAssert.AreEqual(null, nullDoubles.ToFloatArray(), "Float array is not null");
            double[] emptyDoubles = new double[0];
            ClassicAssert.AreEqual(Array.Empty<float>(), emptyDoubles.ToFloatArray(), "Float array is not empty");
            double[] doubles = new double[] { -1.125d, -1d, 2d, 2.122d };
            ClassicAssert.AreEqual(new float[] { -1.125f, -1f, 2f, 2.122f }, doubles.ToFloatArray(), "Double array is not convert to Float array correctly");
        }
    }
}
