using HelixToolkit.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace Export;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Export", "Exports a model to Kerkythea or Octane (.obj).")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Export_Click(object? sender, RoutedEventArgs e)
    {
        var d = new SaveFileDialog
        {
            Filter = Exporters.Filter,
            DefaultExt = Exporters.DefaultExtension
        };

        if (d.ShowDialog() != true)
        {
            return;
        }

        Viewport3DHelper.Export(view1.Viewport, d.FileName);

        //using (var exporter = new KerkytheaExporter(d.FileName))
        //{
        //    var m1 = this.Resources["m1"] as Material;
        //    exporter.RegisterMaterial(m1, @"Materials\water.xml");
        //    exporter.Export(view1.Viewport);
        //}
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ZoomExtents_Click(object? sender, RoutedEventArgs e)
    {
        view1.ZoomExtents(500);
    }

    private void View1_MouseDown(object? sender, MouseButtonEventArgs e)
    {
        var p = e.GetPosition(view1);
        var v = Viewport3DHelper.FindNearestVisual(view1.Viewport, p);
        // Left-clicking with control creates a bounding box around the object
        if (v != null && Keyboard.IsKeyDown(Key.LeftCtrl))
        {
            var rect = Visual3DHelper.FindBounds(v, Transform3D.Identity);
            view1.Children.Add(new BoundingBoxVisual3D() { BoundingBox = rect });
            return;
        }

        // Left-clicking adds a blue sphere at the nearest hit point
        var pt = Viewport3DHelper.FindNearestPoint(view1.Viewport, p);
        if (pt.HasValue)
            view1.Children.Add(new SphereVisual3D() { Center = pt.Value, Radius = 0.03 });
    }

    private void Print_Click(object? sender, RoutedEventArgs e)
    {
        var dlg = new PrintDialog();
        if (dlg.ShowDialog().GetValueOrDefault())
        {
            dlg.PrintVisual(view1.Viewport, this.Title);
        }
    }

    private void OpenOctane_Click(object? sender, RoutedEventArgs e)
    {
        var filename = "octanetest.obj";
        Viewport3DHelper.Export(view1.Viewport, filename);
        var fullPath = Path.GetFullPath(filename);

        var os = new OctaneLauncher { MeshNode = filename, MeshFile = fullPath, ProjectFile = "octanetest.ocs" };
        os.SetCamera((ProjectionCamera)view1.Viewport.Camera);
        os.Start();
    }

    private void OpenKerkythea_Click(object? sender, RoutedEventArgs e)
    {
        Cursor = Cursors.Wait;
        var input = "kerkytheatest.xml";
        var output = "kerkytheatest.png";
        Viewport3DHelper.Export(view1.Viewport, input);
        var ktl = new KerkytheaLauncher { InputFile = input, OutputFile = output };
        var p = ktl.Start();
        p?.WaitForExit();
        Process.Start(output);
        Cursor = Cursors.Arrow;
    }
}
