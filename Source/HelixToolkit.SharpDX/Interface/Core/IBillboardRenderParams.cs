using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IBillboardRenderParams
{
    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    BillboardType Type
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets a value indicating whether [fixed size].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [fixed size]; otherwise, <c>false</c>.
    /// </value>
    bool FixedSize
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the sampler description.
    /// </summary>
    /// <value>
    /// The sampler description.
    /// </value>
    SamplerStateDescription SamplerDescription
    {
        set; get;
    }
}
