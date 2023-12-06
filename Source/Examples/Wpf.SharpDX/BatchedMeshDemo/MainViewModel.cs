using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace BatchedMeshDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private IList<BatchedMeshGeometryConfig>? batchedMeshes;

    [ObservableProperty]
    private IList<Material>? batchedMaterials;

    public Media3D.Transform3D BatchedTransform
    {
        get;
    } = new Media3D.ScaleTransform3D(0.1, 0.1, 0.1);

    [ObservableProperty]
    private Geometry3D? selectedGeometry;

    partial void OnSelectedGeometryChanged(Geometry3D? value)
    {
        SelectedTransform = new Media3D.MatrixTransform3D(
            BatchedMeshes!
            .Where(x => x.Geometry == value)
            .Select(x => x.ModelTransform)
            .First()
            .ToMatrix3D() * BatchedTransform.Value);
    }

    [ObservableProperty]
    private Media3D.Transform3D? selectedTransform;

    public Material MainMaterial { get; } = PhongMaterials.White;

    public Material SelectedMaterial { get; } = new PhongMaterial() { EmissiveColor = Color.Yellow };

    public Geometry3D FloorModel { private set; get; }

    public Material FloorMaterial { private set; get; } = PhongMaterials.Pearl;

    private readonly SynchronizationContext? context = SynchronizationContext.Current;

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();
        Camera = new PerspectiveCamera() { Position = new Point3D(0, 0, 200), LookDirection = new Vector3D(0, 0, -200), UpDirection = new Vector3D(0, 1, 0), FarPlaneDistance = 1000 };
        Task.Run(LoadModels);
        var builder = new MeshBuilder(true);
        builder.AddBox(new Vector3(0, -65, 0).ToVector(), 600, 1, 600);
        FloorModel = builder.ToMesh().ToMeshGeometry3D();

        if (MainMaterial is PhongMaterial mainPhong)
        {
            mainPhong.NormalMap = new TextureModel("TextureNoise1_dot3.jpg");
            mainPhong.RenderShadowMap = true;
        }

        if (FloorMaterial is PhongMaterial floorPhong)
        {
            floorPhong.RenderShadowMap = true;
        }
    }

    private void LoadModels()
    {
        var models = Load3ds("Car.3DS");
        int count = 0;
        Dictionary<MaterialCore, int> materialDict = new();
        //materialDict.Add(new PhongMaterialCore() { DiffuseColor = new Color4(1, 0, 0, 1) }, count);
        foreach (var model in models)
        {
            if (model.Material is null || materialDict.ContainsKey(model.Material))
            {
                continue;
            }

            materialDict.Add(model.Material, count++);
        }

        var modelList = new List<BatchedMeshGeometryConfig>(models.Count);
        foreach (var model in models)
        {
            if (model.Geometry is null || model.Material is null)
            {
                continue;
            }

            model.Geometry.UpdateOctree();

            if (model.Transform is not null)
            {
                foreach (var transform in model.Transform)
                {
                    modelList.Add(new BatchedMeshGeometryConfig(model.Geometry, transform, materialDict[model.Material]));
                    //modelList.Add(new BatchedMeshGeometryConfig(model.Geometry, transform, 0));
                }
            }
            else
            {
                modelList.Add(new BatchedMeshGeometryConfig(model.Geometry, Matrix.Identity, materialDict[model.Material]));
                //modelList.Add(new BatchedMeshGeometryConfig(model.Geometry, Matrix.Identity, 0));
            }
        }

        Material[] materials = new Material[materialDict.Count];
        foreach (var m in materialDict.Keys)
        {
            if (m is null)
            {
                continue;
            }

            materials[materialDict[m]] = m.ConvertToMaterial()!;
        }

        context?.Post((o) =>
        {
            BatchedMeshes = modelList;
            BatchedMaterials = materials;
        }, null);
    }

    public List<Object3D> Load3ds(string path)
    {
        if (path.EndsWith(".obj", StringComparison.CurrentCultureIgnoreCase))
        {
            var reader = new ObjReader();
            var list = reader.Read(path);
            return list ?? new List<Object3D>();
        }
        else if (path.EndsWith(".3ds", StringComparison.CurrentCultureIgnoreCase))
        {
            var reader = new StudioReader();
            var list = reader.Read(path);
            return list ?? new List<Object3D>();
        }
        else
        {
            return new List<Object3D>();
        }
    }
}
