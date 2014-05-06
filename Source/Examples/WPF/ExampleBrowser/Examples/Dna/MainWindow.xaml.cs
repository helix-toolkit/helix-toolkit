// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DnaDemo
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Shows a double helix, the first exmaple of this library :)")]
    public partial class MainWindow : Window
    {
        private static readonly Random r = new Random();

        private readonly Brush[] baseBrush1 = new Brush[] { Brushes.Blue, Brushes.Yellow, Brushes.Red, Brushes.Green };
        private readonly Brush[] baseBrush2 = new Brush[] { Brushes.Yellow, Brushes.Blue, Brushes.Green, Brushes.Red };

        public MainWindow()
        {
            InitializeComponent();

            // Adding elements programatically
            AddBases(model, 24, 3, 30);
        }

        private void AddBases(ModelVisual3D model, int number, double turns, double length)
        {
            double b = turns * 2 * Math.PI;
            double l = length;
            double p1 = 0;
            double p2 = 3.14;
            for (int i = 0; i < number; i++)
            {
                double u = (double)i / (number - 1);
                double x1 = Math.Cos(b * u + p1) + Math.Cos(b * u + p1);
                double y1 = Math.Sin(b * u + p1) + Math.Sin(b * u + p1);
                double z = u * l;
                double x2 = Math.Cos(b * u + p2) + Math.Cos(b * u + p2);
                double y2 = Math.Sin(b * u + p2) + Math.Sin(b * u + p2);
                var pt1 = new Point3D(x1, y1, z);
                var pt2 = new Point3D(x2, y2, z);
                var pt3 = new Point3D(0, 0, z);

                int j = r.Next(4);
                Brush brush1 = baseBrush1[j];
                Brush brush2 = baseBrush2[j];

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

    public enum Base
    {
        A,
        D,
        T,
        C
    } ;
}