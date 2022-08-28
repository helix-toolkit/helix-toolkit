using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Assimp;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.WinUI;
using ModelViewer.Services;
using SharpDX;

namespace ModelViewer.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public IEffectsManager EffectsManager
    {
        get;
    }

    public Camera Camera
    {
        get;
    } = new OrthographicCamera() { NearPlaneDistance = 1e-2, FarPlaneDistance = 1e4 };

    public SceneNodeGroupModel3D Root
    {
        get;
    } = new SceneNodeGroupModel3D();

    public Geometry3D Axis
    {
        private set;
        get;
    }

    [ObservableProperty]
    private bool showAxis = true;

    [ObservableProperty]
    private Vector3 modelCentroid = default;
    [ObservableProperty]
    private bool showWireframe = false;
    [ObservableProperty]
    private BoundingBox boundingBox = default;

    private HelixToolkitScene scene = null;

    public MainViewModel(IEffectsManager effectsManager)
    {
        EffectsManager = effectsManager;
    }

    [RelayCommand]
    private async Task<bool> LoadModel()
    {
        var path = await App.GetService<FilePickerService>().StartFilePicker(Importer.SupportedFormats);
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        Root.Clear();
        scene = null;
        ResetSettings();
        var syncContext = SynchronizationContext.Current;
        return await Task.Factory.StartNew<HelixToolkitScene>(() =>
        {
            var importer = new Importer();
            var newScene = importer.Load(path);
            if (newScene != null)
            {
                /// Pre-attach and calculate all scene info in a separate task.
                newScene.Root.Attach(EffectsManager);
                newScene.Root.UpdateAllTransformMatrix();
                if (newScene.Root.TryGetBound(out var bound))
                {
                    /// Must use UI thread to set value back.
                    syncContext.Post((o) => { BoundingBox = bound; }, null);
                }
                if (newScene.Root.TryGetCentroid(out var centroid))
                {
                    /// Must use UI thread to set value back.
                    syncContext.Post((o) => { ModelCentroid = centroid; }, null);
                }
                foreach (var n in newScene.Root.Traverse())
                {
                    n.Tag = new AttachedNodeViewModel(n);
                }
                return newScene;
            }
            return null;
        }).ContinueWith((result) =>
        {
            scene = result.Result;
            if (result.IsCompleted && result.Result != null)
            {
                Root.AddNode(result.Result.Root);
                UpdateAxis();
                FocusCameraToScene();
            }
            return true;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void ResetSettings()
    {
        ShowWireframe = false;
    }

    private void FocusCameraToScene()
    {
        var maxWidth = Math.Max(Math.Max(boundingBox.Width, boundingBox.Height), boundingBox.Depth);
        var pos = boundingBox.Center + new Vector3(0, 0, maxWidth);
        Camera.Position = pos;
        Camera.LookDirection = boundingBox.Center - pos;
        Camera.UpDirection = Vector3.UnitY;
        if (Camera is OrthographicCamera orthCam)
        {
            orthCam.Width = maxWidth;
        }
    }

    partial void OnShowWireframeChanged(bool value)
    {
        if (scene != null && scene.Root != null)
        {
            foreach (var node in scene.Root.Traverse())
            {
                if (node is MeshNode meshNode)
                {
                    meshNode.RenderWireframe = value;
                }
            }
        }
    }

    private void UpdateAxis()
    {
        float multiplier = 1.25f;
        var builder = new LineBuilder();
        builder.AddLine(ModelCentroid - new Vector3(boundingBox.Width / 2 * multiplier, 0, 0), ModelCentroid + new Vector3(boundingBox.Width / 2 * multiplier, 0, 0));
        builder.AddLine(ModelCentroid - new Vector3(0, boundingBox.Height / 2 * multiplier, 0), ModelCentroid + new Vector3(0, boundingBox.Height / 2 * multiplier, 0));
        builder.AddLine(ModelCentroid - new Vector3(0, 0, boundingBox.Depth / 2 * multiplier), ModelCentroid + new Vector3(0, 0, boundingBox.Depth / 2 * multiplier));
        Axis = builder.ToLineGeometry3D();
        Axis.Colors = new Color4Collection();
        Axis.Colors.Resize(Axis.Positions.Count, true);
        Axis.Colors[0] = Axis.Colors[1] = Color.Red;
        Axis.Colors[2] = Axis.Colors[3] = Color.Green;
        Axis.Colors[4] = Axis.Colors[5] = Color.Blue;
        OnPropertyChanged(nameof(Axis));
    }
}

