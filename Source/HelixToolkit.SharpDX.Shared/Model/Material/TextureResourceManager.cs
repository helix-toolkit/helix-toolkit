using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using global::SharpDX.Direct3D11;
    using Utilities;
    /// <summary>
    /// Use for texture resource sharing between models. It uses texture stream as key for each texture.
    /// <para>Call Register to get(if already exists) or create a new shared texture.</para>
    /// <para>Call Unregister to detach the texture from model. Call detach from SharedTextureResourceProxy achieves the same result.</para>
    /// </summary>
    public class TextureResourceManager : DisposeObject, ITextureResourceManager
    {
        private readonly Dictionary<Stream, SharedTextureResourceProxy> resourceDictionary = new Dictionary<Stream, SharedTextureResourceProxy>();
        private readonly Device device;
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureResourceManager"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public TextureResourceManager(Device device)
        {
            this.device = device;
        }

        /// <summary>
        /// Registers the specified material unique identifier.
        /// </summary>
        /// <param name="modelGuid">The material unique identifier.</param>
        /// <param name="textureStream">The texture steam.</param>
        /// <returns></returns>
        public SharedTextureResourceProxy Register(Guid modelGuid, Stream textureStream)
        {
            SharedTextureResourceProxy proxy;
            lock (resourceDictionary)
            {
                if (resourceDictionary.TryGetValue(textureStream, out proxy))
                {
                    proxy.Attach(modelGuid);
                }
                else
                {
                    proxy = new SharedTextureResourceProxy(device, textureStream);
                    proxy.Attach(modelGuid);
                    proxy.Disposed += (s, e) =>
                    {
                        lock (resourceDictionary)
                        {
                            resourceDictionary.Remove(textureStream);
                        }
                    };
                    resourceDictionary.Add(textureStream, proxy);
                }
            }
            return proxy;
        }
        /// <summary>
        /// Unregisters the specified material unique identifier.
        /// </summary>
        /// <param name="modelGuid">The material unique identifier.</param>
        /// <param name="textureStream">The texture stream.</param>
        public void Unregister(Guid modelGuid, Stream textureStream)
        {
            SharedTextureResourceProxy proxy;
            lock (resourceDictionary)
            {
                if (resourceDictionary.TryGetValue(textureStream, out proxy))
                {
                    proxy.Detach(modelGuid);
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                lock (resourceDictionary)
                {
                    foreach(var resource in resourceDictionary.Values.ToArray())
                    {
                        resource.Dispose();
                    }
                    resourceDictionary.Clear();
                }
            }
            base.OnDispose(disposeManagedResources);
        }
    }

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
        public ShaderResourceView TextureView { get { return resource.TextureView; } }
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
        public static implicit operator ShaderResourceView(SharedTextureResourceProxy proxy)
        {
            return proxy == null ? null : proxy.resource;
        }
    }
}
