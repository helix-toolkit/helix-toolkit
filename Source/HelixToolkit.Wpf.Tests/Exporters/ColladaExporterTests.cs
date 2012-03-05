// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColladaExporterTests.cs" company="Helix 3D Toolkit">
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
    public class ColladaExporterTests : ExporterTests
    {

        [Test]
        public void Export_SimpleModel_ValidOutput()
        {
            string path = "temp.dae";
            using (var e = new ColladaExporter(path))
            {
                ExportSimpleModel(e);
            }
            var result = Validate(path);
            Assert.IsNull(result, result);
        }
       
        string Validate(string path)
        {
            var sc = new XmlSchemaSet();
            string dir = @"..\..\..\..\Schemas\Collada\";
            sc.Add("http://www.collada.org/2008/03/COLLADASchema", dir + "collada_schema_1_5.xsd");
            return Validate(path, sc);
        }
    }
}