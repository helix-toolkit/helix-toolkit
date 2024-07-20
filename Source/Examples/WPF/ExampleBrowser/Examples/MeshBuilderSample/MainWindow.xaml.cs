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
        private Material GetMaterial(bool useTexture)
        {
            Brush brush;
            if (useTexture)
            {
                brush = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Examples/MeshBuilderSample/TextureTestImage.png", UriKind.RelativeOrAbsolute)));
            }
            else
            {
                brush = Brushes.Green;
            }
            return new DiffuseMaterial(brush);
        }
        #region Tube
        private void AddTube_Click(object sender, RoutedEventArgs e)
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
                else if (tubeSection.Text.Equals("Wavy section", StringComparison.CurrentCultureIgnoreCase))
                {
                    section = GetWavy();
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
                    isTubeClosed: isTubeClose,
                    isSectionClosed: isSectionClose,
                    frontCap: isFrontCap,
                    backCap: isBackCap);
                meshVisual3D.MeshGeometry = builder.ToMesh().ToWndMeshGeometry3D();

                bool useTextureMat = !string.IsNullOrEmpty(tubexTextureCoordinates.Text);
                meshVisual3D.Material = GetMaterial(useTextureMat);
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
        private IList<Vector2> GetWavy()
        {
            int number = 36;
            IList<Vector2> wavy = new Vector2[number];
            float unitRad = 2 * (float)Math.PI / number;
            for (int i = 0; i < number; i++)
            {
                wavy[i] = new Vector2(i * unitRad, (float)Math.Sin(unitRad * i));
            }

            return wavy;
        }
        #endregion
        #region Triangle
        private void AddTriangle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Vector3[] points = Array.Empty<Vector3>();
                if (!string.IsNullOrEmpty(trianglePoints.Text))
                {
                    points = Vector3DCollection.Parse(trianglePoints.Text).ToVector3Collection().ToArray();
                }
                Vector2[]? textures = null;
                if (!string.IsNullOrEmpty(triangleTextures.Text))
                {
                    textures = VectorCollection.Parse(triangleTextures.Text).ToVector2Collection().ToArray();
                }

                var builder = new MeshBuilder(true, true);
                if (points is not null && textures is null)
                {
                    builder.AddTriangle(points[0], points[1], points[2]);
                }
                else if (points is not null && textures is not null)
                {
                    builder.AddTriangle(points[0], points[1], points[2], textures[0], textures[1], textures[2]);
                }
                else
                {
                    // builder.AddTriangle(vertexIndices);
                }
                var mesh = builder.ToMesh().ToWndMeshGeometry3D();
                GeometryModel3D g = new GeometryModel3D(mesh, GetMaterial(triangleUseTexttureMat.IsChecked == true));
                triangleVisual3D.Content = g;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        #endregion
    }
}
