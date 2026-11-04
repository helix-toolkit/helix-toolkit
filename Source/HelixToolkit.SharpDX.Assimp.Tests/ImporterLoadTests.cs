using NUnit.Framework;
using NUnit.Framework.Legacy;
using SharpAssimp;
using SharpAssimp.Configs;
using System.Diagnostics.CodeAnalysis;

namespace HelixToolkit.SharpDX.Assimp.Tests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class ImporterLoadTests
    {
        [Test]
        public void Load_WithPropertyConfigs_AppliesConfigsToConfiguration()
        {
            var configs = new PropertyConfig[] { new RemoveDegeneratePrimitivesConfig(true) };

            using var importer = new Importer();
            importer.Load("missing.unsupported", true, PostProcessSteps.Triangulate, configs);

            ClassicAssert.IsTrue(importer.Configuration.EnableParallelProcessing);
            ClassicAssert.AreEqual(PostProcessSteps.Triangulate, importer.Configuration.AssimpPostProcessSteps);
            Assert.That(importer.Configuration.AssimpPropertyConfig, Is.SameAs(configs));
        }
    }
}
