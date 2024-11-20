using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace ScreenSpaceDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private ObservableElement3DCollection modelGeometry;

    [ObservableProperty]
    private MeshGeometry3D? model;

    [ObservableProperty]
    private LineGeometry3D? lines;

    [ObservableProperty]
    private LineGeometry3D? grid;

    [ObservableProperty]
    private PhongMaterial? redMaterial;

    [ObservableProperty]
    private PhongMaterial? defaultMaterial;

    [ObservableProperty]
    private Color gridColor;

    [ObservableProperty]
    private Transform3D modelTransform;

    [ObservableProperty]
    private Vector3 directionalLightDirection1;

    [ObservableProperty]
    private Vector3 directionalLightDirection2;

    [ObservableProperty]
    private Color4 directionalLightColor;

    [ObservableProperty]
    private Color4 ambientLightColor;

    [ObservableProperty]
    private Color4 backgroundColor;

    public MainViewModel()
    {
        // ----------------------------------------------
        // titles
        this.Title = "Screen Space Ambient Occlusion Demo";
        this.SubTitle = "WPF & SharpDX";

        // camera setup
        this.Camera = new PerspectiveCamera
        {
            Position = new Point3D(1.5, 2.5, 2.5),
            LookDirection = new Vector3D(-1.5, -2.5, -2.5),
            UpDirection = new Vector3D(0, 1, 0)
        };

        // default render technique
        EffectsManager = new DefaultEffectsManager();

        // background
        this.BackgroundColor = (Color4)Color.White;

        // setup lighting            
        this.AmbientLightColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
        this.DirectionalLightColor = Color.White;
        this.DirectionalLightDirection1 = new Vector3(-2, -5, -2);
        this.DirectionalLightDirection2 = new Vector3(+2, +5, +5);

        // model materials            
        this.DefaultMaterial = PhongMaterials.DefaultVRML;

        //load model
        var reader = new ObjReader();
        List<Object3D> objModel = reader.Read(@"./Media/CornellBox-Glossy.obj") ?? new List<Object3D>();

        this.modelGeometry = new ObservableElement3DCollection();
        foreach (var model in objModel.Select(x => new MeshGeometryModel3D()
        {
            Geometry = x.Geometry as MeshGeometry3D,
            Material = GetMaterialFromMaterialCore(x.Material as PhongMaterialCore),
        }))
        {
            this.ModelGeometry.Add(model);
        }

        // model trafos
        this.modelTransform = new Media3D.TranslateTransform3D(0, 0, 0);
    }

    private static Material? GetMaterialFromMaterialCore(PhongMaterialCore? material)
    {
        if (material is null)
        {
            return null;
        }

        var mat = new PhongMaterial
        {
            Name = material.Name,
            SpecularColor = material.SpecularColor,
            SpecularShininess = material.SpecularShininess,
            AmbientColor = material.AmbientColor,
            DiffuseColor = material.DiffuseColor,
            DiffuseMap = material.DiffuseMap,
            DiffuseMapSampler = material.DiffuseMapSampler,
            DiffuseAlphaMap = material.DiffuseAlphaMap,
            DisplacementMap = material.DisplacementMap,
            DisplacementMapSampler = material.DisplacementMapSampler,
            DisplacementMapScaleMask = material.DisplacementMapScaleMask,
            EmissiveColor = material.EmissiveColor,
            EnableTessellation = material.EnableTessellation,
            MaxDistanceTessellationFactor = material.MaxDistanceTessellationFactor,
            MaxTessellationDistance = material.MaxTessellationDistance,
            MinDistanceTessellationFactor = material.MinDistanceTessellationFactor,
            MinTessellationDistance = material.MinTessellationDistance,
            NormalMap = material.NormalMap,
            ReflectiveColor = material.ReflectiveColor,
            RenderDiffuseAlphaMap = material.RenderDiffuseAlphaMap,
            RenderDiffuseMap = material.RenderDiffuseMap,
            RenderDisplacementMap = material.RenderDisplacementMap,
            RenderNormalMap = material.RenderNormalMap,
        };

        return mat;
    }
}
