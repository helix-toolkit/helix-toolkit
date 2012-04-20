// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ContourDemo
{
    using System.Diagnostics;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Plane3D ContourPlane;

        public MainWindow()
        {
            InitializeComponent();
            AddContours(model1, 8, 8, 8);
        }

        private void AddContours(Visual3D model1, int o, int m, int n)
        {
            var bounds = Visual3DHelper.FindBounds(model1, Transform3D.Identity);
            for (int i = 1; i < n; i++)
            {
                this.ContourPlane = new Plane3D(new Point3D(0, 0, bounds.Location.Z + bounds.Size.Z * i / n), new Vector3D(0, 0, 1));
                Visual3DHelper.Traverse<GeometryModel3D>(model1, this.AddContours);
            }
            for (int i = 1; i < m; i++)
            {
                this.ContourPlane = new Plane3D(new Point3D(0, bounds.Location.Y + bounds.Size.Y * i / m, 0), new Vector3D(0, 1, 0));
                Visual3DHelper.Traverse<GeometryModel3D>(model1, this.AddContours);
            }
            for (int i = 1; i < o; i++)
            {
                this.ContourPlane = new Plane3D(new Point3D(bounds.Location.X + bounds.Size.X * i / o, 0, 0), new Vector3D(1, 0, 0));
                Visual3DHelper.Traverse<GeometryModel3D>(model1, this.AddContours);
            }
        }

        private void AddContours(GeometryModel3D model, Transform3D transform)
        {
            var p = ContourPlane.Position;
            var n = ContourPlane.Normal;
            var segments = MeshGeometryHelper.GetContourSegments(model.Geometry as MeshGeometry3D, p, n).ToList();
            foreach (IList<Point3D> contour in MeshGeometryHelper.CombineSegments(segments, 1e-6).ToList())
            {
                if (contour.Count == 0)
                    continue;
                view2.Children.Add(new TubeVisual3D { Diameter = 0.03, Path = contour, Fill = Brushes.Green });
            }
        }

        private void view2_cameraChanged(object sender, RoutedEventArgs e)
        {
            CameraHelper.Copy(view2.Camera, view1.Camera);
        }

        private void view1_cameraChanged(object sender, RoutedEventArgs e)
        {
            CameraHelper.Copy(view1.Camera, view2.Camera);
        }
    }
}