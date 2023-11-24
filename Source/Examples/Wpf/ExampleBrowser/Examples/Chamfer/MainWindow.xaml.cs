using HelixToolkit;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace Chamfer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Chamfer", "Chamfers the corner of a cube by the MeshBuilder.ChamferCorner method.")]
public partial class MainWindow : Window
{
    private readonly DispatcherTimer dt;

    public MainWindow()
    {
        InitializeComponent();
        dt = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(0.3)
        };
        dt.Tick += TimerTick;
        dt.Start();
    }

    private void TimerTick(object? sender, EventArgs e)
    {
        dt.Stop();
        view1.Children.Add(CreateDice());
        view1.ZoomExtents();
    }

    private ModelVisual3D CreateDice()
    {
        var diceMesh = new MeshBuilder();
        diceMesh.AddBox(new Point3D(0, 0, 0).ToVector(), 1, 1, 1);
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
                for (int k = 0; k < 2; k++)
                {
                    var points = new List<Vector3>();
                    diceMesh.ChamferCorner(new Point3D(i - 0.5, j - 0.5, k - 0.5).ToVector(), 0.1f, 1e-6f, points);
                    //foreach (var p in points)
                    //    diceMesh.ChamferCorner(p, 0.03f);
                }

        return new ModelVisual3D
        {
            Content = new GeometryModel3D
            {
                Geometry = diceMesh.ToMesh().ToMeshGeometry3D(),
                Material = Materials.White
            }
        };
    }
}
