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

namespace Stereo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Stereo", "Stereo view")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        AddCube(stereoView1.Children);
        AddCube(anaglyphView1.Children);
        AddCube(interlacedView1.Children);
        AddCube(wiggleView1.Children);

        Loaded += this.WindowLoaded;
    }

    private void AddCube(IList<Visual3D> coll)
    {
        //            coll.Add(new CubeVisual3D {Fill = CreateDrawingBrush()});
        var brush = CreateDrawingBrush();
        brush.Freeze();
        for (int i = -5; i < 2; i++)
        {
            coll.Add(new CubeVisual3D
            {
                Fill = brush,
                Center = new Point3D(0, i * 4, 0)
            });
        }
    }

    private Brush CreateDrawingBrush()
    {
        var db = new DrawingBrush
        {
            TileMode = TileMode.Tile,
            ViewportUnits = BrushMappingMode.Absolute,
            Viewport = new Rect(0, 0, 0.1, 0.1),
            Viewbox = new Rect(0, 0, 1, 1),
            ViewboxUnits = BrushMappingMode.Absolute
        };
        var dg = new DrawingGroup();
        dg.Children.Add(new GeometryDrawing { Geometry = new RectangleGeometry(new Rect(0, 0, 1, 1)), Brush = Brushes.White });
        dg.Children.Add(new GeometryDrawing { Geometry = new RectangleGeometry(new Rect(0.25, 0.25, 0.5, 0.5)), Brush = Brushes.Black });

        db.Drawing = dg;
        return db;
    }

    private Brush CreateVisualBrush()
    {
        var vb = new VisualBrush
        {
            TileMode = TileMode.Tile,
            ViewportUnits = BrushMappingMode.Absolute,
            Viewport = new Rect(0, 0, 0.1, 0.1),
            Viewbox = new Rect(0, 0, 1, 1),
            ViewboxUnits = BrushMappingMode.Absolute
        };
        var c = new Canvas();
        c.Children.Add(new Rectangle { Fill = Brushes.White, Width = 1, Height = 1 });
        var r = new Rectangle { Fill = Brushes.Black, Width = 0.5, Height = 0.5 };
        Canvas.SetLeft(r, 0.25);
        Canvas.SetTop(r, 0.25);
        c.Children.Add(r);
        vb.Visual = c;
        vb.Freeze();
        return vb;
    }

    private void WindowLoaded(object? sender, RoutedEventArgs e)
    {
        anaglyphView1.SynchronizeStereoModel();
        interlacedView1.SynchronizeStereoModel();
        stereoView1.SynchronizeStereoModel();
        wiggleView1.SynchronizeStereoModel();
    }
}
