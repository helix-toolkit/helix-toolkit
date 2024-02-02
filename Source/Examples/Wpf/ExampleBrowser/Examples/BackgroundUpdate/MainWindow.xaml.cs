using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.Geometry;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace BackgroundUpdate;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[ExampleBrowser.Example("BackgroundUpdate", "Updates the visual model in a background thread.")]
[ObservableObject]
public partial class MainWindow : Window
{
    private CancellationTokenSource source = new();

    private readonly Material mat2 = MaterialHelper.CreateMaterial(Colors.Red);

    private int n = 10;

    [ObservableProperty]
    private int count1;

    [ObservableProperty]
    private int count2;

    [ObservableProperty]
    private int count3;

    [ObservableProperty]
    private int count4;

    private int runningWorkers;

    public MainWindow()
    {
        this.InitializeComponent();
        this.DataContext = this;
        this.Loaded += this.MainWindowLoaded;
        this.Closing += this.MainWindowClosing;
        this.AddPoints = true;
        this.AddFrozenGeometry = true;
        this.AddFrozenModel = true;
        this.AddToModelGroup = true;
    }

    public bool AddPoints { get; set; }

    public bool AddFrozenGeometry { get; set; }

    public bool AddFrozenModel { get; set; }

    public bool AddToModelGroup { get; set; }

    void MainWindowClosing(object? sender, CancelEventArgs e)
    {
        this.source.Cancel();
    }

    void MainWindowLoaded(object? sender, RoutedEventArgs e)
    {
        Materials.Gold.Freeze();

        this.source = new CancellationTokenSource();
        Task.Factory.StartNew(this.Worker1, this.Dispatcher, source.Token);
        Task.Factory.StartNew(this.Worker2, this.Dispatcher, source.Token);
        Task.Factory.StartNew(this.Worker3, this.Dispatcher, source.Token);
        Task.Factory.StartNew(this.Worker4, this.Dispatcher, source.Token);
    }

    private void Worker1(object? d)
    {
        if (d is null)
        {
            return;
        }

        var dispatcher = (Dispatcher)d;
        Interlocked.Increment(ref this.runningWorkers);
        while (!this.source.IsCancellationRequested)
        {
            if (!this.AddPoints || this.runningWorkers < 4)
            {
                Thread.Yield();
                continue;
            }

            for (int i = 1; i <= n && this.AddPoints; i++)
            {
                for (int j = 1; j <= n && this.AddPoints; j++)
                {
                    for (int k = 1; k <= n && this.AddPoints; k++)
                    {
                        dispatcher.Invoke(new Action<Point3D, ModelVisual3D>(this.Add), new Point3D(-i, j, k), this.model1);
                    }
                }
            }

            dispatcher.Invoke((Action)(() => this.Count1++));
            dispatcher.Invoke(new Action<ModelVisual3D>(this.Clear), this.model1);
        }
    }

    private void Worker2(object? d)
    {
        if (d is null)
        {
            return;
        }

        var dispatcher = (Dispatcher)d;
        Interlocked.Increment(ref this.runningWorkers);
        while (!this.source.IsCancellationRequested)
        {
            if (!this.AddFrozenGeometry || this.runningWorkers < 4)
            {
                Thread.Yield();
                continue;
            }

            for (int i = 1; i <= n; i++)
            {
                var b = new MeshBuilder();
                for (int j = 1; j <= n; j++)
                {
                    for (int k = 1; k <= n; k++)
                    {
                        b.AddBox(new Vector3(i, j, k), 0.8f, 0.8f, 0.8f);
                    }
                }

                dispatcher.Invoke(new Action<MeshGeometry3D, Material, ModelVisual3D>(this.Add), b.ToMesh().ToWndMeshGeometry3D(true), mat2, model2);
            }

            dispatcher.Invoke((Action)(() => this.Count2++));

            dispatcher.Invoke(new Action<ModelVisual3D>(this.Clear), this.model2);
        }
    }

    private void Worker3(object? d)
    {
        if (d is null)
        {
            return;
        }

        var dispatcher = (Dispatcher)d;
        var m = MaterialHelper.CreateMaterial(Colors.Green);
        Interlocked.Increment(ref this.runningWorkers);
        while (!this.source.IsCancellationRequested)
        {
            if (!this.AddFrozenModel || this.runningWorkers < 4)
            {
                Thread.Yield();
                continue;
            }

            for (int i = 1; i <= n; i++)
            {
                var b = new MeshBuilder();
                for (int j = 1; j <= n; j++)
                {
                    for (int k = 1; k <= n; k++)
                    {
                        b.AddBox(new Vector3(i, j, -k), 0.8f, 0.8f, 0.8f);
                    }
                }

                var box = new GeometryModel3D { Geometry = b.ToMesh().ToWndMeshGeometry3D(false), Material = m };
                box.Freeze();

                dispatcher.Invoke(new Action<Model3D, ModelVisual3D>(this.Add), box, this.model3);
            }

            dispatcher.Invoke((Action)(() => this.Count3++));
            dispatcher.Invoke(new Action<ModelVisual3D>(this.Clear), model3);
        }
    }

    private void Worker4(object? d)
    {
        if (d is null)
        {
            return;
        }

        var dispatcher = (Dispatcher)d;
        var m = MaterialHelper.CreateMaterial(Colors.Gold);
        Model3DGroup? mg = null;
        dispatcher.Invoke(new Action(() => this.model4.Content = mg = new Model3DGroup()));

        Interlocked.Increment(ref this.runningWorkers);

        while (!this.source.IsCancellationRequested)
        {
            if (!this.AddToModelGroup || this.runningWorkers < 4)
            {
                Thread.Yield();
                continue;
            }

            for (int i = 1; i <= n; i++)
            {
                var b = new MeshBuilder();
                for (int j = 1; j <= n; j++)
                {
                    for (int k = 1; k <= n; k++)
                    {
                        b.AddBox(new Vector3(-i, j, -k), 0.8f, 0.8f, 0.8f);
                    }
                }

                var box = new GeometryModel3D { Geometry = b.ToMesh().ToWndMeshGeometry3D(false), Material = m };
                box.Freeze();
                dispatcher.Invoke(new Action(() => mg!.Children.Add(box)));
            }
            dispatcher.Invoke((Action)(() => this.Count4++));
            dispatcher.Invoke((Action)(() => mg!.Children.Clear()));
        }
    }

    private void Clear(ModelVisual3D visual)
    {
        visual.Children.Clear();
    }

    private void Add(Point3D p, ModelVisual3D visual)
    {
        // Create the box visual on the UI thread
        var box = new BoxVisual3D { Center = p, Width = 0.8, Height = 0.8, Length = 0.8 };
        visual.Children.Add(box);
    }

    private void Add(MeshGeometry3D g, Material m, ModelVisual3D visual)
    {
        var box = new GeometryModel3D { Geometry = g, Material = m };
        visual.Children.Add(new ModelVisual3D { Content = box });
    }

    private void Add(Model3D model, ModelVisual3D visual)
    {
        visual.Children.Add(new ModelVisual3D { Content = model });
    }
}
