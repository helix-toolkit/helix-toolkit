// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjExporterTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using HelixToolkit.Wpf;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class ObjExporterTests : ExporterTests
    {
        [Test]
        public void Export_SimpleModel_ValidOutput()
        {
            string path = "temp.obj";
            var e = new ObjExporter();
            using (var stream = File.Create(path))
            {
                this.ExportSimpleModel(e, stream);
            }
        }

        [Test]
        public void Export_BoxWithGradientTexture_TextureExportedAsPng()
        {
            var path = "box_gradient_png.obj";
            var e = new ObjExporter();
            using (var stream = File.Create(path))
            {
                this.ExportModel(e, stream, () => new BoxVisual3D { Material = Materials.Rainbow });
            }
        }

        [Test]
        public void Export_BoxWithGradientTexture_TextureExportedAsJpg()
        {
            var path = "box_gradient_jpg.obj";
            var e = new ObjExporter { TextureExtension = ".jpg" };
            using (var stream = File.Create(path))
            {
                this.ExportModel(e, stream, () => new BoxVisual3D { Material = Materials.Rainbow });
            }
        }
    }
}