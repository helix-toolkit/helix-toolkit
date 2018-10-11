using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media3D = System.Windows.Media.Media3D;
using SDX = SharpDX;
namespace MemoryLeakTester
{
    public class TestWindowViewModel : BaseViewModel
    {
        public MeshGeometry3D Mesh { get; private set; }
        public SDX.Color4 DirectionalLightColor { get; private set; } = SDX.Color.White;

        public PhongMaterial Material { get; } = PhongMaterials.Blue;
        public TestWindowViewModel()
        {
            this.Camera = new PerspectiveCamera { Position = new Media3D.Point3D(8, 9, 7), LookDirection = new Media3D.Vector3D(-5, -12, -5), UpDirection = new Media3D.Vector3D(0, 1, 0) };            
            EffectsManager = new DefaultEffectsManager();
            var builder = new MeshBuilder();
            builder.AddBox(new SDX.Vector3(), 2, 2, 2);
            Mesh = builder.ToMesh();
        }
    }
}
