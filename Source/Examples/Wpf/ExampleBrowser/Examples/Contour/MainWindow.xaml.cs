using HelixToolkit;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
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

namespace Contour;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Contour", "Calculates the contours of an imported model by the MeshGeometryHelper.")]
public partial class MainWindow : Window
{
    private Plane3D? ContourPlane;

    public MainWindow()
    {
        InitializeComponent();
        AddContours(model1, 8, 8, 8);
    }

    private void AddContours(Visual3D model1, int o, int m, int n)
    {
        var bounds = Visual3DHelper.FindBounds(model1, Transform3D.Identity);
        for (int i = 1; i < n; i++)
        {
            this.ContourPlane = new Plane3D(new Point3D(0, 0, bounds.Location.Z + bounds.Size.Z * i / n).ToVector(), new Vector3D(0, 0, 1).ToVector());
            Visual3DHelper.Traverse<GeometryModel3D>(model1, this.AddContours);
        }
        for (int i = 1; i < m; i++)
        {
            this.ContourPlane = new Plane3D(new Point3D(0, bounds.Location.Y + bounds.Size.Y * i / m, 0).ToVector(), new Vector3D(0, 1, 0).ToVector());
            Visual3DHelper.Traverse<GeometryModel3D>(model1, this.AddContours);
        }
        for (int i = 1; i < o; i++)
        {
            this.ContourPlane = new Plane3D(new Point3D(bounds.Location.X + bounds.Size.X * i / o, 0, 0).ToVector(), new Vector3D(1, 0, 0).ToVector());
            Visual3DHelper.Traverse<GeometryModel3D>(model1, this.AddContours);
        }
    }

    private void AddContours(GeometryModel3D model, Transform3D transform)
    {
        var p = ContourPlane!.Position;
        var n = ContourPlane!.Normal;

        var mesh = model.Geometry as MeshGeometry3D;
        var segments = mesh?.ToWndMeshGeometry3D()?.GetContourSegments(p, n)?.ToList() ?? new();

        foreach (var contour in MeshGeometryHelper.CombineSegments(segments, 1e-6f).ToList())
        {
            if (contour.Count == 0)
                continue;
            view2.Children.Add(new TubeVisual3D { Diameter = 0.03, Path = new Point3DCollection(contour.Select(t => t.ToWndPoint())), Fill = Brushes.Green });
        }
    }

    private void View2_CameraChanged(object? sender, RoutedEventArgs e)
    {
        view2.Camera?.CopyTo(view1.Camera);
    }

    private void View1_CameraChanged(object? sender, RoutedEventArgs e)
    {
        view1.Camera?.CopyTo(view2.Camera);
    }
}
