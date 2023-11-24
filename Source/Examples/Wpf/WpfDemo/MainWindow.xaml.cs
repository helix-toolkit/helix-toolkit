using DependencyPropertyGenerator;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[DependencyProperty<int>("BrickRows", DefaultValue = 2)]
[DependencyProperty<int>("BrickColumns", DefaultValue = 4)]
[DependencyProperty<int>("BrickHeight", DefaultValue = 3)]
[DependencyProperty<Brush>("CurrentBrush", DefaultValueExpression = "brushes[0]")]
public partial class MainWindow : Window
{
    // http://guide.lugnet.com/color/

    private static readonly Brush[] brushes = new[]
                                           {
                                               Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Yellow, Brushes.White,
                                               Brushes.Gray, Brushes.Black, BrushHelper.ChangeOpacity(Brushes.Red, 0.3)
                                           };

    private int currentBrushIndex;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        if (view1.Camera is null)
        {
            return;
        }

        view1.Camera.NearPlaneDistance = LegoVisual3D.GridUnit;
        view1.Camera.FarPlaneDistance = 10;
        view1.ZoomExtents();
    }

    private void view1_MouseDown(object? sender, MouseButtonEventArgs e)
    {
        var lego = new LegoVisual3D();
        lego.Rows = BrickRows;
        lego.Columns = BrickColumns;
        lego.Height = BrickHeight;
        lego.Fill = CurrentBrush!;
        Point3D? pt = view1.FindNearestPoint(e.GetPosition(view1));
        if (pt.HasValue)
        {
            Point3D p = pt.Value;
            double gu = LegoVisual3D.GridUnit;
            double hu = LegoVisual3D.HeightUnit;
            p.X = gu * Math.Floor(p.X / gu);
            p.Y = gu * Math.Floor(p.Y / gu);
            p.Z = hu * Math.Floor(p.Z / hu);

            lego.Transform = new TranslateTransform3D(p.X, p.Y, p.Z);
            view1.Children.Add(lego);
        }
    }

    private void CurrentColor_Click(object? sender, RoutedEventArgs routedEventArgs)
    {
        currentBrushIndex++;
        CurrentBrush = brushes[currentBrushIndex % brushes.Length];
    }

    private void Export_Click(object? sender, RoutedEventArgs e)
    {
        var d = new SaveFileDialog
        {
            Filter = Exporters.Filter
        };

        if (d.ShowDialog() == true)
        {
            Export(d.FileName);
        }
    }

    private void Export(string fileName)
    {
        if (fileName != null)
        {
            view1.Export(fileName);
        }
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
