/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

namespace HelixToolkit.Wpf.SharpDX.Tests.Utilities
{
    using System.Numerics;
    using System.IO;
    using System.Threading;
    using System.Windows.Media.Media3D;

    using NUnit.Framework;

    [TestFixture]
    public class ObjExporterTests
    {
        [Test, Apartment(ApartmentState.STA)]
        public void SharpDX_Export_Triangle_Valid()
        {
            var b1 = new MeshBuilder();
            b1.AddTriangle(new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0));
            var meshGeometry = b1.ToMeshGeometry3D();

            var mesh = new MeshGeometryModel3D();
            mesh.Geometry = meshGeometry;
            mesh.Material = PhongMaterials.Green;
            mesh.Transform = new TranslateTransform3D(2, 0, 0);

            var viewport = new Viewport3DX();
            viewport.Items.Add(mesh);

            string temp = Path.GetTempPath();
            var objPath = temp + "model.obj";
            var mtlPath = temp + "model.mtl";

            try
            {
                using (var exporter = new ObjExporter(objPath))
                {
                    exporter.Export(viewport);
                }

                string contentObj = File.ReadAllText(objPath);
                string expectedObj = @"mtllib ./model.mtl
o object1
g group1
usemtl mat1
v 2 0 0
v 2 1 0
v 2 0 -1
# 3 vertices
vt 0 1
vt 1 1
vt 0 0
# 3 texture coordinates
f 1/1 2/2 3/3
# 1 faces

";

                Assert.AreEqual(expectedObj.Replace("\r\n", "\n"), contentObj.Replace("\r\n", "\n"));

                string contentMtl = File.ReadAllText(mtlPath);
            }
            finally
            {
                if (File.Exists(objPath))
                    File.Delete(objPath);

                if (File.Exists(mtlPath))
                    File.Delete(mtlPath);
            }
        }
    }
}
