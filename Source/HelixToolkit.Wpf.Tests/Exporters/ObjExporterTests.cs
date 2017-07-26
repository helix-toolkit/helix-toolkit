// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjExporterTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

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
        public void ShouldThrowExceptionIfMaterialsFileIsNotSpecified()
        {
            string path = "temp.obj";

            try
            {
                var e = new ObjExporter();
                using (var stream = File.Create(path))
                {
                    Assert.Throws<InvalidOperationException>(() => this.ExportSimpleModel(e, stream));
                }
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        [Test]
        public void Export_SimpleModel_ValidOutput()
        {
            string path = "temp.obj";
            string mtlPath = Path.ChangeExtension(path, ".mtl");

            try
            {
                var e = new ObjExporter { MaterialsFile = mtlPath };
                using (var stream = File.Create(path))
                {
                    this.ExportSimpleModel(e, stream);
                }
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);

                if(File.Exists(mtlPath))
                    File.Delete(mtlPath);
            }
        }

        [Test]
        public void Export_BoxWithGradientTexture_TextureExportedAsPng()
        {
            var path = "box_gradient_png.obj";
            var mtlPath = Path.ChangeExtension(path, ".mtl");

            try
            {
                var e = new ObjExporter { MaterialsFile = mtlPath };
                using (var stream = File.Create(path))
                {
                    this.ExportModel(e, stream, () => new BoxVisual3D { Material = Materials.Rainbow });
                }
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);

                if (File.Exists(mtlPath))
                    File.Delete(mtlPath);

                if (File.Exists("mat1.png"))
                    File.Delete("mat1.png");
            }
        }

        [Test]
        public void Export_BoxWithGradientTexture_TextureExportedAsJpg()
        {
            var path = "box_gradient_jpg.obj";
            var mtlPath = Path.ChangeExtension(path, ".mtl");

            try
            {
                var e = new ObjExporter { TextureExtension = ".jpg", MaterialsFile = mtlPath };
                using (var stream = File.Create(path))
                {
                    this.ExportModel(e, stream, () => new BoxVisual3D { Material = Materials.Rainbow });
                }
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);

                if (File.Exists(mtlPath))
                    File.Delete(mtlPath);

                if (File.Exists("mat1.jpg"))
                    File.Delete("mat1.jpg");
            }
        }
    }
}