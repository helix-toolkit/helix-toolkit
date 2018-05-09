using SharpDX.Direct3D11;
using System.Collections.Concurrent;
using Format = SharpDX.DXGI.Format;
#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{    
    using Utilities;

    public sealed class PingPongColorBuffers : DisposeObject
    {
        /// <summary>
        /// Gets the current ShaderResourceView.
        /// </summary>
        /// <value>
        /// The current SRV.
        /// </value>
        public ShaderResourceView CurrentSRV
        {
            get
            {
                return textures[0].TextureView;
            }
        }

        /// <summary>
        /// Gets the next SRV.
        /// </summary>
        /// <value>
        /// The next SRV.
        /// </value>
        public ShaderResourceView NextSRV
        {
            get
            {
                return textures[1].TextureView;
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
        public RenderTargetView CurrentRTV
        {
            get
            {
                return textures[0].RenderTargetView;
            }
        }

        /// <summary>
        /// Gets the next RTV.
        /// </summary>
        /// <value>
        /// The next RTV.
        /// </value>
        public RenderTargetView NextRTV
        {
            get
            {
                return textures[1].RenderTargetView;
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
        /// Initializes a new instance of the <see cref="PostEffectMeshOutlineBlurCore"/> class.
        /// </summary>
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
        private readonly ConcurrentDictionary<Format, ConcurrentBag<ShaderResourceViewProxy>> pool = new ConcurrentDictionary<Format, ConcurrentBag<ShaderResourceViewProxy>>();
        private readonly IDevice3DResources deviceResourse;
        private Texture2DDescription description;

        public TexturePool(IDevice3DResources deviceResourse, Texture2DDescription desc)
        {
            this.deviceResourse = deviceResourse;
            description = desc;
        }

        public ShaderResourceViewProxy Get(Format format)
        {
            if (IsDisposed)
            {
                return null;
            }
            ConcurrentBag<ShaderResourceViewProxy> bag;
            ShaderResourceViewProxy proxy;
            if(pool.TryGetValue(format, out bag) && bag.TryTake(out proxy))
            {
                return proxy;
            }
            else
            {
                description.Format = format;
                var texture = Collect(new ShaderResourceViewProxy(deviceResourse.Device, description));
                if((description.BindFlags & BindFlags.RenderTarget) != 0)
                {
                    texture.CreateRenderTargetView();
                }
                if((description.BindFlags & BindFlags.ShaderResource) != 0)
                {
                    texture.CreateTextureView();
                }
                if((description.BindFlags & BindFlags.DepthStencil) != 0)
                {
                    texture.CreateDepthStencilView();
                }
                return texture;
            }
        }

        public void Put(Format format, ShaderResourceViewProxy proxy)
        {
            if (IsDisposed)
            {
                return;
            }
            ConcurrentBag<ShaderResourceViewProxy> bag = pool.GetOrAdd(format, new System.Func<Format, ConcurrentBag<ShaderResourceViewProxy>>((d) => { return new ConcurrentBag<ShaderResourceViewProxy>(); }));
            bag.Add(proxy);
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            ShaderResourceViewProxy proxy;
            foreach(var bag in pool.Values)
            {
                while(bag.TryTake(out proxy))
                {
                    continue;
                }
            }
            pool.Clear();
            base.OnDispose(disposeManagedResources);
        }
    }
}
