using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// Point Geometry Buffer Model. Use for point rendering
/// </summary>
/// <typeparam name="VertexStruct"></typeparam>
public abstract class PointGeometryBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
{
    protected static readonly VertexStruct[] emptyVerts = Array.Empty<VertexStruct>();
    /// <summary>
    /// Initializes a new instance of the <see cref="PointGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="structSize">Size of the structure.</param>
    /// <param name="dynamic">Create dynamic buffer or immutable buffer</param>
    public PointGeometryBufferModel(int structSize, bool dynamic = false)
        : base(PrimitiveTopology.PointList,
        dynamic ? new DynamicBufferProxy(structSize, BindFlags.VertexBuffer) : new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer) as IElementsBufferProxy,
        null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="vertexBuffer"></param>
    public PointGeometryBufferModel(IElementsBufferProxy vertexBuffer) : base(PrimitiveTopology.PointList,
        vertexBuffer, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="vertexBuffer"></param>
    public PointGeometryBufferModel(IElementsBufferProxy[] vertexBuffer) : base(PrimitiveTopology.PointList,
        vertexBuffer, null)
    {
    }


    /// <summary>
    /// Called when [create index buffer].
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="buffer">The buffer.</param>
    /// <param name="geometry">The geometry.</param>
    /// <param name="deviceResources">The device resources.</param>
    protected override void OnCreateIndexBuffer(DeviceContextProxy context, IElementsBufferProxy buffer, Geometry3D? geometry, IDeviceResources? deviceResources)
    {

    }
}
