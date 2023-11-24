using HelixToolkit.SharpDX.Model;
using SharpDX;

namespace HelixToolkit.SharpDX;

public class Object3D
{
    public Geometry3D? Geometry
    {
        get; set;
    }
    public MaterialCore? Material
    {
        get; set;
    }
    public List<Matrix>? Transform
    {
        get; set;
    }
    public string Name
    {
        get; set;
    } = string.Empty;
}
