using NUnit.Framework;
using System.IO;

namespace HelixToolkit.Wpf.Tests.Importers;

/// <summary>
/// Provides unit tests for the <see cref="OffReader" /> class.
/// </summary>
[TestFixture]
public class OffReaderTests
{
    [SetUp]
    public void SetUp()
    {
        string dir = Path.GetDirectoryName(typeof(OffReaderTests).Assembly.Location) ?? "";
        dir = Path.Combine(dir!, string.Concat(Enumerable.Repeat("..\\", 5)));
        Directory.SetCurrentDirectory(dir);
    }

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
                Assert.That(reader.Faces, Is.Not.Empty);
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
                Assert.That(reader.Faces, Is.Not.Empty);
            }
        }
    }
}
