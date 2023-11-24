using CommunityToolkit.Mvvm.Input;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ViewportFeatures;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("ViewportFeatures", "Demonstrates features of the HelixViewport3D.")]
public partial class MainWindow : Window
{
    private bool _firstTime2 = true;

    private bool _firstTime7 = true;

    public MainWindow()
    {
        this.InitializeComponent();
        this.DataContext = this;
        this.view2.Loaded += this.View2Loaded;
        this.view7.Loaded += this.View7Loaded;
        this.ResetCommand = new RelayCommand(() => this.view8.FitView(new Vector3D(1, -1, -1), new Vector3D(0, 0, 1), 500));
    }

    public ICommand ResetCommand { get; private set; }

    private void View2Loaded(object? sender, RoutedEventArgs e)
    {
        if (_firstTime2)
        {
            _firstTime2 = false;
            // add visuals for all lights in the scene
            foreach (Light? light in Viewport3DHelper.GetLights(view2.Viewport))
            {
                if (light is null)
                {
                    continue;
                }

                view2.Children.Add(new LightVisual3D { Light = light });
            }
        }
    }

    private void View7Loaded(object? sender, RoutedEventArgs e)
    {
        if (view7.CameraController != null)
        {
            if (_firstTime7)
            {
                _firstTime7 = false;

                // add a box that shows the bounds
                Rect3D bounds = Visual3DHelper.FindBounds(view7.Children);
                view7.Children.Add(new BoundingBoxVisual3D { BoundingBox = bounds });

                // add a coordinate system that shows the origin
                view7.Children.Add(new CoordinateSystemVisual3D());
            }
        }
    }

    private void Hyperlink_RequestNavigate(object? sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        e.Handled = true;
    }

    public class ViewModel
    {

    }
}
