// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjExporterTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkitTests
{
    using System.Diagnostics.CodeAnalysis;
    using HelixToolkit.Wpf;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
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