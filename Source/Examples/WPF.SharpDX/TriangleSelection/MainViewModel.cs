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

        private readonly Dictionary<int, int> selectedTriangles = new();

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            var meshbuilder = new MeshBuilder();
            meshbuilder.AddSphere(Vector3.Zero, 10);
            sphereMesh = meshbuilder.ToMeshGeometry3D();
            selectionMesh = new MeshGeometry3D()
            {
                Positions = SphereMesh.Positions,
                Normals = SphereMesh.Normals,
                Indices = new IntCollection(SphereMesh.Indices!.Capacity)
            };

            Camera!.Position = new System.Windows.Media.Media3D.Point3D(100, 0, 0);
            Camera!.LookDirection = new System.Windows.Media.Media3D.Vector3D(-100, 0, 0);
        }

        public void HandleMouseDown(HitTestResult hit)
        {
            if (hit.ModelHit is MeshGeometryModel3D model && model.Geometry == SphereMesh)
            {
                if (hit.Tag is int loc)
                {
                    if (!selectedTriangles.ContainsKey(loc))
                    {
                        selectedTriangles.Add(loc, SelectionMesh.Indices!.Count);
                        SelectionMesh.Indices!.Add(hit.TriangleIndices!.Item1);
                        SelectionMesh.Indices!.Add(hit.TriangleIndices!.Item2);
                        SelectionMesh.Indices!.Add(hit.TriangleIndices!.Item3);
                        SelectionMesh.UpdateTriangles();
                    }
                    else
                    {
                        var pos = selectedTriangles[loc];
                        selectedTriangles.Remove(loc);
                        SelectionMesh.Indices!.RemoveRange(pos, 3);
                        SelectionMesh.UpdateTriangles();
                        foreach(var tri in selectedTriangles)
                        {
                            if (tri.Value > pos)
                            {
                                selectedTriangles[tri.Key] = tri.Value - 3;
                            }
                        }
                    }
                }
            }
        }
    }
}
