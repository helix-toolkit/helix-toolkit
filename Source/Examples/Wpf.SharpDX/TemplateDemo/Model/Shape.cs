using CommunityToolkit.Mvvm.ComponentModel;
using HelixToolkit.SharpDX;
using HelixToolkit.Wpf.SharpDX;

namespace TemplateDemo;

public abstract partial class Shape : ObservableObject
{
    public Geometry3D Geometry => this.GetGeometry();

    protected abstract Geometry3D GetGeometry();

    [ObservableProperty]
    private System.Windows.Media.Media3D.Transform3D? transform;

    [ObservableProperty]
    private Material? material;
}
