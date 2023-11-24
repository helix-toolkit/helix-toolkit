using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Media3D;

namespace MvvmCamera;

public sealed partial class ViewportViewModel : ObservableObject
{
    [ObservableProperty]
    private PerspectiveCamera? _camera;
}
