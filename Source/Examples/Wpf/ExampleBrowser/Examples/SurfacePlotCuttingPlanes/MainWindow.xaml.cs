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

namespace SurfacePlotCuttingPlanes;

/// <summary>
/// Interaction logic for Window.xaml
/// </summary>
[ExampleBrowser.Example("SurfacePlotCuttingPlanes", "Applies cutting planes to a surface that utilises texture coordinates.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.DataContext = new SurfacePlot.MainViewModel();
    }

    private void FileExit_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
