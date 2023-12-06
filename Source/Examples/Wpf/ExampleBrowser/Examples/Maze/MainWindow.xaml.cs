using HelixToolkit.Wpf;
using Microsoft.Win32;
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

namespace Maze;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Maze", "Generates a simple maze. Using 'WalkAround' camera mode.")]
public partial class MainWindow
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();

        view.SubTitle = "E - export model\n1 - WalkAround camera mode\n2 - Inspect camera mode\nWASD - Move";

        if (view.Camera is not null)
        {
            view.Camera.NearPlaneDistance = 0.001;
            view.Camera.FarPlaneDistance = 1000;
        }

        if (view.Camera is PerspectiveCamera pc)
        {
            pc.FieldOfView = 90;
        }

        this.Loaded += this.MainWindow_Loaded;
    }

    void MainWindow_Loaded(object? sender, System.Windows.RoutedEventArgs e)
    {
        overview.SetView(new Point3D(0, 0, 60), new Vector3D(0, 0, -60), new Vector3D(0, 1, 0), 0);
    }

    /// <summary>
    /// Handles the key down event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
    private void WindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.E)
        {
            var d = new SaveFileDialog
            {
                Title = "Export maze",
                Filter = Exporters.Filter,
                DefaultExt = Exporters.DefaultExtension
            };

            if (d.ShowDialog() == true)
            {
                view.Export(d.FileName);
            }
        }

        if (e.Key == Key.D2)
        {
            view.CameraMode = CameraMode.Inspect;
        }

        if (e.Key == Key.D1)
        {
            view.CameraMode = CameraMode.WalkAround;
        }
    }

    private void CameraChanged(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm || view.Camera is null)
        {
            return;
        }

        var newPosition = vm.CoercePosition(view.Camera.Position);
        if (newPosition != null)
        {
            view.Camera.Position = newPosition.Value;
        }
    }
}
