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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;

namespace AnaglyphDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var cubes = new ModelVisual3D();
            AddCubes(cubes.Children, Brushes.White, 0);
            AddCubes(cubes.Children, Brushes.Green, -4);
            AddCubes(cubes.Children, Brushes.Green, 4);
            ((ModelItem)model.Items[0]).Model = cubes;

            Loaded += OnLoaded;
        }

        private void AddCubes(Visual3DCollection c, Brush brush, double x)
        {
            for (double y = -5; y <= 5; y += 10)
                c.Add(new CubeVisual3D { Fill = brush, Center = new Point3D(x, y, 0) });
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            model.SelectedIndex = 0;
        }

        private void model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (anaglyphView1 == null || anaglyphView1.Children.Count == 0)
                return;
            var lights = anaglyphView1.Children[0];
            anaglyphView1.Clear();
            var m = model.SelectedItem as ModelItem;
            anaglyphView1.Children.Add(lights);
            anaglyphView1.Children.Add(m.Model);
            anaglyphView1.SynchronizeStereoModel();
            // anaglyphView1.ZoomExtents();
        }
    }

    public class ModelItem
    {
        public string Title { get; set; }
        public ModelVisual3D Model { get; set; }
    }
}