using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// Mesh Geometry Buffer Model.
/// </summary>
/// <typeparam name="VertexStruct"></typeparam>
public abstract class MeshGeometryBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
{
    protected static readonly VertexStruct[] emptyVerts = Array.Empty<VertexStruct>();
    protected static readonly int[] emptyIndices = Array.Empty<int>();

    /// <summary>
    /// Initializes a new instance of the <see cref="MeshGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="structSize">Size of the structure.</param>
    /// <param name="dynamic">Create dynamic buffer or immutable buffer</param>
    public MeshGeometryBufferModel(int structSize, bool dynamic = false)
        : base(PrimitiveTopology.TriangleList,
              dynamic ? new DynamicBufferProxy(structSize, BindFlags.VertexBuffer) : new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer) as IElementsBufferProxy,
              dynamic ? new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer) : new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer) as IElementsBufferProxy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MeshGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="structSize">Size of the structure.</param>
    /// <param name="topology">The topology.</param>
    /// <param name="dynamic">Create dynamic buffer or immutable buffer</param>
    public MeshGeometryBufferModel(int structSize, PrimitiveTopology topology, bool dynamic = false)
        : base(topology,
              dynamic ? new DynamicBufferProxy(structSize, BindFlags.VertexBuffer) : new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer) as IElementsBufferProxy,
              dynamic ? new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer) : new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer) as IElementsBufferProxy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MeshGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="topology">The topology.</param>
    /// <param name="vertexBuffers"></param>
    /// <param name="dynamic">Create dynamic buffer or immutable buffer</param>
    public MeshGeometryBufferModel(PrimitiveTopology topology, IElementsBufferProxy[] vertexBuffers, bool dynamic = false)
        : base(topology,
              vertexBuffers,
              dynamic ? new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer) : new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer) as IElementsBufferProxy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MeshGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="topology">The topology.</param>
    /// <param name="vertexBuffer">The vertex buffer.</param>
    /// <param name="indexBuffer">The index buffer.</param>
    protected MeshGeometryBufferModel(PrimitiveTopology topology, IElementsBufferProxy vertexBuffer, IElementsBufferProxy indexBuffer)
        : base(topology, vertexBuffer, indexBuffer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MeshGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="topology">The topology.</param>
    /// <param name="vertexBuffer">The vertex buffer.</param>
    /// <param name="indexBuffer">The index buffer.</param>
    protected MeshGeometryBufferModel(PrimitiveTopology topology, IElementsBufferProxy[] vertexBuffer, IElementsBufferProxy indexBuffer)
        : base(topology, vertexBuffer, indexBuffer)
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
        if (geometry != null && geometry.Indices != null && geometry.Indices.Count > 0)
        {
            buffer.UploadDataToBuffer(context, geometry.Indices, geometry.Indices.Count, 0, geometry.PreDefinedIndexCount);
        }
        else
        {
            buffer.UploadDataToBuffer(context, emptyIndices, 0);
            //buffer.DisposeAndClear();
        }
    }
}
