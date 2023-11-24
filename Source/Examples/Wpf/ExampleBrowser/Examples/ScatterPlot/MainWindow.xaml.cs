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

namespace ScatterPlot;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("ScatterPlot", "3D scatter plot.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.DataContext = new MainViewModel();
    }

    private void FileExit_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
