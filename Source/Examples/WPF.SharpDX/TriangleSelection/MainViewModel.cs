using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using MeshGeometry3D = HelixToolkit.SharpDX.MeshGeometry3D;

namespace TriangleSelection
{
    internal partial class MainViewModel :  DemoCore.BaseViewModel
    {
        [ObservableProperty]
        private MeshGeometry3D sphereMesh;

        [ObservableProperty]
        private MeshGeometry3D selectionMesh;

        [ObservableProperty]
        private Material sphereMaterial = new PhongMaterial() { DiffuseColor = Color.Green };

        [ObservableProperty]
        private Material selectionMaterial = new DiffuseMaterial() { DiffuseColor = Color.Yellow };

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            var meshbuilder = new MeshBuilder();
            meshbuilder.AddSphere(Vector3.Zero, 10);
            SphereMesh = meshbuilder.ToMeshGeometry3D();
            SelectionMesh = new MeshGeometry3D()
            {
                Positions = SphereMesh.Positions,
                Normals = SphereMesh.Normals,
                Indices = new IntCollection(SphereMesh.Indices.Capacity)
            };

            Camera.Position = new System.Windows.Media.Media3D.Point3D(100, 0, 0);
            Camera.LookDirection = new System.Windows.Media.Media3D.Vector3D(-100, 0, 0);
        }

        public void HandleMouseDown(HitTestResult hit)
        {
            if (hit.ModelHit is MeshGeometryModel3D model && model.Geometry == SphereMesh)
            {
                SelectionMesh.Indices.Add(hit.TriangleIndices.Item1);
                SelectionMesh.Indices.Add(hit.TriangleIndices.Item2);
                SelectionMesh.Indices.Add(hit.TriangleIndices.Item3);
                SelectionMesh.UpdateTriangles();
            }
        }
    }
}
