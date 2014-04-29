// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OffReaderTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   Provides unit tests for the <see cref="OffReader" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
{
    using System.IO;

    using NUnit.Framework;

    /// <summary>
    /// Provides unit tests for the <see cref="OffReader" /> class.
    /// </summary>
    [TestFixture]
    public class OffReaderTests
    {
        /// <summary>
        /// Tests the <see cref="OffReader.Load" /> method.
        /// </summary>
        public class Load
        {
            /// <summary>
            /// Given a simple test file, the loading should be successful.
            /// </summary>
            [Test]
            public void BoxCube()
            {
                var reader = new OffReader();
                using (var stream = File.OpenRead(@"Models\off\boxcube.off"))
                {
                    reader.Load(stream);
                    Assert.IsTrue(reader.Faces.Count > 0);
                }
            }

            /// <summary>
            /// Given a test file where integer values are formatted with decimals, the loading should be successful.
            /// </summary>
            [Test]
            public void BadIntValues()
            {
                var reader = new OffReader();
                using (var stream = File.OpenRead(@"Models\off\BadIntValues.off"))
                {
                    reader.Load(stream);
                    Assert.IsTrue(reader.Faces.Count > 0);
                }
            }
        }
    }
}