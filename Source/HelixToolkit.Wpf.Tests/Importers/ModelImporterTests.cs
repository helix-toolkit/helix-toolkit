// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelImporterTests.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkitTests
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
            var model = ModelImporter.Load(@"Models\obj\test.obj");
            int countVertices = 0;
            Visual3DHelper.TraverseModel<GeometryModel3D>(
                model,
                (geometryModel, transform) =>
                {
                    var mesh = (MeshGeometry3D)geometryModel.Geometry;
                    countVertices += mesh.Positions.Count;
                });

            Assert.AreEqual(8, countVertices);
        }
   }
}