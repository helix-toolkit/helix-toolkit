using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Media3D;

namespace MvvmCamera;

public sealed partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = string.Empty;

    public ViewportViewModel Viewport1 { get; set; }

    public ViewportViewModel Viewport2 { get; set; }

    public ShellViewModel()
    {
        this.Title = "MvvmCameraDemo";
        this.Viewport1 = new ViewportViewModel();
        this.Viewport2 = new ViewportViewModel();

        var camera = new PerspectiveCamera()
        {
            Position = new Point3D(0, -10, 0),
            LookDirection = new Vector3D(0, 10, 0),
            UpDirection = new Vector3D(0, 0, 1),
            FieldOfView = 60,
        };

        this.Viewport1.Camera = camera;
        this.Viewport2.Camera = camera;
    }
}
