using HelixToolkit;
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

namespace UIElement;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("UIElement", "Test of UIElement3D in HelixViewport3D.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var container = new ContainerUIElement3D();
        var element = new ModelUIElement3D();
        var geometry = new GeometryModel3D();
        var meshBuilder = new MeshBuilder();
        meshBuilder.AddSphere(new Point3D(0, 0, 0).ToVector(), 2, 100, 50);
        geometry.Geometry = meshBuilder.ToMesh().ToMeshGeometry3D();
        geometry.Material = Materials.Green;
        element.Model = geometry;
        element.Transform = new TranslateTransform3D(5, 0, 0);
        element.MouseDown += this.ContainerElementMouseDown;
        container.Children.Add(element);
        view1.Children.Add(container);
    }

    private void ContainerElementMouseDown(object? sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var element = sender as ModelUIElement3D;

            if (element?.Model is GeometryModel3D model)
            {
                model.Material = model.Material == Materials.Green ? Materials.Gray : Materials.Green;
            }

            e.Handled = true;
        }
    }

    private void ZoomExtents_Click(object? sender, RoutedEventArgs e)
    {
        view1.ZoomExtents(500);
    }
}
