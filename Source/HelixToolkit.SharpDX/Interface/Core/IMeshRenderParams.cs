using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IMeshRenderParams : IInvertNormal, IMaterialRenderParams
{
    bool RenderWireframe
    {
        set; get;
    }
    Color4 WireframeColor
    {
        set; get;
    }
}
