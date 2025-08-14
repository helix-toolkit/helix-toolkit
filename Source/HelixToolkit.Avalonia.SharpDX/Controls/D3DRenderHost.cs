using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using Avalonia.VisualTree;
using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using DeviceContext = SharpDX.Direct3D11.DeviceContext;
using Microsoft.Extensions.Logging;

namespace HelixToolkit.Avalonia.SharpDX.Controls;

internal sealed class D3DRenderHost : DefaultRenderHost
{
    private Visual _parent;
    //private static readonly ILogger logger = Logger.LogManager.Create<D3DRenderHost>();

    private CompositionSurfaceVisual? _visual;
    private Compositor? _compositor;
    private string _info = string.Empty;
    private bool _updateQueued;
    private bool _initialized;

    public CompositionDrawingSurface? Surface { get; private set; }

    public string Info => _info;

    private Device? _device;
    private DeviceContext? _context;
    private D3D11Swapchain? _swapchain;
    private PixelSize _pixelSize;

    public D3DRenderHost(Visual parent)
    {
        _parent = parent;
    }

    protected override void OnStartD3D()
    {
        base.OnStartD3D();
        Initialize().Wait();
        _parent.PropertyChanged += ParentPropertyChanged;
    }

    protected override void OnEndingD3D()
    {
        _parent.PropertyChanged -= ParentPropertyChanged;
        if (_initialized)
        {
            FreeGraphicsResources();
        }
        _initialized = false;
        base.OnEndingD3D();
    }

    private async Task Initialize()
    {
        try
        {
            var selfVisual = ElementComposition.GetElementVisual(_parent)!;
            _compositor = selfVisual.Compositor;

            Surface = _compositor.CreateDrawingSurface();
            _visual = _compositor.CreateSurfaceVisual();
            Rect bounds = _parent.Bounds;
            _visual.Size = new(bounds.Width, bounds.Height);
            _visual.Surface = Surface;
            ElementComposition.SetElementChildVisual(_parent, _visual);
            var (res, info) = await DoInitialize(_compositor, Surface);
            _info = info;
            _initialized = res;
            //QueueNextFrame();
        }
        catch (Exception e)
        {
            _info = e.ToString();
        }
    }

    private void ParentPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == Visual.BoundsProperty)
        {
            Rect bounds = (Rect)e.NewValue!;
            Resize((int)bounds.Width, (int)bounds.Height);
            Restart(false);

            //EffectsManager?.DisposeAllResources();
            //EffectsManager?.Reinitialize();

            QueueNextFrame();
        }
    }

    private void UpdateFrame()
    {
        _updateQueued = false;
        var root = _parent.GetVisualRoot();
        if (root == null)
            return;

        Rect bounds = _parent.Bounds;
        _visual!.Size = new(bounds.Width, bounds.Height);
        //_pixelSize = PixelSize.FromSize(bounds.Size, root.RenderScaling);
        _pixelSize = PixelSize.FromSize(bounds.Size, 1.0);
        Resize(_pixelSize.Width, _pixelSize.Height);
        RenderFrame();
        QueueNextFrame();
    }

    private void QueueNextFrame()
    {
        if (_initialized && !_updateQueued && _compositor != null)
        {
            _updateQueued = true;
            _compositor?.RequestCompositionUpdate(UpdateFrame);
        }
    }

    private async Task<(bool success, string info)> DoInitialize(
        Compositor compositor,
        CompositionDrawingSurface compositionDrawingSurface)
    {
        var interop = await compositor.TryGetCompositionGpuInterop();
        if (interop == null)
            return (false, "Compositor doesn't support interop for the current backend");

        return InitializeGraphicsResources(compositor, compositionDrawingSurface, interop);
    }

    private (bool success, string info) InitializeGraphicsResources(Compositor compositor,
        CompositionDrawingSurface compositionDrawingSurface, ICompositionGpuInterop gpuInterop)
    {
        if (gpuInterop.SupportedImageHandleTypes.Contains(KnownPlatformGraphicsExternalImageHandleTypes
                .D3D11TextureGlobalSharedHandle) != true)
            return (false, "DXGI shared handle import is not supported by the current graphics backend");

        if (EffectsManager is null || EffectsManager.Device is null)
        {
            return (true, string.Empty);
        }

        _device = EffectsManager.Device;
        _swapchain = new D3D11Swapchain(_device, gpuInterop, compositionDrawingSurface);
        _context = _device.ImmediateContext;

        return (true, $"D3D11 ({EffectsManager.Device.FeatureLevel})");
    }

    private void FreeGraphicsResources()
    {
        _swapchain?.Dispose();
        _swapchain = null;
    }

    private void RenderFrame()
    {
        if (_pixelSize == default || _device is null || _context is null || _swapchain is null)
            return;

        //this.InvalidateRender();

        if (this.UpdateRequested)
        {
            this.SetDefaultRenderTargets(true);
            this.UpdateAndRender();
        }
    }

    protected override void PostRender()
    {
        base.PostRender();

        if (_pixelSize == default || _device is null || _context is null || _swapchain is null || this.RenderTargetBufferView is null)
            return;

        var surface = this.D2DTarget?.D2DTarget?.Bitmap.Surface;

        if (surface is null)
        {
            return;
        }

        using var texture = surface.QueryInterface<Texture2D>();

        //using var texture = this.RenderTargetBufferView.ResourceAs<Texture2D>();
        //ScreenCapture.SaveWICTextureToFile(EffectsManager!, texture, "temp.png", HelixToolkit.SharpDX.Direct2DImageFormat.Png);

        _swapchain.BeginDraw(_pixelSize);
        _context.CopyResource(texture, _swapchain.Texture);
        _swapchain.EndDraw();
        _context.Flush();
    }
}
