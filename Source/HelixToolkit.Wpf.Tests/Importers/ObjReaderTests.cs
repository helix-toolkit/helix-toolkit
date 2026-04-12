using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace HelixToolkit.Wpf.Tests.Importers;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class ObjReaderTests
{
    private ObjReader _objReader;

    [SetUp]
    public void SetUp()
    {
        string dir = Path.GetDirectoryName(typeof(ObjReaderTests).Assembly.Location) ?? "";
        dir = Path.Combine(dir!, string.Concat(Enumerable.Repeat("..\\", 5)));

        if (Path.GetFullPath(dir).EndsWith("Source\\", StringComparison.OrdinalIgnoreCase))
        {
            dir = Path.Combine(dir, "..\\");
        }

        Directory.SetCurrentDirectory(dir);

        _objReader = new ObjReader();
    }

    [Test, Ignore("")]
    public void Read_Bunny_ValidModel()
    {
        var model = _objReader.Read(@"Models\obj\bunny\bunny.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(2, model.Children.Count);
        var gm1 = model.Children[1] as GeometryModel3D;
        Assert.That(gm1, Is.Not.Null);
        var mg1 = gm1.Geometry as MeshGeometry3D;
        Assert.That(mg1, Is.Not.Null);
        ClassicAssert.AreEqual(69451, mg1.TriangleIndices.Count / 3);
    }

    [Test]
    public void Read_CornellBox_ValidModel()
    {
        var model = _objReader.Read(@"Models\obj\cornell_box\cornell_box.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(9, model.Children.Count);
        var gm1 = model.Children[1] as GeometryModel3D;
        Assert.That(gm1, Is.Not.Null);
        var mg1 = gm1.Geometry as MeshGeometry3D;
        Assert.That(mg1, Is.Not.Null);
        //// ClassicAssert.AreEqual(69451, mg1.TriangleIndices.Count / 3);
    }

    [Test, Ignore("")]
    public void Read_Ducky_ValidModel()
    {
        var model = _objReader.Read(@"Models\obj\ducky.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(4, model.Children.Count);
        var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
        var m1 = (MeshGeometry3D)((GeometryModel3D)model.Children[1]).Geometry;
        var m2 = (MeshGeometry3D)((GeometryModel3D)model.Children[2]).Geometry;
        var m3 = (MeshGeometry3D)((GeometryModel3D)model.Children[3]).Geometry;
        ClassicAssert.AreEqual(5632, m0.TriangleIndices.Count / 3);
        ClassicAssert.AreEqual(4800, m1.TriangleIndices.Count / 3);
        ClassicAssert.AreEqual(3024, m2.TriangleIndices.Count / 3);
        ClassicAssert.AreEqual(672, m3.TriangleIndices.Count / 3);
    }

    [Test]
    public void Read_Test_ValidModel()
    {
        var model = _objReader.Read(@"Models\obj\test\test.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(2, model.Children.Count);
        var gm1 = model.Children[0] as GeometryModel3D;
        Assert.That(gm1, Is.Not.Null);
        var mg1 = gm1.Geometry as MeshGeometry3D;
        Assert.That(mg1, Is.Not.Null);
        ClassicAssert.AreEqual(12, mg1.TriangleIndices.Count / 3);
    }

    [Test]
    public void Read_SmoothingOff_ValidModel()
    {
        var model = _objReader.Read(@"Models\obj\SmoothingOff.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var gm1 = model.Children[0] as GeometryModel3D;
        Assert.That(gm1, Is.Not.Null);
        var mg1 = gm1.Geometry as MeshGeometry3D;
        Assert.That(mg1, Is.Not.Null);
        ClassicAssert.AreEqual(4, mg1.TriangleIndices.Count / 3);
    }

    [Test]
    public void Read_SmoothingGroup0_ValidModel()
    {
        var model = _objReader.Read(@"Models\obj\SmoothingGroup0.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var gm1 = model.Children[0] as GeometryModel3D;
        Assert.That(gm1, Is.Not.Null);
        var mg1 = gm1.Geometry as MeshGeometry3D;
        Assert.That(mg1, Is.Not.Null);
        ClassicAssert.AreEqual(4, mg1.TriangleIndices.Count / 3);
    }

    [Test]
    public void Read_SmoothingGroup1_ValidModel()
    {
        var model = _objReader.Read(@"Models\obj\SmoothingGroup1.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var gm1 = model.Children[0] as GeometryModel3D;
        Assert.That(gm1, Is.Not.Null);
        var mg1 = gm1.Geometry as MeshGeometry3D;
        Assert.That(mg1, Is.Not.Null);
        ClassicAssert.AreEqual(4, mg1.TriangleIndices.Count / 3);
    }

    [Test]
    public void CanParseFaceWithRelativeIndices()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\face_relative_vertices.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var mesh = model.Children[0].GetMesh();
        mesh.Positions.AssertContains(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void CanParseFaceWithAbsoluteNormals()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\simple_triangle_with_normals.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var mesh = model.Children[0].GetMesh();
        mesh.Normals.AssertContains(new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d });
    }

    [Test]
    public void CanParseFaceWithRelativeNormals()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\face_relative_vertex_normals.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var mesh = model.Children[0].GetMesh();
        mesh.Normals.AssertContains(new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d }, new[] { 0d, 1d, 0d });
    }

    [Test]
    public void CanParseFaceWithAbsoluteTextureCoords()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\simple_triangle_with_texture.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var mesh = model.Children[0].GetMesh();
        mesh.TextureCoordinates.AssertContains(new[] { 0d, 0d }, new[] { 0d, 0d }, new[] { 0d, 0d });
    }

    [Test]
    public void CanParseFaceWithRelativeTextureCoords()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\face_relative_texture_vertices.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var mesh = model.Children[0].GetMesh();
        mesh.TextureCoordinates.AssertContains(new[] { 0d, 0d }, new[] { 0d, 0d }, new[] { 0d, 0d });
    }

    [Test]
    public void CanParseSimpleTriangle()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\simple_triangle.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        model.Children[0].AssertHasVertices(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void CanParseLineContinuations()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\line_continuation_single.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        model.Children[0].AssertHasVertices(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void CanParseLineContinuationsWithMultipleBreaks()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\line_continuation_multiple_breaks.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        model.Children[0].AssertHasVertices(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void CanParseLineContinuationsWithEmptyContinuations()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\line_continuation_empty_continuation.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        model.Children[0].AssertHasVertices(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void CanParseLineContinuationsWithEmptyLineInMiddle()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\line_continuation_empty_line.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        model.Children[0].AssertHasVertices(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
    }

    [Test]
    public void CanParseLineContinuationsInComments()
    {
        var model = _objReader.Read(@"Models\obj\obj_format_tests\line_continuation_comment.obj");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        model.Children[0].AssertHasVertices(new[] { -1d, 0d, 1d }, new[] { 1d, 0d, 1d }, new[] { -1d, 0d, -1d });
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

        var expectedPositions = new Point3D[]
        {
                new Point3D(0.0, 0.0, 0.0),
                new Point3D(1.0, 0.0, 0.0),
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(1.0, 1.0, 0.0),
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(1.0, 0.0, 0.0)
        };

        var expectedTextureCoordinates = new Point[]
        {
                new Point(0.0, 1.0),
                new Point(0.5, 1.0),
                new Point(0.0, 0.5),
                new Point(1.0, 0.0),
                new Point(0.0, 0.0),
                new Point(1.0, 1.0)
        };

        var buffer = Encoding.UTF8.GetBytes(content);

        using var stream = new MemoryStream(buffer, false);
        var model = _objReader.Read(stream);
        var mesh = model.Children[0].GetMesh();

        ClassicAssert.AreEqual(6, mesh.TriangleIndices.Count);
        ClassicAssert.AreEqual(6, mesh.Positions.Count);
        ClassicAssert.AreEqual(6, mesh.TextureCoordinates.Count);

        Assert.That(mesh.Positions, Is.EqualTo(expectedPositions).AsCollection);
        Assert.That(mesh.TextureCoordinates, Is.EqualTo(expectedTextureCoordinates).AsCollection);
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

        var expectedPositions = new Point3D[]
        {
                new Point3D(0.0, 0.0, 0.0),
                new Point3D(1.0, 0.0, 0.0),
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(1.0, 1.0, 0.0),
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(1.0, 0.0, 0.0)
        };

        var expectedTextureCoordinates = new Point[]
        {
                new Point(0.0, 1.0),
                new Point(0.5, 1.0),
                new Point(0.0, 0.5),
                new Point(1.0, 0.0),
                new Point(0.0, 0.0),
                new Point(1.0, 1.0)
        };

        var buffer = Encoding.UTF8.GetBytes(content);

        using var stream = new MemoryStream(buffer, false);
        var model = _objReader.Read(stream);
        var mesh = model.Children[0].GetMesh();

        ClassicAssert.AreEqual(6, mesh.TriangleIndices.Count);
        ClassicAssert.AreEqual(6, mesh.Positions.Count);
        ClassicAssert.AreEqual(6, mesh.TextureCoordinates.Count);

        Assert.That(mesh.Positions, Is.EqualTo(expectedPositions).AsCollection);
        Assert.That(mesh.TextureCoordinates, Is.EqualTo(expectedTextureCoordinates).AsCollection);
    }

    [TestCase("")]
    [TestCase("/")]
    [TestCase("./")]
    public void TexturePath_AbsoluteOrRelative_Valid(string prefix)
    {
        var tempObj = Path.GetTempFileName();
        var tempMtl = Path.GetTempFileName();
        var tempTexDiffuse = Path.GetTempFileName();
        var tempTexAmbient = Path.GetTempFileName();

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
map_Ka " + prefix + Path.GetFileName(tempTexAmbient) + @"
");

            using (var image = new System.Drawing.Bitmap(1, 1))
            {
                image.Save(tempTexDiffuse);
                image.Save(tempTexAmbient);
            }

            var model = _objReader.Read(tempObj);
            Assert.That(model, Is.Not.Null);
            var geometry = (GeometryModel3D)model.Children[0];
            var materialGroup = (MaterialGroup)geometry.Material;

            var diffuseMaterial = (DiffuseMaterial)materialGroup.Children[0];
            var diffuseSource = ((ImageBrush)diffuseMaterial.Brush).ImageSource.ToString();
            ClassicAssert.AreEqual(tempTexDiffuse, diffuseSource);

            var ambientMaterial = (EmissiveMaterial)materialGroup.Children[1];
            var ambientSource = ((ImageBrush)ambientMaterial.Brush).ImageSource.ToString();
            ClassicAssert.AreEqual(tempTexAmbient, ambientSource);
        }
        finally
        {
            File.Delete(tempObj);
            File.Delete(tempMtl);
            File.Delete(tempTexDiffuse);
            File.Delete(tempTexAmbient);
        }
    }

    [Test]
    public void TexturePath_Absolute_Valid()
    {
        var tempObj = Path.GetTempFileName();
        var tempMtl = Path.GetTempFileName();
        var tempTexDiffuse = Path.GetTempFileName();
        var tempTexAmbient = Path.GetTempFileName();

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
map_Ka " + tempTexAmbient + @"
");

            using (var image = new System.Drawing.Bitmap(1, 1))
            {
                image.Save(tempTexDiffuse);
                image.Save(tempTexAmbient);
            }

            var model = _objReader.Read(tempObj);
            Assert.That(model, Is.Not.Null);
            var geometry = (GeometryModel3D)model.Children[0];
            var materialGroup = (MaterialGroup)geometry.Material;

            var diffuseMaterial = (DiffuseMaterial)materialGroup.Children[0];
            var diffuseSource = ((ImageBrush)diffuseMaterial.Brush).ImageSource.ToString();
            ClassicAssert.AreEqual(tempTexDiffuse, diffuseSource);

            var ambientMaterial = (EmissiveMaterial)materialGroup.Children[1];
            var ambientSource = ((ImageBrush)ambientMaterial.Brush).ImageSource.ToString();
            ClassicAssert.AreEqual(tempTexAmbient, ambientSource);
        }
        finally
        {
            File.Delete(tempObj);
            File.Delete(tempMtl);
            File.Delete(tempTexDiffuse);
            File.Delete(tempTexAmbient);
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
            ClassicAssert.AreEqual(Color.FromRgb(255, 255, 255), _objReader.Materials["TestMaterial"].Diffuse);
        }
        finally
        {
            File.Delete(tempObj);
            File.Delete(tempMtl);
        }
    }

    [Test]
    public void Name_Valid()
    {
        string content = @"
g group1

v 0 0 0
v 1 0 0
v 0 1 0

f 1 2 3
";

        string expectedName = "group1";

        var buffer = Encoding.UTF8.GetBytes(content);

        using var stream = new MemoryStream(buffer, false);
        var model = new ObjReader().Read(stream);
        var mesh = model.Children[0];

        string name = mesh.GetName();

        ClassicAssert.AreEqual(name, expectedName);
    }
}
