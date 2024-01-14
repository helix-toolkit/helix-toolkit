using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;

using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace MouseDragDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    public MeshGeometry3D MeshGeometry { get; private set; }
    public LineGeometry3D Lines { get; private set; }
    public LineGeometry3D Grid { get; private set; }

    public PhongMaterial RedMaterial { get; private set; }
    public PhongMaterial GreenMaterial { get; private set; }
    public PhongMaterial BlueMaterial { get; private set; }
    public Color GridColor { get; private set; }

    public List<Matrix> Model1Instances { get; private set; }

    public Transform3D Model1Transform { get; private set; }
    public Transform3D Model2Transform { get; private set; }
    public Transform3D Model3Transform { get; private set; }
    public Transform3D GridTransform { get; private set; }


    public Vector3D DirectionalLightDirection { get; private set; }
    public Color DirectionalLightColor { get; private set; }
    public Color AmbientLightColor { get; private set; }

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();

        // titles
        this.Title = "Mouse Drag Demo";
        this.SubTitle = "WPF & SharpDX";

        // camera setup
        this.Camera = new PerspectiveCamera { Position = new Point3D(0, 0, 9), LookDirection = new Vector3D(-0, -0, -9), UpDirection = new Vector3D(0, 1, 0) };

        // setup lighting            
        this.AmbientLightColor = Colors.DimGray;
        this.DirectionalLightColor = Colors.White;
        this.DirectionalLightDirection = new Vector3D(-2, -5, -2);

        // floor plane grid
        this.Grid = LineBuilder.GenerateGrid(Vector3.UnitZ, -5, 5);
        this.GridColor = Colors.Black;
        this.GridTransform = new Media3D.TranslateTransform3D(-0, -0, -0);

        // scene model3d
        var b1 = new MeshBuilder();
        b1.AddSphere(new Vector3(0, 0, 0), 0.65f);
        b1.AddBox(new Vector3(0, 0, 0), 1, 1, 1);
        var meshGeometry = b1.ToMeshGeometry3D();
        meshGeometry.Colors = meshGeometry.TextureCoordinates is null 
            ? null 
            : new Color4Collection(meshGeometry.TextureCoordinates.Select(x => x.ToColor4(1f,1f)));
        this.MeshGeometry = meshGeometry;
        this.Model1Instances = new List<Matrix>();
        for (int i = 0; i < 5; i++)
        {
            this.Model1Instances.Add(Matrix.CreateTranslation(0, i, 0));
        }

        // lines model3d
        var e1 = new LineBuilder();
        e1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2);
        this.Lines = e1.ToLineGeometry3D();

        // model trafos
        this.Model1Transform = new Media3D.TranslateTransform3D(0, 0, 0.0);
        this.Model2Transform = new Media3D.TranslateTransform3D(-2, 0, 0);
        this.Model3Transform = new Media3D.TranslateTransform3D(+2, 0, 0);

        // model materials
        this.RedMaterial = PhongMaterials.Red;
        this.GreenMaterial = PhongMaterials.Green;
        this.BlueMaterial = PhongMaterials.Blue;

        // ---
        this.Shape3DCollection = new ObservableCollection<Shape3D>
            {
                new()
                {
                    Geometry = this.MeshGeometry,
                    Material = this.BlueMaterial,
                    Transform = this.Model3Transform,
                    Instances = new List<Matrix>{Matrix.Identity},
                    DragZ = false,
                },
                new()
                {
                    Geometry = this.MeshGeometry,
                    Material = this.RedMaterial,
                    Transform = this.Model1Transform,
                    Instances = new List<Matrix>{Matrix.Identity},
                    DragZ = true,
                },
            };

        this.Element3DCollection = new ObservableCollection<Element3D>()
            {
                new DraggableGeometryModel3D()
                {
                    Geometry = this.MeshGeometry,
                    Material = this.BlueMaterial,
                    Transform = this.Model3Transform,
                },

                new DraggableGeometryModel3D()
                {
                    Geometry = this.MeshGeometry,
                    Material = this.RedMaterial,
                    Transform = this.Model1Transform,
                },
            };
    }

    [RelayCommand]
    public void AddShape()
    {
        this.Element3DCollection.Add(
            new DraggableGeometryModel3D()
            {
                Geometry = this.MeshGeometry,
                Material = this.GreenMaterial,
                Transform = this.Model2Transform,
                Instances = new List<Matrix> {
                        Matrix.CreateTranslation(-1,0,0), Matrix.CreateTranslation(+1,0,0),
                        Matrix.CreateTranslation(0,-1,0), Matrix.CreateTranslation(0,+1,0),
                        Matrix.CreateTranslation(0,0,-1), Matrix.CreateTranslation(0,0,+1),
                },
            });

        var shape = new Shape3D()
        {
            Geometry = this.MeshGeometry,
            Material = this.GreenMaterial,
            Transform = this.Model2Transform,
        };
        this.Shape3DCollection.Add(shape);
    }

    [RelayCommand]
    public void DelShape()
    {
        //this.Element3DCollection = null;
        //this.Element3DCollection = new ObservableCollection<Element3D>();
        if (SelectedItem is Element3D element)
        {
            this.Element3DCollection.Remove(element);
        }

        //this.Shape3DCollection = null;
        //this.Shape3DCollection = new ObservableCollection<Shape3D>();
        if (SelectedItem is Shape3D shape)
        {
            this.Shape3DCollection.Remove(shape);
        }
    }

    public partial class Shape3D : DemoCore.BaseViewModel
    {
        [ObservableProperty]
        private Geometry3D? geometry;

        [ObservableProperty]
        private System.Windows.Media.Media3D.Transform3D? transform;

        [ObservableProperty]
        private Material? material;

        [ObservableProperty]
        private IList<Matrix>? instances;

        [ObservableProperty]
        private bool isSelected;

        [ObservableProperty]
        private bool dragZ;
    }

    [ObservableProperty]
    private IList<Shape3D> shape3DCollection = new ObservableCollection<Shape3D>();

    [ObservableProperty]
    private IList<Element3D> element3DCollection = new ObservableCollection<Element3D>();

    [ObservableProperty]
    private object? selectedItem;
}
