using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Color = System.Windows.Media.Color;
using Plane = SharpDX.Plane;
using Vector3 = SharpDX.Vector3;
using Colors = System.Windows.Media.Colors;
using Color4 = SharpDX.Color4;
using System.IO;

namespace MaterialDemo
{
    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public MeshGeometry3D Floor { get; private set; }

        public Material FloorMaterial { get; } = PhongMaterials.Gray;

        public PhongMaterial PhongMaterial { private set; get; }

        public Material NormalMaterial { get; } = new NormalMaterial();

        public DiffuseMaterial DiffuseMaterial { get; } = DiffuseMaterials.LightGray;

        public Material PositionMaterial { get; } = new PositionColorMaterial();

        public Material VertMaterial { get; } = new VertColorMaterial();

        public Stream EnvironmentMap { private set; get; }

        public Transform3D Transform1 { get; } = new Media3D.TranslateTransform3D(-16, 0, 0);
        public Transform3D Transform2 { get; } = new Media3D.TranslateTransform3D(-8, 0, 0);
        public Transform3D Transform3 { get; } = new Media3D.TranslateTransform3D(0, 0, 0);
        public Transform3D Transform4 { get; } = new Media3D.TranslateTransform3D(8, 0, 0);
        public Transform3D Transform5 { get; } = new Media3D.TranslateTransform3D(16, 0, 0);

        private Random rnd = new Random();

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            this.Camera = new PerspectiveCamera { Position = new Point3D(50, 50, -50), LookDirection = new Vector3D(-50, -50, 50), UpDirection = new Vector3D(0, 1, 0) };

            var builder = new MeshBuilder();
            builder.AddBox(new Vector3(0, -6, 0), 100, 2, 100);

            Floor = builder.ToMesh();

            builder = new MeshBuilder();
            builder.AddSphere(Vector3.Zero, 2);
            Model = builder.ToMesh();
            Model.Colors = new HelixToolkit.Wpf.SharpDX.Core.Color4Collection();
            for(int i=0; i< Model.Positions.Count; ++i)
            {
                Model.Colors.Add(new Color4(rnd.Next(0, 255) / 255f, rnd.Next(0, 255) / 255f, rnd.Next(0, 255) / 255f, 1));
            }

            PhongMaterial = PhongMaterials.White;
            var texture = LoadFileToMemory("TextureCheckerboard2.jpg");
            PhongMaterial.DiffuseMap = DiffuseMaterial.DiffuseMap = texture;

            EnvironmentMap = LoadFileToMemory("Cubemap_Grandcanyon.dds");
        }
    }
}
