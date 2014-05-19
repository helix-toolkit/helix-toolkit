// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChamferDemo
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;

    using ExampleBrowser;
    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Chamfers the corner of a cube by the MeshBuilder.ChamferCorner method.")]
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer dt;

        public MainWindow()
        {
            InitializeComponent();
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(0.3);
            dt.Tick += TimerTick;
            dt.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            dt.Stop();
            view1.Children.Add(CreateDice());
            view1.ZoomExtents();
        }

        private ModelVisual3D CreateDice()
        {
            var diceMesh = new MeshBuilder();
            diceMesh.AddBox(new Point3D(0, 0, 0), 1, 1, 1);
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < 2; k++)
                    {
                        var points = new List<Point3D>();
                        diceMesh.ChamferCorner(new Point3D(i - 0.5, j - 0.5, k - 0.5), 0.1, 1e-6, points);
                        //foreach (var p in points)
                        //    b.ChamferCorner(p, 0.03);
                    }

            return new ModelVisual3D { Content = new GeometryModel3D { Geometry = diceMesh.ToMesh(), Material = Materials.White } };
        }
    }
}