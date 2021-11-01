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

        protected override void Dispose(bool disposeManagedResources)
        {
            Resource?.Dispose();
            base.Dispose(disposeManagedResources);
        }
    }
}