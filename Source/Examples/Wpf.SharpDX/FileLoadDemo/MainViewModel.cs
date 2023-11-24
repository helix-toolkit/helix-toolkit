using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Animations;
using HelixToolkit.SharpDX.Assimp;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Controls;
using Microsoft.Win32;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Point3D = System.Windows.Media.Media3D.Point3D;

namespace FileLoadDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    private readonly string OpenFileFilter = $"{HelixToolkit.SharpDX.Assimp.Importer.SupportedFormatsString}";
    private readonly string ExportFileFilter = $"{HelixToolkit.SharpDX.Assimp.Exporter.SupportedFormatsString}";

    [ObservableProperty]
    private bool showWireframe = false;

    partial void OnShowWireframeChanged(bool value)
    {
        ShowWireframeFunct(value);
    }

    [ObservableProperty]
    private bool renderFlat = false;

    partial void OnRenderFlatChanged(bool value)
    {
        RenderFlatFunct(value);
    }

    [ObservableProperty]
    private bool renderEnvironmentMap = true;

    partial void OnRenderEnvironmentMapChanged(bool value)
    {
        if (scene?.Root is not null)
        {
            foreach (var node in scene.Root.Traverse())
            {
                if (node is MaterialGeometryNode m && m.Material is PBRMaterialCore material)
                {
                    material.RenderEnvironmentMap = value;
                }
            }
        }
    }

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool isPlaying = false;

    [ObservableProperty]
    private float startTime;

    [ObservableProperty]
    private float endTime;

    [ObservableProperty]
    private float currAnimationTime = 0;

    partial void OnCurrAnimationTimeChanged(float value)
    {
        if (EndTime == 0)
        {
            return;
        }

        CurrAnimationTime = value % EndTime + StartTime;
        animationUpdater?.Update(value, 1);
    }

    public ObservableCollection<IAnimationUpdater> Animations { get; } = new();

    public SceneNodeGroupModel3D GroupModel { get; } = new SceneNodeGroupModel3D();

    [ObservableProperty]
    private IAnimationUpdater? selectedAnimation = null;

    partial void OnSelectedAnimationChanged(IAnimationUpdater? value)
    {
        StopAnimation();
        CurrAnimationTime = 0;
        if (value is not null)
        {
            animationUpdater = value;
            animationUpdater.Reset();
            animationUpdater.RepeatMode = AnimationRepeatMode.Loop;
            StartTime = value.StartTime;
            EndTime = value.EndTime;
        }
        else
        {
            animationUpdater = null;
            StartTime = EndTime = 0;
        }
    }

    [ObservableProperty]
    private float speed = 1.0f;

    [ObservableProperty]
    private Point3D modelCentroid = default;

    [ObservableProperty]
    private BoundingBox modelBound = new();
    public TextureModel? EnvironmentMap { get; }

    private readonly SynchronizationContext? context = SynchronizationContext.Current;
    private HelixToolkitScene? scene;
    private IAnimationUpdater? animationUpdater;
    private List<BoneSkinMeshNode> boneSkinNodes = new();
    private List<BoneSkinMeshNode> skeletonNodes = new();
    private CompositionTargetEx compositeHelper = new();
    private long initTimeStamp = 0;

    private MainWindow? mainWindow = null;

    public MainViewModel(MainWindow? window)
    {
        mainWindow = window;

        EffectsManager = new DefaultEffectsManager();

        Camera = new OrthographicCamera()
        {
            LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -10, -10),
            Position = new System.Windows.Media.Media3D.Point3D(0, 10, 10),
            UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0),
            FarPlaneDistance = 5000,
            NearPlaneDistance = 0.1f
        };

        EnvironmentMap = TextureModel.Create("Cubemap_Grandcanyon.dds");
    }

    [RelayCommand]
    private void ResetCamera()
    {
        if (Camera is not OrthographicCamera c)
        {
            return;
        }

        c.Reset();
        c.FarPlaneDistance = 5000;
        c.NearPlaneDistance = 0.1f;
    }

    [RelayCommand]
    private void Play()
    {
        if (!IsPlaying && SelectedAnimation != null)
        {
            StartAnimation();
        }
        else
        {
            StopAnimation();
        }
    }

    [RelayCommand]
    private void CopyAsBitmap()
    {
        Viewport3DX? viewport = mainWindow?.view;

        if (viewport is null)
        {
            return;
        }

        var bitmap = ViewportExtensions.RenderBitmap(viewport);
        try
        {
            Clipboard.Clear();
            Clipboard.SetImage(bitmap);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }

    [RelayCommand]
    private void CopyAsHiResBitmap()
    {
        Viewport3DX? viewport = mainWindow?.view;

        if (viewport is null)
        {
            return;
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var bitmap = ViewportExtensions.RenderBitmap(viewport, 1920, 1080);
        try
        {
            Clipboard.Clear();
            Clipboard.SetImage(bitmap);
            stopwatch.Stop();
            Debug.WriteLine($"creating bitmap needs {stopwatch.ElapsedMilliseconds} ms");
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }

    [RelayCommand]
    private void OpenFile()
    {
        if (IsLoading)
        {
            return;
        }

        string? path = OpenFileDialog(OpenFileFilter);
        if (path is null)
        {
            return;
        }

        StopAnimation();
        var syncContext = SynchronizationContext.Current;
        IsLoading = true;
        Task.Run(() =>
        {
            var loader = new Importer();
            var scene = loader.Load(path);

            if (scene is null)
            {
                return null;
            }

            scene.Root.Attach(EffectsManager); // Pre attach scene graph
            scene.Root.UpdateAllTransformMatrix();
            if (scene.Root.TryGetBound(out var bound))
            {
                // Must use UI thread to set value back.
                syncContext?.Post((o) => { ModelBound = bound; }, null);
            }
            if (scene.Root.TryGetCentroid(out var centroid))
            {
                // Must use UI thread to set value back.
                syncContext?.Post((o) => { ModelCentroid = centroid.ToPoint3D(); }, null);
            }
            return scene;
        }).ContinueWith((result) =>
        {
            IsLoading = false;
            if (result.IsCompleted)
            {
                scene = result.Result;

                if (scene is null)
                {
                    return;
                }

                Animations.Clear();
                var oldNode = GroupModel.SceneNode.Items.ToArray();
                GroupModel.Clear(false);
                Task.Run(() =>
                {
                    foreach (var node in oldNode)
                    { node.Dispose(); }
                });
                if (scene is not null)
                {
                    if (scene.Root is not null)
                    {
                        foreach (var node in scene.Root.Traverse())
                        {
                            if (node is MaterialGeometryNode m)
                            {
                                //m.Geometry.SetAsTransient();
                                if (m.Material is PBRMaterialCore pbr)
                                {
                                    pbr.RenderEnvironmentMap = RenderEnvironmentMap;
                                }
                                else if (m.Material is PhongMaterialCore phong)
                                {
                                    phong.RenderEnvironmentMap = RenderEnvironmentMap;
                                }
                            }
                        }
                    }

                    if (scene.Root is not null)
                    {
                        GroupModel.AddNode(scene.Root);
                    }

                    if (scene.HasAnimation && scene.Animations is not null)
                    {
                        var dict = scene.Animations.CreateAnimationUpdaters();
                        foreach (var ani in dict.Values)
                        {
                            Animations.Add(ani);
                        }
                    }

                    if (scene.Root is not null)
                    {
                        foreach (var n in scene.Root.Traverse())
                        {
                            n.Tag = new AttachedNodeViewModel(n);
                        }
                    }

                    FocusCameraToScene();
                }
            }
            else if (result.IsFaulted && result.Exception != null)
            {
                MessageBox.Show(result.Exception.Message);
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public void StartAnimation()
    {
        initTimeStamp = Stopwatch.GetTimestamp();
        compositeHelper.Rendering += CompositeHelper_Rendering;
        IsPlaying = true;
    }

    public void StopAnimation()
    {
        IsPlaying = false;
        compositeHelper.Rendering -= CompositeHelper_Rendering;
    }

    private void CompositeHelper_Rendering(object? sender, System.Windows.Media.RenderingEventArgs e)
    {
        if (animationUpdater is not null)
        {
            var elapsed = (Stopwatch.GetTimestamp() - initTimeStamp) * Speed;
            CurrAnimationTime = elapsed / Stopwatch.Frequency;
        }
    }

    private void FocusCameraToScene()
    {
        if (Camera is null)
        {
            return;
        }

        var maxWidth = Math.Max(Math.Max(ModelBound.Width, ModelBound.Height), ModelBound.Depth);
        var pos = ModelBound.Center + new Vector3(0, 0, maxWidth);
        Camera.Position = pos.ToPoint3D();
        Camera.LookDirection = (ModelBound.Center - pos).ToVector3D();
        Camera.UpDirection = Vector3.UnitY.ToVector3D();

        if (Camera is OrthographicCamera orthCam)
        {
            orthCam.Width = maxWidth;
        }
    }

    [RelayCommand]
    private void Export()
    {
        var index = SaveFileDialog(ExportFileFilter, out var path);

        if (!string.IsNullOrEmpty(path) && index >= 0)
        {
            var id = HelixToolkit.SharpDX.Assimp.Exporter.SupportedFormats[index].FormatId;
            var exporter = new HelixToolkit.SharpDX.Assimp.Exporter();
            exporter.ExportToFile(path, scene, id);
            return;
        }
    }


    private string? OpenFileDialog(string filter)
    {
        var d = new OpenFileDialog();
        d.CustomPlaces.Clear();

        d.Filter = filter;

        if (d.ShowDialog() == true)
        {
            return d.FileName;
        }

        return null;
    }

    private int SaveFileDialog(string filter, out string path)
    {
        var d = new SaveFileDialog
        {
            Filter = filter
        };

        if (d.ShowDialog() == true)
        {
            path = d.FileName;
            return d.FilterIndex - 1; //This is tarting from 1. So must minus 1
        }
        else
        {
            path = "";
            return -1;
        }
    }

    private void ShowWireframeFunct(bool show)
    {
        foreach (var node in GroupModel.GroupNode.Items.PreorderDFT(node => node.IsRenderable))
        {
            if (node is MeshNode m)
            {
                m.RenderWireframe = show;
            }
        }
    }

    private void RenderFlatFunct(bool show)
    {
        foreach (var node in GroupModel.GroupNode.Items.PreorderDFT(node => node.IsRenderable))
        {
            if (node is MeshNode m)
            {
                if (m.Material is PhongMaterialCore phong)
                {
                    phong.EnableFlatShading = show;
                }
                else if (m.Material is PBRMaterialCore pbr)
                {
                    pbr.EnableFlatShading = show;
                }
            }
        }
    }
}
