using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// Line Geometry Buffer Model. Used for line rendering
/// </summary>
/// <typeparam name="VertexStruct"></typeparam>
public abstract class LineGeometryBufferModel<VertexStruct> : GeometryBufferModel where VertexStruct : struct
{
    protected static readonly VertexStruct[] emptyVertices = Array.Empty<VertexStruct>();
    protected static readonly int[] emptyIndices = Array.Empty<int>();

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="structSize">Size of the structure.</param>
    /// <param name="dynamic">Create dynamic buffer or immutable buffer</param>
    public LineGeometryBufferModel(int structSize, bool dynamic = false)
        : base(PrimitiveTopology.LineList,
        dynamic ? new DynamicBufferProxy(structSize, BindFlags.VertexBuffer) : new ImmutableBufferProxy(structSize, BindFlags.VertexBuffer) as IElementsBufferProxy,
        dynamic ? new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer) : new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer) as IElementsBufferProxy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="vertexBuffer"></param>
    /// <param name="dynamic">Create dynamic buffer or immutable buffer</param> 
    public LineGeometryBufferModel(IElementsBufferProxy vertexBuffer, bool dynamic = false)
        : base(PrimitiveTopology.LineList,
        vertexBuffer,
        dynamic ? new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer) : new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer) as IElementsBufferProxy)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="vertexBuffer"></param>
    /// <param name="dynamic">Create dynamic buffer or immutable buffer</param> 
    public LineGeometryBufferModel(IElementsBufferProxy[] vertexBuffer, bool dynamic = false)
        : base(PrimitiveTopology.LineList,
        vertexBuffer,
        dynamic ? new DynamicBufferProxy(sizeof(int), BindFlags.IndexBuffer) : new ImmutableBufferProxy(sizeof(int), BindFlags.IndexBuffer) as IElementsBufferProxy)
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="LineGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="vertexBuffer"></param>
    /// <param name="indexBuffer"></param>
    public LineGeometryBufferModel(IElementsBufferProxy vertexBuffer, IElementsBufferProxy indexBuffer)
        : base(PrimitiveTopology.LineList,
        vertexBuffer, indexBuffer)
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="LineGeometryBufferModel{VertexStruct}"/> class.
    /// </summary>
    /// <param name="vertexBuffer"></param>
    /// <param name="indexBuffer"></param>
    public LineGeometryBufferModel(IElementsBufferProxy[] vertexBuffer, IElementsBufferProxy indexBuffer)
        : base(PrimitiveTopology.LineList,
        vertexBuffer, indexBuffer)
    {
    }
}
