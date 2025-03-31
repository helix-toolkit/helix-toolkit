using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.Importers;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class LwoReaderTests
{
    [SetUp]
    public void SetUp()
    {
        string dir = Path.GetDirectoryName(typeof(LwoReaderTests).Assembly.Location) ?? "";
        dir = Path.Combine(dir!, string.Concat(Enumerable.Repeat("..\\", 5)));

        if (Path.GetFullPath(dir).EndsWith("Source\\", StringComparison.OrdinalIgnoreCase))
        {
            dir = Path.Combine(dir, "..\\");
        }

        Directory.SetCurrentDirectory(dir);
    }

    [Test]
    public void Read_Teddy_ValidModel()
    {
        var r = new LwoReader();
        var model = r.Read(@"Models\lwo\teddy.lwo");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(3, model.Children.Count);
        var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
        var m1 = (MeshGeometry3D)((GeometryModel3D)model.Children[1]).Geometry;
        var m2 = (MeshGeometry3D)((GeometryModel3D)model.Children[2]).Geometry;
        ClassicAssert.AreEqual(1848, m0.TriangleIndices.Count / 3);
        ClassicAssert.AreEqual(96, m1.TriangleIndices.Count / 3);
        ClassicAssert.AreEqual(168, m2.TriangleIndices.Count / 3);
    }

    [Test, Ignore("")]
    public void Read_Apple_ValidModel()
    {
        // LWO2 not yet supported
        var r = new LwoReader();
        var model = r.Read(@"Models\lwo\apple.lwo");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
        ClassicAssert.AreEqual(0, m0.TriangleIndices.Count / 3);
    }
}
