using HelixToolkit.SharpDX.Utilities;
using Microsoft.Extensions.Logging;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Use for texture resource sharing between models. It uses texture stream as key for each texture.
/// <para>Call Register to get(if already exists) or create a new shared texture.</para>
/// <para>Call Unregister to detach the texture from model. Call detach from SharedTextureResourceProxy achieves the same result.</para>
/// </summary>
public sealed class TextureResourceManager : IDisposable, ITextureResourceManager
{
    private static readonly ILogger logger = Logger.LogManager.Create<TextureResourceManager>();
    public int Count
    {
        get
        {
            return resourceDictionaryMipMaps.Count + resourceDictionaryNoMipMaps.Count;
        }
    }
    private readonly Dictionary<Guid, ShaderResourceViewProxy> resourceDictionaryMipMaps = new();
    private readonly Dictionary<Guid, ShaderResourceViewProxy> resourceDictionaryNoMipMaps = new();
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
    /// Registers the specified texture model. This creates mipmaps automatically
    /// </summary>
    /// <param name="textureStream">The texture model.</param>
    /// <returns></returns>
    public ShaderResourceViewProxy? Register(TextureModel? textureStream)
    {
        return Register(textureStream, true);
    }
    /// <summary>
    /// Registers the specified material unique identifier.
    /// </summary>
    /// <param name="textureModel">The texture model.</param>
    /// <param name="enableAutoGenMipMap">Enable generate mipmaps automatically</param>
    /// <returns></returns>
    public ShaderResourceViewProxy? Register(TextureModel? textureModel, bool enableAutoGenMipMap)
    {
        if (textureModel == null)
        {
            return null;
        }
        var targetDict = enableAutoGenMipMap ? resourceDictionaryMipMaps : resourceDictionaryNoMipMaps;
        lock (targetDict)
        {
            if (targetDict.TryGetValue(textureModel.Guid, out var view))
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Re-using existing texture resource");
                }
                view.IncRef();
                return view;
            }
            else
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Creating new texture resource");
                }
                var proxy = new ShaderResourceViewProxy(device);
                proxy.CreateView(textureModel, true, enableAutoGenMipMap);
                proxy.Guid = textureModel.Guid;
                proxy.Disposed += (s, e) =>
                {
                    lock (targetDict)
                    {
                        targetDict.Remove(proxy.Guid);
                    }
                };
                targetDict.Add(textureModel.Guid, proxy);
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
