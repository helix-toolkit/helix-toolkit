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

namespace Voxels;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Voxels", "Edit a voxel scene by clicking the sides of the voxels.")]
public partial class MainWindow : Window
{
    private readonly MainViewModel vm = new();

    public MainWindow()
    {
        this.InitializeComponent();
        this.vm.TryLoad("MyModel.xml");
        this.DataContext = vm;
        this.Loaded += this.MainWindowLoaded;
    }

    void MainWindowLoaded(object? sender, RoutedEventArgs e)
    {
        view1.ZoomExtents(500);
        view1.Focus();
    }

    protected override void OnClosed(EventArgs e)
    {
        this.vm.Save("MyModel.xml");
        base.OnClosed(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        switch (e.Key)
        {
            case Key.Space:
                vm.PaletteIndex++;
                vm.CurrentColor = vm.GetPaletteColor();
                break;
            case Key.A:
                view1.ZoomExtents(500);
                break;
            case Key.C:
                vm.Clear();
                break;
        }
    }

    Model3D? FindSource(Point p, out Vector3D normal)
    {
        var hits = Viewport3DHelper.FindHits(view1.Viewport, p);

        if (hits is not null)
        {
            foreach (var h in hits)
            {
                if (h.Model == vm.PreviewModel)
                    continue;
                normal = h.Normal;
                return h.Model;
            }
        }

        normal = new Vector3D();
        return null;
    }

    private void View1_MouseDown(object? sender, MouseButtonEventArgs e)
    {
        bool shift = (Keyboard.IsKeyDown(Key.LeftShift));
        var p = e.GetPosition(view1);

        var source = FindSource(p, out Vector3D n);
        if (source != null)
        {
            if (shift)
                vm.Remove(source);
            else
                vm.Add(source, n);
        }
        else
        {
            var ray = Viewport3DHelper.Point2DtoRay3D(view1.Viewport, p);
            if (ray != null)
            {
                var pi = ray.PlaneIntersection(new Point3D(0, 0, 0.5), new Vector3D(0, 0, 1));
                if (pi.HasValue)
                {
                    var pRound = new Point3D(Math.Round(pi.Value.X), Math.Round(pi.Value.Y), 0);
                    //    var pRound = new Point3D(Math.Floor(pi.Value.X), Math.Floor(pi.Value.Y), Math.Floor(pi.Value.Z));
                    //var pRound = new Point3D((int)pi.Value.X, (int)pi.Value.Y, (int)pi.Value.Z);
                    vm.AddVoxel(pRound);
                }
            }
        }
        UpdatePreview();
        //CaptureMouse();
    }

    private void View1_MouseMove(object? sender, MouseEventArgs e)
    {
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        var p = Mouse.GetPosition(view1);
        bool shift = (Keyboard.IsKeyDown(Key.LeftShift));
        var source = FindSource(p, out Vector3D n);
        if (shift)
        {
            vm.PreviewVoxel(null);
            vm.HighlightVoxel(source);
        }
        else
        {
            vm.PreviewVoxel(source, n);
            vm.HighlightVoxel(null);
        }

    }

    private void view1_MouseUp(object? sender, MouseButtonEventArgs e)
    {
        //  ReleaseMouseCapture();
    }

    private void view1_KeyUp(object? sender, KeyEventArgs e)
    {
        // Should update preview voxel when shift is released
        UpdatePreview();
    }

    private void view1_KeyDown(object? sender, KeyEventArgs e)
    {
        // Should update preview voxel when shift is pressed
        UpdatePreview();
    }
}
