using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public sealed class EmptyGeometryBufferModel : IGeometryBufferModel
{
    public static readonly IGeometryBufferModel Empty = new EmptyGeometryBufferModel();
    /// <summary>
    /// Gets or sets the geometry.
    /// </summary>
    /// <value>
    /// The geometry.
    /// </value>
    public Geometry3D? Geometry
    {
        set; get;
    }
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    /// <value>
    /// The unique identifier.
    /// </value>
    public Guid GUID
    {
        get;
    } = Guid.NewGuid();
    /// <summary>
    /// Gets the index buffer.
    /// </summary>
    /// <value>
    /// The index buffer.
    /// </value>
    public IElementsBufferProxy? IndexBuffer
    {
        get
        {
            return null;
        }
    }
    /// <summary>
    /// Gets or sets the topology.
    /// </summary>
    /// <value>
    /// The topology.
    /// </value>
    public PrimitiveTopology Topology
    {
        get
        {
            return PrimitiveTopology.Undefined;
        }
        set
        {
        }
    }
    /// <summary>
    /// Gets the vertex buffer.
    /// </summary>
    /// <value>
    /// The vertex buffer.
    /// </value>
    public IElementsBufferProxy[] VertexBuffer
    {
        get;
    } = Array.Empty<IElementsBufferProxy>();
    /// <summary>
    /// Gets the size of the vertex structure.
    /// </summary>
    /// <value>
    /// The size of the vertex structure.
    /// </value>
    public IEnumerable<int> VertexStructSize
    {
        get
        {
            yield return 0;
        }
    }
    /// <summary>
    /// Gets or sets the effects manager.
    /// </summary>
    /// <value>
    /// The effects manager.
    /// </value>
    public IEffectsManager? EffectsManager
    {
        set; get;
    }
#pragma warning disable CS0067
    public event EventHandler? VertexBufferUpdated;
    public event EventHandler? IndexBufferUpdated;
#pragma warning restore CS0067
    /// <summary>
    /// Attaches this instance.
    /// </summary>
    public void Attach()
    {

    }
    /// <summary>
    /// Attaches the buffers.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="vertexBufferStartSlot">The vertex buffer start slot. Returns next available bind slot</param>
    /// <param name="deviceResources"></param>
    /// <returns></returns>
    public bool AttachBuffers(DeviceContextProxy context, ref int vertexBufferStartSlot, IDeviceResources? deviceResources)
    {
        return false;
    }


    /// <summary>
    /// Detaches this instance.
    /// </summary>
    public void Detach()
    {

    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {

    }

    public bool UpdateBuffers(DeviceContextProxy context, IDeviceResources? deviceResources)
    {
        return false;
    }
}
