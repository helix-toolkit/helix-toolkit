using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ScatterPlot;

public sealed partial class MainViewModel : ObservableObject
{
    public Point3D[] Data { get; set; } = Array.Empty<Point3D>();

    public double[] Values { get; set; } = Array.Empty<double>();

    public Model3DGroup Lights
    {
        get
        {
            var group = new Model3DGroup();
            group.Children.Add(new AmbientLight(Colors.White));
            return group;
        }
    }

    public Brush SurfaceBrush
    {
        get
        {
            // return BrushHelper.CreateGradientBrush(Colors.White, Colors.Blue);
            return GradientBrushes.RainbowStripes;
            // return GradientBrushes.BlueWhiteRed;
        }
    }

    public MainViewModel()
    {
        UpdateModel();
    }

    private void UpdateModel()
    {
        Data = Enumerable.Range(0, 7 * 7 * 7).Select(i => new Point3D(i % 7, (i % 49) / 7, i / 49)).ToArray();

        var rnd = new Random();
        this.Values = Data.Select(d => rnd.NextDouble()).ToArray();

        OnPropertyChanged(nameof(Data));
        OnPropertyChanged(nameof(Values));
        OnPropertyChanged(nameof(SurfaceBrush));
    }
}
