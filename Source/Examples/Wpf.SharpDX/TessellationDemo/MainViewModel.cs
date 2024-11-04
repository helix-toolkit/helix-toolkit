using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.Geometry;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Linq;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using MeshGeometry3D = HelixToolkit.SharpDX.MeshGeometry3D;

using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace TessellationDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private Geometry3D? defaultModel;

    [ObservableProperty]
    private Geometry3D grid;

    [ObservableProperty]
    private Geometry3D floorModel;

    [ObservableProperty]
    private PhongMaterial defaultMaterial;

    [ObservableProperty]
    private PhongMaterial floorMaterial = PhongMaterials.Silver;

    [ObservableProperty]
    private Color gridColor;

    [ObservableProperty]
    private Transform3D defaultTransform;

    [ObservableProperty]
    private Transform3D gridTransform;

    [ObservableProperty]
    private Vector3D directionalLightDirection1;

    [ObservableProperty]
    private Vector3D directionalLightDirection2;

    [ObservableProperty]
    private Vector3D directionalLightDirection3;

    [ObservableProperty]
    private Color directionalLightColor;

    [ObservableProperty]
    private Color ambientLightColor;

    [ObservableProperty]
    private FillMode fillMode = FillMode.Solid;

    [ObservableProperty]
    private bool wireframe = false;

    partial void OnWireframeChanged(bool value)
    {
        FillMode = value ? FillMode.Wireframe : FillMode.Solid;
    }

    [ObservableProperty]
    private MeshTopologyEnum meshTopology = MeshTopologyEnum.PNTriangles;

    partial void OnMeshTopologyChanged(MeshTopologyEnum value)
    {
        // if topology is changes, reload the model with proper type of faces
        this.LoadModel(
            @"./Media/teapot_quads_tex.obj",
            value == MeshTopologyEnum.PNTriangles
            ? MeshFaces.Default
            : MeshFaces.QuadPatches);
    }

    [ObservableProperty]
    private IList<Matrix> instances;

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();
        // ----------------------------------------------
        // titles
        this.Title = "Hardware Tessellation Demo";
        this.SubTitle = "WPF & SharpDX";

        // ---------------------------------------------
        // camera setup
        this.Camera = new PerspectiveCamera
        {
            Position = new Point3D(7, 10, 12),
            LookDirection = new Vector3D(-7, -10, -12),
            UpDirection = new Vector3D(0, 1, 0)
        };

        // ---------------------------------------------
        // setup lighting            
        this.AmbientLightColor = Color.FromArgb(1, 12, 12, 12);
        this.DirectionalLightColor = Colors.White;
        this.DirectionalLightDirection1 = new Vector3D(-0, -20, -20);
        this.DirectionalLightDirection2 = new Vector3D(-0, -1, +50);
        this.DirectionalLightDirection3 = new Vector3D(0, +1, 0);

        // ---------------------------------------------
        // model trafo
        this.defaultTransform = new Media3D.TranslateTransform3D(0, -0, 0);

        // ---------------------------------------------
        // model material
        this.defaultMaterial = new PhongMaterial
        {
            AmbientColor = Colors.Gray.ToColor4(),
            DiffuseColor = Colors.Red.ToColor4(), // Colors.LightGray,
            SpecularColor = Colors.White.ToColor4(),
            SpecularShininess = 100f,
            DiffuseMap = TextureModel.Create(new System.Uri(@"./Media/TextureCheckerboard2.dds", System.UriKind.RelativeOrAbsolute).ToString()),
            NormalMap = TextureModel.Create(new System.Uri(@"./Media/TextureCheckerboard2_dot3.dds", System.UriKind.RelativeOrAbsolute).ToString()),
            EnableTessellation = true,
            RenderShadowMap = true
        };
        FloorMaterial.RenderShadowMap = true;
        // ---------------------------------------------
        // init model
        this.LoadModel(@"./Media/teapot_quads_tex.obj", this.meshTopology == MeshTopologyEnum.PNTriangles ?
         MeshFaces.Default : MeshFaces.QuadPatches);
        // ---------------------------------------------
        // floor plane grid
        this.grid = LineBuilder.GenerateGrid(10);
        this.GridColor = Colors.Black;
        this.gridTransform = new Media3D.TranslateTransform3D(-5, -4, -5);

        var builder = new MeshBuilder(true, true, true);
        builder.AddBox(new Vector3(0, -5, 0), 60, 0.5f, 60, BoxFaces.All);
        floorModel = builder.ToMeshGeometry3D();

        instances = new Matrix[] { Matrix.Identity, 
            Matrix.CreateTranslation(10, 0, 10), 
            Matrix.CreateTranslation(-10, 0, 10), 
            Matrix.CreateTranslation(10, 0, -10), 
            Matrix.CreateTranslation(-10, 0, -10), };
    }

    /// <summary>
    /// load the model from obj-file
    /// </summary>
    /// <param name="filename">filename</param>
    /// <param name="faces">Determines if facades should be treated as triangles (Default) or as quads (Quads)</param>
    private void LoadModel(string filename, MeshFaces faces)
    {
        // load model
        var reader = new ObjReader();
        List<Object3D> objModel = reader.Read(filename, new ModelInfo() { Faces = faces }) ?? new();
        var model = objModel[0].Geometry as MeshGeometry3D;

        if (model is not null)
        {
            model.Colors = model.Positions is null ? null : new Color4Collection(model.Positions.Select(x => new Color4(1, 0, 0, 1)));
        }

        DefaultModel = model;
    }

}
