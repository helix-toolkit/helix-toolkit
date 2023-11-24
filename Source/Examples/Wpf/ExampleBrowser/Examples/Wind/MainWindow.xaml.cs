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
using System.Windows.Shapes;
using WiimoteLib;

namespace Wind;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Wind", "Head tracking by the Wii remote.")]
public partial class MainWindow
{
    private readonly ModelVisual3D model;

    public MainWindow()
    {
        InitializeComponent();

        model = new ModelVisual3D();

        const int rows = 5;
        const int columns = 4;
        const double distance = 120;

        var turbine = new WindTurbine();
        var r = new Random();
        for (int i = 0; i < rows; i++)
        {
            double y = i * distance;
            for (int j = 0; j + (i % 2) * 0.5 <= columns - 1; j++)
            {
                double x = (j + (i % 2) * 0.5) * distance;
                var visual = new WindTurbineVisual3D
                {
                    RotationAngle = r.Next(360),
                    RotationSpeed = 20,
                    WindTurbine = turbine,
                    Transform = new TranslateTransform3D(x, y, 0)
                };
                model.Children.Add(visual);
            }
        }

        var seasurface = new RectangleVisual3D
        {
            DivWidth = 100,
            DivLength = 100,
            Origin = new Point3D((rows - 2) * distance * 0.5, (columns) * distance * 0.5, 0),
            Width = rows * distance * 2,
            Length = columns * distance * 2
        };
        seasurface.Material = seasurface.BackMaterial = MaterialHelper.CreateMaterial(Colors.SeaGreen, 0.8);

        model.Children.Add(new GridLinesVisual3D() { Center = seasurface.Origin, Fill = Brushes.Gray, Width = seasurface.Width, Length = seasurface.Length });

        model.Children.Add(seasurface);
        view1.Children.Add(model);

        Loaded += MainWindowLoaded;
        Closed += MainWindowClosed;
    }

    private Wiimote? wm;

    void MainWindowClosed(object? sender, EventArgs e)
    {
        try
        {
            if (wm != null)
                wm.Disconnect();
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex);
        }
    }

    void MainWindowLoaded(object? sender, System.Windows.RoutedEventArgs e)
    {
        try
        {
            wm = new Wiimote();
            wm.WiimoteChanged += OnWiimoteChanged;
            wm.Connect();
            wm.SetReportType(InputReport.IRAccel, true);
            wm.SetLEDs(false, false, false, false);
        }
        catch
        {
            wm = null;
        }
    }

    private void OnWiimoteChanged(object? sender, WiimoteChangedEventArgs e)
    {
        HeadTracking(e.WiimoteState.IRState);
    }

    // headtracking 'light'
    // computes the angles to the observer around the camera up and right directions
    // also calculates the distance between two IR points - used for scaling
    // todo: take screen distance into account
    private void HeadTracking(IRState irState)
    {
        if (irState.IRSensors[0].Found)
        {
            var p0 = irState.IRSensors[0].RawPosition;
            double mx = p0.X;
            double my = p0.Y;
            double scale = 1;

            if (irState.IRSensors[1].Found)
            {
                var p1 = irState.IRSensors[1].RawPosition;
                double dx = p0.X - p1.X;
                double dy = p0.Y - p1.Y;
                double d = Math.Sqrt(dx * dx + dy * dy);
                mx = (p0.X + p1.X) * 0.5;
                my = (p0.Y + p1.Y) * 0.5;
                scale = d / 200.0;
            }
            double theta = 20.0 * (mx - 512) / 512;
            double phi = 20.0 * (my - 384) / 384;
            Dispatcher.BeginInvoke(new Action(() => SetTransform(scale, theta, phi)));
        }
        //else
        //    Dispatcher.BeginInvoke(new Action(() => SetTransform(1, 0, 0)));
    }

    private void SetTransform(double scale, double theta, double phi)
    {
        if (view1.Camera is null)
        {
            return;
        }

        var tg = new Transform3DGroup();
        tg.Children.Add(new ScaleTransform3D(scale, scale, scale));
        var center = view1.Camera.Position + view1.Camera.LookDirection;
        var up = view1.Camera.UpDirection;
        var right = Vector3D.CrossProduct(view1.Camera.LookDirection, up);
        tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(up, theta), center));
        tg.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(right, phi), center));
        model.Transform = tg;
    }
}
