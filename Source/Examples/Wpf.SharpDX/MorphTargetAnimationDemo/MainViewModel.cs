using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Animations;
using HelixToolkit.SharpDX.Assimp;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Media3D = System.Windows.Media.Media3D;

namespace MorphTargetAnimationDemo;

public partial class MainViewModel : DemoCore.BaseViewModel
{
    [ObservableProperty]
    private SceneNodeGroupModel3D modelGroup;

    [ObservableProperty]
    private string debugLabel = string.Empty;

    private HelixToolkitScene? scn;
    private readonly CompositionTargetEx compositeHelper = new();
    private List<IAnimationUpdater> animationUpdaters;

    [ObservableProperty]
    private double endTime = 0;

    [ObservableProperty]
    private double currTime = 0;

    partial void OnCurrTimeChanged(double value)
    {
        foreach (IAnimationUpdater updater in animationUpdaters)
        {
            updater.Update((float)value, 1);
        }
    }

    [ObservableProperty]
    private bool isPlaying = false;

    private long initTime = 0;

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();
        modelGroup = new SceneNodeGroupModel3D();

        //Test importing
        Importer importer = new();
        importer.Configuration.CreateSkeletonForBoneSkinningMesh = true;
        importer.Configuration.SkeletonSizeScale = 0.01f;
        importer.Configuration.GlobalScale = 0.1f;
        scn = importer.Load("Gunan_animated.fbx");

        //Add to model group for rendering
        if (scn is not null)
        {
            ModelGroup.AddNode(scn.Root);
        }

        //Setup each animation, this will actively play all (not always desired)
        animationUpdaters = scn?.Animations is null ? new() : new List<IAnimationUpdater>(scn.Animations.CreateAnimationUpdaters().Values);
        EndTime = scn?.Animations is null ? 0 : scn.Animations.Max(x => x.EndTime);
    }

    [RelayCommand]
    private void Play()
    {
        if (!IsPlaying)
        {
            initTime = 0;
            compositeHelper.Rendering += Render;
        }
        else
        {
            compositeHelper.Rendering -= Render;
        }

        IsPlaying = !IsPlaying;
    }

    private void Render(object? sender, System.Windows.Media.RenderingEventArgs e)
    {
        //Animation with perf testing
        long t = Stopwatch.GetTimestamp();
        if (initTime == 0)
        {
            initTime = t;
        }
        //Update animation. Ensures all animation times are in sync
        CurrTime = ((t - initTime) / (double)Stopwatch.Frequency) % EndTime;
        t = Stopwatch.GetTimestamp() - t;
        DebugLabel = t.ToString();
    }
}
