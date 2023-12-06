using HelixToolkit.SharpDX;

namespace HelixToolkit.Wpf.SharpDX;

public interface ITransformable : ITransform
{
    Transform3D Transform
    {
        get; set;
    }
}
