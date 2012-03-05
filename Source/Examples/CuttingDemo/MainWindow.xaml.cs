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

namespace CuttingPlanesDemo
{
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*var mb = new MeshBuilder();
            mb.AddSphere(new Point3D(0, 0, 0), 1);
            var mesh = mb.ToMesh();
            var n = new Vector3D(0, 0.2, 1);
            var p = new Point3D(0, 0, 0.5);
            var geo = MeshGeometryHelper.Cut(mesh, p,n);
            var m = new GeometryModel3D(geo, Materials.Blue);
            m.BackMaterial = Materials.Red;
            var mv = new ModelVisual3D();
            mv.Content = m;
            view1.Children.Add(mv);
            var segments = MeshGeometryHelper.GetContourSegments(mesh, p,n).ToList();
            foreach (IList<Point3D> contour in MeshGeometryHelper.CombineSegments(segments, 1e-6).ToList())
            {
                if (contour.Count == 0)
                    continue;
                view1.Children.Add(new TubeVisual3D { Diameter = 0.02, Path = contour, Fill = Brushes.Green });
            }
            view1.Children.Add(new RectangleVisual3D { Origin = p, Normal = n, Fill = new SolidColorBrush(Color.FromArgb(80, 255, 0, 0)) });*/
        }
    }
}