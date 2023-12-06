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

namespace ViewMatrix;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("ViewMatrix", "Visualization of view and projection matrices (under construction).")]
public partial class MainWindow : Window
{
    private readonly MainViewModel vm;

    public MainWindow()
    {
        this.InitializeComponent();
        this.vm = new MainViewModel();
        this.DataContext = this.vm;
    }

    private void OnCameraChanged(object? sender, RoutedEventArgs e)
    {
        var vp = sender as HelixViewport3D;
        vm.ViewMatrix = vp?.Viewport.GetViewMatrix() ?? Matrix3D.Identity;
        vm.ViewportMatrix = vp?.Viewport.GetViewportTransform() ?? Matrix3D.Identity;
        vm.ProjectionMatrix = vp?.Viewport.GetProjectionMatrix() ?? Matrix3D.Identity;
    }
}
