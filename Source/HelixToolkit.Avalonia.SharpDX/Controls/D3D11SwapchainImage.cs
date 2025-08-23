using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using D3DDevice = SharpDX.Direct3D11.Device;
using DxgiResource = SharpDX.DXGI.Resource;

namespace HelixToolkit.Avalonia.SharpDX.Controls;

internal sealed class D3D11SwapchainImage
{
    public PixelSize Size { get; }
    private readonly ICompositionGpuInterop _interop;
    private readonly CompositionDrawingSurface _target;
    private readonly Texture2D _texture;
    private readonly KeyedMutex _mutex;
    private readonly IntPtr _handle;
    private PlatformGraphicsExternalImageProperties _properties;
    private ICompositionImportedGpuImage? _imported;
    public Task? LastPresent { get; private set; }
    public RenderTargetView RenderTargetView { get; }

    public Texture2D Texture => _texture;

    public D3D11SwapchainImage(D3DDevice device, PixelSize size,
        ICompositionGpuInterop interop,
        CompositionDrawingSurface target)
    {
        Size = size;
        _interop = interop;
        _target = target;

        _texture = new Texture2D(device,
            new Texture2DDescription
            {
                Format = Format.B8G8R8A8_UNorm,
                Width = size.Width,
                Height = size.Height,
                ArraySize = 1,
                MipLevels = 1,
                SampleDescription = new SampleDescription { Count = 1, Quality = 0 },
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.SharedKeyedmutex,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource
            });

        _mutex = _texture.QueryInterface<KeyedMutex>();

        using (var res = _texture.QueryInterface<DxgiResource>())
            _handle = res.SharedHandle;

        _properties = new PlatformGraphicsExternalImageProperties
        {
            Width = size.Width,
            Height = size.Height,
            Format = PlatformGraphicsExternalImageFormat.B8G8R8A8UNorm,
            TopLeftOrigin = true
        };

        RenderTargetView = new RenderTargetView(device, _texture);
    }

    public void BeginDraw()
    {
        //_mutex.Acquire(0, int.MaxValue);
        _mutex.Acquire(0, 1000);
    }

    public void EndDraw()
    {
        try
        {
            _mutex.Release(1);
            _imported ??= _interop.ImportImage(
                new PlatformHandle(_handle, KnownPlatformGraphicsExternalImageHandleTypes.D3D11TextureGlobalSharedHandle),
                _properties);
            LastPresent = _target.UpdateWithKeyedMutexAsync(_imported, 1, 0);
        }
        catch
        {
        }
    }


    public void Dispose()
    {
        if (LastPresent is not null)
        {
            LastPresent.Wait();
            LastPresent.Dispose();
        }

        RenderTargetView.Dispose();
        _mutex.Dispose();
        _texture.Dispose();
    }
}
