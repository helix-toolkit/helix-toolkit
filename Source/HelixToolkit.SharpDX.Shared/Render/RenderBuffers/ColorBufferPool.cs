using SharpDX.Direct3D11;
using System.Collections.Concurrent;
using Format = SharpDX.DXGI.Format;
using System.Diagnostics;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Render
    {
        using Utilities;
        /// <summary>
        /// 
        /// </summary>
        public sealed class PingPongColorBuffers : DisposeObject
        {
            /// <summary>
            /// Gets the current ShaderResourceViewProxy.
            /// </summary>
            /// <value>
            /// The current SRV.
            /// </value>
            public ShaderResourceViewProxy CurrentSRV
            {
                get
                {
                    return textures[0];
                }
            }

            /// <summary>
            /// Gets the next SRV.
            /// </summary>
            /// <value>
            /// The next SRV.
            /// </value>
            public ShaderResourceViewProxy NextSRV
            {
                get
                {
                    return textures[1];
                }
            }

            public int Width { get { return texture2DDesc.Width; } }

            public int Height { get { return texture2DDesc.Height; } }

            /// <summary>
            /// Gets the current RenderTargetView.
            /// </summary>
            /// <value>
            /// The current RTV.
            /// </value>
            public ShaderResourceViewProxy CurrentRTV
            {
                get
                {
                    return textures[0];
                }
            }

            /// <summary>
            /// Gets the next RTV.
            /// </summary>
            /// <value>
            /// The next RTV.
            /// </value>
            public ShaderResourceViewProxy NextRTV
            {
                get
                {
                    return textures[1];
                }
            }

            public Resource CurrentTexture
            {
                get
                {
                    return textures[0].Resource;
                }
            }
            #region Texture Resources

            private const int NumPingPongBlurBuffer = 2;

            private readonly ShaderResourceViewProxy[] textures = new ShaderResourceViewProxy[NumPingPongBlurBuffer];

            private Texture2DDescription texture2DDesc = new Texture2DDescription()
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Usage = ResourceUsage.Default,
                ArraySize = 1,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new global::SharpDX.DXGI.SampleDescription(1, 0)
            };

            #endregion Texture Resources
            private readonly IDevice3DResources deviceResources;
            public bool Initialized { private set; get; } = false;
            /// <summary>
            /// Initializes a new instance of the <see cref="PingPongColorBuffers"/> class.
            /// </summary>
            /// <param name="textureFormat">The texture format.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <param name="deviceRes">The device resource.</param>
            public PingPongColorBuffers(global::SharpDX.DXGI.Format textureFormat, int width, int height, IDevice3DResources deviceRes)
            {
                texture2DDesc.Format = textureFormat;
                deviceResources = deviceRes;
                texture2DDesc.Width = width;
                texture2DDesc.Height = height;
            }

            /// <summary>
            /// Initializes this instance.
            /// </summary>
            public void Initialize()
            {
                if (Initialized)
                {
                    return;
                }
                for (int i = 0; i < NumPingPongBlurBuffer; ++i)
                {
                    textures[i] = Collect(new ShaderResourceViewProxy(deviceResources.Device, texture2DDesc));
                    textures[i].CreateRenderTargetView();
                    textures[i].CreateTextureView();
                }
                Initialized = true;
            }

            /// <summary>
            /// Swaps the targets.
            /// </summary>
            public void SwapTargets()
            {
                //swap buffer
                var current = textures[0];
                textures[0] = textures[1];
                textures[1] = current;
            }
        }


        public sealed class TexturePool : DisposeObject
        {
            private sealed class PooledShaderResourceViewProxy : ShaderResourceViewProxy
            {
                private readonly ConcurrentBag<ShaderResourceViewProxy> pool;

                public PooledShaderResourceViewProxy(Device device, Texture2DDescription textureDesc, ConcurrentBag<ShaderResourceViewProxy> pool) 
                    : base(device, textureDesc)
                {
                    this.pool = pool;
                    IsPooled = true;
                }

                protected override void OnPutBackToPool()
                {
                    pool.Add(this);
                }
            }

            private readonly ConcurrentDictionary<Format, ConcurrentBag<ShaderResourceViewProxy>> pool = new ConcurrentDictionary<Format, ConcurrentBag<ShaderResourceViewProxy>>();
            private readonly IDevice3DResources deviceResourse;
            private Texture2DDescription description;
            public int Width { get => description.Width; }
            public int Height { get => description.Height; }

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
                if(pool.TryGetValue(format, out var bag) && bag.TryTake(out var proxy) && !proxy.IsDisposed)
                {
                    return proxy;
                }
                else
                {
                    bag = bag ?? pool.GetOrAdd(format, new System.Func<Format, ConcurrentBag<ShaderResourceViewProxy>>((d) =>
                    { return new ConcurrentBag<ShaderResourceViewProxy>(); }));
                    var desc = description;
                    desc.Format = format;
                    ShaderResourceViewProxy texture = null;
                
                    if((desc.BindFlags & BindFlags.RenderTarget) != 0)
                    {
                        texture = Collect(new PooledShaderResourceViewProxy(deviceResourse.Device, desc, bag));
                        texture.CreateRenderTargetView();
                        if((desc.BindFlags & BindFlags.ShaderResource) != 0)
                        {
                            texture.CreateTextureView();
                        }
                    }
                    else if((desc.BindFlags & BindFlags.DepthStencil) != 0)
                    {
                        if (format == Format.R32_Typeless)// Special handle for depth buffer used as both depth stencil and shader resource
                        {
                            desc.BindFlags |= BindFlags.ShaderResource;
                        }
                        texture = Collect(new PooledShaderResourceViewProxy(deviceResourse.Device, desc, bag));
                        if (format == Format.R32_Typeless)// Special handle for depth buffer used as both depth stencil and shader resource
                        {
                            texture.CreateView(new DepthStencilViewDescription() { Format = Format.D32_Float, Dimension = DepthStencilViewDimension.Texture2D });
                            texture.CreateView(new ShaderResourceViewDescription()
                            {
                                Format = Format.R32_Float,
                                Dimension = global::SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                                Texture2D = new ShaderResourceViewDescription.Texture2DResource() { MipLevels = desc.MipLevels }
                            });
                        }
                        else
                        {
                            texture.CreateDepthStencilView();
                        }
                    }
                    Debug.WriteLine("Create New Full Screen Texture");
                    return texture;
                }
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                foreach(var bag in pool.Values)
                {
                    while(bag.TryTake(out var proxy))
                    {
                        proxy.IsPooled = false; // Set flag to false so it can be disposed
                        continue;
                    }
                }
                pool.Clear();
                base.OnDispose(disposeManagedResources);
            }
        }
    }

}
