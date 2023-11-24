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

namespace MemoryLeakTest;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("MemoryLeakTest", "Testing for memory leaks.")]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Open1Click(object? sender, RoutedEventArgs e)
    {
        new Window1().Show();
    }

    private void Open2Click(object? sender, RoutedEventArgs e)
    {
        new Window2().Show();
    }

    private void Open3Click(object? sender, RoutedEventArgs e)
    {
        new Window3().Show();
    }

    private void CollectClick(object? sender, RoutedEventArgs e)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}
