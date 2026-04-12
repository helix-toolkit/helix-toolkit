using HelixToolkit.Avalonia.SharpDX;
using HelixToolkit.SharpDX;
using System;

namespace EmptyDemo.ViewModels;

public partial class MainViewModel : ViewModelBase, IDisposable
{
    public string Greeting => "Welcome to Avalonia!";

    public EffectsManager? EffectsManager { get; }

    public Camera Camera { get; }

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();
        Camera = new PerspectiveCamera();
    }

    public void Dispose()
    {
        EffectsManager?.Dispose();
    }
}
