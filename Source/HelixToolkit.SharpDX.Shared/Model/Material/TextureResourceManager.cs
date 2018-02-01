using System;
using System.Collections.Generic;
using System.IO;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using global::SharpDX.Direct3D11;
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public class TextureResourceManager : DisposeObject, ITextureResourceManager
    {
        private readonly Dictionary<Stream, TextureResourceProxy> resourceDictionary = new Dictionary<Stream, TextureResourceProxy>();
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
        public TextureResourceProxy Register(Guid modelGuid, Stream textureStream)
        {
            TextureResourceProxy proxy;
            lock (resourceDictionary)
            {
                if (resourceDictionary.TryGetValue(textureStream, out proxy))
                {
                    proxy.Attach(modelGuid);
                }
                else
                {
                    proxy = new TextureResourceProxy(device, textureStream);
                    proxy.Attach(modelGuid);
                    proxy.Disposing += (s, e) => { resourceDictionary.Remove(textureStream); };
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
            TextureResourceProxy proxy;
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
        protected override void Dispose(bool disposeManagedResources)
        {
            resourceDictionary.Clear();
            base.Dispose(disposeManagedResources);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextureResourceProxy : DisposeObject
    {
        private ShaderResouceViewProxy resource;
        private readonly HashSet<Guid> hashSet = new HashSet<Guid>();
        /// <summary>
        /// Gets the texture view.
        /// </summary>
        /// <value>
        /// The texture view.
        /// </value>
        public ShaderResourceView TextureView { get { return resource.TextureView; } }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureResourceProxy"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="stream">The stream.</param>
        public TextureResourceProxy(Device device, Stream stream)
        {
            resource = Collect(new ShaderResouceViewProxy(device));
            resource.CreateView(stream);
        }
        /// <summary>
        /// Attaches the specified model unique identifier.
        /// </summary>
        /// <param name="modelGuid">The model unique identifier.</param>
        public void Attach(Guid modelGuid)
        {
            hashSet.Add(modelGuid);
        }
        /// <summary>
        /// Detaches the specified model unique identifier.
        /// </summary>
        /// <param name="modelGuid">The model unique identifier.</param>
        public void Detach(Guid modelGuid)
        {
            hashSet.Remove(modelGuid);
            if (hashSet.Count == 0)
            {
                this.Dispose();
            }
        }
        /// <summary>
        /// Performs an implicit conversion from <see cref="TextureResourceProxy"/> to <see cref="ShaderResouceViewProxy"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ShaderResouceViewProxy(TextureResourceProxy proxy)
        {
            return proxy.resource;
        }
        /// <summary>
        /// Performs an implicit conversion from <see cref="TextureResourceProxy"/> to <see cref="ShaderResourceView"/>.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ShaderResourceView(TextureResourceProxy proxy)
        {
            return proxy.resource;
        }
    }
}
