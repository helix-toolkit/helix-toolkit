using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Media3D = System.Windows.Media.Media3D;

namespace OrderIndependantTransparentRenderingDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    private const string OpenFileFilter = "3D model files (*.obj;*.3ds;*.stl|*.obj;*.3ds;*.stl;";

    public ObservableElement3DCollection ModelGeometry { get; private set; }

    public ObservableElement3DCollection? PlaneGeometry { private set; get; }

    public LineGeometry3D? GridModel { private set; get; }

    public Media3D.Transform3D? GridTransform { private set; get; }

    [ObservableProperty]
    private bool showWireframe = false;

    partial void OnShowWireframeChanged(bool value)
    {
        foreach (var model in ModelGeometry)
        {
            if (model is MeshGeometryModel3D mesh)
            {
                mesh.RenderWireframe = value;
            }
        }
    }

    [ObservableProperty]
    private OutlineMode drawMode = OutlineMode.Merged;

    [ObservableProperty]
    private bool highlightSeparated = false;

    partial void OnHighlightSeparatedChanged(bool value)
    {
        DrawMode = value ? OutlineMode.Separated : OutlineMode.Merged;
    }

    [ObservableProperty]
    private bool oITWeightedModeEnabled = false;

    [ObservableProperty]
    private bool oITDepthPeelModeEnabled = true;

    [ObservableProperty]
    private OITRenderType oITRenderType = OITRenderType.DepthPeeling;

    partial void OnOITRenderTypeChanged(OITRenderType value)
    {
        switch (value)
        {
            case OITRenderType.None:
                OITDepthPeelModeEnabled = OITWeightedModeEnabled = false;
                break;
            case OITRenderType.DepthPeeling:
                OITDepthPeelModeEnabled = true;
                OITWeightedModeEnabled = false;
                break;
            case OITRenderType.SinglePassWeighted:
                OITDepthPeelModeEnabled = false;
                OITWeightedModeEnabled = true;
                break;
        }
    }

    [ObservableProperty]
    private MaterialType materialType = MaterialType.BlinnPhong;

    partial void OnMaterialTypeChanged(MaterialType value)
    {
        UpdateMaterials();
    }

    public OITWeightMode[] OITWeights { get; } = new OITWeightMode[] { OITWeightMode.Linear0, OITWeightMode.Linear1, OITWeightMode.Linear2, OITWeightMode.NonLinear };

    public OITRenderType[] OITRenderTypes { get; } = new OITRenderType[] { OITRenderType.None, OITRenderType.DepthPeeling, OITRenderType.SinglePassWeighted };

    public MaterialType[] MaterialTypes { get; } = new MaterialType[] { MaterialType.BlinnPhong, MaterialType.PBR, MaterialType.Diffuse };

    [ObservableProperty]
    private int redPlaneOpacity = 60;

    partial void OnRedPlaneOpacityChanged(int value)
    {
        if (PlaneGeometry is null
            || PlaneGeometry.Count == 0
            || PlaneGeometry[0] is not MeshGeometryModel3D model
            || model.Material is not PhongMaterial material)
        {
            return;
        }
        material.DiffuseColor = new Color4(1, 0, 0, value / 100f);
    }
    [ObservableProperty]
    private int greenPlaneOpacity = 60;

    partial void OnGreenPlaneOpacityChanged(int value)
    {
        if (PlaneGeometry is null
            || PlaneGeometry.Count < 2
            || PlaneGeometry[1] is not MeshGeometryModel3D model
            || model.Material is not PhongMaterial material)
        {
            return;
        }
        material.DiffuseColor = new Color4(0, 1, 0, value / 100f);
    }
    [ObservableProperty]
    private int bluePlaneOpacity = 60;

    partial void OnBluePlaneOpacityChanged(int value)
    {
        if (PlaneGeometry is null
            || PlaneGeometry.Count < 3
            || PlaneGeometry[2] is not MeshGeometryModel3D model
            || model.Material is not PhongMaterial material)
        {
            return;
        }
        material.DiffuseColor = new Color4(0, 0, 1, value / 100f);
    }
    private readonly SynchronizationContext? context = SynchronizationContext.Current;

    private readonly Random rnd = new();

    public MainViewModel()
    {
        this.ModelGeometry = new ObservableElement3DCollection();

        EffectsManager = new DefaultEffectsManager();

        Camera = new OrthographicCamera()
        {
            LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -50, -50),
            Position = new System.Windows.Media.Media3D.Point3D(0, 50, 50),
            FarPlaneDistance = 500,
            NearPlaneDistance = 0.1,
            Width = 100
        };

        Task.Run(() => { Load3ds("NITRO_ENGINE.3ds"); });

        BuildGrid();
        BuildPlanes();
    }

    [RelayCommand]
    private void ResetCamera()
    {
        Camera?.Reset();
    }

    private void BuildGrid()
    {
        var builder = new LineBuilder();
        int zOff = -45;
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                builder.AddLine(new Vector3(-i * 5, 0, j * 5), new Vector3(i * 5, 0, j * 5));
                builder.AddLine(new Vector3(-i * 5, 0, -j * 5), new Vector3(i * 5, 0, -j * 5));
                builder.AddLine(new Vector3(i * 5, 0, -j * 5), new Vector3(i * 5, 0, j * 5));
                builder.AddLine(new Vector3(-i * 5, 0, -j * 5), new Vector3(-i * 5, 0, j * 5));
                builder.AddLine(new Vector3(-i * 5, j * 5, zOff), new Vector3(i * 5, j * 5, zOff));
                builder.AddLine(new Vector3(i * 5, 0, zOff), new Vector3(i * 5, j * 5, zOff));
                builder.AddLine(new Vector3(-i * 5, 0, zOff), new Vector3(-i * 5, j * 5, zOff));
            }
        }
        GridModel = builder.ToLineGeometry3D();
        GridTransform = new Media3D.TranslateTransform3D(new Media3D.Vector3D(0, -10, 0));
    }

    private void BuildPlanes()
    {
        PlaneGeometry = new ObservableElement3DCollection();
        var builder = new MeshBuilder(true);
        builder.AddBox(new Vector3(0, 0, 0), 15, 15, 0.5f);
        var mesh = builder.ToMeshGeometry3D();

        var material = new PhongMaterial
        {
            DiffuseColor = new Color4(1, 0, 0, RedPlaneOpacity / 100f)
        };

        var model = new MeshGeometryModel3D()
        {
            Geometry = mesh,
            Material = material,
            Transform = new Media3D.TranslateTransform3D(-15, 0, 0),
            IsTransparent = true,
            CullMode = SharpDX.Direct3D11.CullMode.Back
        };
        PlaneGeometry.Add(model);

        material = new PhongMaterial
        {
            DiffuseColor = new Color4(0, 1, 0, GreenPlaneOpacity / 100f)
        };

        model = new MeshGeometryModel3D()
        {
            Geometry = mesh,
            Material = material,
            Transform = new Media3D.TranslateTransform3D(-20, 5, -10),
            IsTransparent = true,
            CullMode = SharpDX.Direct3D11.CullMode.Back
        };
        PlaneGeometry.Add(model);

        material = new PhongMaterial
        {
            DiffuseColor = new Color4(0, 0, 1, BluePlaneOpacity / 100f)
        };

        model = new MeshGeometryModel3D()
        {
            Geometry = mesh,
            Material = material,
            Transform = new Media3D.TranslateTransform3D(-25, 10, -20),
            IsTransparent = true,
            CullMode = SharpDX.Direct3D11.CullMode.Back
        };
        PlaneGeometry.Add(model);
    }

    public void Load3ds(string path)
    {
        var reader = new StudioReader();
        var objCol = reader.Read(path);
        AttachModelList(objCol);
    }

    public void LoadObj(string path)
    {
        var reader = new ObjReader();
        var objCol = reader.Read(path);
        AttachModelList(objCol);
    }

    public void LoadStl(string path)
    {
        var reader = new StLReader();
        var objCol = reader.Read(path);
        AttachModelList(objCol);
    }

    public void AttachModelList(List<Object3D>? objs)
    {
        if (objs is null)
        {
            return;
        }

        var rnd = new Random();

        foreach (var ob in objs)
        {
            ob.Geometry?.UpdateOctree();
            Task.Delay(50).Wait(); //Only for async loading demo
            context?.Post((o) =>
            {
                var s = new MeshGeometryModel3D
                {
                    Geometry = ob.Geometry,
                    IsTransparent = true,
                    DepthBias = -100
                };
                UpdateMaterial(s);
                this.ModelGeometry.Add(s);
            }, null);
        }
    }

    private void UpdateMaterials()
    {
        foreach (var geo in ModelGeometry)
        {
            if (geo is MeshGeometryModel3D mesh)
            {
                UpdateMaterial(mesh);
            }
        }
    }

    private void UpdateMaterial(MeshGeometryModel3D mesh)
    {
        var diffuse = new Color4
        {
            Red = (float)rnd.NextDouble(),
            Green = (float)rnd.NextDouble(),
            Blue = (float)rnd.NextDouble(),
            Alpha = 1
        };
        diffuse.Alpha = 0.6f;
        Material? material = null;
        switch (MaterialType)
        {
            case MaterialType.BlinnPhong:
                material = new PhongMaterial()
                {
                    DiffuseColor = diffuse
                };
                break;
            case MaterialType.PBR:
                material = new PBRMaterial()
                {
                    AlbedoColor = diffuse,
                    MetallicFactor = 0.7f,
                    RoughnessFactor = 0.6f,
                    ReflectanceFactor = 0.2,
                };
                break;
            case MaterialType.Diffuse:
                material = new DiffuseMaterial()
                {
                    DiffuseColor = diffuse
                };
                break;
        }
        mesh.Material = material;
    }
}
