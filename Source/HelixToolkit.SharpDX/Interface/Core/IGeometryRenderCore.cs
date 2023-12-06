using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public interface IGeometryRenderCore
{
    /// <summary>
    /// Gets or sets the instance buffer.
    /// </summary>
    /// <value>
    /// The instance buffer.
    /// </value>
    IElementsBufferModel InstanceBuffer
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the geometry buffer.
    /// </summary>
    /// <value>
    /// The geometry buffer.
    /// </value>
    IAttachableBufferModel? GeometryBuffer
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the raster description.
    /// </summary>
    /// <value>
    /// The raster description.
    /// </value>
    RasterizerStateDescription RasterDescription
    {
        set; get;
    }
}
