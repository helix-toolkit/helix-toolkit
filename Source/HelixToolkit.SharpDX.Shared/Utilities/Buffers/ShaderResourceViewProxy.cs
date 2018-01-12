using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    /// <summary>
    /// A proxy container to handle view resources
    /// </summary>
    public sealed class ShaderResouceViewProxy : DisposeObject
    {
        public ShaderResourceView TextureView { get { return textureView; } }
        private ShaderResourceView textureView;

        public DepthStencilView DepthStencilView { get { return depthStencilView; } }
        private DepthStencilView depthStencilView;

        public RenderTargetView RenderTargetView { get { return renderTargetView; } }
        private RenderTargetView renderTargetView;

        public Resource Resource { get { return resource; } }
        private Resource resource;

        private readonly Device device;

        public ShaderResouceViewProxy(Device device) { this.device = device; }
        public ShaderResouceViewProxy(Device device, Texture1DDescription textureDesc) : this(device)
        {
            resource = Collect(new Texture1D(device, textureDesc));
        }

        public ShaderResouceViewProxy(Device device, Texture2DDescription textureDesc) : this(device)
        {
            resource = Collect(new Texture2D(device, textureDesc));
        }

        public ShaderResouceViewProxy(Device device, Texture3DDescription textureDesc) : this(device)
        {
            resource = Collect(new Texture3D(device, textureDesc));
        }

        public void CreateView(System.IO.Stream stream)
        {
            this.DisposeAndClear();
            if (stream != null && device != null)
            {
                textureView = Collect(TextureLoader.FromMemoryAsShaderResourceView(device, stream));
            }
        }

        public void CreateView(ShaderResourceViewDescription desc)
        {
            RemoveAndDispose(ref textureView);
            textureView = Collect(new ShaderResourceView(device, resource, desc));
        }

        public void CreateView(DepthStencilViewDescription desc)
        {
            RemoveAndDispose(ref depthStencilView);
            depthStencilView = Collect(new DepthStencilView(device, resource, desc));
        }
        public void CreateView(RenderTargetViewDescription desc)
        {
            RemoveAndDispose(ref renderTargetView);
            renderTargetView = Collect(new RenderTargetView(device, resource, desc));
        }

        public static implicit operator ShaderResourceView(ShaderResouceViewProxy proxy)
        {
            return proxy.textureView;
        }

        public static implicit operator DepthStencilView(ShaderResouceViewProxy proxy)
        {
            return proxy.depthStencilView;
        }

        public static implicit operator RenderTargetView(ShaderResouceViewProxy proxy)
        {
            return proxy.renderTargetView;
        }
    }
}
