/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;

using SharpDX.Direct3D11;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// A RenderTarget1D front end to <see cref="SharpDX.Direct3D11.Texture1D"/>.
    /// </summary>
    /// <remarks>
    /// This class instantiates a <see cref="Texture1D"/> with the binding flags <see cref="BindFlags.RenderTarget"/>.
    /// This class is also castable to <see cref="RenderTargetView"/>.
    /// </remarks>
    public class RenderTarget1D : Texture1DBase
    {
        internal RenderTarget1D(Device device, Texture1DDescription description1D)
            : base(device, description1D)
        {
        }

        internal RenderTarget1D(Device device, Direct3D11.Texture1D texture)
            : base(device, texture)
        {
        }

        /// <summary>
        /// RenderTargetView casting operator.
        /// </summary>
        /// <param name="from">Source for the.</param>
        public static implicit operator RenderTargetView(RenderTarget1D from)
        {
            return from == null ? null : from.renderTargetViews != null ? from.renderTargetViews[0] : null;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeViews()
        {
            // Perform default initialization
            base.InitializeViews();

            if ((this.Description.BindFlags & BindFlags.RenderTarget) != 0)
            {
                this.renderTargetViews = new TextureView[GetViewCount()];
                GetRenderTargetView(ViewType.Full, 0, 0);
            }
        }

        internal override TextureView GetRenderTargetView(ViewType viewType, int arrayOrDepthSlice, int mipIndex)
        {
            if ((this.Description.BindFlags & BindFlags.RenderTarget) == 0)
                return null;

            if (viewType == ViewType.MipBand)
                throw new NotSupportedException("ViewSlice.MipBand is not supported for render targets");

            int arrayCount;
            int mipCount;
            GetViewSliceBounds(viewType, ref arrayOrDepthSlice, ref mipIndex, out arrayCount, out mipCount);

            var rtvIndex = GetViewIndex(viewType, arrayOrDepthSlice, mipIndex);

            lock (this.renderTargetViews)
            {
                var rtv = this.renderTargetViews[rtvIndex];

                // Creates the shader resource view
                if (rtv == null)
                {
                    // Create the render target view
                    var rtvDescription = new RenderTargetViewDescription() { Format = this.Description.Format };

                    if (this.Description.ArraySize > 1)
                    {
                        rtvDescription.Dimension = RenderTargetViewDimension.Texture1DArray;
                        rtvDescription.Texture1DArray.ArraySize = arrayCount;
                        rtvDescription.Texture1DArray.FirstArraySlice = arrayOrDepthSlice;
                        rtvDescription.Texture1DArray.MipSlice = mipIndex;
                    }
                    else
                    {
                        rtvDescription.Dimension = RenderTargetViewDimension.Texture1D;
                        rtvDescription.Texture1D.MipSlice = mipIndex;
                    }

                    rtv = new TextureView(this, new RenderTargetView(GraphicsDevice, Resource, rtvDescription));
                    this.renderTargetViews[rtvIndex] = ToDispose(rtv);
                }

                return rtv;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Texture Clone()
        {
            return new RenderTarget1D(GraphicsDevice, this.Description);
        }

        /// <summary>
        /// Creates a new <see cref="RenderTarget1D"/> from a <see cref="Texture1DDescription"/>.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="description">The description.</param>
        /// <returns>
        /// A new instance of <see cref="RenderTarget1D"/> class.
        /// </returns>
        /// <msdn-id>ff476520</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture1D([In] const D3D11_TEXTURE1D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture1D** ppTexture1D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture1D</unmanaged-short>	
        public static RenderTarget1D New(Device device, Texture1DDescription description)
        {
            return new RenderTarget1D(device, description);
        }

        /// <summary>
        /// Creates a new <see cref="RenderTarget1D"/> from a <see cref="Direct3D11.Texture1D"/>.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="texture">The native texture <see cref="Direct3D11.Texture1D"/>.</param>
        /// <returns>
        /// A new instance of <see cref="RenderTarget1D"/> class.
        /// </returns>
        /// <msdn-id>ff476520</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture1D([In] const D3D11_TEXTURE1D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture1D** ppTexture1D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture1D</unmanaged-short>	
        public static RenderTarget1D New(Device device, Direct3D11.Texture1D texture)
        {
            return new RenderTarget1D(device, texture);
        }

        /// <summary>
        /// Creates a new <see cref="RenderTarget1D" /> with a single mipmap.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="width">The width.</param>
        /// <param name="format">Describes the format to use.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <param name="arraySize">Size of the texture 1D array, default to 1.</param>
        /// <returns>A new instance of <see cref="RenderTarget1D" /> class.</returns>
        /// <msdn-id>ff476520</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture1D([In] const D3D11_TEXTURE1D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture1D** ppTexture1D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture1D</unmanaged-short>	
        public static RenderTarget1D New(Device device, int width, PixelFormat format, TextureFlags flags = TextureFlags.RenderTarget | TextureFlags.ShaderResource, int arraySize = 1)
        {
            return New(device, width, false, format, flags | TextureFlags.RenderTarget, arraySize);
        }

        /// <summary>
        /// Creates a new <see cref="RenderTarget1D" />.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="width">The width.</param>
        /// <param name="mipCount">Number of mipmaps, set to true to have all mipmaps, set to an int >=1 for a particular mipmap count.</param>
        /// <param name="format">Describes the format to use.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <param name="arraySize">Size of the texture 1D array, default to 1.</param>
        /// <returns>A new instance of <see cref="RenderTarget1D" /> class.</returns>
        /// <msdn-id>ff476520</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture1D([In] const D3D11_TEXTURE1D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture1D** ppTexture1D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture1D</unmanaged-short>	
        public static RenderTarget1D New(Device device, int width, MipMapCount mipCount, PixelFormat format, TextureFlags flags = TextureFlags.RenderTarget | TextureFlags.ShaderResource, int arraySize = 1)
        {
            return new RenderTarget1D(device, NewRenderTargetDescription(width, format, flags | TextureFlags.RenderTarget, mipCount, arraySize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="format"></param>
        /// <param name="textureFlags"></param>
        /// <param name="mipCount"></param>
        /// <param name="arraySize"></param>
        /// <returns></returns>
        protected static Texture1DDescription NewRenderTargetDescription(int width, PixelFormat format, TextureFlags textureFlags, int mipCount, int arraySize)
        {
            var desc = Texture1DBase.NewDescription(width, format, textureFlags, mipCount, arraySize, ResourceUsage.Default);
            return desc;
        }
    }
}