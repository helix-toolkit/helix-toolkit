using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShadowMapDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Closed += (s, e) => (DataContext as IDisposable)?.Dispose();
    }

    private void Button_Click(object? sender, RoutedEventArgs e)
    {
        var win = new MultiViewportWindow()
        {
            DataContext = this.DataContext
        };

        win.Show();
    }
}