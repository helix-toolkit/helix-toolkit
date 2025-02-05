using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Schema;

namespace HelixToolkit.Wpf.Tests.Exporters;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
[TestFixture]
public class X3DExporterTests : ExporterTests
{
    [SetUp]
    public void SetUp()
    {
        string dir = Path.GetDirectoryName(typeof(X3DExporterTests).Assembly.Location) ?? "";
        dir = Path.Combine(dir!, string.Concat(Enumerable.Repeat("..\\", 5)));
        Directory.SetCurrentDirectory(dir);
    }

    [Test]
    public void Export_SimpleModel_ValidOutput()
    {
        string temp = Path.GetTempFileName();
        string path = temp + "_temp.x3d";

        try
        {
            var e = new X3DExporter();
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

        // Add the schema to the collection.
        string dir = @"Schemas\x3d\";
        sc.Add("http://www.web3d.org/specifications/x3d-3.1.xsd", dir + "x3d-3.1.xsd");
        //// sc.Add("http://www.web3d.org/specifications/x3d-3.1-Web3dExtensionsPublic.xsd", dir + "x3d-3.1-Web3dExtensionsPublic.xsd");
        //// sc.Add("http://www.web3d.org/specifications/x3d-3.1-Web3dExtensionsPrivate.xsd", dir + "x3d-3.1-Web3dExtensionsPrivate.xsd");

        return this.Validate(path, sc);
    }
}
