/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.IO;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.IO;
using DeviceChild = SharpDX.Direct3D11.DeviceChild;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// Base class for texture resources.
    /// </summary>
    public abstract class Texture : GraphicsResource, IComparable<Texture>
    {
        private long textureId;

        /// <summary>
        /// Common description for this texture.
        /// </summary>
        public readonly TextureDescription Description;

        /// <summary>
        /// Gets a boolean indicating whether this <see cref="Texture"/> is a using a block compress format (BC1, BC2, BC3, BC4, BC5, BC6H, BC7).
        /// </summary>
        public readonly bool IsBlockCompressed;

        /// <summary>
        /// The width stride in bytes (number of bytes per row).
        /// </summary>
        internal readonly int RowStride;

        /// <summary>
        /// The depth stride in bytes (number of bytes per depth slice).
        /// </summary>
        internal readonly int DepthStride;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="description"></param>
        protected Texture(Direct3D11.Device device, TextureDescription description) : base(device)
        {
            Description = description;
            IsBlockCompressed = FormatHelper.IsCompressed(description.Format);
            RowStride = this.Description.Width * ((PixelFormat)this.Description.Format).SizeInBytes;
            DepthStride = RowStride * this.Description.Height;
        }

        /// <summary>	
        /// <dd> <p>Texture width (in texels). The  range is from 1 to <see cref="SharpDX.Direct3D11.Resource.MaximumTexture1DSize"/> (16384). However, the range is actually constrained by the feature level at which you create the rendering device. For more information about restrictions, see Remarks.</p> </dd>	
        /// </summary>	
        /// <remarks>
        /// This field is valid for all textures: <see cref="Texture1D"/>, <see cref="Texture2D"/>, <see cref="Texture3D"/> and <see cref="TextureCube"/>.
        /// </remarks>
        public int Width
        {
            get
            {
                return Description.Width;
            }
        }

        /// <summary>	
        /// <dd> <p>Texture height (in texels). The  range is from 1 to <see cref="SharpDX.Direct3D11.Resource.MaximumTexture3DSize"/> (2048). However, the range is actually constrained by the feature level at which you create the rendering device. For more information about restrictions, see Remarks.</p> </dd>	
        /// </summary>	
        /// <remarks>
        /// This field is only valid for <see cref="Texture2D"/>, <see cref="Texture3D"/> and <see cref="TextureCube"/>.
        /// </remarks>
        public int Height
        {
            get
            {
                return Description.Height;
            }
        }

        /// <summary>	
        /// <dd> <p>Texture depth (in texels). The  range is from 1 to <see cref="SharpDX.Direct3D11.Resource.MaximumTexture3DSize"/> (2048). However, the range is actually constrained by the feature level at which you create the rendering device. For more information about restrictions, see Remarks.</p> </dd>	
        /// </summary>	
        /// <remarks>
        /// This field is only valid for <see cref="Texture3D"/>.
        /// </remarks>
        public int Depth
        {
            get
            {
                return Description.Depth;
            }
        }

        /// <summary>
        /// Gets the texture format.
        /// </summary>
        /// <value>The texture format.</value>
        public PixelFormat Format
        {
            get
            {
                return Description.Format;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resource"></param>
        protected override void Initialize(DeviceChild resource)
        {
            // Be sure that we are storing only the main device (which contains the immediate context).
            base.Initialize(resource);
            // Gets a Texture ID
            textureId = resource.NativePointer.ToInt64();
        }

        /// <summary>
        /// Calculates the number of miplevels for a Texture 1D.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="mipLevels">A <see cref="MipMapCount"/>, set to true to calculates all mipmaps, to false to calculate only 1 miplevel, or > 1 to calculate a specific amount of levels.</param>
        /// <returns>The number of miplevels.</returns>
        public static int CalculateMipLevels(int width, MipMapCount mipLevels)
        {
            if (mipLevels > 1)
            {
                var maxMips = CountMips(width);
                if (mipLevels > maxMips)
                    throw new InvalidOperationException(String.Format("MipLevels must be <= {0}", maxMips));
            }
            else if (mipLevels == 0)
            {
                mipLevels = CountMips(width);
            }
            else
            {
                mipLevels = 1;
            }
            return mipLevels;
        }

        /// <summary>
        /// Calculates the number of miplevels for a Texture 2D.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="mipLevels">A <see cref="MipMapCount"/>, set to true to calculates all mipmaps, to false to calculate only 1 miplevel, or > 1 to calculate a specific amount of levels.</param>
        /// <returns>The number of miplevels.</returns>
        public static int CalculateMipLevels(int width, int height, MipMapCount mipLevels)
        {
            if (mipLevels > 1)
            {
                var maxMips = CountMips(width, height);
                if (mipLevels > maxMips)
                    throw new InvalidOperationException(String.Format("MipLevels must be <= {0}", maxMips));
            }
            else if (mipLevels == 0)
            {
                mipLevels = CountMips(width, height);
            }
            else
            {
                mipLevels = 1;
            }
            return mipLevels;
        }

        /// <summary>
        /// Calculates the number of miplevels for a Texture 2D.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="depth">The depth of the texture.</param>
        /// <param name="mipLevels">A <see cref="MipMapCount"/>, set to true to calculates all mipmaps, to false to calculate only 1 miplevel, or > 1 to calculate a specific amount of levels.</param>
        /// <returns>The number of miplevels.</returns>
        public static int CalculateMipLevels(int width, int height, int depth, MipMapCount mipLevels)
        {
            if (mipLevels > 1)
            {
                if (!IsPow2(width) || !IsPow2(height) || !IsPow2(depth))
                    throw new InvalidOperationException("Width/Height/Depth must be power of 2");

                var maxMips = CountMips(width, height, depth);
                if (mipLevels > maxMips)
                    throw new InvalidOperationException(String.Format("MipLevels must be <= {0}", maxMips));
            }
            else if (mipLevels == 0)
            {
                if (!IsPow2(width) || !IsPow2(height) || !IsPow2(depth))
                    throw new InvalidOperationException("Width/Height/Depth must be power of 2");

                mipLevels = CountMips(width, height, depth);
            }
            else
            {
                mipLevels = 1;
            }
            return mipLevels;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="mipLevel"></param>
        /// <returns></returns>
        public static int CalculateMipSize(int width, int mipLevel)
        {
            mipLevel = Math.Min(mipLevel, CountMips(width));
            width = width >> mipLevel;
            return width > 0 ? width : 1;
        }

        /// <summary>
        /// Gets the absolute sub-resource index from the array and mip slice.
        /// </summary>
        /// <param name="arraySlice">The array slice index.</param>
        /// <param name="mipSlice">The mip slice index.</param>
        /// <returns>A value equals to arraySlice * Description.MipLevels + mipSlice.</returns>
        public int GetSubResourceIndex(int arraySlice, int mipSlice)
        {
            return arraySlice * Description.MipLevels + mipSlice;
        }

        /// <summary>
        /// Calculates the expected width of a texture using a specified type.
        /// </summary>
        /// <typeparam name="TData">The type of the T pixel data.</typeparam>
        /// <returns>The expected width</returns>
        /// <exception cref="System.ArgumentException">If the size is invalid</exception>
        public int CalculateWidth<TData>(int mipLevel = 0) where TData : struct
        {
            var widthOnMip = CalculateMipSize((int)Description.Width, mipLevel);
            var rowStride = widthOnMip * ((PixelFormat)Description.Format).SizeInBytes;

            var dataStrideInBytes = Utilities.SizeOf<TData>() * widthOnMip;
            var width = ((double)rowStride / dataStrideInBytes) * widthOnMip;
            if (Math.Abs(width - (int)width) > Double.Epsilon)
                throw new ArgumentException("sizeof(TData) / sizeof(Format) * Width is not an integer");

            return (int)width;
        }

        /// <summary>
        /// Calculates the number of pixel data this texture is requiring for a particular mip level.
        /// </summary>
        /// <typeparam name="TData">The type of the T pixel data.</typeparam>
        /// <param name="mipLevel">The mip level.</param>
        /// <returns>The number of pixel data.</returns>
        /// <remarks>This method is used to allocated a texture data buffer to hold pixel data: var textureData = new T[ texture.CalculatePixelCount&lt;T&gt;() ] ;.</remarks>
        public int CalculatePixelDataCount<TData>(int mipLevel = 0) where TData : struct
        {
            return CalculateWidth<TData>(mipLevel) * CalculateMipSize(Description.Height, mipLevel) * CalculateMipSize(Description.Depth, mipLevel);
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
        public abstract Texture Clone();

        /// <summary>
        /// Makes a copy of this texture with type casting.
        /// </summary>
        /// <remarks>
        /// This method doesn't copy the content of the texture.
        /// </remarks>
        /// <returns>
        /// A copy of this texture.
        /// </returns>
        public T Clone<T>() where T : Texture
        {
            return (T)this.Clone();
        }

        /// <summary>
        /// Creates a new texture with the specified generic texture description.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="description">The description.</param>
        /// <returns>A Texture instance, either a RenderTarget or DepthStencilBuffer or Texture, depending on Binding flags.</returns>
        public static Texture New(Direct3D11.Device graphicsDevice, TextureDescription description)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentNullException("graphicsDevice");
            }

            if ((description.BindFlags & BindFlags.RenderTarget) != 0)
            {
                throw new NotSupportedException("RenderTarget is not supported.");
            }
            else if ((description.BindFlags & BindFlags.DepthStencil) != 0)
            {
                throw new NotSupportedException("DepthStencil is not supported.");
            }
            else
            {
                switch (description.Dimension)
                {
                    case TextureDimension.Texture1D:
                        return Texture1D.New(graphicsDevice, description);
                    case TextureDimension.Texture2D:
                        return Texture2D.New(graphicsDevice, description);
                    case TextureDimension.Texture3D:
                        return Texture3D.New(graphicsDevice, description);
                    case TextureDimension.TextureCube:
                        return TextureCube.New(graphicsDevice, description);
                }
            }

            return null;
        }

        /// <summary>
        /// Loads a texture from a stream.
        /// </summary>
        /// <param name="device">The <see cref="Direct3D11.Device"/>.</param>
        /// <param name="stream">The stream to load the texture from.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <param name="usage">Usage of the resource. Default is <see cref="ResourceUsage.Immutable"/> </param>
        /// <returns>A texture</returns>
        public static Texture Load(Direct3D11.Device device, Stream stream, TextureFlags flags = TextureFlags.ShaderResource, ResourceUsage usage = ResourceUsage.Immutable)
        {
            stream.Position = 0;
            var image = Image.Load(stream);
            if (image == null)
            {
                stream.Position = 0;
                return null;
            }

            try
            {
                switch (image.Description.Dimension)
                {
                    case TextureDimension.Texture1D:
                        return Texture1D.New(device, image, flags, usage);
                    case TextureDimension.Texture2D:
                        return Texture2D.New(device, image, flags, usage);
                    case TextureDimension.Texture3D:
                        return Texture3D.New(device, image, flags, usage);
                    case TextureDimension.TextureCube:
                        return TextureCube.New(device, image, flags, usage);
                }
            }
            finally
            {
                image.Dispose();
                stream.Position = 0;
            }
            throw new InvalidOperationException("Dimension not supported");
        }

        /// <summary>
        /// Loads a texture from a file.
        /// </summary>
        /// <param name="device">Specify the device used to load and create a texture from a file.</param>
        /// <param name="filePath">The file to load the texture from.</param>
        /// <param name="flags">Sets the texture flags (for unordered access...etc.)</param>
        /// <param name="usage">Usage of the resource. Default is <see cref="ResourceUsage.Immutable"/> </param>
        /// <returns>A texture</returns>
        public static Texture Load(Direct3D11.Device device, string filePath, TextureFlags flags = TextureFlags.ShaderResource, ResourceUsage usage = ResourceUsage.Immutable)
        {
            using (var stream = new NativeFileStream(filePath, NativeFileMode.Open, NativeFileAccess.Read))
                return Load(device, stream, flags, usage);
        }

        /// <summary>
        /// Calculates the mip map count from a requested level.
        /// </summary>
        /// <param name="requestedLevel">The requested level.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <returns>The resulting mipmap count (clamp to [1, maxMipMapCount] for this texture)</returns>
        internal static int CalculateMipMapCount(MipMapCount requestedLevel, int width, int height = 0, int depth = 0)
        {
            var size = Math.Max(Math.Max(width, height), depth);
            var maxMipMap = 1 + (int)Math.Log(size, 2);

            return requestedLevel == 0 ? maxMipMap : Math.Min(requestedLevel, maxMipMap);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="format"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <param name="textureData"></param>
        /// <param name="fixedPointer"></param>
        /// <returns></returns>
        internal static DataBox GetDataBox<T>(Format format, int width, int height, int depth, T[] textureData, IntPtr fixedPointer) where T : unmanaged
        {
            // Check that the textureData size is correct
            if (textureData == null)
                throw new ArgumentNullException("textureData");
            int rowPitch;
            int slicePitch;
            int widthCount;
            int heightCount;
            Image.ComputePitch(format, width, height, out rowPitch, out slicePitch, out widthCount, out heightCount);
            if (Utilities.SizeOf(textureData) != (slicePitch * depth))
                throw new ArgumentException("Invalid size for TextureData");

            return new DataBox(fixedPointer, rowPitch, slicePitch);
        }

        internal static TextureDescription CreateTextureDescriptionFromImage(Image image, TextureFlags flags, ResourceUsage usage)
        {
            var desc = (TextureDescription)image.Description;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.Usage = usage;
            if ((flags & TextureFlags.UnorderedAccess) != 0)
                desc.Usage = ResourceUsage.Default;

            desc.BindFlags = GetBindFlagsFromTextureFlags(flags);

            desc.CpuAccessFlags = GetCpuAccessFlagsFromUsage(usage);
            return desc;
        }

        internal void GetViewSliceBounds(ViewType viewType, ref int arrayOrDepthIndex, ref int mipIndex, out int arrayOrDepthCount, out int mipCount)
        {
            var arrayOrDepthSize = this.Description.Depth > 1 ? this.Description.Depth : this.Description.ArraySize;

            switch (viewType)
            {
                case ViewType.Full:
                    arrayOrDepthIndex = 0;
                    mipIndex = 0;
                    arrayOrDepthCount = arrayOrDepthSize;
                    mipCount = this.Description.MipLevels;
                    break;
                case ViewType.Single:
                    arrayOrDepthCount = 1;
                    mipCount = 1;
                    break;
                case ViewType.MipBand:
                    arrayOrDepthCount = arrayOrDepthSize - arrayOrDepthIndex;
                    mipCount = 1;
                    break;
                case ViewType.ArrayBand:
                    arrayOrDepthCount = 1;
                    mipCount = Description.MipLevels - mipIndex;
                    break;
                default:
                    arrayOrDepthCount = 0;
                    mipCount = 0;
                    break;
            }
        }

        internal int GetViewCount()
        {
            var arrayOrDepthSize = this.Description.Depth > 1 ? this.Description.Depth : this.Description.ArraySize;
            return GetViewIndex((ViewType)4, arrayOrDepthSize, this.Description.MipLevels);
        }

        internal int GetViewIndex(ViewType viewType, int arrayOrDepthIndex, int mipIndex)
        {
            var arrayOrDepthSize = this.Description.Depth > 1 ? this.Description.Depth : this.Description.ArraySize;
            return (((int)viewType) * arrayOrDepthSize + arrayOrDepthIndex) * this.Description.MipLevels + mipIndex;
        }

        private static bool IsPow2(int x)
        {
            return ((x != 0) && (x & (x - 1)) == 0);
        }

        private static int CountMips(int width)
        {
            var mipLevels = 1;

            while (width > 1)
            {
                ++mipLevels;

                if (width > 1)
                    width >>= 1;
            }

            return mipLevels;
        }

        private static int CountMips(int width, int height)
        {
            var mipLevels = 1;

            while (height > 1 || width > 1)
            {
                ++mipLevels;

                if (height > 1)
                    height >>= 1;

                if (width > 1)
                    width >>= 1;
            }

            return mipLevels;
        }

        private static int CountMips(int width, int height, int depth)
        {
            var mipLevels = 1;

            while (height > 1 || width > 1 || depth > 1)
            {
                ++mipLevels;

                if (height > 1)
                    height >>= 1;

                if (width > 1)
                    width >>= 1;

                if (depth > 1)
                    depth >>= 1;
            }

            return mipLevels;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(Texture obj)
        {
            return textureId.CompareTo(obj.textureId);
        }

        internal static BindFlags GetBindFlagsFromTextureFlags(TextureFlags flags)
        {
            var bindFlags = BindFlags.None;
            if ((flags & TextureFlags.ShaderResource) != 0)
                bindFlags |= BindFlags.ShaderResource;

            if ((flags & TextureFlags.UnorderedAccess) != 0)
                bindFlags |= BindFlags.UnorderedAccess;

            if ((flags & TextureFlags.RenderTarget) != 0)
                bindFlags |= BindFlags.RenderTarget;

            return bindFlags;
        }

        internal struct TextureViewKey : IEquatable<TextureViewKey>
        {
            public TextureViewKey(Format viewFormat, ViewType viewType, int arrayOrDepthSlice, int mipIndex)
            {
                ViewFormat = viewFormat;
                ViewType = viewType;
                ArrayOrDepthSlice = arrayOrDepthSlice;
                MipIndex = mipIndex;
            }

            public readonly DXGI.Format ViewFormat;

            public readonly ViewType ViewType;

            public readonly int ArrayOrDepthSlice;

            public readonly int MipIndex;

            public bool Equals(TextureViewKey other)
            {
                return ViewFormat == other.ViewFormat && ViewType == other.ViewType && ArrayOrDepthSlice == other.ArrayOrDepthSlice && MipIndex == other.MipIndex;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                return obj is TextureViewKey && Equals((TextureViewKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (int)ViewFormat;
                    hashCode = (hashCode * 397) ^ (int)ViewType;
                    hashCode = (hashCode * 397) ^ ArrayOrDepthSlice;
                    hashCode = (hashCode * 397) ^ MipIndex;
                    return hashCode;
                }
            }

            public static bool operator ==(TextureViewKey left, TextureViewKey right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(TextureViewKey left, TextureViewKey right)
            {
                return !left.Equals(right);
            }
        }
    }
}