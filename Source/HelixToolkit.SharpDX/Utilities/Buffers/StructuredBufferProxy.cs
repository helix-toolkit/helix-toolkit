using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace HelixToolkit.SharpDX.Utilities;

public sealed class StructuredBufferProxy : DynamicBufferProxy
{
    private ShaderResourceViewProxy? srv;
    public ShaderResourceViewProxy? SRV => srv;

    /// <summary>
    /// Initializes a new instance of the <see cref="StructuredBufferProxy"/> class.
    /// </summary>
    /// <param name="structureSize">Size of the structure.</param>
    /// <param name="lazyResize">If existing data size is smaller than buffer size, reuse existing.
    /// Otherwise create a new buffer with exact same size</param>
    public StructuredBufferProxy(int structureSize, bool lazyResize = true)
        : base(structureSize, BindFlags.ShaderResource, ResourceOptionFlags.BufferStructured, lazyResize)
    {

    }

    protected override void OnBufferChanged(Buffer newBuffer)
    {
        RemoveAndDispose(ref srv);
        srv = new ShaderResourceViewProxy(newBuffer.Device, newBuffer);
        srv.CreateTextureView();
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref srv);
        base.OnDispose(disposeManagedResources);
    }

    public static implicit operator ShaderResourceViewProxy?(StructuredBufferProxy? proxy)
    {
        return proxy?.srv;
    }

    public static implicit operator ShaderResourceView?(StructuredBufferProxy? proxy)
    {
        return proxy?.srv;
    }
}
