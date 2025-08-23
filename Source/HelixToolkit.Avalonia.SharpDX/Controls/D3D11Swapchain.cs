using Avalonia;
using Avalonia.Rendering.Composition;
using SharpDX.Direct3D11;
using D3DDevice = SharpDX.Direct3D11.Device;

namespace HelixToolkit.Avalonia.SharpDX.Controls;

internal sealed class D3D11Swapchain
{
    private readonly D3DDevice _device;
    private readonly ICompositionGpuInterop _interop;
    private readonly CompositionDrawingSurface _target;

    private PixelSize _size;
    private D3D11SwapchainImage? _image;

    public D3D11Swapchain(D3DDevice device, ICompositionGpuInterop interop, CompositionDrawingSurface target)
    {
        _device = device;
        _interop = interop;
        _target = target;
    }

    public RenderTargetView? RenderTargetView => _image?.RenderTargetView;

    public Texture2D? Texture => _image?.Texture;

    public void Dispose()
    {
        _size = default;
        _image?.Dispose();
    }

    public void Resize(PixelSize size)
    {
        Dispose();

        _size = size;
        _image = new D3D11SwapchainImage(_device, size, _interop, _target);
    }

    public void BeginDraw(PixelSize size)
    {
        if (_size != size)
        {
            Resize(size);
        }

        _image?.BeginDraw();
    }

    public void EndDraw()
    {
        _image?.EndDraw();
    }
}
