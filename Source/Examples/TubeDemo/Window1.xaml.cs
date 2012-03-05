// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Window1.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace TubeDemo
{
    using System.Collections.Generic;

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
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

        private IList<Point3D> CreatePath(double min, double max, int n, Func<double, double> fx, Func<double, double> fy, Func<double, double> fz)
        {
            var list = new List<Point3D>(n);
            for (int i = 0; i < n; i++)
            {
                double u = min + (max - min) * i / (n - 1);
                list.Add(new Point3D(fx(u), fy(u), fz(u)));
            }
            return list;
        }

        public IList<Point3D> Ring1 { get; set; }
        public IList<Point3D> Ring2 { get; set; }
        public IList<Point3D> Ring3 { get; set; }
    }
}