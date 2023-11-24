using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Tests.Elements3D;
using NUnit.Framework;
using SharpDX;
using System.IO;
using System.Text;

namespace HelixToolkit.Wpf.SharpDX.Tests.Importers;

[TestFixture]
class ObjReaderTests
{
    private ObjReader _objReader;

    [SetUp]
    public void SetUp()
    {
        _objReader = new ObjReader();
        string dir = Path.GetDirectoryName(typeof(ObjReaderTests).Assembly.Location) ?? "";
        dir = Path.Combine(dir!, string.Concat(Enumerable.Repeat("..\\", 5)));
        Directory.SetCurrentDirectory(dir);
    }

    /// <summary>
    /// Test for pull request #52.
    /// Fix ArgumentOutOfRangeException in MeshBuilder.ComputeNormals.
    /// </summary>
    [Test]
    public void LoadModelWithoutNormals()
    {
        var objects = _objReader.Read(@"Models\obj\cornell_box\cornell_box.obj");

        Assert.That(objects, Is.Not.Null);
        Assert.AreEqual(9, objects.Count);

        var floorGeometry = objects[0].Geometry as MeshGeometry3D;

        Assert.That(floorGeometry, Is.Not.Null);
        Assert.That(floorGeometry.Positions, Is.Not.Null);
        Assert.AreEqual(4, floorGeometry.Positions.Count);
        Assert.That(floorGeometry.Normals, Is.Not.Null);
        Assert.AreEqual(4, floorGeometry.Normals.Count);
    }


