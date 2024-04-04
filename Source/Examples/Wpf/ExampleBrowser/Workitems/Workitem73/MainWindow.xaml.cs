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

namespace Workitem73;

/// <summary>
/// Interaction logic for the main window.
/// </summary>
[ExampleBrowser.Example("Work item 73", "TubeVisual3D")]
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();

        var path1 = new List<Point3D>() { new Point3D(1, 1, 0.1), new Point3D(1, 1, -0.2), new Point3D(1, 1, 0.0001) };
        var path2 = new List<Point3D>() { new Point3D(2, 2, 0.1), new Point3D(2, 2, -0.2), new Point3D(2, 2, 0.1) };

        AddTube(path1, Colors.Green, 0.1f, 2);
        AddTube(path2, Colors.Red, 0.1f, 2);
    }


    void AddTube(List<Point3D> path, Color color, float diameter, int thetaDiv, bool isTubeClosed = false)
    {
        var mb = new MeshBuilder();

        mb.AddTube(path.ToVector3Collection()!, diameter, thetaDiv, isTubeClosed);

        // create a model
        var geom = new GeometryModel3D
        {
            Geometry = mb.ToMesh().ToWndMeshGeometry3D(true),
            Material = MaterialHelper.CreateMaterial(color)
        };

        var model = new ModelVisual3D();
        model.Content = geom;

        _Helix.Children.Add(model);
    }
}
