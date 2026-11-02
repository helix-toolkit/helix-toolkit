using HelixToolkit.SharpDX.Model;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using SharpAssimp;
using System.Diagnostics.CodeAnalysis;

namespace HelixToolkit.SharpDX.Assimp.Tests
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class ImporterMaterialTests
    {
        private sealed class TestImporter : Importer
        {
            public PBRMaterialCore CreatePBRMaterial(Material material)
            {
                return OnCreatePBRMaterial(material);
            }
        }

        [Test]
        public void CreatePBRMaterial_ReadsGltfMetallicAndRoughnessFactors()
        {
            var material = new Material();
            material.AddProperty(new MaterialProperty(GLTFMatKeys.AI_MATKEY_GLTF_METALLIC_FACTOR, 0.2f));
            material.AddProperty(new MaterialProperty(GLTFMatKeys.AI_MATKEY_GLTF_ROUGHNESS_FACTOR, 0.7f));

            using var importer = new TestImporter();
            var pbr = importer.CreatePBRMaterial(material);

            ClassicAssert.AreEqual(0.2f, pbr.MetallicFactor, 1e-6f);
            ClassicAssert.AreEqual(0.7f, pbr.RoughnessFactor, 1e-6f);
        }
    }
}
