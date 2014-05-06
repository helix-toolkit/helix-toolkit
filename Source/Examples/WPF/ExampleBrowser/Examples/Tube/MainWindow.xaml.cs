// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace TubeDemo
{
    using System;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Shows Borromean rings using the TubeVisual3D.")]
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
}