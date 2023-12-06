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

namespace SolarSystem;

/// <summary>
/// Interaction logic for Window1.xaml
/// </summary>
[ExampleBrowser.Example("SolarSystem", "Solar system demo.")]
public partial class MainWindow : Window
{
    private SolarSystem3D? SolarSystem;

    public MainWindow()
    {
        InitializeComponent();

        if (view1.Camera is not null)
        {
            view1.Camera.Position = new Point3D(0, 400, 500);
            view1.Camera.LookDirection = new Vector3D(0, -400, -500);
        }

        SolarSystem = view1.Children[2] as SolarSystem3D;
        DataContext = SolarSystem;

        Loaded += new RoutedEventHandler(Window1_Loaded);
    }

    void Window1_Loaded(object? sender, RoutedEventArgs e)
    {
        SolarSystem?.InitModel();
        SolarSystem?.UpdateModel();
    }
}
