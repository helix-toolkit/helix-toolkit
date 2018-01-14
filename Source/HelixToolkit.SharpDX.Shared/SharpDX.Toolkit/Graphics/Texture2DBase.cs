/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Abstract class front end to <see cref="SharpDX.Direct3D11.Texture2D"/>.
    /// </summary>
    public abstract class Texture2DBase : Texture
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly new Direct3D11.Texture2D Resource;
        private DXGI.Surface dxgiSurface;

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture2DBase" /> class.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="description2D">The description.</param>
        /// <msdn-id>ff476521</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>	
        protected internal Texture2DBase(Direct3D11.Device device, Texture2DDescription description2D)
            : base(device, description2D)
        {
            Resource = new Direct3D11.Texture2D(device, description2D);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture2DBase" /> class.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="description2D">The description.</param>
        /// <param name="dataBoxes">A variable-length parameters list containing data rectangles.</param>
        /// <msdn-id>ff476521</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>	
        protected internal Texture2DBase(Direct3D11.Device device, Texture2DDescription description2D, DataBox[] dataBoxes)
            : base(device ,description2D)
        {
            Resource = new Direct3D11.Texture2D(device, description2D, dataBoxes);
        }

        /// <summary>
        /// Specialised constructor for use only by derived classes.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="texture">The texture.</param>
        /// <msdn-id>ff476521</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>	
        protected internal Texture2DBase(Direct3D11.Device device, Direct3D11.Texture2D texture)
            : base(device, texture.Description)
        {
            Resource = texture;
        }

        /// <summary>
        /// Return an equivalent staging texture CPU read-writable from this instance.
        /// </summary>
        /// <returns></returns>
        public override Texture ToStaging()
        {
            return new Texture2D(this.GraphicsDevice, this.Description.ToStagingDescription());            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual DXGI.Format GetDefaultViewFormat()
        {
            return this.Description.Format;
        }

        internal override TextureView GetShaderResourceView(Format viewFormat, ViewType viewType, int arrayOrDepthSlice, int mipIndex)
        {
            if ((this.Description.BindFlags & BindFlags.ShaderResource) == 0)
                return null;

            int arrayCount;
            int mipCount;
            GetViewSliceBounds(viewType, ref arrayOrDepthSlice, ref mipIndex, out arrayCount, out mipCount);

            var textureViewKey = new TextureViewKey(viewFormat, viewType, arrayOrDepthSlice, mipIndex);

            lock (this.shaderResourceViews)
            {
                TextureView srv;
                // Creates the shader resource view
                if (!shaderResourceViews.TryGetValue(textureViewKey, out srv))
                {
                    // Create the view
                    var srvDescription = new ShaderResourceViewDescription() { Format = viewFormat };

                    // Initialize for texture arrays or texture cube
                    if (this.Description.ArraySize > 1)
                    {
                        // If texture cube
                        if ((this.Description.OptionFlags & ResourceOptionFlags.TextureCube) != 0)
                        {
                            srvDescription.Dimension = ShaderResourceViewDimension.TextureCube;
                            srvDescription.TextureCube.MipLevels = mipCount;
                            srvDescription.TextureCube.MostDetailedMip = mipIndex;
                        }
                        else
                        {
                            // Else regular Texture2D
                            srvDescription.Dimension = this.Description.SampleDescription.Count > 1 ? ShaderResourceViewDimension.Texture2DMultisampledArray : ShaderResourceViewDimension.Texture2DArray;

                            // Multisample?
                            if (this.Description.SampleDescription.Count > 1)
                            {
                                srvDescription.Texture2DMSArray.ArraySize = arrayCount;
                                srvDescription.Texture2DMSArray.FirstArraySlice = arrayOrDepthSlice;
                            }
                            else
                            {
                                srvDescription.Texture2DArray.ArraySize = arrayCount;
                                srvDescription.Texture2DArray.FirstArraySlice = arrayOrDepthSlice;
                                srvDescription.Texture2DArray.MipLevels = mipCount;
                                srvDescription.Texture2DArray.MostDetailedMip = mipIndex;
                            }
                        }
                    }
                    else
                    {
                        srvDescription.Dimension = this.Description.SampleDescription.Count > 1 ? ShaderResourceViewDimension.Texture2DMultisampled : ShaderResourceViewDimension.Texture2D;
                        if (this.Description.SampleDescription.Count <= 1)
                        {
                            srvDescription.Texture2D.MipLevels = mipCount;
                            srvDescription.Texture2D.MostDetailedMip = mipIndex;
                        }
                    }

                    srv = new TextureView(this, new ShaderResourceView(this.GraphicsDevice, this.Resource, srvDescription));
                    this.shaderResourceViews.Add(textureViewKey, ToDispose(srv));
                }

                return srv;
            }
        }

        internal override UnorderedAccessView GetUnorderedAccessView(int arrayOrDepthSlice, int mipIndex)
        {
            if ((this.Description.BindFlags & BindFlags.UnorderedAccess) == 0)
                return null;

            int arrayCount = 1;

            // Use Full although we are binding to a single array/mimap slice, just to get the correct index
            var uavIndex = GetViewIndex(ViewType.Full, arrayOrDepthSlice, mipIndex);

            lock (this.unorderedAccessViews)
            {
                var uav = this.unorderedAccessViews[uavIndex];

                // Creates the unordered access view
                if (uav == null)
                {
                    var uavDescription = new UnorderedAccessViewDescription() {
                        Format = this.Description.Format,
                        Dimension = this.Description.ArraySize > 1 ? UnorderedAccessViewDimension.Texture2DArray : UnorderedAccessViewDimension.Texture2D
                    };

                    if (this.Description.ArraySize > 1)
                    {
                        uavDescription.Texture2DArray.ArraySize = arrayCount;
                        uavDescription.Texture2DArray.FirstArraySlice = arrayOrDepthSlice;
                        uavDescription.Texture2DArray.MipSlice = mipIndex;
                    }
                    else
                    {
                        uavDescription.Texture2D.MipSlice = mipIndex;
                    }

                    uav = new UnorderedAccessView(GraphicsDevice, Resource, uavDescription) { Tag = this };
                    this.unorderedAccessViews[uavIndex] = ToDispose(uav);
                }

                return uav;
            }
        }

        /// <summary>
        /// <see cref="SharpDX.DXGI.Surface"/> casting operator.
        /// </summary>
        /// <param name="from">From the Texture1D.</param>
        public static implicit operator SharpDX.DXGI.Surface(Texture2DBase from)
        {
            // Don't bother with multithreading here
            return from == null ? null : from.dxgiSurface ?? (from.dxgiSurface = from.ToDispose(from.Resource.QueryInterface<DXGI.Surface>()));
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeViews()
        {
            // Creates the shader resource view
            if ((this.Description.BindFlags & BindFlags.ShaderResource) != 0)
            {
                this.shaderResourceViews = new Dictionary<TextureViewKey, TextureView>();

                // Pre initialize by default the view on the first array/mipmap
                var viewFormat = GetDefaultViewFormat();
                if(!FormatHelper.IsTypeless(viewFormat))
                {
                    // Only valid for non-typeless viewformat
                    defaultShaderResourceView = GetShaderResourceView(viewFormat, ViewType.Full, 0, 0);
                }
            }

            // Creates the unordered access view
            if ((this.Description.BindFlags & BindFlags.UnorderedAccess) != 0)
            {
                // Initialize the unordered access views
                this.unorderedAccessViews = new UnorderedAccessView[GetViewCount()];

                // Pre initialize by default the view on the first array/mipmap
                GetUnorderedAccessView(0, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <param name="textureFlags"></param>
        /// <param name="mipCount"></param>
        /// <param name="arraySize"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        protected static Texture2DDescription NewDescription(int width, int height, PixelFormat format, TextureFlags textureFlags, int mipCount, int arraySize, ResourceUsage usage)
        {
            if ((textureFlags & TextureFlags.UnorderedAccess) != 0)
                usage = ResourceUsage.Default;

            var desc = new Texture2DDescription()
                           {
                               Width = width,
                               Height = height,
                               ArraySize = arraySize,
                               SampleDescription = new DXGI.SampleDescription(1, 0),
                               BindFlags = GetBindFlagsFromTextureFlags(textureFlags),
                               Format = format,
                               MipLevels = CalculateMipMapCount(mipCount, width, height),
                               Usage = usage,
                               CpuAccessFlags = GetCpuAccessFlagsFromUsage(usage),
                               OptionFlags = ResourceOptionFlags.None
                           };


            // If the texture is a RenderTarget + ShaderResource + MipLevels > 1, then allow for GenerateMipMaps method
            if ((desc.BindFlags & BindFlags.RenderTarget) != 0 && (desc.BindFlags & BindFlags.ShaderResource) != 0 && desc.MipLevels > 1)
            {
                desc.OptionFlags |= ResourceOptionFlags.GenerateMipMaps;
            }

            return desc;
        }
    }
}