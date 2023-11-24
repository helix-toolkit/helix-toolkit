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

namespace Torus;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Torus", "Shows three torus objects using the TorusVisual3D.")]
public partial class MainWindow : Window
{
    public Transform3D Transform1 = new TranslateTransform3D(new Vector3D(-4, 0, 0));
    public Transform3D Transform3 = new TranslateTransform3D(new Vector3D(4, 0, 0));

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        torus1.Transform = Transform1;
        torus3.Transform = Transform3;

        if (viewPort.Camera is not null)
        {
            viewPort.Camera.Position = new Point3D(0, 0, 10);
        }
    }
}
