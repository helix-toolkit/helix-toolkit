/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Runtime.InteropServices;

using SharpDX.DXGI;
using SharpDX.Direct3D11;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// A Common description for all textures.
    /// </summary>
    /// <remarks>
    /// This class exposes the union of all fields exposed by fields in <see cref="Direct3D11.Texture1DDescription"/>, 
    /// <see cref="Direct3D11.Texture2DDescription"/>, <see cref="Direct3D11.Texture3DDescription"/>.
    /// It provides also 2-way implicit conversions for 1D, 2D, 3D textures descriptions.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct TextureDescription : IEquatable<TextureDescription>
    {
        /// <summary>
        /// The dimension of a texture.
        /// </summary>
        public TextureDimension Dimension;

        /// <summary>	
        /// <dd> <p>Texture width (in texels). The  range is from 1 to <see cref="SharpDX.Direct3D11.Resource.MaximumTexture1DSize"/> (16384). However, the range is actually constrained by the feature level at which you create the rendering device. For more information about restrictions, see Remarks.</p> </dd>	
        /// </summary>	
        /// <remarks>
        /// This field is valid for all textures: <see cref="Texture1D"/>, <see cref="Texture2D"/>, <see cref="Texture3D"/> and <see cref="TextureCube"/>.
        /// </remarks>
        /// <msdn-id>ff476252</msdn-id>	
        /// <unmanaged>unsigned int Width</unmanaged>	
        /// <unmanaged-short>unsigned int Width</unmanaged-short>	
        public int Width;

        /// <summary>	
        /// <dd> <p>Texture height (in texels). The  range is from 1 to <see cref="SharpDX.Direct3D11.Resource.MaximumTexture3DSize"/> (2048). However, the range is actually constrained by the feature level at which you create the rendering device. For more information about restrictions, see Remarks.</p> </dd>	
        /// </summary>	
        /// <remarks>
        /// This field is only valid for <see cref="Texture2D"/>, <see cref="Texture3D"/> and <see cref="TextureCube"/>.
        /// </remarks>
        /// <msdn-id>ff476254</msdn-id>	
        /// <unmanaged>unsigned int Height</unmanaged>	
        /// <unmanaged-short>unsigned int Height</unmanaged-short>	
        public int Height;

        /// <summary>	
        /// <dd> <p>Texture depth (in texels). The  range is from 1 to <see cref="SharpDX.Direct3D11.Resource.MaximumTexture3DSize"/> (2048). However, the range is actually constrained by the feature level at which you create the rendering device. For more information about restrictions, see Remarks.</p> </dd>	
        /// </summary>	
        /// <remarks>
        /// This field is only valid for <see cref="Texture3D"/>.
        /// </remarks>
        /// <msdn-id>ff476254</msdn-id>	
        /// <unmanaged>unsigned int Depth</unmanaged>	
        /// <unmanaged-short>unsigned int Depth</unmanaged-short>	
        public int Depth;

        /// <summary>	
        /// <dd> <p>Number of textures in the array. The  range is from 1 to <see cref="SharpDX.Direct3D11.Resource.MaximumTexture1DArraySize"/> (2048). However, the range is actually constrained by the feature level at which you create the rendering device. For more information about restrictions, see Remarks.</p> </dd>	
        /// </summary>	
        /// <remarks>
        /// This field is only valid for <see cref="Texture1D"/>, <see cref="Texture2D"/> and <see cref="TextureCube"/>
        /// </remarks>
        /// <remarks>
        /// This field is only valid for textures: <see cref="Texture1D"/>, <see cref="Texture2D"/> and <see cref="TextureCube"/>.
        /// </remarks>
        /// <msdn-id>ff476252</msdn-id>	
        /// <unmanaged>unsigned int ArraySize</unmanaged>	
        /// <unmanaged-short>unsigned int ArraySize</unmanaged-short>	
        public int ArraySize;

        /// <summary>	
        /// <dd> <p>The maximum number of mipmap levels in the texture. See the remarks in <strong><see cref="SharpDX.Direct3D11.ShaderResourceViewDescription.Texture1DResource"/></strong>. Use 1 for a multisampled texture; or 0 to generate a full set of subtextures.</p> </dd>	
        /// </summary>	
        /// <msdn-id>ff476252</msdn-id>	
        /// <unmanaged>unsigned int MipLevels</unmanaged>	
        /// <unmanaged-short>unsigned int MipLevels</unmanaged-short>	
        public int MipLevels;

        /// <summary>	
        /// <dd> <p>Texture format (see <strong><see cref="SharpDX.DXGI.Format"/></strong>).</p> </dd>	
        /// </summary>	
        /// <msdn-id>ff476252</msdn-id>	
        /// <unmanaged>DXGI_FORMAT Format</unmanaged>	
        /// <unmanaged-short>DXGI_FORMAT Format</unmanaged-short>	
        public DXGI.Format Format;

        /// <summary>	
        /// <dd> <p>Structure that specifies multisampling parameters for the texture. See <strong><see cref="SharpDX.DXGI.SampleDescription"/></strong>.</p> </dd>	
        /// </summary>	
        /// <remarks>
        /// This field is only valid for <see cref="Texture2D"/>.
        /// </remarks>
        /// <msdn-id>ff476253</msdn-id>	
        /// <unmanaged>DXGI_SAMPLE_DESC SampleDesc</unmanaged>	
        /// <unmanaged-short>DXGI_SAMPLE_DESC SampleDesc</unmanaged-short>	
        public SharpDX.DXGI.SampleDescription SampleDescription;

        /// <summary>	
        /// <dd> <p>Value that identifies how the texture is to be read from and written to. The most common value is <see cref="SharpDX.Direct3D11.ResourceUsage.Default"/>; see <strong><see cref="SharpDX.Direct3D11.ResourceUsage"/></strong> for all possible values.</p> </dd>	
        /// </summary>	
        /// <msdn-id>ff476252</msdn-id>	
        /// <unmanaged>D3D11_USAGE Usage</unmanaged>	
        /// <unmanaged-short>D3D11_USAGE Usage</unmanaged-short>	
        public SharpDX.Direct3D11.ResourceUsage Usage;

        /// <summary>	
        /// <dd> <p>Flags (see <strong><see cref="SharpDX.Direct3D11.BindFlags"/></strong>) for binding to pipeline stages. The flags can be combined by a logical OR. For a 1D texture, the allowable values are: <see cref="SharpDX.Direct3D11.BindFlags.ShaderResource"/>, <see cref="SharpDX.Direct3D11.BindFlags.RenderTarget"/> and <see cref="SharpDX.Direct3D11.BindFlags.DepthStencil"/>.</p> </dd>	
        /// </summary>	
        /// <msdn-id>ff476252</msdn-id>	
        /// <unmanaged>D3D11_BIND_FLAG BindFlags</unmanaged>	
        /// <unmanaged-short>D3D11_BIND_FLAG BindFlags</unmanaged-short>	
        public SharpDX.Direct3D11.BindFlags BindFlags;

        /// <summary>	
        /// <dd> <p>Flags (see <strong><see cref="SharpDX.Direct3D11.CpuAccessFlags"/></strong>) to specify the types of CPU access allowed. Use 0 if CPU access is not required. These flags can be combined with a logical OR.</p> </dd>	
        /// </summary>	
        /// <msdn-id>ff476252</msdn-id>	
        /// <unmanaged>D3D11_CPU_ACCESS_FLAG CPUAccessFlags</unmanaged>	
        /// <unmanaged-short>D3D11_CPU_ACCESS_FLAG CPUAccessFlags</unmanaged-short>	
        public SharpDX.Direct3D11.CpuAccessFlags CpuAccessFlags;

        /// <summary>	
        /// <dd> <p>Flags (see <strong><see cref="SharpDX.Direct3D11.ResourceOptionFlags"/></strong>) that identify other, less common resource options. Use 0 if none of these flags apply. These flags can be combined with a logical OR.</p> </dd>	
        /// </summary>	
        /// <msdn-id>ff476252</msdn-id>	
        /// <unmanaged>D3D11_RESOURCE_MISC_FLAG MiscFlags</unmanaged>	
        /// <unmanaged-short>D3D11_RESOURCE_MISC_FLAG MiscFlags</unmanaged-short>	
        public SharpDX.Direct3D11.ResourceOptionFlags OptionFlags;

        /// <summary>
        /// Gets the staging description for this instance..
        /// </summary>
        /// <returns>A Staging description</returns>
        public TextureDescription ToStagingDescription()
        {
            var copy = this;
            copy.BindFlags = BindFlags.None;
            copy.CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write;
            copy.Usage = ResourceUsage.Staging;
            copy.OptionFlags = copy.Dimension == TextureDimension.TextureCube ? ResourceOptionFlags.TextureCube : ResourceOptionFlags.None;
            return copy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TextureDescription other)
        {
            return Dimension.Equals(other.Dimension) && Width == other.Width && Height == other.Height && Depth == other.Depth && ArraySize == other.ArraySize && MipLevels == other.MipLevels && Format.Equals(other.Format) && SampleDescription.Equals(other.SampleDescription) && Usage.Equals(other.Usage) && BindFlags.Equals(other.BindFlags) && CpuAccessFlags.Equals(other.CpuAccessFlags) && OptionFlags.Equals(other.OptionFlags);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is TextureDescription && Equals((TextureDescription)obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Dimension.GetHashCode();
                hashCode = (hashCode * 397) ^ Width;
                hashCode = (hashCode * 397) ^ Height;
                hashCode = (hashCode * 397) ^ Depth;
                hashCode = (hashCode * 397) ^ ArraySize;
                hashCode = (hashCode * 397) ^ MipLevels;
                hashCode = (hashCode * 397) ^ Format.GetHashCode();
                hashCode = (hashCode * 397) ^ SampleDescription.GetHashCode();
                hashCode = (hashCode * 397) ^ Usage.GetHashCode();
                hashCode = (hashCode * 397) ^ BindFlags.GetHashCode();
                hashCode = (hashCode * 397) ^ CpuAccessFlags.GetHashCode();
                hashCode = (hashCode * 397) ^ OptionFlags.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(TextureDescription left, TextureDescription right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(TextureDescription left, TextureDescription right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Texture2DDescription"/> to <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="description">The texture description.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TextureDescription(Texture1DDescription description)
        {
            return new TextureDescription() {
                Dimension = TextureDimension.Texture1D,
                Width = description.Width,
                Height = 1,
                Depth = 1,
                MipLevels = description.MipLevels,
                ArraySize = description.ArraySize,
                Format = description.Format,
                SampleDescription = new SampleDescription(1, 0),
                Usage = description.Usage,
                BindFlags = description.BindFlags,
                CpuAccessFlags = description.CpuAccessFlags,
                OptionFlags = description.OptionFlags,
            };
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="TextureDescription"/> to <see cref="Texture2DDescription"/>.
        /// </summary>
        /// <param name="description">The texture description.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Texture1DDescription(TextureDescription description)
        {
            return new Texture1DDescription()
            {
                Width = description.Width,
                MipLevels = description.MipLevels,
                ArraySize = description.ArraySize,
                Format = description.Format,
                Usage = description.Usage,
                BindFlags = description.BindFlags,
                CpuAccessFlags = description.CpuAccessFlags,
                OptionFlags = description.OptionFlags,
            };
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Texture2DDescription"/> to <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="description">The texture description.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TextureDescription(Texture2DDescription description)
        {
            var dimension = (description.ArraySize == 6 && (description.OptionFlags & ResourceOptionFlags.TextureCube) != 0)
                                ? TextureDimension.TextureCube
                                : TextureDimension.Texture2D;

            return new TextureDescription()
            {
                Dimension = dimension,
                Width = description.Width,
                Height = description.Height,
                Depth = 1,
                MipLevels = description.MipLevels,
                ArraySize = description.ArraySize,
                Format = description.Format,
                SampleDescription = description.SampleDescription,
                Usage = description.Usage,
                BindFlags = description.BindFlags,
                CpuAccessFlags = description.CpuAccessFlags,
                OptionFlags = description.OptionFlags,
            };
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="TextureDescription"/> to <see cref="Texture2DDescription"/>.
        /// </summary>
        /// <param name="description">The texture description.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Texture2DDescription(TextureDescription description)
        {
            return new Texture2DDescription()
            {
                Width = description.Width,
                Height = description.Height,
                MipLevels = description.MipLevels,
                ArraySize = description.ArraySize,
                Format = description.Format,
                SampleDescription = description.SampleDescription,
                Usage = description.Usage,
                BindFlags = description.BindFlags,
                CpuAccessFlags = description.CpuAccessFlags,
                OptionFlags = description.OptionFlags,
            };
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Texture2DDescription"/> to <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="description">The texture description.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TextureDescription(Texture3DDescription description)
        {
            return new TextureDescription()
            {
                Dimension =  TextureDimension.Texture3D,
                Width = description.Width,
                Height = description.Height,
                Depth = description.Depth,
                ArraySize = 1,
                MipLevels = description.MipLevels,
                Format = description.Format,
                SampleDescription = new SampleDescription(1, 0),
                Usage = description.Usage,
                BindFlags = description.BindFlags,
                CpuAccessFlags = description.CpuAccessFlags,
                OptionFlags = description.OptionFlags,
            };
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="TextureDescription"/> to <see cref="Texture3DDescription"/>.
        /// </summary>
        /// <param name="description">The texture description.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Texture3DDescription(TextureDescription description)
        {
            return new Texture3DDescription()
            {
                Width = description.Width,
                Height = description.Height,
                Depth = description.Depth,
                MipLevels = description.MipLevels,
                Format = description.Format,
                Usage = description.Usage,
                BindFlags = description.BindFlags,
                CpuAccessFlags = description.CpuAccessFlags,
                OptionFlags = description.OptionFlags,
            };
        }


        /// <summary>
        /// Performs an explicit conversion from <see cref="ImageDescription"/> to <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="description">The image description.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TextureDescription(ImageDescription description)
        {
            return new TextureDescription()
            {
                Dimension = description.Dimension,
                Width = description.Width,
                Height = description.Height,
                Depth = description.Depth,
                ArraySize = description.ArraySize,
                MipLevels = description.MipLevels,
                Format = description.Format,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = description.Dimension == TextureDimension.TextureCube ? ResourceOptionFlags.TextureCube : ResourceOptionFlags.None,
            };
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="ImageDescription"/> to <see cref="TextureDescription"/>.
        /// </summary>
        /// <param name="description">The image description.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ImageDescription(TextureDescription description)
        {
            return new ImageDescription()
            {
                Dimension = description.Dimension,
                Width = description.Width,
                Height = description.Height,
                Depth = description.Depth,
                ArraySize = description.ArraySize,
                MipLevels = description.MipLevels,
                Format = description.Format,
            };
        }
    }
}