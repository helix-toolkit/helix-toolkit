// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace UIElementDemo
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var container = new ContainerUIElement3D();
            var element = new ModelUIElement3D();
            var geometry = new GeometryModel3D();
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddSphere(new Point3D(0, 0, 0), 2, 100, 50);
            geometry.Geometry = meshBuilder.ToMesh();
            geometry.Material = Materials.Green;
            element.Model = geometry;
            element.Transform = new TranslateTransform3D(5, 0, 0);
            element.MouseDown += this.ContainerElementMouseDown;
            container.Children.Add(element);
            view1.Children.Add(container);
        }

        private void ContainerElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var element = sender as ModelUIElement3D;
                var model = element.Model as GeometryModel3D;
                model.Material = model.Material == Materials.Green ? Materials.Gray : Materials.Green;
                e.Handled = true;
            }
        }
       
        private void ZoomExtents_Click(object sender, RoutedEventArgs e)
        {
            view1.ZoomExtents(500);
        }
    }
}