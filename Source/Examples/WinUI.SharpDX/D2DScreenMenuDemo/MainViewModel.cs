using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX;
using HelixToolkit.WinUI.SharpDX;
using System;
using System.Numerics;

namespace D2DScreenMenuDemo;

public partial class MainViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    private EffectsManager? effectsManager;

    [ObservableProperty]
    private Camera? camera;

    public ViewModel3D VM3D { get; } = new();

    public ViewModel2D VM2D { get; } = new();

    public MainViewModel()
    {
        this.EffectsManager = new DefaultEffectsManager();

        this.Camera = new PerspectiveCamera
        {
            Position = new Vector3(8, 9, 7),
            LookDirection = new Vector3(-5, -12, -5),
            UpDirection = new Vector3(0, 1, 0)
        };
    }

    public void Dispose()
    {
        this.EffectsManager?.Dispose();
    }
}
