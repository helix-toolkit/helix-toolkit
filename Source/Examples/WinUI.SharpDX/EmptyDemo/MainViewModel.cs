using HelixToolkit.SharpDX;
using HelixToolkit.WinUI.SharpDX;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EmptyDemo;

public class MainViewModel : INotifyPropertyChanged, IDisposable
{
    public EffectsManager? EffectsManager { get; }

    public Camera Camera { get; }

    public MainViewModel()
    {
        EffectsManager = new DefaultEffectsManager();
        Camera = new PerspectiveCamera();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string info = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
    }

    protected bool Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
    {
        if (object.Equals(backingField, value))
        {
            return false;
        }

        backingField = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public void Dispose()
    {
        EffectsManager?.Dispose();
    }
}
