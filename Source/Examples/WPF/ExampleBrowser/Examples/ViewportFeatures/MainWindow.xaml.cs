// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ViewportFeaturesDemo
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ExampleBrowser;

    using HelixToolkit.Wpf;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Example(null, "Demonstrates features of the HelixViewport3D.")]
    public partial class MainWindow : Window
    {
        private bool _firstTime2 = true;

        private bool _firstTime7 = true;

        public MainWindow()
        {
            this.InitializeComponent();
            view2.Loaded += this.View2Loaded;
            view7.Loaded += this.View7Loaded;
        }

        private void View2Loaded(object sender, RoutedEventArgs e)
        {
            if (_firstTime2)
            {
                _firstTime2 = false;
                // add visuals for all lights in the scene
                foreach (Light light in Viewport3DHelper.GetLights(view2.Viewport))
                    view2.Children.Add(new LightVisual3D {Light = light});
            }
        }

        private void View7Loaded(object sender, RoutedEventArgs e)
        {
            if (view7.CameraController != null)
            {
                if (_firstTime7)
                {
                    _firstTime7 = false;

                    // add a box that shows the bounds
                    Rect3D bounds = Visual3DHelper.FindBounds(view7.Children);
                    view7.Children.Add(new BoundingBoxVisual3D {BoundingBox = bounds});

                    // add a coordinate system that shows the origin
                    view7.Children.Add(new CoordinateSystemVisual3D());
                }
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}