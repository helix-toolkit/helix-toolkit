using SharpDX;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IPostEffectOutlineBlur : IPostEffect
{
    /// <summary>
    /// Gets or sets the color of the border.
    /// </summary>
    /// <value>
    /// The color of the border.
    /// </value>
    Color4 Color
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the scale x.
    /// </summary>
    /// <value>
    /// The scale x.
    /// </value>
    float ScaleX
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the scale y.
    /// </summary>
    /// <value>
    /// The scale y.
    /// </value>
    float ScaleY
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the number of blur pass.
    /// </summary>
    /// <value>
    /// The number of blur pass.
    /// </value>
    int NumberOfBlurPass
    {
        set; get;
    }
}
