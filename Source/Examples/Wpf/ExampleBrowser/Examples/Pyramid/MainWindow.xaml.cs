using HelixToolkit;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace Pyramid;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Pyramid", "Performance test showing a pyramid.")]
public partial class MainWindow : Window
{
    // testing performance on big model...

    // Great pyramid of giza 280:440:440 cubits,  total ~ 2 301 370 cubits

    // http://en.wikipedia.org/wiki/Great_Pyramid_of_Giza
    // http://en.wikipedia.org/wiki/Cubits

    // cheating: only drawing outside layer of cubits
    // cheating: divided each dimension by 10...

    private readonly CubeVisual3D cubit;
    private const double a = 0.524;
    private const double b = 0.95;
    private const int nside = 150;
    private const int nvert = 150;

    public MainWindow()
    {
        InitializeComponent();

        cubit = new CubeVisual3D { SideLength = a * b, Fill = Brushes.SandyBrown };
        Loaded += this.MainWindowLoaded;
    }

    private void MainWindowLoaded(object? sender, RoutedEventArgs e)
    {
        // cubit.Model.Freeze();
        // cubit.Material.Freeze();
        var cuboidGeometry = cubit.Model?.Geometry as MeshGeometry3D;

        var vis = new ModelVisual3D();

        bool allInOneGeometry = true;

        MeshBuilder? builder = null;

        if (allInOneGeometry)
            builder = new MeshBuilder();

        int countAll = 0;
        int countModelled = 0;
        for (int i = 0; i < nvert; i++)
        {
            double m = nside - i * (double)nside / nvert;
            var mx = (int)m;
            Debug.WriteLine(i + ": " + mx);
            for (int j = 0; j <= mx; j++)
            {
                for (int k = 0; k <= mx; k++)
                {
                    countAll++;

                    // only adding blocks on the outside...
                    if (j > 0 && j < mx - 1 && i > 0 && i < nvert - 1 && k > 0 && k < mx - 1)
                        continue;

                    countModelled++;

                    var center = new Point3D(a * (j - (double)mx / 2), a * (k - (double)mx / 2), (i + 0.5) * a);

                    if (allInOneGeometry)
                        builder!.AddBox(center.ToVector3(), (float)(a * b), (float)(a * b), (float)(a * b));
                    else
                    {
                        // add a new visual for each cubit, but reuse the geometry
                        var transform = new TranslateTransform3D(center.ToWndVector3D());
                        var c = new ModelVisual3D
                        {
                            Content = new GeometryModel3D()
                            {
                                Geometry = cuboidGeometry,
                                Material = cubit.Material,
                                Transform = transform
                            }
                        };
                        vis.Children.Add(c);
                    }
                }
            }
        }

        if (allInOneGeometry)
        {
            var c = new ModelVisual3D
            {
                Content = new GeometryModel3D()
                {
                    Geometry = builder!.ToMesh().ToWndMeshGeometry3D(),
                    Material = cubit.Material
                }
            };
            view1.Children.Add(c);
        }
        else
        {
            view1.Children.Add(vis);
        }

        var mg = cubit.Model?.Geometry as MeshGeometry3D;
        int ntri = (mg?.TriangleIndices.Count ?? 0) / 3;
        int ntriTotal = ntri * countModelled;

        if (allInOneGeometry)
            ntriTotal = builder!.TriangleIndices.Count / 3;

        Title = "Mini Pyramid, total " + countAll + " cubits, rendered with " + countModelled + " cubits / " + ntriTotal +
                " triangles.";

        view1.LookAt(new Point3D(0, 0, a * nvert / 2), new Vector3D(-70, -60, -100), 0);
    }
}
