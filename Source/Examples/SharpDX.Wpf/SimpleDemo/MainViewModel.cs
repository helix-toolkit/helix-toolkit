namespace SimpleDemo
{
    using HelixToolkit.SharpDX;
    using HelixToolkit.SharpDX.Wpf;

    using SharpDX;

    using Point3D = System.Windows.Media.Media3D.Point3D;
    using Vector3D = System.Windows.Media.Media3D.Vector3D;

    public class MainViewModel
    {
        public Camera Camera { get; private set; }
        public MeshGeometry3D CubeFaces { get; private set; }
        public LineGeometry3D CubeEdges { get; private set; }
        public DiffuseMaterial Material1 { get; private set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }

        public MainViewModel()
        {
            Camera = new PerspectiveCamera { Position = new Point3D(0, 0, -5), LookDirection = new Vector3D(0, 0, 5), UpDirection = new Vector3D(0, 1, 0) };
            Title = "Test";
            SubTitle = "WPF + SharpDX";

            var b1 = new MeshBuilder();
            b1.AppendBox(new Vector3(0, 0, 0), 2, 2, 2, BoxFaces.All);
            b1.AppendBox(new Vector3(-1, -1, -1), 1, 1, 1, BoxFaces.All);
            CubeFaces = b1.ToMesh();

            //var b2 = new LineBuilder();
            //b2.AppendBox(new Vector3(0, 0, 0), 5, 5, 5);
            //CubeEdges = b2.ToLine();

            Material1 = new DiffuseMaterial { Color = Color.Red };
        }
    }
}
