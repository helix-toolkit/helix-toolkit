using System.Windows;

using ExampleBrowser.Examples.Terrain.ViewModels;

namespace Terrain;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Terrain", "Rendering a terrain loaded from different terrain files (.bt, .hgt).")]
public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel = new MainWindowViewModel();

    public MainWindow()
    {
        this.InitializeComponent();

        this.DataContext = _viewModel;
        this.Loaded += this.MainWindowLoaded;
    }

    void MainWindowLoaded(object? sender, RoutedEventArgs e)
    {
        view1.ZoomExtents();
    }
}
