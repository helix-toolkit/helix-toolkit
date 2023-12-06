using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Cloth;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("Cloth", "Simulates cloth physics.")]
public partial class MainWindow : Window
{
    public GeometryModel3D? FlagModel { get; set; }
    public Flag? Flag { get; private set; }

    private readonly Stopwatch watch;

    public MainWindow()
    {
        InitializeComponent();

        CreateFlag();

        DataContext = this;
        Loaded += MainWindow_Loaded;

        watch = new Stopwatch();
        watch.Start();

        Task.Factory.StartNew(IntegrationWorker);

        CompositionTarget.Rendering += this.OnCompositionTargetRendering;
    }

    private void IntegrationWorker()
    {
        while (true)
        {
            double dt = 1.0 * watch.ElapsedTicks / Stopwatch.Frequency;
            watch.Restart();
            Flag?.Update(dt);
        }
    }

    void OnCompositionTargetRendering(object? sender, EventArgs e)
    {
        Flag?.Transfer();
    }

    void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        view1.ZoomExtents();
    }

    private void CreateFlag()
    {
        Flag = new("pack://application:,,,/Examples/Cloth/FlagOfNorway.png");
        Flag.Init();

        FlagModel = new GeometryModel3D
        {
            Geometry = Flag.Mesh,
            Material = Flag.Material,
            BackMaterial = Flag.Material
        };
    }
}
