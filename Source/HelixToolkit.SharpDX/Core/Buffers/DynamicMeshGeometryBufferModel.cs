using HelixToolkit.SharpDX.Utilities;
using SharpDX;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Core;

/// <summary>
/// 
/// </summary>
public sealed class DynamicMeshGeometryBufferModel : DefaultMeshGeometryBufferModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicMeshGeometryBufferModel"/> class.
    /// </summary>
    public DynamicMeshGeometryBufferModel()
        : base(new[]
        {
            new DynamicBufferProxy(DefaultVertex.SizeInBytes, BindFlags.VertexBuffer),
            new DynamicBufferProxy(NativeHelper.SizeOf<Vector2>(), BindFlags.VertexBuffer),
            new DynamicBufferProxy(NativeHelper.SizeOf<Vector4>(), BindFlags.VertexBuffer)
        } as IElementsBufferProxy[], true)
    {
    }
}
