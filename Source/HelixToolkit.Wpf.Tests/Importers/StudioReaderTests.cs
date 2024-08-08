using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.Tests.Importers;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class StudioReaderTests
{
    [SetUp]
    public void SetUp()
    {
        string dir = Path.GetDirectoryName(typeof(StudioReaderTests).Assembly.Location) ?? "";
        dir = Path.Combine(dir!, string.Concat(Enumerable.Repeat("..\\", 5)));
        Directory.SetCurrentDirectory(dir);
    }

    [Test]
    public void Read_Skeleton_ValidModel()
    {
        var r = new StudioReader();
        var model = r.Read(@"Models\3ds\skeleton.3ds");
        Assert.That(model, Is.Not.Null);
        ClassicAssert.AreEqual(1, model.Children.Count);
        var m0 = (MeshGeometry3D)((GeometryModel3D)model.Children[0]).Geometry;
        ClassicAssert.AreEqual(4595, m0.TriangleIndices.Count / 3);
    }
}
