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
        private readonly Dictionary<Stream, ShaderResourceViewProxy> resourceDictionary = new Dictionary<Stream, ShaderResourceViewProxy>();
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
        /// <param name="textureStream">The texture steam.</param>
        /// <returns></returns>
        public ShaderResourceViewProxy Register(Stream textureStream)
        {
            if (textureStream == null)
            {
                return null;
            }
            lock (resourceDictionary)
            {
                if (resourceDictionary.TryGetValue(textureStream, out ShaderResourceViewProxy view))
                {
                    view.IncRef();
                    return view;
                }
                else
                {
                    var proxy = new ShaderResourceViewProxy(device);
                    proxy.CreateView(textureStream);
                    proxy.Disposed += (s, e) =>
                    {
                        lock (resourceDictionary)
                        {
                            resourceDictionary.Remove(textureStream);
                        }
                    };
                    resourceDictionary.Add(textureStream, proxy);
                    return proxy;
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
}
