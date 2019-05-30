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
    using System.Threading;
    using System.Windows.Media.Media3D;

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

        [Test, Apartment(ApartmentState.STA)]
        public void Wpf_Export_Triangle_Valid()
        {
            var b1 = new MeshBuilder();
            b1.AddTriangle(new Point3D(0, 0, 0), new Point3D(0, 0, 1), new Point3D(0, 1, 0));
            var meshGeometry = b1.ToMesh();

            var mesh = new MeshGeometryVisual3D();
            mesh.MeshGeometry = meshGeometry;
            mesh.Material = Materials.Green;
            mesh.Transform = new TranslateTransform3D(2, 0, 0);

            var viewport = new HelixViewport3D();
            viewport.Items.Add(mesh);

            string temp = Path.GetTempPath();
            var objPath = temp + "model.obj";
            var mtlPath = temp + "model.mtl";

            try
            {
                viewport.Export(objPath);

                string contentObj = File.ReadAllText(objPath);
                string expectedObj = @"mtllib ./model.mtl
o object1
g group1
usemtl mat1
v 2 0 0
v 2 1 0
v 2 0 -1
# 3 vertices
vt 0 1
vt 1 1
vt 0 0
# 3 texture coordinates
f 1/1 2/2 3/3
# 1 faces

";

                Assert.AreEqual(expectedObj.Replace("\r\n", "\n"), contentObj.Replace("\r\n", "\n"));

                string contentMtl = File.ReadAllText(mtlPath);
            }
            finally
            {
                if (File.Exists(objPath))
                    File.Delete(objPath);

                if (File.Exists(mtlPath))
                    File.Delete(mtlPath);
            }
        }
    }
}