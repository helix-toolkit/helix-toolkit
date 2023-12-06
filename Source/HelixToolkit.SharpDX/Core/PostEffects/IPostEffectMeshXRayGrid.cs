using SharpDX;

namespace HelixToolkit.SharpDX.Core;

public interface IPostEffectMeshXRayGrid : IPostEffect
{
    Color4 Color
    {
        set; get;
    }
    int GridDensity
    {
        set; get;
    }
    float DimmingFactor
    {
        set; get;
    }
    float BlendingFactor
    {
        set; get;
    }
    string XRayDrawingPassName
    {
        set; get;
    }
}
