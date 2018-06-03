using global::SharpDX.Direct3D11;
using System.IO;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    /// <summary>
    /// Shared texture resource proxy. Used in Texture Resource Manager for texture resource sharing
    /// <para>When using this proxy, do not dispose this object. Instead, call detach(Model GUID) to remove it from the model. It will be disposed automatically when no model is detached.</para>
    /// </summary>
    public sealed class SharedTextureResourceProxy : ResourceSharedObject
    {
        private ShaderResourceViewProxy resource;

        /// <summary>
        /// Gets the texture view.
        /// </summary>
        /// <value>
        /// The texture view.
        /// </value>
        public ShaderResourceViewProxy TextureView { get { return resource; } }
        /// <summary>
        /// Initializes a new instance of the <see cref="SharedTextureResourceProxy"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="stream">The stream.</param>
        public SharedTextureResourceProxy(Device device, Stream stream)
        {
            resource = Collect(new ShaderResourceViewProxy(device));
            resource.CreateView(stream);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SharedTextureResourceProxy"/> to <see cref="ShaderResourceViewProxy"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ShaderResourceViewProxy(SharedTextureResourceProxy proxy)
        {
            return proxy == null ? null : proxy.resource;
        }
        /// <summary>
        /// Performs an implicit conversion from <see cref="SharedTextureResourceProxy"/> to <see cref="ShaderResourceView"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator ShaderResourceView(SharedTextureResourceProxy proxy)
        {
            return proxy == null ? null : proxy.resource;
        }
    }
}
