using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IPostEffectMeshXRay : IPostEffect
{
    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    Color4 Color
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the outline fading factor.
    /// </summary>
    /// <value>
    /// The outline fading factor.
    /// </value>
    float OutlineFadingFactor
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets a value indicating whether [double pass]. Double pass uses stencil buffer to reduce overlapping artifacts
    /// </summary>
    /// <value>
    ///   <c>true</c> if [double pass]; otherwise, <c>false</c>.
    /// </value>
    bool EnableDoublePass
    {
        set; get;
    }
}
