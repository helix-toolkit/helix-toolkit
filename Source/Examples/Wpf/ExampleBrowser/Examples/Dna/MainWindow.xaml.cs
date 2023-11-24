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

namespace Dna;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Dna", "Shows a double helix, the first example of this library :)")]
public partial class MainWindow : Window
{
    private static readonly Random r = new();

    private static readonly Brush[] BaseBrush1 = { Brushes.Blue, Brushes.Yellow, Brushes.Red, Brushes.Green };

    private static readonly Brush[] BaseBrush2 = { Brushes.Yellow, Brushes.Blue, Brushes.Green, Brushes.Red };

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();

        // Adding elements programatically
        this.AddBases(this.model, 24, 3, 30);
    }

    private void AddBases(ModelVisual3D model, int number, double turns, double length)
    {
        var b = turns * 2 * Math.PI;
        var l = length;
        var p1 = 0d;
        var p2 = 3.14;
        for (int i = 0; i < number; i++)
        {
            var u = (double)i / (number - 1);
            var bu = b * u;
            var x1 = Math.Cos(bu + p1) + Math.Cos(bu + p1);
            var y1 = Math.Sin(bu + p1) + Math.Sin(bu + p1);
            var z = u * l;
            var x2 = Math.Cos(bu + p2) + Math.Cos(bu + p2);
            var y2 = Math.Sin(bu + p2) + Math.Sin(bu + p2);
            var pt1 = new Point3D(x1, y1, z);
            var pt2 = new Point3D(x2, y2, z);
            var pt3 = new Point3D(0, 0, z);

            var j = r.Next(4);
            var brush1 = BaseBrush1[j];
            var brush2 = BaseBrush2[j];

            var ts = new PipeVisual3D
            {
                Point1 = pt1,
                Point2 = pt3,
                Diameter = 0.4,
                Material = MaterialHelper.CreateMaterial(brush1)
            };
            model.Children.Add(ts);

            var ts2 = new PipeVisual3D
            {
                Point1 = pt3,
                Point2 = pt2,
                Diameter = 0.4,
                Material = MaterialHelper.CreateMaterial(brush2)
            };
            model.Children.Add(ts2);
        }
    }
}
