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

namespace Panorama;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Panorama", "Panorama demo.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var camera = view1.Camera as PerspectiveCamera;

        if (camera is null)
        {
            return;
        }

        camera.Position = new Point3D(0, 0, 0);
        camera.LookDirection = new Vector3D(0, 1, 0);
        camera.UpDirection = new Vector3D(0, 0, 1);
        camera.FieldOfView = 120;
    }
}
