using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IAttachableBufferModel : IGUID, IDisposable
{
    /// <summary>
    /// Gets or sets the topology.
    /// </summary>
    /// <value>
    /// The topology.
    /// </value>
    PrimitiveTopology Topology
    {
        set; get;
    }

    /// <summary>
    /// Gets the vertex buffer.
    /// </summary>
    /// <value>
    /// The vertex buffer.
    /// </value>
    IElementsBufferProxy?[] VertexBuffer
    {
        get;
    }
    /// <summary>
    /// Gets the size of the vertex structure.
    /// </summary>
    /// <value>
    /// The size of the vertex structure.
    /// </value>
    IEnumerable<int> VertexStructSize
    {
        get;
    }
    /// <summary>
    /// Gets the index buffer.
    /// </summary>
    /// <value>
    /// The index buffer.
    /// </value>
    IElementsBufferProxy? IndexBuffer
    {
        get;
    }
    /// <summary>
    /// Attaches the buffers.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="vertexBufferStartSlot">The vertex buffer slot. It will be changed to next available slot after binding.</param>
    /// <param name="deviceResources"></param>
    /// <returns></returns>
    bool AttachBuffers(DeviceContextProxy context, ref int vertexBufferStartSlot, IDeviceResources? deviceResources);
    /// <summary>
    /// Updates the buffers.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="deviceResources">The device resources.</param>
    /// <returns>True if buffer updated.</returns>
    bool UpdateBuffers(DeviceContextProxy context, IDeviceResources? deviceResources);
}
