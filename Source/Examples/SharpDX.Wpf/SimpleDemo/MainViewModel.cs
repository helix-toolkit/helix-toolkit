namespace SimpleDemo
{
    using HelixToolkit.SharpDX;
    using HelixToolkit.SharpDX.Wpf;

    using SharpDX;

    public class MainViewModel
    {
        public Camera Camera { get; private set; }
        public MeshGeometry3D CubeFaces { get; private set; }
        public LineGeometry3D CubeEdges { get; private set; }
        public DiffuseMaterial Material1 { get; private set; }

        public MainViewModel()
        {
            Camera = new PerspectiveCamera { Position = new Vector3(15f, 15f, 15f), LookDirection = new Vector3(-15f, -15f, -15f), UpDirection = new Vector3(0, 0, 1) };

            var b1 = new MeshBuilder();
            b1.AppendBox(new Vector3(0, 0, 0), 5, 5, 5);
            CubeFaces = b1.ToMesh();

            var b2 = new LineBuilder();
            b2.AppendBox(new Vector3(0, 0, 0), 5, 5, 5);
            CubeEdges = b2.ToLine();

            Material1 = new DiffuseMaterial() { Color = Color.Red };
        }
    }
}
