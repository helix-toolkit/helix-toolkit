using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;

namespace DemoCore;

/// <summary>
/// Base ViewModel for Demo Applications
/// </summary>
[Disposer.Disposable]
public abstract partial class BaseViewModel : ObservableObject
{
    public const string Orthographic = "Orthographic Camera";

    public const string Perspective = "Perspective Camera";

    [ObservableProperty]
    private string cameraModel = string.Empty;

    [ObservableProperty]
    private Camera? camera;

    [ObservableProperty]
    private string subTitle = string.Empty;

    [ObservableProperty]
    private string title = string.Empty;

    public List<string> CameraModelCollection { get; private set; }

    [ObservableProperty]
    private IEffectsManager? effectsManager;

    protected OrthographicCamera defaultOrthographicCamera = new()
    {
        Position = new System.Windows.Media.Media3D.Point3D(0, 0, 5),
        LookDirection = new System.Windows.Media.Media3D.Vector3D(-0, -0, -5),
        UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0),
        NearPlaneDistance = 1,
        FarPlaneDistance = 100
    };

    protected PerspectiveCamera defaultPerspectiveCamera = new()
    {
        Position = new System.Windows.Media.Media3D.Point3D(0, 0, 5),
        LookDirection = new System.Windows.Media.Media3D.Vector3D(-0, -0, -5),
        UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0),
        NearPlaneDistance = 0.5,
        FarPlaneDistance = 150
    };

    protected BaseViewModel()
    {
        // camera models
        CameraModelCollection = new List<string>()
        {
            Orthographic,
            Perspective,
        };

        // default camera model
        CameraModel = Perspective;

        Title = "Demo (HelixToolkitDX)";
        SubTitle = "Default Base View Model";
    }

    partial void OnCameraModelChanged(string value)
    {
        if (CameraModel == Orthographic)
        {
            if (Camera is not OrthographicCamera)
                Camera = defaultOrthographicCamera;
        }
        else if (CameraModel == Perspective)
        {
            if (Camera is not PerspectiveCamera)
                Camera = defaultPerspectiveCamera;
        }
        else
        {
            throw new HelixToolkitException("Camera Model Error.");
        }
    }

    partial void OnCameraChanged(Camera? value)
    {
        CameraModel = value is PerspectiveCamera ? Perspective
            : value is OrthographicCamera ? Orthographic
            : string.Empty;
    }

    partial void DisposeManaged()
    {
        this.OnDisposeManaged();
    }

    partial void DisposeUnmanaged()
    {
        this.OnDisposeUnmanaged();
    }

    protected virtual void OnDisposeManaged()
    {
    }

    protected virtual void OnDisposeUnmanaged()
    {
        if (EffectsManager != null)
        {
            var effectManager = EffectsManager as IDisposable;
            HelixToolkit.SharpDX.Disposer.RemoveAndDispose(ref effectManager);
        }
    }
}
