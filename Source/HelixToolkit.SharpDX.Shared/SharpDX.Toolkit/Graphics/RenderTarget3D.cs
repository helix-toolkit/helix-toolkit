/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;

using SharpDX.Direct3D11;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// A RenderTarget3D front end to <see cref="SharpDX.Direct3D11.Texture3D"/>.
    /// </summary>
    /// <remarks>
    /// This class instantiates a <see cref="Texture3D"/> with the binding flags <see cref="BindFlags.RenderTarget"/>.
    /// This class is also castable to <see cref="RenderTargetView"/>.
    /// </remarks>
    public class RenderTarget3D : Texture3DBase
    {
        internal RenderTarget3D(Device device, Texture3DDescription description3D)
            : base(device, description3D)
        {
        }

        internal RenderTarget3D(Device device, Direct3D11.Texture3D texture)
            : base(device, texture)
        {
        }

        /// <summary>
        /// RenderTargetView casting operator.
        /// </summary>
        /// <param name="from">Source for the.</param>
        public static implicit operator RenderTargetView(RenderTarget3D from)
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
                    var rtvDescription = new RenderTargetViewDescription()
                                             {
                                                 Format = this.Description.Format,
                                                 Dimension = RenderTargetViewDimension.Texture3D,
                                                 Texture3D =
                                                 {
                                                     DepthSliceCount = arrayCount,
                                                     FirstDepthSlice = arrayOrDepthSlice,
                                                     MipSlice = mipIndex,
                                                 }
                                             };

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
            return new RenderTarget3D(GraphicsDevice, this.Description);
        }

        /// <summary>
        /// Creates a new <see cref="RenderTarget3D"/> from a <see cref="Texture3DDescription"/>.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="description">The description.</param>
        /// <returns>
        /// A new instance of <see cref="RenderTarget3D"/> class.
        /// </returns>
        /// <msdn-id>ff476521</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture3D([In] const D3D11_TEXTURE3D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture3D** ppTexture3D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture3D</unmanaged-short>	
        public static RenderTarget3D New(Device device, Texture3DDescription description)
        {
            return new RenderTarget3D(device, description);
        }

        /// <summary>
        /// Creates a new <see cref="RenderTarget3D"/> from a <see cref="Direct3D11.Texture3D"/>.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="texture">The native texture <see cref="Direct3D11.Texture3D"/>.</param>
        /// <returns>
        /// A new instance of <see cref="RenderTarget3D"/> class.
        /// </returns>
        /// <msdn-id>ff476521</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture3D([In] const D3D11_TEXTURE3D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture3D** ppTexture3D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture3D</unmanaged-short>	
        public static RenderTarget3D New(Device device, Direct3D11.Texture3D texture)
        {
            return new RenderTarget3D(device, texture);
        }

        /// <summary>
        /// Creates a new <see cref="RenderTarget3D" /> with a single mipmap.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="format">Describes the format to use.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <param name="arraySize">Size of the texture 3D array, default to 1.</param>
        /// <returns>A new instance of <see cref="RenderTarget3D" /> class.</returns>
        /// <msdn-id>ff476521</msdn-id>
        ///   <unmanaged>HRESULT ID3D11Device::CreateTexture3D([In] const D3D11_TEXTURE3D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture3D** ppTexture3D)</unmanaged>
        ///   <unmanaged-short>ID3D11Device::CreateTexture3D</unmanaged-short>
        public static RenderTarget3D New(Device device, int width, int height, int depth,  PixelFormat format, TextureFlags flags = TextureFlags.RenderTarget | TextureFlags.ShaderResource, int arraySize = 1)
        {
            return New(device, width, height, depth, false, format, flags | TextureFlags.RenderTarget, arraySize);
        }

        /// <summary>
        /// Creates a new <see cref="RenderTarget3D" />.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="mipCount">Number of mipmaps, set to true to have all mipmaps, set to an int >=1 for a particular mipmap count.</param>
        /// <param name="format">Describes the format to use.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <param name="arraySize">Size of the texture 3D array, default to 1.</param>
        /// <returns>A new instance of <see cref="RenderTarget3D" /> class.</returns>
        /// <msdn-id>ff476521</msdn-id>
        ///   <unmanaged>HRESULT ID3D11Device::CreateTexture3D([In] const D3D11_TEXTURE3D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture3D** ppTexture3D)</unmanaged>
        ///   <unmanaged-short>ID3D11Device::CreateTexture3D</unmanaged-short>
        public static RenderTarget3D New(Device device, int width, int height, int depth, MipMapCount mipCount, PixelFormat format, TextureFlags flags = TextureFlags.RenderTarget | TextureFlags.ShaderResource, int arraySize = 1)
        {
            return new RenderTarget3D(device, NewRenderTargetDescription(width, height, depth, format, flags | TextureFlags.RenderTarget, mipCount));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <param name="format"></param>
        /// <param name="textureFlags"></param>
        /// <param name="mipCount"></param>
        /// <returns></returns>
        protected static Texture3DDescription NewRenderTargetDescription(int width, int height, int depth, PixelFormat format, TextureFlags textureFlags,int mipCount)
        {
            var desc = Texture3DBase.NewDescription(width, height, depth, format, textureFlags, mipCount, ResourceUsage.Default);
            return desc;
        }
    }
}