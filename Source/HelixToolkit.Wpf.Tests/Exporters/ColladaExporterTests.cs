using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Schema;

namespace HelixToolkit.Wpf.Tests.Exporters;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class ColladaExporterTests : ExporterTests
{
    [SetUp]
    public void SetUp()
    {
        string dir = Path.GetDirectoryName(typeof(ColladaExporterTests).Assembly.Location) ?? "";
        dir = Path.Combine(dir!, string.Concat(Enumerable.Repeat("..\\", 5)));

        if (Path.GetFullPath(dir).EndsWith("Source\\", StringComparison.OrdinalIgnoreCase))
        {
            dir = Path.Combine(dir, "..\\");
        }

        Directory.SetCurrentDirectory(dir);
    }

    [Test]
    public void Export_SimpleModel_ValidOutput()
    {
        string temp = Path.GetTempFileName();
        string path = temp + "_temp.dae";

        try
        {
            var e = new ColladaExporter();
            using (var stream = File.Create(path))
            {
                this.ExportSimpleModel(e, stream);
            }

            var result = this.Validate(path);
            Assert.That(result, Is.Null);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);

            if (File.Exists(temp))
                File.Delete(temp);
        }
    }

    private string? Validate(string path)
    {
        var sc = new XmlSchemaSet();
        string dir = @"Schemas\Collada\";
        sc.Add("http://www.collada.org/2008/03/COLLADASchema", dir + "collada_schema_1_5.xsd");
        return this.Validate(path, sc);
    }
}
