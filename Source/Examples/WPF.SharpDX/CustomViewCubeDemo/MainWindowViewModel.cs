using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomViewCubeDemo
{
    public class MainWindowViewModel : DemoCore.BaseViewModel
    {
        public Geometry3D Geometry { private set; get; }
        public Material Material { private set; get; }
        public Geometry3D ViewCubeGeometry1 { private set; get; }
        public Geometry3D ViewCubeGeometry2 { private set; get; }
        public Material ViewCubeMaterial1 { private set; get; }
        public Material ViewCubeMaterial2 { private set; get; }
        public Material ViewCubeMaterial3 { private set; get; }
        public Material ViewCubeMaterial4 { private set; get; }

        public System.Windows.Media.Media3D.Transform3D ViewCubeTransform3 { private set; get; }

        public MainWindowViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = new PerspectiveCamera()
            {
                Position = new System.Windows.Media.Media3D.Point3D(0, 0, 10),
                LookDirection = new System.Windows.Media.Media3D.Vector3D(0, 0, -10),
                UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0)
            };
            InitializeModels();
            InitializeViewCubes();
        }

        private void InitializeModels()
        {
            var builder = new MeshBuilder();
            builder.AddBox(Vector3.Zero, 1, 1, 1);
            var reader = new ObjReader();
            var models = reader.Read("bunny.obj");
            Geometry = models[0].Geometry;
            Material = PhongMaterials.Red;
        }

        private void InitializeViewCubes()
        {
            var builder = new MeshBuilder();
            builder.AddPyramid(Vector3.Zero, 10, 10, true);
            ViewCubeGeometry1 = builder.ToMesh();
            ViewCubeMaterial1 = DiffuseMaterials.Orange;

            builder = new MeshBuilder();
            builder.AddDodecahedron(Vector3.Zero, Vector3.UnitX, Vector3.UnitY, 5);
            ViewCubeGeometry2 = builder.ToMesh();
            ViewCubeMaterial2 = DiffuseMaterials.Blue;

            ViewCubeMaterial3 = DiffuseMaterials.Gray;
            ViewCubeMaterial4 = DiffuseMaterials.Pearl;
            //Center the model first and do scaling
            var transform = Matrix.Translation(0, -2, 0) * Matrix.Scaling(3.5f);
            ViewCubeTransform3 = new System.Windows.Media.Media3D.MatrixTransform3D(transform.ToMatrix3D());
        }
    }
}
