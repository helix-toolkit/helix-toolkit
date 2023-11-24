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

namespace Tube;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Tube", "Shows Borromean rings using the TubeVisual3D.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // http://en.wikipedia.org/wiki/Borromean_rings
        // http://paulbourke.net/geometry/borromean/

        int n = 180;
        double r = Math.Sqrt(3) / 3;
        Ring1 = this.CreatePath(0, Math.PI * 2, n, u => Math.Cos(u), u => Math.Sin(u) + r, u => Math.Cos(3 * u) / 3);
        Ring2 = this.CreatePath(0, Math.PI * 2, n, u => Math.Cos(u) + 0.5, u => Math.Sin(u) - r / 2, u => Math.Cos(3 * u) / 3);
        Ring3 = this.CreatePath(0, Math.PI * 2, n, u => Math.Cos(u) - 0.5, u => Math.Sin(u) - r / 2, u => Math.Cos(3 * u) / 3);

        DataContext = this;
    }

    private Point3DCollection CreatePath(double min, double max, int n, Func<double, double> fx, Func<double, double> fy, Func<double, double> fz)
    {
        var list = new Point3DCollection(n);
        for (int i = 0; i < n; i++)
        {
            double u = min + (max - min) * i / n;
            list.Add(new Point3D(fx(u), fy(u), fz(u)));
        }
        return list;
    }

    public Point3DCollection Ring1 { get; set; }
    public Point3DCollection Ring2 { get; set; }
    public Point3DCollection Ring3 { get; set; }
}
