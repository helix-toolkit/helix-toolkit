using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IShadowMapRenderParams
{
    /// <summary>
    /// 
    /// </summary>
    int Width
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    int Height
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    float Bias
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    float Intensity
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    Matrix LightView
    {
        set; get;
    }
    /// <summary>
    /// 
    /// </summary>
    Matrix LightProjection
    {
        set; get;
    }
    /// <summary>
    /// Update shadow map every N frames
    /// </summary>
    int UpdateFrequency
    {
        set; get;
    }
}
