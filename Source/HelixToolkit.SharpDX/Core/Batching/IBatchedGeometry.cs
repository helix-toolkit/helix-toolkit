using SharpDX;

namespace HelixToolkit.SharpDX.Core;

public interface IBatchedGeometry
{
    Geometry3D Geometry
    {
        get;
    }
    Matrix ModelTransform
    {
        get;
    }
}
