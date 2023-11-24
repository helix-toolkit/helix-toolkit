using HelixToolkit.SharpDX.Render;
using SharpDX.Direct3D11;
using System.Diagnostics.CodeAnalysis;
using SDX11 = SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// 
/// </summary>
public sealed class UAVBufferViewProxy : IDisposable
{
    private SDX11.Resource? resource;
    public Resource? Resource => resource;

    private UnorderedAccessView? uav;
    /// <summary>
    /// Get UnorderedAccessView
    /// </summary>
    public UnorderedAccessView? UAV => uav;
    private ShaderResourceViewProxy? srv;
    /// <summary>
    /// Get ShaderResourceView
    /// </summary>
    public ShaderResourceViewProxy? SRV => srv;

    /// <summary>
    /// Create a raw buffer based UAV
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="bufferDesc">The buffer desc.</param>
    /// <param name="uavDesc">The uav desc.</param>
    /// <param name="srvDesc">The SRV desc.</param>
    public UAVBufferViewProxy(Device device, ref BufferDescription bufferDesc,
        ref UnorderedAccessViewDescription uavDesc, ref ShaderResourceViewDescription srvDesc)
        : this(device, ref bufferDesc, ref uavDesc)
    {
        srv = new ShaderResourceViewProxy(device, resource!);
        srv.CreateTextureView();
    }
    /// <summary>
    /// Create a raw buffer based UAV
    /// </summary>
    /// <param name="device"></param>
    /// <param name="bufferDesc"></param>
    /// <param name="uavDesc"></param>
    public UAVBufferViewProxy(Device device, ref BufferDescription bufferDesc, ref UnorderedAccessViewDescription uavDesc)
    {
        resource = new SDX11.Buffer(device, bufferDesc);
        uav = new UnorderedAccessView(device, resource, uavDesc);
    }
    /// <summary>
    /// Create a texture2D based UAV
    /// </summary>
    /// <param name="device"></param>
    /// <param name="texture2DDesc"></param>
    /// <param name="uavDesc"></param>
    /// <param name="srvDesc"></param>
    public UAVBufferViewProxy(Device device, ref Texture2DDescription texture2DDesc,
        ref UnorderedAccessViewDescription uavDesc, ref ShaderResourceViewDescription srvDesc)
    {
        resource = new SDX11.Texture2D(device, texture2DDesc);
        srv = new ShaderResourceViewProxy(device, resource);
        srv.CreateTextureView(ref srvDesc);
        uav = new UnorderedAccessView(device, resource, uavDesc);
    }

    /// <summary>
    /// Copies the count.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="destBuffer">The dest buffer.</param>
    /// <param name="offset">The offset.</param>
    public void CopyCount(DeviceContextProxy device, SDX11.Buffer destBuffer, int offset)
    {
        device.CopyStructureCount(destBuffer, offset, UAV);
    }

    public static implicit operator UnorderedAccessView?(UAVBufferViewProxy? proxy)
    {
        return proxy?.uav;
    }

    public static implicit operator ShaderResourceViewProxy?(UAVBufferViewProxy? proxy)
    {
        return proxy?.srv;
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    [SuppressMessage("Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "False positive.")]
    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Disposer.RemoveAndDispose(ref uav);
                Disposer.RemoveAndDispose(ref srv);
                Disposer.RemoveAndDispose(ref resource);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            disposedValue = true;
        }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~BufferViewProxy() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
    }
    #endregion
}
