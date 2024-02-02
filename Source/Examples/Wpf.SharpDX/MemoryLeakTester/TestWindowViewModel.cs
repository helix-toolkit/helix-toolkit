using HelixToolkit;
using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using Media3D = System.Windows.Media.Media3D;
using MeshGeometry3D = HelixToolkit.SharpDX.MeshGeometry3D;

namespace MemoryLeakTester;

public partial class TestWindowViewModel : DemoCore.BaseViewModel
{
    public MeshGeometry3D Mesh { get; private set; }
    public Color4 DirectionalLightColor { get; private set; } = Color.White;

    public PhongMaterial Material { get; } = PhongMaterials.Blue;
    public TestWindowViewModel()
    {
        this.Camera = new PerspectiveCamera { Position = new Media3D.Point3D(8, 9, 7), LookDirection = new Media3D.Vector3D(-5, -12, -5), UpDirection = new Media3D.Vector3D(0, 1, 0) };
        EffectsManager = new DefaultEffectsManager();

        var builder = new MeshBuilder();
        builder.AddBox(new Vector3(), 2, 2, 2);
        Mesh = builder.ToMeshGeometry3D();
    }
}
