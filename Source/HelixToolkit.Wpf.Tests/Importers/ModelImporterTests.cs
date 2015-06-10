// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelImporterTests.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    [TestFixture]
    public class ModelImporterTests
    {
        [Test]
        public void Load_TestObj_ValidNumberOfVertices()
        {
            var importer = new ModelImporter();
            var model = importer.Load(@"Models\obj\test.obj");
            int countVertices = 0;
            model.Traverse<GeometryModel3D>((geometryModel, transform) =>
                {
                    var mesh = (MeshGeometry3D)geometryModel.Geometry;
                    countVertices += mesh.Positions.Count;
                });

            Assert.AreEqual(17, countVertices);
        }
    }
}