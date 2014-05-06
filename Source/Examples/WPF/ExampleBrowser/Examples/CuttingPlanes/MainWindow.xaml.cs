// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CuttingPlanesDemo
{
    using System.Windows;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Applies cutting planes to a model.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*var mb = new MeshBuilder();
            mb.AddSphere(new Point3D(0, 0, 0), 1);
            var mesh = mb.ToMesh();
            var n = new Vector3D(0, 0.2, 1);
            var p = new Point3D(0, 0, 0.5);
            var geo = MeshGeometryHelper.Cut(mesh, p,n);
            var m = new GeometryModel3D(geo, Materials.Blue);
            m.BackMaterial = Materials.Red;
            var mv = new ModelVisual3D();
            mv.Content = m;
            view1.Children.Add(mv);
            var segments = MeshGeometryHelper.GetContourSegments(mesh, p,n).ToList();
            foreach (IList<Point3D> contour in MeshGeometryHelper.CombineSegments(segments, 1e-6).ToList())
            {
                if (contour.Count == 0)
                    continue;
                view1.Children.Add(new TubeVisual3D { Diameter = 0.02, Path = contour, Fill = Brushes.Green });
            }
            view1.Children.Add(new RectangleVisual3D { Origin = p, Normal = n, Fill = new SolidColorBrush(Color.FromArgb(80, 255, 0, 0)) });*/
        }
    }
}