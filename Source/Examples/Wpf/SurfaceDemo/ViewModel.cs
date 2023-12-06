using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows.Media;

namespace SurfaceDemo;

public partial class ViewModel : ObservableObject
{
    [ObservableProperty]
    private Brush? _brush;

    [ObservableProperty]
    private int _meshSizeU = 140;

    [ObservableProperty]
    private int _meshSizeV = 140;

    [ObservableProperty]
    private double _parameterW = 1;

    [ObservableProperty]
    private double _stereoBase = 0.05;

    [ObservableProperty]
    private ViewMode _viewMode = SurfaceDemo.ViewMode.Normal;

    [ObservableProperty]
    private string _modelTitle = string.Empty;
}
