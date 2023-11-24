using HelixToolkit.SharpDX.Utilities;
using Microsoft.Extensions.Logging;
using SharpDX.Direct3D11;
using System.Collections.Concurrent;
using Format = SharpDX.DXGI.Format;

namespace HelixToolkit.SharpDX.Render;

public sealed class TexturePool : DisposeObject
{
    private static readonly ILogger logger = Logger.LogManager.Create<TexturePool>();
    private sealed class PooledShaderResourceViewProxy : ShaderResourceViewProxy
    {
        private readonly ConcurrentBag<ShaderResourceViewProxy> pool;

        public PooledShaderResourceViewProxy(Device device, Texture2DDescription textureDesc, ConcurrentBag<ShaderResourceViewProxy> pool)
            : base(device, textureDesc)
        {
            this.pool = pool;
            AddBackToPool = (o) => { pool.Add(this); };
        }
    }

    private readonly ConcurrentDictionary<Format, ConcurrentBag<ShaderResourceViewProxy>> pool = new();
    private readonly IDevice3DResources deviceResourse;
    private Texture2DDescription description;
    public int Width
    {
        get => description.Width;
    }
    public int Height
    {
        get => description.Height;
    }

    public TexturePool(IDevice3DResources deviceResourse, Texture2DDescription desc)
    {
        this.deviceResourse = deviceResourse;
        description = desc;
    }
    /// <summary>
    /// Gets the off screen texture with specified format. After using it, make sure to call Dispose() to return it back into the pool.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public ShaderResourceViewProxy Get(Format format)
    {
        if (IsDisposed)
        {
            return ShaderResourceViewProxy.Empty;
        }
        if (pool.TryGetValue(format, out var bag) && bag.TryTake(out var proxy) && !proxy.IsDisposed)
        {
            proxy.IncRef();
            return proxy;
        }
        else
        {
            bag ??= pool.GetOrAdd(format, new System.Func<Format, ConcurrentBag<ShaderResourceViewProxy>>((d) =>
            {
                return new ConcurrentBag<ShaderResourceViewProxy>();
            }));
            var desc = description;
            desc.Format = format;
            ShaderResourceViewProxy? texture = null;

            if ((desc.BindFlags & BindFlags.RenderTarget) != 0)
            {
                if (deviceResourse.Device is not null)
                {
                    texture = new PooledShaderResourceViewProxy(deviceResourse.Device, desc, bag);
                    texture.CreateRenderTargetView();
                    if ((desc.BindFlags & BindFlags.ShaderResource) != 0)
                    {
                        texture.CreateTextureView();
                    }
                }
            }
            else if ((desc.BindFlags & BindFlags.DepthStencil) != 0)
            {
                desc.Format = DepthStencilFormatHelper.ComputeTextureFormat(format, out var canUseAsShaderResource);
                if (canUseAsShaderResource)
                {
                    desc.BindFlags |= BindFlags.ShaderResource;
                }
                if (deviceResourse.Device is not null)
                {
                    texture = new PooledShaderResourceViewProxy(deviceResourse.Device, desc, bag);
                    texture.CreateView(new DepthStencilViewDescription()
                    {
                        Format = DepthStencilFormatHelper.ComputeDSVFormat(format),
                        Dimension = DepthStencilViewDimension.Texture2D
                    });
                    if (canUseAsShaderResource)
                    {
                        texture.CreateView(new ShaderResourceViewDescription()
                        {
                            Format = DepthStencilFormatHelper.ComputeSRVFormat(format),
                            Dimension = global::SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                            Texture2D = new ShaderResourceViewDescription.Texture2DResource() { MipLevels = desc.MipLevels }
                        });
                    }
                }
            }
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Create New Full Screen Texture");
            }
            texture?.IncRef();
            return texture ?? ShaderResourceViewProxy.Empty;
        }
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        foreach (var bag in pool.Values)
        {
            while (bag.TryTake(out var proxy))
            {
                proxy.Dispose(); // Set flag to false so it can be disposed
                continue;
            }
        }
        pool.Clear();
        base.OnDispose(disposeManagedResources);
    }
}
