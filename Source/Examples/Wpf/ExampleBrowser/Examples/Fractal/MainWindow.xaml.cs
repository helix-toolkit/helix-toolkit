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
using System.Windows.Shapes;

namespace Fractal;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Fractal", "Performance test on self-similar geometries.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
        Loaded += MainWindow_Loaded;
    }

    void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        view1.ZoomExtents();
    }

    private void ZoomExtents_Click(object? sender, RoutedEventArgs e)
    {
        view1.ZoomExtents(400);
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
            view1.Export(fileName);
    }

    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}

public class MenuItemList : MenuItem
{

}
