// --------------------------------------------------------------------------------------------------------------------
// <copyright file="X3DExporterTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Schema;
using HelixToolkit.Wpf;
using NUnit.Framework;
using NUnitHelpers;

namespace HelixToolkitTests
{
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
            var result = Validate(path);
            Assert.IsNull(result, result);
        }

        string Validate(string path)
        {
            var sc = new XmlSchemaSet();

            // Add the schema to the collection.
            string dir = @"..\..\..\..\Schemas\x3d\";
            sc.Add("http://www.web3d.org/specifications/x3d-3.1.xsd", dir + "x3d-3.1.xsd");
            // sc.Add("http://www.web3d.org/specifications/x3d-3.1-Web3dExtensionsPublic.xsd", dir + "x3d-3.1-Web3dExtensionsPublic.xsd");
            // sc.Add("http://www.web3d.org/specifications/x3d-3.1-Web3dExtensionsPrivate.xsd", dir + "x3d-3.1-Web3dExtensionsPrivate.xsd");
            return Validate(path, sc);
        }
    }
}