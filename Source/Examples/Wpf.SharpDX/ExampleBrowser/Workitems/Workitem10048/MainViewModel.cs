using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Workitem10048;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    private static readonly Point3D NoHit = new(double.NaN, double.NaN, double.NaN);

    [ObservableProperty]
    private Point3D pointHit = NoHit;

    public MainViewModel()
    {
        // titles
        this.Title = "Simple Demo (Workitem 10048 and 10052)";
        this.SubTitle = "Select lines with left mouse button.\nRotate or zoom around a point on a line if the cursor is above one.";

        EffectsManager = new DefaultEffectsManager();
    }

    public void OnMouseDown3D(object? sender, RoutedEventArgs e)
    {
        this.PointHit = (e as MouseDown3DEventArgs)?.HitTestResult?.PointHit.ToPoint3D() ?? NoHit;
    }
}
