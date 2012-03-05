// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjExporterTests.cs" company="Helix 3D Toolkit">
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
    public class ObjExporterTests : ExporterTests
    {

        [Test]
        public void Export_SimpleModel_ValidOutput()
        {
            string path = "temp.obj";
            using (var e = new ObjExporter(path))
            {
                ExportSimpleModel(e);
            }
        }
    }
}