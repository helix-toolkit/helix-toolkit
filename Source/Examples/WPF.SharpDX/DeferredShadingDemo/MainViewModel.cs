namespace DeferredShadingDemo
{
    using System.Linq;
    using System.Windows.Media.Imaging;

    using DemoCore;

    using HelixToolkit.Wpf.SharpDX;

    using SharpDX;

    using Media3D = System.Windows.Media.Media3D;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class MainViewModel : BaseViewModel
    {
        public MeshGeometry3D Model { get; private set; }
        public LineGeometry3D Lines { get; private set; }
        public LineGeometry3D Grid { get; private set; }

        public PhongMaterial RedMaterial { get; private set; }
        public PhongMaterial GreenMaterial { get; private set; }
        public PhongMaterial BlueMaterial { get; private set; }
        public SharpDX.Color GridColor { get; private set; }
        public SharpDX.Color4 BackgroundColor { get; private set; }

        public Media3D.Transform3D Model1Transform { get; private set; }
        public Media3D.Transform3D Model2Transform { get; private set; }
        public Media3D.Transform3D Model3Transform { get; private set; }
        public Media3D.Transform3D GridTransform { get; private set; }

        public Vector3 DirectionalLightDirection1 { get; private set; }
        public Vector3 DirectionalLightDirection2 { get; private set; }
        public Vector3 DirectionalLightDirection3 { get; private set; }
        public Color4 DirectionalLightColor1 { get; private set; }
        public Color4 DirectionalLightColor2 { get; private set; }
        public Color4 DirectionalLightColor3 { get; private set; }
        public Color4 AmbientLightColor { get; private set; }


        public MainViewModel()
        {
            // titles
            this.Title = "Deferred Shading Demo";    
        
            // camera setup
            this.Camera = new PerspectiveCamera { Position = new Point3D(3, 3, 5), LookDirection = new Vector3D(-3, -3, -5), UpDirection = new Vector3D(0, 1, 0) };

            // clear color 
            this.BackgroundColor = (Color4)Color.White;

            // default render technique
            this.RenderTechnique = Techniques.RenderDeferred;

            // setup lighting            
            this.AmbientLightColor = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
            this.DirectionalLightColor2 = Color.Red;
            this.DirectionalLightColor1 = Color.Green;
            this.DirectionalLightColor3 = Color.Blue;
            this.DirectionalLightDirection1 = new Vector3(-0,  -50, -0);
            this.DirectionalLightDirection2 = new Vector3(-0,  -50,  -50);
            this.DirectionalLightDirection3 = new Vector3(-50, -50,  -0);

            // floor plane grid
            this.Grid = LineBuilder.GenerateGrid();
            this.GridColor = SharpDX.Color.Black;
            this.GridTransform = new Media3D.TranslateTransform3D(-5, -1, -5);

            // scene model3d
            var b1 = new MeshBuilder(true,true,true);
            b1.AddSphere(new Vector3(0, 0, 0), 0.5);
            b1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2, BoxFaces.All);
            var meshGeometry = b1.ToMeshGeometry3D();
            meshGeometry.Colors = meshGeometry.TextureCoordinates.Select(x => x.ToColor4()).ToArray();
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
            this.RedMaterial = PhongMaterials.White;
            this.RedMaterial.DiffuseMap = new BitmapImage(new System.Uri(@"TextureCheckerboard2.jpg", System.UriKind.RelativeOrAbsolute));
            this.RedMaterial.NormalMap = new BitmapImage(new System.Uri(@"TextureCheckerboard2_dot3.jpg", System.UriKind.RelativeOrAbsolute));
            this.GreenMaterial = PhongMaterials.DefaultVRML;
            //this.GreenMaterial.TextureMap = this.RedMaterial.TextureMap;
            this.BlueMaterial = PhongMaterials.Silver;
            //this.BlueMaterial.TextureMap = this.RedMaterial.TextureMap;
        }
    }
}
