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
    /// Abstract class front end to <see cref="SharpDX.Direct3D11.Texture1D"/>.
    /// </summary>
    public abstract class Texture1DBase : Texture
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly new Direct3D11.Texture1D Resource;
        private DXGI.Surface dxgiSurface;

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture1DBase" /> class.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="description1D">The description.</param>
        /// <msdn-id>ff476520</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture1D([In] const D3D11_TEXTURE1D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture1D** ppTexture1D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture1D</unmanaged-short>	
        protected internal Texture1DBase(Direct3D11.Device device, Texture1DDescription description1D)
            : base(device, description1D)
        {
            Resource = new Direct3D11.Texture1D(device, description1D);
            Initialize(Resource);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture1DBase" /> class.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="description1D">The description.</param>
        /// <param name="dataBox">A variable-length parameters list containing data rectangles.</param>
        /// <msdn-id>ff476520</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture1D([In] const D3D11_TEXTURE1D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture1D** ppTexture1D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture1D</unmanaged-short>	
        protected internal Texture1DBase(Direct3D11.Device device, Texture1DDescription description1D, DataBox[] dataBox)
            : base(device, description1D)
        {
            Resource = new Direct3D11.Texture1D(device, description1D, dataBox);
            Initialize(Resource);
        }

        /// <summary>
        /// Specialised constructor for use only by derived classes.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="texture">The texture.</param>
        /// <msdn-id>ff476520</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture1D([In] const D3D11_TEXTURE1D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture1D** ppTexture1D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture1D</unmanaged-short>	
        protected internal Texture1DBase(Direct3D11.Device device, Direct3D11.Texture1D texture)
            : base(device, texture.Description)
        {
            Resource = texture;
            Initialize(Resource);
        }

        /// <summary>
        /// Return an equivalent staging texture CPU read-writable from this instance.
        /// </summary>
        /// <returns></returns>
        public override Texture ToStaging()
        {
            return new Texture1D(this.GraphicsDevice, this.Description.ToStagingDescription());
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
                    var srvDescription = new ShaderResourceViewDescription() { Format = this.Description.Format };

                    // Initialize for texture arrays or texture cube
                    if (this.Description.ArraySize > 1)
                    {
                        // Else regular Texture1D
                        srvDescription.Dimension = ShaderResourceViewDimension.Texture1DArray;
                        srvDescription.Texture1DArray.ArraySize = arrayCount;
                        srvDescription.Texture1DArray.FirstArraySlice = arrayOrDepthSlice;
                        srvDescription.Texture1DArray.MipLevels = mipCount;
                        srvDescription.Texture1DArray.MostDetailedMip = mipIndex;
                    }
                    else
                    {
                        srvDescription.Dimension = ShaderResourceViewDimension.Texture1D;
                        srvDescription.Texture1D.MipLevels = mipCount;
                        srvDescription.Texture1D.MostDetailedMip = mipIndex;
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
                        Dimension = this.Description.ArraySize > 1 ? UnorderedAccessViewDimension.Texture1DArray : UnorderedAccessViewDimension.Texture1D
                    };

                    if (this.Description.ArraySize > 1)
                    {
                        uavDescription.Texture1DArray.ArraySize = arrayCount;
                        uavDescription.Texture1DArray.FirstArraySlice = arrayOrDepthSlice;
                        uavDescription.Texture1DArray.MipSlice = mipIndex;
                    }
                    else
                    {
                        uavDescription.Texture1D.MipSlice = mipIndex;
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
        public static implicit operator SharpDX.DXGI.Surface(Texture1DBase from)
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
                shaderResourceViews = new Dictionary<TextureViewKey, TextureView>();

                // Pre initialize by default the view on the first array/mipmap
                var viewFormat = Format;
                if (!FormatHelper.IsTypeless(viewFormat))
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
        /// <param name="format"></param>
        /// <param name="textureFlags"></param>
        /// <param name="mipCount"></param>
        /// <param name="arraySize"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        protected static Texture1DDescription NewDescription(int width, PixelFormat format, TextureFlags textureFlags, int mipCount, int arraySize, ResourceUsage usage)
        {
            if ((textureFlags & TextureFlags.UnorderedAccess) != 0)
                usage = ResourceUsage.Default;

            var desc = new Texture1DDescription()
                           {
                               Width = width,
                               ArraySize = arraySize,
                               BindFlags = GetBindFlagsFromTextureFlags(textureFlags),
                               Format = format,
                               MipLevels = CalculateMipMapCount(mipCount, width),
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