using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.Importers;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class ModelImporterTests
{
    [SetUp]
    public void SetUp()
    {
        string dir = Path.GetDirectoryName(typeof(ModelImporterTests).Assembly.Location) ?? "";
        dir = Path.Combine(dir!, string.Concat(Enumerable.Repeat("..\\", 5)));
        Directory.SetCurrentDirectory(dir);
    }

    [Test]
    public void Load_TestObj_ValidNumberOfVertices()
    {
        var importer = new ModelImporter();
        var model = importer.Load(@"Models\obj\test\test.obj");
        Assert.That(model, Is.Not.Null);
        int countVertices = 0;
        model.Traverse<GeometryModel3D>((geometryModel, transform) =>
        {
            var mesh = (MeshGeometry3D)geometryModel.Geometry;
            countVertices += mesh.Positions.Count;
        });

        Assert.AreEqual(17, countVertices);
    }
}
