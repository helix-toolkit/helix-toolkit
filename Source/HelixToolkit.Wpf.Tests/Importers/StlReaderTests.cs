using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.IO;

namespace HelixToolkit.Wpf.Tests.Importers;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class StlReaderTests
{
    //// test models from http://orion.math.iastate.edu/burkardt/data/stl/stl.html

    [SetUp]
    public void SetUp()
    {
        string dir = Path.GetDirectoryName(typeof(StlReaderTests).Assembly.Location) ?? "";
        dir = Path.Combine(dir!, string.Concat(Enumerable.Repeat("..\\", 5)));

        if (Path.GetFullPath(dir).EndsWith("Source\\", StringComparison.OrdinalIgnoreCase))
        {
            dir = Path.Combine(dir, "..\\");
        }

        Directory.SetCurrentDirectory(dir);
    }

    [Test]
    public void Read_Bottle_ValidModel()
    {
        var r = new StLReader();
        var model = r.Read(@"Models\stl\bottle.stl");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual("FLIRIS", r.Header);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
        ClassicAssert.AreEqual(1240, m0.TriangleIndices.Count / 3);
    }

    [Test]
    public void Read_Bottle_DefaultMaterialChanged()
    {
        var r = new StLReader();
        r.DefaultMaterial = MaterialHelper.CreateMaterial(Colors.Aqua);
        var model = r.Read(@"Models\stl\bottle.stl");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var gm0 = (GeometryModel3D)model.Children[0];
        var m0 = (MaterialGroup)gm0.Material;
        var dm = (DiffuseMaterial)m0.Children[0];
        ClassicAssert.AreEqual(Colors.Aqua, ((SolidColorBrush)dm.Brush).Color);
    }

    [Test]
    public void Read_BinaryBottle_ValidModel()
    {
        // ASCII format converted to BINARY by MeshLab
        var r = new StLReader();
        var model = r.Read(@"Models\stl\binary\bottle.stl");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual("VCG", r.Header);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
        ClassicAssert.AreEqual(1240, m0.TriangleIndices.Count / 3);
    }

    [Test]
    public void Read_Teapot_ValidModel()
    {
        var r = new StLReader();
        var model = r.Read(@"Models\stl\teapot.stl");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
        ClassicAssert.AreEqual(2016, m0.TriangleIndices.Count / 3);
    }

    [Test]
    public void Read_Magnolia_ValidModel()
    {
        var r = new StLReader();
        var model = r.Read(@"Models\stl\magnolia.stl");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
        ClassicAssert.AreEqual(1247, m0.TriangleIndices.Count / 3);
    }

    [Test]
    public void Read_Sphere_ValidModel()
    {
        var r = new StLReader();
        var model = r.Read(@"Models\stl\sphere.stl");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
        ClassicAssert.AreEqual(228, m0.TriangleIndices.Count / 3);
    }

    [Test]
    public void Read_Cube_ValidModel()
    {
        // reader does not yet support quads
        var r = new StLReader();
        var model = r.Read(@"Models\stl\cube.stl");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;

        // Expects 6 quad faces => 12 triangles
        ClassicAssert.AreEqual(12, m0.TriangleIndices.Count / 3);
    }

    [Test]
    public void Read_BottleFromStream_ValidModel()
    {
        Model3DGroup? model;
        using (var s = File.OpenRead(@"Models\stl\bottle.stl"))
        {
            var r = new StLReader();
            model = r.Read(s);
        }

        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
        ClassicAssert.AreEqual(1240, m0.TriangleIndices.Count / 3);
    }
}
