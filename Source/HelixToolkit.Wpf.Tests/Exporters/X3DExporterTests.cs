// --------------------------------------------------------------------------------------------------------------------
// <copyright file="X3DExporterTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkitTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Schema;
    using HelixToolkit.Wpf;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class X3DExporterTests : ExporterTests
    {
        [Test]
        public void Export_SimpleModel_ValidOutput()
        {
            string path = "temp.x3d";
            using (var e = new X3DExporter(path))
            {
                ExportSimpleModel(e);
            }

            var result = this.Validate(path);
            Assert.IsNull(result, result);
        }

        private string Validate(string path)
        {
            var sc = new XmlSchemaSet();

            // Add the schema to the collection.
            string dir = @"..\..\..\..\Schemas\x3d\";
            sc.Add("http://www.web3d.org/specifications/x3d-3.1.xsd", dir + "x3d-3.1.xsd");
            //// sc.Add("http://www.web3d.org/specifications/x3d-3.1-Web3dExtensionsPublic.xsd", dir + "x3d-3.1-Web3dExtensionsPublic.xsd");
            //// sc.Add("http://www.web3d.org/specifications/x3d-3.1-Web3dExtensionsPrivate.xsd", dir + "x3d-3.1-Web3dExtensionsPrivate.xsd");

            return Validate(path, sc);
        }
    }
}