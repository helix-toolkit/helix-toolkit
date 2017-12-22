/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.IO;
using SharpDX.Direct3D11;
using SharpDX.IO;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// A TextureCube front end to <see cref="SharpDX.Direct3D11.Texture2D"/>.
    /// </summary>
    public class TextureCube : Texture2DBase
    {
        internal TextureCube(Device device, Texture2DDescription description2D, params DataBox[] dataBoxes) : base(device, description2D, dataBoxes)
        {
            Initialize(Resource);
        }

        internal TextureCube(Device device, Direct3D11.Texture2D texture) : base(device, texture)
        {
            Initialize(Resource);
        }

        internal override TextureView GetRenderTargetView(ViewType viewType, int arrayOrDepthSlice, int mipMapSlice)
        {
            throw new System.NotSupportedException();
        }

        /// <summary>
        /// Makes a copy of this texture.
        /// </summary>
        /// <remarks>
        /// This method doesn't copy the content of the texture.
        /// </remarks>
        /// <returns>
        /// A copy of this texture.
        /// </returns>
        public override Texture Clone()
        {
            return new TextureCube(GraphicsDevice, this.Description);
        }

        /// <summary>
        /// Creates a new texture from a <see cref="Texture2DDescription"/>.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="description">The description.</param>
        /// <returns>
        /// A new instance of <see cref="TextureCube"/> class.
        /// </returns>
        /// <msdn-id>ff476521</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>	
        public static TextureCube New(Device device, Texture2DDescription description)
        {
            return new TextureCube(device, description);
        }

        /// <summary>
        /// Creates a new texture from a <see cref="Direct3D11.Texture2D"/>.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="texture">The native texture <see cref="Direct3D11.Texture2D"/>.</param>
        /// <returns>
        /// A new instance of <see cref="TextureCube"/> class.
        /// </returns>
        /// <msdn-id>ff476521</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>	
        public static TextureCube New(Device device, Direct3D11.Texture2D texture)
        {
            return new TextureCube(device, texture);
        }

        /// <summary>
        /// Creates a new <see cref="TextureCube"/>.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="size">The size (in pixels) of the top-level faces of the cube texture.</param>
        /// <param name="format">Describes the format to use.</param>
        /// <param name="usage">The usage.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <returns>
        /// A new instance of <see cref="Texture2D"/> class.
        /// </returns>
        /// <msdn-id>ff476521</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>	
        public static TextureCube New(Device device, int size, PixelFormat format, TextureFlags flags = TextureFlags.ShaderResource, ResourceUsage usage = ResourceUsage.Default)
        {
            return New(device, size, false, format, flags, usage);
        }

        /// <summary>
        /// Creates a new <see cref="TextureCube"/>.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="size">The size (in pixels) of the top-level faces of the cube texture.</param>
        /// <param name="mipCount">Number of mipmaps, set to true to have all mipmaps, set to an int >=1 for a particular mipmap count.</param>
        /// <param name="format">Describes the format to use.</param>
        /// <param name="usage">The usage.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <returns>
        /// A new instance of <see cref="Texture2D"/> class.
        /// </returns>
        /// <msdn-id>ff476521</msdn-id>	
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>	
        /// <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>	
        public static TextureCube New(Device device, int size, MipMapCount mipCount, PixelFormat format, TextureFlags flags = TextureFlags.ShaderResource, ResourceUsage usage = ResourceUsage.Default)
        {
            return new TextureCube(device, NewTextureCubeDescription(size, format, flags | TextureFlags.ShaderResource, mipCount, usage));
        }

        /// <summary>
        /// Creates a new <see cref="TextureCube" /> from a initial data..
        /// </summary>
        /// <typeparam name="T">Type of a pixel data</typeparam>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="size">The size (in pixels) of the top-level faces of the cube texture.</param>
        /// <param name="format">Describes the format to use.</param>
        /// <param name="usage">The usage.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <param name="textureData">an array of 6 textures. See remarks</param>
        /// <returns>A new instance of <see cref="TextureCube" /> class.</returns>
        /// <msdn-id>ff476521</msdn-id>
        ///   <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>
        ///   <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>
        /// <remarks>
        /// The first dimension of mipMapTextures describes the number of array (TextureCube Array), the second is the texture data for a particular cube face.
        /// </remarks>
        public unsafe static TextureCube New<T>(Device device, int size, PixelFormat format, T[][] textureData, TextureFlags flags = TextureFlags.ShaderResource, ResourceUsage usage = ResourceUsage.Immutable) where T : struct
        {
            if (textureData.Length != 6)
                throw new ArgumentException("Invalid texture data. First dimension must be equal to 6", "textureData");

            DataBox dataBox1 = new DataBox();
            DataBox dataBox2 = new DataBox();
            DataBox dataBox3 = new DataBox();
            DataBox dataBox4 = new DataBox();
            DataBox dataBox5 = new DataBox();
            DataBox dataBox6 = new DataBox();

            Utilities.Pin(textureData[0], ptr => dataBox1 = GetDataBox(format, size, size, 1, textureData[0], ptr));
            Utilities.Pin(textureData[1], ptr => dataBox2 = GetDataBox(format, size, size, 1, textureData[0], ptr));
            Utilities.Pin(textureData[2], ptr => dataBox3 = GetDataBox(format, size, size, 1, textureData[0], ptr));
            Utilities.Pin(textureData[3], ptr => dataBox4 = GetDataBox(format, size, size, 1, textureData[0], ptr));
            Utilities.Pin(textureData[4], ptr => dataBox5 = GetDataBox(format, size, size, 1, textureData[0], ptr));
            Utilities.Pin(textureData[5], ptr => dataBox6 = GetDataBox(format, size, size, 1, textureData[0], ptr));

            return new TextureCube(device, NewTextureCubeDescription(size, format, flags | TextureFlags.ShaderResource, 1, usage), dataBox1, dataBox2, dataBox3, dataBox4, dataBox5, dataBox6);
        }

        /// <summary>
        /// Creates a new <see cref="TextureCube" /> from a initial data..
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="size">The size (in pixels) of the top-level faces of the cube texture.</param>
        /// <param name="format">Describes the format to use.</param>
        /// <param name="usage">The usage.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <param name="textureData">an array of 6 textures. See remarks</param>
        /// <returns>A new instance of <see cref="TextureCube" /> class.</returns>
        /// <msdn-id>ff476521</msdn-id>
        ///   <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>
        ///   <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>
        /// <remarks>
        /// The first dimension of mipMapTextures describes the number of array (TextureCube Array), the second is the texture data for a particular cube face.
        /// </remarks>
        public static TextureCube New(Device device, int size, PixelFormat format, DataBox[] textureData, TextureFlags flags = TextureFlags.ShaderResource, ResourceUsage usage = ResourceUsage.Immutable)
        {
            if (textureData.Length != 6)
                throw new ArgumentException("Invalid texture data. First dimension must be equal to 6", "textureData");

            return new TextureCube(device, NewTextureCubeDescription(size, format, flags | TextureFlags.ShaderResource, 1, usage), textureData);
        }

        /// <summary>
        /// Creates a new <see cref="TextureCube" /> directly from an <see cref="Image"/>.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="image">An image in CPU memory.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <param name="usage">The usage.</param>
        /// <returns>A new instance of <see cref="TextureCube" /> class.</returns>
        /// <msdn-id>ff476521</msdn-id>
        /// <unmanaged>HRESULT ID3D11Device::CreateTexture2D([In] const D3D11_TEXTURE2D_DESC* pDesc,[In, Buffer, Optional] const D3D11_SUBRESOURCE_DATA* pInitialData,[Out, Fast] ID3D11Texture2D** ppTexture2D)</unmanaged>
        /// <unmanaged-short>ID3D11Device::CreateTexture2D</unmanaged-short>
        public static TextureCube New(Device device, Image image, TextureFlags flags = TextureFlags.ShaderResource, ResourceUsage usage = ResourceUsage.Immutable)
        {
            if (image == null) throw new ArgumentNullException("image");
            if (image.Description.Dimension != TextureDimension.TextureCube)
                throw new ArgumentException("Invalid image. Must be Cube", "image");

            return new TextureCube(device, CreateTextureDescriptionFromImage(image, flags | TextureFlags.ShaderResource, usage), image.ToDataBox());
        }

        /// <summary>
        /// Loads a Cube texture from a stream.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="stream">The stream to load the texture from.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <param name="usage">Usage of the resource. Default is <see cref="ResourceUsage.Immutable"/> </param>
        /// <exception cref="ArgumentException">If the texture is not of type Cube</exception>
        /// <returns>A texture</returns>
        public static new TextureCube Load(Device device, Stream stream, TextureFlags flags = TextureFlags.ShaderResource, ResourceUsage usage = ResourceUsage.Immutable)
        {
            var texture = Texture.Load(device, stream, flags | TextureFlags.ShaderResource, usage);
            if (!(texture is TextureCube))
                throw new ArgumentException(string.Format("Texture is not type of [TextureCube] but [{0}]", texture.GetType().Name));
            return (TextureCube)texture;
        }

        /// <summary>
        /// Loads a Cube texture from a stream.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="filePath">The file to load the texture from.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <param name="usage">Usage of the resource. Default is <see cref="ResourceUsage.Immutable"/> </param>
        /// <exception cref="ArgumentException">If the texture is not of type Cube</exception>
        /// <returns>A texture</returns>
        public static new TextureCube Load(Device device, string filePath, TextureFlags flags = TextureFlags.ShaderResource, ResourceUsage usage = ResourceUsage.Immutable)
        {
            using (var stream = new NativeFileStream(filePath, NativeFileMode.Open, NativeFileAccess.Read))
                return Load(device, stream, flags | TextureFlags.ShaderResource, usage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="format"></param>
        /// <param name="flags"></param>
        /// <param name="mipCount"></param>
        /// <param name="usage"></param>
        /// <returns></returns>
        protected static Texture2DDescription NewTextureCubeDescription(int size, PixelFormat format, TextureFlags flags, int mipCount, ResourceUsage usage)
        {
            var desc = NewDescription(size, size, format, flags, mipCount, 6, usage);
            desc.OptionFlags = ResourceOptionFlags.TextureCube;
            return desc;
        }
    }
}