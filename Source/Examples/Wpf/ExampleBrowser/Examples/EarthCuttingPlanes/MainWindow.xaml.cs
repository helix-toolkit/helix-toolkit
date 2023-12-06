using DependencyPropertyGenerator;
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

namespace EarthCuttingPlanes;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("EarthCuttingPlanes", "Applies cutting planes to the Earth.")]
[DependencyProperty<Material>("CloudsMaterial")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.CloudsMaterial = MaterialHelper.CreateImageMaterial("pack://application:,,,/Examples/Earth/clouds.jpg", 0.5);
        this.DataContext = this;
    }
}
