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
using System.Windows.Shapes;

namespace Building;

/// <summary>
/// Interaction logic for Window1.xaml
/// </summary>
[ExampleBrowser.Example("Building", "Using MeshBuilder to create buildings.")]
public partial class MainWindow : Window
{
    private ViewModel viewModel;

    public MainWindow()
    {
        this.InitializeComponent();
        this.DataContext = this.viewModel = new ViewModel();
    }

    private void UIElement_OnMouseDown(object? sender, MouseButtonEventArgs e)
    {
        var viewport = sender as HelixViewport3D;
        var firstHit = viewport?.Viewport.FindHits(e.GetPosition(viewport))?.FirstOrDefault();
        if (firstHit != null)
        {
            this.viewModel.Select(firstHit.Visual);
        }
        else
        {
            this.viewModel.Select(null);
        }
    }
}