    [Test]
    public void CanParseFaceWithRelativeIndices()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\face_relative_vertices.obj");

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(1, model.Count);
        Assert.That(model[0].Geometry, Is.Not.Null);
        var geometry = (MeshGeometry3D)model[0].Geometry;
        Assert.That(geometry, Is.Not.Null);
        Assert.That(geometry.Positions, Is.Not.Null);
        geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void CanParseFaceWithAbsoluteNormals()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\simple_triangle_with_normals.obj");

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(1, model.Count);
        Assert.That(model[0].Geometry, Is.Not.Null);
        var geometry = (MeshGeometry3D)model[0].Geometry;
        Assert.That(geometry, Is.Not.Null);
        Assert.That(geometry.Normals, Is.Not.Null);
        geometry.Normals.AssertContains(new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d });
    }

    [Test]
    public void CanParseFaceWithRelativeNormals()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\face_relative_vertex_normals.obj");

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(1, model.Count);
        Assert.That(model[0].Geometry, Is.Not.Null);
        var geometry = (MeshGeometry3D)model[0].Geometry;
        Assert.That(geometry, Is.Not.Null);
        Assert.That(geometry.Normals, Is.Not.Null);
        geometry.Normals.AssertContains(new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d });
    }

    [Test]
    public void CanParseFaceWithAbsoluteTextureCoords()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\simple_triangle_with_texture.obj");

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(1, model.Count);
        Assert.That(model[0].Geometry, Is.Not.Null);
        var geometry = (MeshGeometry3D)model[0].Geometry;
        Assert.That(geometry, Is.Not.Null);
        Assert.That(geometry.TextureCoordinates, Is.Not.Null);
        geometry.TextureCoordinates.AssertContains(new[] { 0d, 0d }, new[] { 0d, 0d }, new[] { 0d, 0d });
    }

    [Test]
    public void CanParseFaceWithRelativeTextureCoords()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\face_relative_texture_vertices.obj");

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(1, model.Count);
        Assert.That(model[0].Geometry, Is.Not.Null);
        var geometry = (MeshGeometry3D)model[0].Geometry;
        Assert.That(geometry, Is.Not.Null);
        Assert.That(geometry.TextureCoordinates, Is.Not.Null);
        geometry.TextureCoordinates.AssertContains(new[] { 0d, 0d }, new[] { 0d, 0d }, new[] { 0d, 0d });
    }

    [Test]
    public void CanParseSimpleTriangle()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\simple_triangle.obj");

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(1, model.Count);
        Assert.That(model[0].Geometry, Is.Not.Null);
        var geometry = model[0].Geometry;
        Assert.That(geometry, Is.Not.Null);
        Assert.That(geometry.Positions, Is.Not.Null);
        geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void CanParseLineContinuations()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\line_continuation_single.obj");

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(1, model.Count);
        Assert.That(model[0].Geometry, Is.Not.Null);
        var geometry = model[0].Geometry;
        Assert.That(geometry, Is.Not.Null);
        Assert.That(geometry.Positions, Is.Not.Null);
        geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void CanParseLineContinuationsWithMultipleBreaks()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\line_continuation_multiple_breaks.obj");

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(1, model.Count);
        Assert.That(model[0].Geometry, Is.Not.Null);
        var geometry = model[0].Geometry;
        Assert.That(geometry, Is.Not.Null);
        Assert.That(geometry.Positions, Is.Not.Null);
        geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void CanParseLineContinuationsWithEmptyContinuations()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\line_continuation_empty_continuation.obj");

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(1, model.Count);
        Assert.That(model[0].Geometry, Is.Not.Null);
        var geometry = model[0].Geometry;
        Assert.That(geometry, Is.Not.Null);
        Assert.That(geometry.Positions, Is.Not.Null);
        geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void CanParseLineContinuationsWithEmptyLineInMiddle()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\line_continuation_empty_line.obj");

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(1, model.Count);
        Assert.That(model[0].Geometry, Is.Not.Null);
        var geometry = model[0].Geometry;
        Assert.That(geometry, Is.Not.Null);
        Assert.That(geometry.Positions, Is.Not.Null);
        geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void CanParseLineContinuationsInComments()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\line_continuation_comment.obj");

        Assert.That(model, Is.Not.Null);
        Assert.AreEqual(1, model.Count);
        Assert.That(model[0].Geometry, Is.Not.Null);
        var geometry = model[0].Geometry;
        Assert.That(geometry, Is.Not.Null);
        Assert.That(geometry.Positions, Is.Not.Null);
        geometry.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void BuildModel_SmoothingOff_Valid()
    {
        string content = @"
s off

v 0 0 0
v 1 0 0
v 0 1 0
v 1 1 0

vt 0.0 0.0
vt 0.5 0.0
vt 0.0 0.5
vt 1.0 1.0
vt 0.0 1.0
vt 1.0 0.0

f 1/1 2/2 3/3
f 4/4 3/5 2/6
";

        var expectedPositions = new Vector3[]
        {
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f)
        };

        var expectedTextureCoordinates = new Vector2[]
        {
                new Vector2(0.0f, 1.0f),
                new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, 0.5f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 1.0f)
        };

        var buffer = Encoding.UTF8.GetBytes(content);

        using (var stream = new MemoryStream(buffer, false))
        {
            var model = _objReader.Read(stream);
            Assert.That(model, Is.Not.Null);
            Assert.That(model[0].Geometry, Is.Not.Null);
            var mesh = (MeshGeometry3D)model[0].Geometry;
            Assert.That(mesh, Is.Not.Null);
            Assert.That(mesh.Indices, Is.Not.Null);
            Assert.That(mesh.Positions, Is.Not.Null);
            Assert.That(mesh.TextureCoordinates, Is.Not.Null);

            Assert.AreEqual(6, mesh.Indices.Count);
            Assert.AreEqual(6, mesh.Positions.Count);
            Assert.AreEqual(6, mesh.TextureCoordinates.Count);

            CollectionAssert.AreEqual(expectedPositions, mesh.Positions);
            CollectionAssert.AreEqual(expectedTextureCoordinates, mesh.TextureCoordinates);
        }
    }

    [Test]
    public void BuildModel_SmoothingOn_Valid()
    {
        string content = @"
s 1

v 0 0 0
v 1 0 0
v 0 1 0
v 1 1 0

vt 0.0 0.0
vt 0.5 0.0
vt 0.0 0.5
vt 1.0 1.0
vt 0.0 1.0
vt 1.0 0.0

f 1/1 2/2 3/3
f 4/4 3/5 2/6
";

        var expectedPositions = new Vector3[]
        {
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(1.0f, 0.0f, 0.0f)
        };

        var expectedTextureCoordinates = new Vector2[]
        {
                new Vector2(0.0f, 1.0f),
                new Vector2(0.5f, 1.0f),
                new Vector2(0.0f, 0.5f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 1.0f)
        };

        var buffer = Encoding.UTF8.GetBytes(content);

        using (var stream = new MemoryStream(buffer, false))
        {
            var model = _objReader.Read(stream);
            Assert.That(model, Is.Not.Null);
            Assert.That(model[0].Geometry, Is.Not.Null);
            var mesh = (MeshGeometry3D)model[0].Geometry;
            Assert.That(mesh, Is.Not.Null);
            Assert.That(mesh.Indices, Is.Not.Null);
            Assert.That(mesh.Positions, Is.Not.Null);
            Assert.That(mesh.TextureCoordinates, Is.Not.Null);
            Assert.AreEqual(6, mesh.Indices.Count);
            Assert.AreEqual(6, mesh.Positions.Count);
            Assert.AreEqual(6, mesh.TextureCoordinates.Count);

            Assert.AreEqual(6, mesh.Indices.Count);
            Assert.AreEqual(6, mesh.Positions.Count);
            Assert.AreEqual(6, mesh.TextureCoordinates.Count);

            CollectionAssert.AreEqual(expectedPositions, mesh.Positions);
            CollectionAssert.AreEqual(expectedTextureCoordinates, mesh.TextureCoordinates);
        }
    }

    [TestCase("")]
    [TestCase("/")]
    [TestCase("./")]
    public void TexturePath_AbsoluteOrRelative_Valid(string prefix)
    {
        var tempObj = Path.GetTempFileName();
        var tempMtl = Path.GetTempFileName();
        var tempTexDiffuse = Path.GetTempFileName();
        var tempTexBump = Path.GetTempFileName();

        try
        {
            File.WriteAllText(tempObj, @"
mtllib " + prefix + Path.GetFileName(tempMtl) + @"
v -0.5 0 0.5
v 0.5 0 0.5
v -0.5 0 -0.5
vt 0 1
usemtl TestMaterial
f 1/1 2/1 3/1
");

            File.WriteAllText(tempMtl, @"
newmtl TestMaterial
map_Kd " + prefix + Path.GetFileName(tempTexDiffuse) + @"
map_bump " + prefix + Path.GetFileName(tempTexBump) + @"
");

            using (var image = new System.Drawing.Bitmap(1, 1))
            {
                image.Save(tempTexDiffuse);
                image.Save(tempTexBump);
            }

            var model = _objReader.Read(tempObj);
            Assert.That(model, Is.Not.Null);
            Assert.That(model[0].Material, Is.Not.Null);
            var material = (PhongMaterialCore)model[0].Material;
            Assert.That(material, Is.Not.Null);
            using (var fs = new FileStream(tempTexDiffuse, FileMode.Open))
            {
                Assert.That(material.DiffuseMap, Is.Not.Null);
                var t = material.DiffuseMap.Load();
                Compare(fs, t.Texture);
                material.DiffuseMap.Complete(t, true);
            }
            using (var fs = new FileStream(tempTexBump, FileMode.Open))
            {
                Assert.That(material.NormalMap, Is.Not.Null);
                var t = material.NormalMap.Load();
                Compare(fs, t.Texture);
                material.NormalMap.Complete(t, true);
            }
        }
        finally
        {
            File.Delete(tempObj);
            File.Delete(tempMtl);
            //File.Delete(tempTexDiffuse);
            //File.Delete(tempTexBump);
        }
    }

    [Test]
    public void TexturePath_Absolute_Valid()
    {
        var tempObj = Path.GetTempFileName();
        var tempMtl = Path.GetTempFileName();
        var tempTexDiffuse = Path.GetTempFileName();
        var tempTexBump = Path.GetTempFileName();

        try
        {
            File.WriteAllText(tempObj, @"
mtllib " + tempMtl + @"
v -0.5 0 0.5
v 0.5 0 0.5
v -0.5 0 -0.5
vt 0 1
usemtl TestMaterial
f 1/1 2/1 3/1
");

            File.WriteAllText(tempMtl, @"
newmtl TestMaterial
map_Kd " + tempTexDiffuse + @"
map_bump " + tempTexBump + @"
");

            using (var image = new System.Drawing.Bitmap(1, 1))
            {
                image.Save(tempTexDiffuse);
                image.Save(tempTexBump);
            }

            var model = _objReader.Read(tempObj);
            Assert.That(model, Is.Not.Null);
            Assert.That(model[0].Material, Is.Not.Null);
            var material = (PhongMaterialCore)model[0].Material;
            Assert.That(material, Is.Not.Null);
            using (var fs = new FileStream(tempTexDiffuse, FileMode.Open))
            {
                Assert.That(material.DiffuseMap, Is.Not.Null);
                var t = material.DiffuseMap.Load();
                Compare(fs, t.Texture);
                material.DiffuseMap.Complete(t, true);
            }
            using (var fs = new FileStream(tempTexBump, FileMode.Open))
            {
                Assert.That(material.NormalMap, Is.Not.Null);
                var t = material.NormalMap.Load();
                Compare(fs, t.Texture);
                material.NormalMap.Complete(t, true);
            }
        }
        finally
        {
            File.Delete(tempObj);
            File.Delete(tempMtl);
            //File.Delete(tempTexDiffuse);
            //File.Delete(tempTexBump);
        }
    }

    public void Compare(Stream s1, Stream s2)
    {
        Assert.AreEqual(s1.Length, s2.Length);
        s2.Position = 0;
        s1.Position = 0;
        for (int i = 0; i < s1.Length; ++i)
        {
            Assert.AreEqual(s1.ReadByte(), s2.ReadByte());
        }
    }

    [Test]
    public void MaterialLib_LoadMultipleTimes_Valid()
    {
        var tempObj = Path.GetTempFileName();
        var tempMtl = Path.GetTempFileName();

        try
        {
            File.WriteAllText(tempObj, @"
mtllib " + Path.GetFileName(tempMtl) + @"
mtllib " + Path.GetFileName(tempMtl) + @"
v -0.5 0 0.5
v 0.5 0 0.5
v -0.5 0 -0.5
vt 0 1
usemtl TestMaterial
f 1/1 2/1 3/1
");

            File.WriteAllText(tempMtl, @"
newmtl TestMaterial
Kd 1 1 1
newmtl TestMaterial
Kd 0 0 0
");

            var model = _objReader.Read(tempObj);
            Assert.AreEqual(new Color4(1, 1, 1, 1), _objReader.Materials["TestMaterial"].Diffuse);
        }
        finally
        {
            File.Delete(tempObj);
            File.Delete(tempMtl);
        }
    }
}
