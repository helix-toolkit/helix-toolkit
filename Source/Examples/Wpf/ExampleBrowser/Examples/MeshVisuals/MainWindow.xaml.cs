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

namespace MeshVisuals;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("MeshVisuals", "Demonstrates the mesh based visual elements.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.DataContext = this;
    }

    public MeshGeometry3D GlassGeometry
    {
        get
        {
            var builder = new MeshBuilder(true, true);
            var profile = new[] { new Point(0, 0.4), new Point(0.06, 0.36), new Point(0.1, 0.1), new Point(0.34, 0.1), new Point(0.4, 0.14), new Point(0.5, 0.5), new Point(0.7, 0.56), new Point(1, 0.46) };
            builder.AddRevolvedGeometry(profile.Select(t => t.ToVector()).ToList(), null, new Point3D(0, 0, 0).ToVector(), new Vector3D(0, 0, 1).ToVector(), 100);
            return builder.ToMesh().ToMeshGeometry3D(true);
        }
    }
}
