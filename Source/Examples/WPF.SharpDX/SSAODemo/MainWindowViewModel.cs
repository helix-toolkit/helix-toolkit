using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media3D = System.Windows.Media.Media3D;
using SharpDX;

namespace SSAODemo
{
    public class MainWindowViewModel : DemoCore.BaseViewModel
    {
        public MeshGeometry3D FloorModel { get; }
        public MeshGeometry3D SphereModel { get; }
        public MeshGeometry3D TeapotModel { get; }

        public PhongMaterial FloorMaterial { get; }
        public PhongMaterial SphereMaterial { get; }
        public Matrix[] SphereInstances { get; }

        public MainWindowViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = new OrthographicCamera()
            {
                Position = new Media3D.Point3D(0, 10, 10),
                LookDirection = new Media3D.Vector3D(0, -10, -10),
                UpDirection = new Media3D.Vector3D(0, 1, 0),
                FarPlaneDistance = 200,
                NearPlaneDistance = 0.1
            };

            var builder = new MeshBuilder();
            builder.AddBox(new Vector3(0, -0.1f, 0), 20, 0.1f, 20);
            builder.AddBox(new Vector3(-7, 2.5f, 0), 5, 5, 5);
            builder.AddBox(new Vector3(-5, 2.5f, -5), 5, 5, 5);
            FloorModel = builder.ToMesh();

            builder = new MeshBuilder();
            builder.AddSphere(Vector3.Zero, 1);
            SphereModel = builder.ToMesh();


            FloorMaterial = PhongMaterials.MediumGray;
            FloorMaterial.AmbientColor = Color.DarkGray;
            SphereMaterial = PhongMaterials.Red;
            SphereMaterial.AmbientColor = Color.DarkGray;
            SphereInstances = new Matrix[4]
            {
                Matrix.Translation(-2, 1, 0),
                Matrix.Translation(2, 1, 0),
                Matrix.Translation(0, 1, -2),
                Matrix.Translation(0, 1, 2)
            };
            
        }
    }
}
