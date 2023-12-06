/*
Model: Sphere Bot Rusty Version. Author: 3DHaupt. Source : https://sketchfab.com/models/d18753fe3e494ddbbc52a8a2e58be7a4
Model: Character. Source : https://github.com/spazzarama/Direct3D-Rendering-Cookbook
*/

using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Animations;
using HelixToolkit.SharpDX.Assimp;
using HelixToolkit.SharpDX.Model.Scene;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Media3D = System.Windows.Media.Media3D;
using Vector3 = global::SharpDX.Vector3;

namespace BoneSkinDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    public SceneNodeGroupModel3D ModelGroup { get; } = new();

    [ObservableProperty]
    private bool showWireframe = false;

    partial void OnShowWireframeChanged(bool value)
    {
        foreach (var m in boneSkinNodes)
        {
            m.RenderWireframe = value;
        }
    }

    [ObservableProperty]
    private bool showSkeleton = false;

    partial void OnShowSkeletonChanged(bool value)
    {
        foreach (var m in skeletonNodes)
        {
            m.Visible = value;
        }
    }

    [ObservableProperty]
    private bool enableAnimation = true;

    partial void OnEnableAnimationChanged(bool value)
    {
        if (value)
        {
            compositeHelper.Rendering += CompositeHelper_Rendering;
        }
        else
        {
            compositeHelper.Rendering -= CompositeHelper_Rendering;
        }
    }

    [ObservableProperty]
    private string? selectedAnimation;

    partial void OnSelectedAnimationChanged(string? value)
    {
        reset = true;
        var curr = value is null ? null : scene?.Animations?.Where(x => x.Name == value).FirstOrDefault();
        animationUpdater = curr is null ? null : new(curr)
        {
            RepeatMode = SelectedRepeatMode
        };
    }

    [ObservableProperty]
    private AnimationRepeatMode selectedRepeatMode = AnimationRepeatMode.Loop;

    partial void OnSelectedRepeatModeChanged(AnimationRepeatMode value)
    {
        reset = true;

        if (animationUpdater is not null)
        {
            animationUpdater.RepeatMode = value;
        }
    }

    public Media3D.Transform3D? ModelTransform { private set; get; }

    public LineGeometry3D HitLineGeometry { get; } = new()
    {
        IsDynamic = true
    };

    public string[]? Animations { set; get; }

    public GridPattern[] GridTypes { get; } = new GridPattern[]
    {
        GridPattern.Tile,
        GridPattern.Grid
    };

    public AnimationRepeatMode[] RepeatModes { get; } = new AnimationRepeatMode[]
    {
        AnimationRepeatMode.Loop,
        AnimationRepeatMode.PlayOnce,
        AnimationRepeatMode.PlayOnceHold
    };

    private const int NumSegments = 100;
    private const int Theta = 24;
    private long startAniTime = 0;
    private CancellationTokenSource cts = new();
    private SynchronizationContext? context = SynchronizationContext.Current;

    private bool reset = true;
    private HelixToolkitScene? scene;
    private NodeAnimationUpdater? animationUpdater;
    private readonly List<BoneSkinMeshNode> boneSkinNodes = new();
    private readonly List<BoneSkinMeshNode> skeletonNodes = new();
    private readonly CompositionTargetEx compositeHelper = new();

    public MainViewModel()
    {
        this.Title = "BoneSkin Demo";
        this.SubTitle = "WPF & SharpDX";
        this.EffectsManager = new DefaultEffectsManager();

        this.Camera = new PerspectiveCamera
        {
            Position = new Media3D.Point3D(50, 50, 50),
            LookDirection = new Media3D.Vector3D(-50, -50, -50),
            UpDirection = new Media3D.Vector3D(0, 1, 0),
            NearPlaneDistance = 1,
            FarPlaneDistance = 2000
        };

        HitLineGeometry.Positions = new Vector3Collection(2)
        {
            Vector3.Zero,
            Vector3.Zero
        };

        HitLineGeometry.Indices = new IntCollection(2)
        {
            0,
            1
        };

        LoadFile();
        compositeHelper.Rendering += CompositeHelper_Rendering;
    }

    private void LoadFile()
    {
        var importer = new Importer();
        importer.Configuration.CreateSkeletonForBoneSkinningMesh = true;
        importer.Configuration.SkeletonSizeScale = 0.04f;
        importer.Configuration.GlobalScale = 0.1f;
        scene = importer.Load("Solus_The_Knight.fbx");

        if (scene is null)
        {
            return;
        }

        ModelGroup.AddNode(scene.Root);
        Animations = scene.Animations?.Select(x => x.Name)?.ToArray();
        foreach (var node in scene.Root.Items.Traverse(false))
        {
            if (node is BoneSkinMeshNode m)
            {
                if (!m.IsSkeletonNode)
                {
                    m.IsThrowingShadow = true;
                    m.WireframeColor = new SharpDX.Color4(0, 0, 1, 1);
                    boneSkinNodes.Add(m);
                    m.MouseDown += HandleMouseDown;
                }
                else
                {
                    skeletonNodes.Add(m);
                    m.Visible = false;
                }
            }
        }
    }

    private void HandleMouseDown(object? sender, SceneNodeMouseDownArgs e)
    {
        var result = e.HitResult;
        HitLineGeometry.Positions![0] = result.PointHit - result.NormalAtHit * 0.5f;
        HitLineGeometry.Positions![1] = result.PointHit + result.NormalAtHit * 0.5f;
        HitLineGeometry.UpdateVertices();
    }

    private void CompositeHelper_Rendering(object? sender, System.Windows.Media.RenderingEventArgs e)
    {
        if (animationUpdater != null)
        {
            if (reset)
            {
                animationUpdater.Reset();
                animationUpdater.RepeatMode = SelectedRepeatMode;
                reset = false;
                startAniTime = 0;
            }
            else
            {
                if (startAniTime == 0)
                {
                    startAniTime = Stopwatch.GetTimestamp();
                }
                var elapsed = Stopwatch.GetTimestamp() - startAniTime;
                animationUpdater.Update(elapsed, Stopwatch.Frequency);
            }
        }
    }

    protected override void OnDisposeUnmanaged()
    {
        cts.Cancel(true);
        compositeHelper.Dispose();
        base.OnDisposeUnmanaged();
    }
}
