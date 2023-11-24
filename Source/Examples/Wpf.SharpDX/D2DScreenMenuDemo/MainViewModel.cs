using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;
using Media3D = System.Windows.Media.Media3D;

namespace D2DScreenMenuDemo;

public class MainViewModel : DemoCore.BaseViewModel
{
    public ViewModel3D VM3D { get; } = new();

    public ViewModel2D VM2D { get; } = new();

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();

        // ----------------------------------------------
        // titles
        this.Title = "D2DScreenMenu Demo";
        this.SubTitle = "WPF & SharpDX";

        // ----------------------------------------------
        // camera setup
        this.Camera = new PerspectiveCamera
        {
            Position = new Media3D.Point3D(8, 9, 7),
            LookDirection = new Media3D.Vector3D(-5, -12, -5),
            UpDirection = new Media3D.Vector3D(0, 1, 0)
        };
    }
}
