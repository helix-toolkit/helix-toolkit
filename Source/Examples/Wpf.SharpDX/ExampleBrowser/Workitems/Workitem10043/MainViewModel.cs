using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using System.Linq;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace Workitem10043;

public class MainViewModel : DemoCore.BaseViewModel
{
    public MeshGeometry3D Model { get; private set; }
    public LineGeometry3D Lines { get; private set; }
    public LineGeometry3D Grid { get; private set; }

    public PhongMaterial RedMaterial { get; private set; }
    public PhongMaterial GreenMaterial { get; private set; }
    public PhongMaterial BlueMaterial { get; private set; }
    public HelixToolkit.Maths.Color GridColor { get; private set; }

    public Media3D.Transform3D Model1Transform { get; private set; }
    public Media3D.Transform3D Model2Transform { get; private set; }
    public Media3D.Transform3D Model3Transform { get; private set; }
    public Media3D.Transform3D GridTransform { get; private set; }

    public Color DirectionalLightColor { get; private set; }
    public Color AmbientLightColor { get; private set; }


    public MainViewModel()
    {
        // titles
        this.Title = "Simple Demo (Workitem 10043)";
        this.SubTitle = "Please switch to Viewport 2 and then back to Viewport 1";

        // camera setup
        this.Camera = new PerspectiveCamera { Position = new Point3D(3, 3, 5), LookDirection = new Vector3D(-3, -3, -5), UpDirection = new Vector3D(0, 1, 0) };

        EffectsManager = new DefaultEffectsManager();

        // setup lighting            
        this.AmbientLightColor = Colors.Black;
        this.DirectionalLightColor = Colors.White;

        // floor plane grid
        this.Grid = LineBuilder.GenerateGrid();
        this.GridColor = HelixToolkit.Maths.Color.Black;
        this.GridTransform = new Media3D.TranslateTransform3D(-5, -1, -5);

        // scene model3d
        var b1 = new MeshBuilder();
        b1.AddSphere(new Vector3(0, 0, 0), 0.5f);
        b1.AddBox(new Vector3(0, 0, 0), 1, 0.5f, 2, BoxFaces.All);

        var meshGeometry = b1.ToMeshGeometry3D();
        meshGeometry.Colors = meshGeometry.TextureCoordinates is null ? null : new Color4Collection(meshGeometry.TextureCoordinates.Select(x => x.ToColor4()));
        this.Model = meshGeometry;

        // lines model3d
        var e1 = new LineBuilder();
        e1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2);
        this.Lines = e1.ToLineGeometry3D();

        // model trafos
        this.Model1Transform = new Media3D.TranslateTransform3D(0, 0, 0);
        this.Model2Transform = new Media3D.TranslateTransform3D(-2, 0, 0);
        this.Model3Transform = new Media3D.TranslateTransform3D(+2, 0, 0);

        // model materials
        this.RedMaterial = PhongMaterials.Red;
        this.GreenMaterial = PhongMaterials.Green;
        this.BlueMaterial = PhongMaterials.Blue;
        //var diffColor = this.RedMaterial.DiffuseColor;
        //diffColor.Alpha = 0.5f;
        //this.RedMaterial.DiffuseColor = diffColor;            
    }
}
