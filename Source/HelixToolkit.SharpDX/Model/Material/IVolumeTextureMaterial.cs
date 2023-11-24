using SharpDX;

namespace HelixToolkit.SharpDX.Model;

public interface IVolumeTextureMaterial
{
    global::SharpDX.Direct3D11.SamplerStateDescription Sampler
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the step size, controls the quality.
    /// </summary>
    /// <value>
    /// The size of the step.
    /// </value>
    double SampleDistance
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the iteration. Usually set to VolumeDepth.
    /// </summary>
    /// <value>
    /// The iteration.
    /// </value>
    int MaxIterations
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the iteration offset. This can be used to achieve cross section
    /// </summary>
    /// <value>
    /// The iteration offset.
    /// </value>
    int IterationOffset
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the iso value. Only data with isovalue > sepecified iso value will be displayed.
    /// Value must be normalized to 0~1. Default = 1, show all data.
    /// </summary>
    /// <value>
    /// The iso value.
    /// </value>
    double IsoValue
    {
        set; get;
    }
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
    /// Gets or sets the transfer map.
    /// </summary>
    /// <value>
    /// The transfer map.
    /// </value>
    Color4[]? TransferMap
    {
        set; get;
    }

    bool EnablePlaneAlignment
    {
        set; get;
    }
}
