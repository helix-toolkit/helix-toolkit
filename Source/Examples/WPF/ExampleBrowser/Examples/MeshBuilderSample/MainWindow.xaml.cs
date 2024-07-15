using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using Polyhedron;

namespace MeshBuilderSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ExampleBrowser.Example("MeshBuilderSample", "Demonstrates the mesh build-in MeshBuilder.")]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public MeshGeometry3D GlassGeometry
        {
            get
            {
                var builder = new MeshBuilder(true, true);
                var profile = new[] { new Point(0, 0.4), new Point(0.06, 0.36), new Point(0.1, 0.1), new Point(0.34, 0.1), new Point(0.4, 0.14), new Point(0.5, 0.5), new Point(0.7, 0.56), new Point(1, 0.46) };
                builder.AddRevolvedGeometry(profile.ToVector2Collection()!, null, new Vector3(0, 0, 0), new Vector3(0, 0, 1), 100);
                return builder.ToMesh().ToWndMeshGeometry3D(true);
            }
        }

        private void Tube_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IList<Vector2>? section = null;
                if (tubeSection.Text.Equals("Circle section", StringComparison.CurrentCultureIgnoreCase))
                {
                    section = MeshBuilder.GetCircle(16, false);
                }
                else if (tubeSection.Text.Equals("Rectangle section", StringComparison.CurrentCultureIgnoreCase))
                {
                    section = MeshBuilder.GetRectangle(3, 1);
                }
                else if (tubeSection.Text.Equals("Off-center rectangle section", StringComparison.CurrentCultureIgnoreCase))
                {
                    section = GetOffCenterRectangle();
                }
                else if (tubeSection.Text.Equals("Polygon section", StringComparison.CurrentCultureIgnoreCase))
                {
                    section = GetPolygon();
                }
                else if (!string.IsNullOrEmpty(tubeSection.Text))
                {
                    section = PointCollection.Parse(tubeSection.Text).ToVector2Collection();
                }
                if (section is null) return;

                Vector3? sectionXAxis = Vector3D.Parse(tubeSectionXAxis.Text).ToVector3();

                bool isSectionClose = tubeIsSectionClose.IsChecked == true;

                IList<Vector3>? paths = null;
                if (!string.IsNullOrEmpty(tubePaths.Text) && !tubePaths.Text.Contains("~"))
                {
                    paths = Vector3DCollection.Parse(tubePaths.Text).ToVector3Collection();
                }
                bool isTubeClose = tubeIsTubeClose.IsChecked == true;

                IList<float>? sectionAngles = null;
                if (!string.IsNullOrEmpty(tubeSectionAngles.Text))
                {
                    sectionAngles = DoubleCollection.Parse(tubeSectionAngles.Text).ToList().ConvertAll(x => (float)x);
                }
                IList<float>? sectionsScales = null;
                if (!string.IsNullOrEmpty(tubeSectionScales.Text))
                {
                    sectionsScales = DoubleCollection.Parse(tubeSectionScales.Text).ToList().ConvertAll(x => (float)x);
                }
                IList<float>? xTextureCoordinates = null;
                if (!string.IsNullOrEmpty(tubexTextureCoordinates.Text))
                {
                    xTextureCoordinates = DoubleCollection.Parse(tubexTextureCoordinates.Text).ToList().ConvertAll(x => (float)x);
                }
                bool isFrontCap = tubeFrontCap.IsChecked == true;
                bool isBackCap = tubeBackCap.IsChecked == true;

                var builder = new MeshBuilder(true, true);
                builder.AddTube(
                    path: paths,
                    sectionAngles: sectionAngles,
                    xTextureCoordinates: xTextureCoordinates,
                    sectionScales: sectionsScales, 
                    section: section,
                    sectionXAxis: sectionXAxis.Value,
                    isTubeClosed:isTubeClose,
                    isSectionClosed:isSectionClose,
                    frontCap: isFrontCap,
                    backCap: isBackCap);
                meshVisual3D.MeshGeometry = builder.ToMesh().ToWndMeshGeometry3D();
                meshVisual3D.Material = new DiffuseMaterial(Brushes.Green);
                meshVisual3D.BackMaterial = new DiffuseMaterial(Brushes.Orange);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private IList<Vector2> GetOffCenterRectangle()
        {
            IList<Vector2> rect = new Vector2[] { new Vector2(-2, 2), new Vector2(4, 2), new Vector2(4, -1), new Vector2(-2, -1) };
            return rect;
        }
        private IList<Vector2> GetPolygon()
        {
            IList<Vector2> rect = new Vector2[] { new Vector2(-1, 2), new Vector2(1, 2), new Vector2(3, 0), new Vector2(0, -3), new Vector2(-1, -3), new Vector2(-2, -1), new Vector2(0, 0) };
            return rect;
        }
    }
}
