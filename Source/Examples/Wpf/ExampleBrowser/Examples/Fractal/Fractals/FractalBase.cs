using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Media3D;

namespace Fractal.Fractals;

public abstract class FractalBase : ObservableObject
{
    public int Level { get; set; }

    public int TriangleCount { get; protected set; }

    public abstract GeometryModel3D? Generate();


}
