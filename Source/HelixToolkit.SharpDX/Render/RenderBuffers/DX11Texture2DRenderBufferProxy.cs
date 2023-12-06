using HelixToolkit.SharpDX.Core2D;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace HelixToolkit.SharpDX.Render;

/// <summary>
/// 
/// </summary>
public class DX11Texture2DRenderBufferProxy : DX11RenderBufferProxyBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DX11Texture2DRenderBufferProxy"/> class.
    /// </summary>
    /// <param name="deviceResources"></param>
    public DX11Texture2DRenderBufferProxy(IDeviceResources deviceResources) : base(deviceResources)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    protected override ShaderResourceViewProxy? OnCreateBackBuffer(int width, int height)
    {
        if (Device is null)
        {
            return null;
        }

        var colordescNMS = new Texture2DDescription
        {
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
            Format = Format,
            Width = width,
            Height = height,
            MipLevels = 1,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default,
            OptionFlags = ResourceOptionFlags.Shared,
            CpuAccessFlags = CpuAccessFlags.None,
            ArraySize = 1
        };

        var backBuffer = new ShaderResourceViewProxy(Device, colordescNMS);
        d2dTarget = new D2DTargetProxy();

        if (backBuffer.Resource is Texture2D texture)
        {
            d2dTarget.Initialize(texture, DeviceContext2D);
        }

        return backBuffer;
    }

    /// <summary>
    /// Presents this instance.
    /// </summary>
    /// <returns></returns>
    public override bool Present()
    {
        if (Device is null)
        {
            return false;
        }

        Device.ImmediateContext.Flush();
        return true;
    }
}
