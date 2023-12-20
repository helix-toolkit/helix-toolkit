using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Collections.Generic;
using Media3D = System.Windows.Media.Media3D;

namespace SSAODemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    public Geometry3D? FloorModel { get; }
    public Geometry3D? SphereModel { get; }
    public Geometry3D? TeapotModel { get; }

    public Geometry3D? BunnyModel { get; }

    public PhongMaterial FloorMaterial { get; }
    public PhongMaterial SphereMaterial { get; }

    public PhongMaterial BunnyMaterial { get; }
    public Matrix[] SphereInstances { get; }

    public Matrix[] BunnyInstances { get; }

    public SSAOQuality[] SSAOQualities { get; } = new SSAOQuality[] { SSAOQuality.High, SSAOQuality.Low };

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();

        Camera = new PerspectiveCamera()
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
        FloorModel = builder.ToMeshGeometry3D();

        builder = new MeshBuilder();
        builder.AddSphere(Vector3.Zero, 1);
        SphereModel = builder.ToMeshGeometry3D();

        var reader = new ObjReader();

        List<Object3D> models = reader.Read("bunny.obj") ?? new List<Object3D>();
        BunnyModel = models[0].Geometry;
        BunnyMaterial = PhongMaterials.Green;
        BunnyMaterial.AmbientColor = BunnyMaterial.DiffuseColor * 0.5f;
        FloorMaterial = PhongMaterials.PureWhite;
        FloorMaterial.AmbientColor = FloorMaterial.DiffuseColor * 0.5f;
        SphereMaterial = PhongMaterials.Red;
        SphereMaterial.AmbientColor = SphereMaterial.DiffuseColor * 0.5f;
        SphereInstances = new Matrix[4]
        {
                Matrix.CreateTranslation(-2.5f, 1, 0),
                Matrix.CreateTranslation(2.5f, 1, 0),
                Matrix.CreateTranslation(0, 1, -2.5f),
                Matrix.CreateTranslation(0, 1, 2.5f)
        };

        BunnyInstances = new Matrix[4]
        {
                Matrix.CreateTranslation(0f, -0.8f, 0),
                Matrix.CreateTranslation(6f, -0.8f, 0),
                Matrix.CreateTranslation(0, -0.8f, -4f),
                Matrix.CreateTranslation(0, -0.8f, 4f)
        };
    }
}
