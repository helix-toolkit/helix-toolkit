using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    public sealed class ShaderResouceViewProxy : DisposeObject
    {
        public ShaderResourceView TextureView { get { return textureView; } }
        private ShaderResourceView textureView;

        public void CreateTextureView(Device device, System.IO.Stream stream)
        {
            RemoveAndDispose(ref textureView);
            if (stream != null && device != null)
            {
                textureView = Collect(TextureLoader.FromMemoryAsShaderResourceView(device, stream));
            }
        }

        public static implicit operator ShaderResourceView(ShaderResouceViewProxy proxy)
        {
            return proxy.textureView;
        }
    }
}
