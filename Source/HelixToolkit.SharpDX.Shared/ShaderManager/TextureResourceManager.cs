using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using global::SharpDX.Direct3D11;
    
    using Utilities;
    /// <summary>
    /// Use for texture resource sharing between models. It uses texture stream as key for each texture.
    /// <para>Call Register to get(if already exists) or create a new shared texture.</para>
    /// <para>Call Unregister to detach the texture from model. Call detach from SharedTextureResourceProxy achieves the same result.</para>
    /// </summary>
    public sealed class TextureResourceManager : IDisposable, ITextureResourceManager
    {
        public int Count { get { return resourceDictionaryMipMaps.Count + resourceDictionaryNoMipMaps.Count; } }
        private readonly Dictionary<Stream, ShaderResourceViewProxy> resourceDictionaryMipMaps = new Dictionary<Stream, ShaderResourceViewProxy>();
        private readonly Dictionary<Stream, ShaderResourceViewProxy> resourceDictionaryNoMipMaps = new Dictionary<Stream, ShaderResourceViewProxy>();
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
        /// Registers the specified texture stream. This creates mipmaps automatically
        /// </summary>
        /// <param name="textureStream">The texture stream.</param>
        /// <returns></returns>
        public ShaderResourceViewProxy Register(Stream textureStream)
        {
            return Register(textureStream, false);
        }
        /// <summary>
        /// Registers the specified material unique identifier.
        /// </summary>
        /// <param name="textureStream">The texture steam.</param>
        /// <param name="disableAutoGenMipMap">Disable generate mipmaps automatically</param>
        /// <returns></returns>
        public ShaderResourceViewProxy Register(Stream textureStream, bool disableAutoGenMipMap)
        {
            if (textureStream == null)
            {
                return null;
            }
            var targetDict = disableAutoGenMipMap ? resourceDictionaryNoMipMaps : resourceDictionaryMipMaps;
            lock (targetDict)
            {
                if (targetDict.TryGetValue(textureStream, out ShaderResourceViewProxy view))
                {
                    view.IncRef();
                    return view;
                }
                else
                {
                    var proxy = new ShaderResourceViewProxy(device);
                    proxy.CreateView(textureStream, disableAutoGenMipMap);
                    proxy.Disposed += (s, e) =>
                    {
                        lock (targetDict)
                        {
                            targetDict.Remove(textureStream);
                        }
                    };
                    targetDict.Add(textureStream, proxy);
                    return proxy;
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    lock (resourceDictionaryMipMaps)
                    {
                        foreach (var resource in resourceDictionaryMipMaps.Values.ToArray())
                        {
                            resource.ForceDispose();
                        }
                        resourceDictionaryMipMaps.Clear();
                    }
                    lock (resourceDictionaryNoMipMaps)
                    {
                        foreach (var resource in resourceDictionaryNoMipMaps.Values.ToArray())
                        {
                            resource.ForceDispose();
                        }
                        resourceDictionaryNoMipMaps.Clear();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TextureResourceManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
}
