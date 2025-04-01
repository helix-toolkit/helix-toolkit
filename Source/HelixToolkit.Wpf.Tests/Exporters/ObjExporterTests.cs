using HelixToolkit.Geometry;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Numerics;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.Exporters;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class ObjExporterTests : ExporterTests
{
    [SetUp]
    public void SetUp()
    {
        string dir = Path.GetDirectoryName(typeof(ObjExporterTests).Assembly.Location) ?? "";
        dir = Path.Combine(dir!, string.Concat(Enumerable.Repeat("..\\", 5)));

        if (Path.GetFullPath(dir).EndsWith("Source\\", StringComparison.OrdinalIgnoreCase))
        {
            dir = Path.Combine(dir, "..\\");
        }

        Directory.SetCurrentDirectory(dir);
    }

    [Test]
    public void ShouldThrowExceptionIfMaterialsFileIsNotSpecified()
    {
        string temp = Path.GetTempFileName();
        string path = temp + "temp.obj";

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

            File.Delete(temp);
        }
    }

    [Test]
    public void Export_SimpleModel_ValidOutput()
    {
        string temp = Path.GetTempFileName();
        string path = temp + "temp.obj";
        string mtlPath = temp + "temp.mtl";

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

            if (File.Exists(mtlPath))
                File.Delete(mtlPath);

            File.Delete(temp);
        }
    }

    [Test]
    public void Export_BoxWithGradientTexture_TextureExportedAsPng()
    {
        string temp = Path.GetTempFileName();
        var path = temp + "box_gradient_png.obj";
        var mtlPath = temp + "box_gradient_png.mtl";

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

            try
            {
                if (File.Exists("mat1.png"))
                    File.Delete("mat1.png");
            }
            catch (IOException)
            {
            }

            File.Delete(temp);
        }
    }

    [Test]
    public void Export_BoxWithGradientTexture_TextureExportedAsJpg()
    {
        string temp = Path.GetTempFileName();
        var path = temp + "box_gradient_jpg.obj";
        var mtlPath = temp + "box_gradient_jpg.mtl";

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

            try
            {
                if (File.Exists("mat1.jpg"))
                    File.Delete("mat1.jpg");
            }
            catch (IOException)
            {
            }

            File.Delete(temp);
        }
    }

    [Test, Apartment(ApartmentState.STA)]
    public void Wpf_Export_Triangle_Valid()
    {
        var b1 = new MeshBuilder();
        b1.AddTriangle(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0));
        var meshGeometry = b1.ToMesh().ToWndMeshGeometry3D();

        var mesh = new MeshGeometryVisual3D();
        mesh.MeshGeometry = meshGeometry;
        mesh.Material = Materials.Green;
        mesh.Transform = new TranslateTransform3D(2, 0, 0);

        var viewport = new HelixViewport3D();
        viewport.Items.Add(mesh);

        string temp = Path.GetTempFileName();
        string tempName = Path.GetFileName(temp);
        var objPath = temp + "model.obj";
        var mtlPath = temp + "model.mtl";

        try
        {
            viewport.Export(objPath);

            string contentObj = File.ReadAllText(objPath);
            string expectedObj = @"mtllib ./" + tempName + @"model.mtl
o object1
g group1
usemtl mat1
v 2 0 0
v 2 0 1
v 2 1 0
# 3 vertices
vt 0 1
vt 1 1
vt 0 0
# 3 texture coordinates
f 1/1 2/2 3/3
# 1 faces

";

            ClassicAssert.AreEqual(expectedObj.Replace("\r\n", "\n"), contentObj.Replace("\r\n", "\n"));

            string contentMtl = File.ReadAllText(mtlPath);
        }
        finally
        {
            if (File.Exists(objPath))
                File.Delete(objPath);

            if (File.Exists(mtlPath))
                File.Delete(mtlPath);

            File.Delete(temp);
        }
    }

    [Test]
    public void Export_SwitchYZ_Default()
    {
        var originalMesh = new MeshGeometry3D
        {
            Positions = { new Point3D(0, 1, 0) },
            Normals = { new Vector3D(0, 1, 0) },
            TriangleIndices = { 0, 0, 0 }
        };

        byte[] buffer;

        using (var memory = new MemoryStream())
        using (var writer = new StreamWriter(memory))
        {
            var exporter = new ObjExporter();
            exporter.ExportNormals = true;
            exporter.ExportMesh(writer, originalMesh, Transform3D.Identity);

            writer.Flush();
            buffer = memory.ToArray();
        }

        Model3DGroup modelGroup;

        using (var memory = new MemoryStream(buffer))
        {
            var reader = new ObjReader();
            modelGroup = reader.Read(memory);
        }

        var model3D = (GeometryModel3D)modelGroup.Children[0];
        var modelMesh = (MeshGeometry3D)model3D.Geometry;

        ClassicAssert.AreEqual(originalMesh.Positions[0], modelMesh.Positions[0]);
        ClassicAssert.AreEqual(originalMesh.Normals[0], modelMesh.Normals[0]);
    }

    [Test]
    public void Export_SwitchYZ_True()
    {
        var originalMesh = new MeshGeometry3D
        {
            Positions = { new Point3D(0, 1, 0) },
            Normals = { new Vector3D(0, 1, 0) },
            TriangleIndices = { 0, 0, 0 }
        };

        byte[] buffer;

        using (var memory = new MemoryStream())
        using (var writer = new StreamWriter(memory))
        {
            var exporter = new ObjExporter();
            exporter.SwitchYZ = true;
            exporter.ExportNormals = true;
            exporter.ExportMesh(writer, originalMesh, Transform3D.Identity);

            writer.Flush();
            buffer = memory.ToArray();
        }

        Model3DGroup modelGroup;

        using (var memory = new MemoryStream(buffer))
        {
            var reader = new ObjReader();
            reader.SwitchYZ = true;
            modelGroup = reader.Read(memory);
        }

        var model3D = (GeometryModel3D)modelGroup.Children[0];
        var modelMesh = (MeshGeometry3D)model3D.Geometry;

        ClassicAssert.AreEqual(originalMesh.Positions[0], modelMesh.Positions[0]);
        ClassicAssert.AreEqual(originalMesh.Normals[0], modelMesh.Normals[0]);
    }

    [Test]
    public void Export_SwitchYZ_False()
    {
        var originalMesh = new MeshGeometry3D
        {
            Positions = { new Point3D(0, 1, 0) },
            Normals = { new Vector3D(0, 1, 0) },
            TriangleIndices = { 0, 0, 0 }
        };

        byte[] buffer;

        using (var memory = new MemoryStream())
        using (var writer = new StreamWriter(memory))
        {
            var exporter = new ObjExporter();
            exporter.SwitchYZ = false;
            exporter.ExportNormals = true;
            exporter.ExportMesh(writer, originalMesh, Transform3D.Identity);

            writer.Flush();
            buffer = memory.ToArray();
        }

        Model3DGroup modelGroup;

        using (var memory = new MemoryStream(buffer))
        {
            var reader = new ObjReader();
            reader.SwitchYZ = false;
            modelGroup = reader.Read(memory);
        }

        var model3D = (GeometryModel3D)modelGroup.Children[0];
        var modelMesh = (MeshGeometry3D)model3D.Geometry;

        ClassicAssert.AreEqual(originalMesh.Positions[0], modelMesh.Positions[0]);
        ClassicAssert.AreEqual(originalMesh.Normals[0], modelMesh.Normals[0]);
    }
}
