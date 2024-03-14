using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Wpf;
using System.Numerics;
using System.Windows;
using System.Windows.Media.Media3D;
namespace HelixToolkit.Wpf.Tests.ExtensionMethods
{
    [TestFixture]
    public class ConverterExtensionsTests
    {
        [Test]
        public void ToFloatArray()
        {
            double[]? nullDoubles = null;
            Assert.AreEqual(null, nullDoubles.ToFloatArray(), "Float array is not null");
            double[] emptyDoubles = new double[0];
            Assert.AreEqual(Array.Empty<float>(), emptyDoubles.ToFloatArray(), "Float array is not empty");
            double[] doubles = new double[] { -1.125d, -1d, 2d, 2.122d };
            Assert.AreEqual(new float[] { -1.125f, -1f, 2f, 2.122f }, doubles.ToFloatArray(), "Double array is not convert to Float array correctly");
        }
    }
}
