/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.DXGI;
using System;
using System.IO;

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
    using Utilities;
    /// <summary>
    /// Texture model contains <see cref="TextureInfoLoader"/> and a Guid to identify specific texture.
    /// <see cref="TextureInfoLoader"/> is being called to load texture during GPU texture resource creation.
    /// <para>
    /// Helixtoolkit provides basic implementation for texture info loaders.
    /// </para>
    /// <para>
    /// User can provide own implementation for <see cref="TextureInfoLoader"/> for better texture resource/data management.
    /// </para>
    /// </summary>
    [System.ComponentModel.TypeConverter(typeof(StreamToTextureModelConverter))]
    public sealed class TextureModel
    {
        internal sealed class NullLoader : ITextureInfoLoader
        {
            public static readonly NullLoader Null = new NullLoader();

            public void Complete(Guid id, TextureInfo info, bool succeeded)
            {

            }

            public TextureInfo Load(Guid id)
            {
                return TextureInfo.Null;
            }
        }

        private sealed class StreamLoader : ITextureInfoLoader
        {
            private readonly TextureInfo info;
            private readonly bool autoClose;

            public StreamLoader(Stream stream, bool autoClose)
            {
                info = new TextureInfo(stream);
                this.autoClose = autoClose;
            }

            public void Complete(Guid id, TextureInfo info, bool succeeded)
            {
                if (autoClose)
                {
                    info.Texture?.Dispose();
                }
            }

            public TextureInfo Load(Guid id)
            {
                return info;
            }
        }

        private sealed class Color4ArrayLoader : ITextureInfoLoader
        {
            private readonly TextureInfo info;

            public Color4ArrayLoader(Color4[] data, int width)
            {
                info = new TextureInfo(data, width);
            }

            public Color4ArrayLoader(Color4[] data, int width, int height)
            {
                info = new TextureInfo(data, width, height);
            }

            public Color4ArrayLoader(Color4[] data, int width, int height, int depth)
            {
                info = new TextureInfo(data, width, height, depth);
            }

            public void Complete(Guid id, TextureInfo info, bool succeeded)
            {
            }

            public TextureInfo Load(Guid id)
            {
                return info;
            }
        }

        private sealed class ByteArrayLoader : ITextureInfoLoader
        {
            private readonly TextureInfo info;

            public ByteArrayLoader(byte[] data, Format pixelFormat, int width)
            {
                info = new TextureInfo(data, pixelFormat, width);
            }

            public ByteArrayLoader(byte[] data, Format pixelFormat, int width, int height)
            {
                info = new TextureInfo(data, pixelFormat, width, height);
            }

            public ByteArrayLoader(byte[] data, Format pixelFormat, int width, int height, int depth)
            {
                info = new TextureInfo(data, pixelFormat, width, height, depth);
            }

            public void Complete(Guid id, TextureInfo info, bool succeeded)
            {
            }

            public TextureInfo Load(Guid id)
            {
                return info;
            }
        }

        private sealed class RawDataLoader : ITextureInfoLoader
        {
            private readonly TextureInfo info;

            public RawDataLoader(IntPtr data, Format pixelFormat, int width)
            {
                info = new TextureInfo(data, pixelFormat, width);
            }

            public RawDataLoader(IntPtr data, Format pixelFormat, int width, int height)
            {
                info = new TextureInfo(data, pixelFormat, width, height);
            }

            public RawDataLoader(IntPtr data, Format pixelFormat, int width, int height, int depth)
            {
                info = new TextureInfo(data, pixelFormat, width, height, depth);
            }

            public void Complete(Guid id, TextureInfo info, bool succeeded)
            {
            }

            public TextureInfo Load(Guid id)
            {
                return info;
            }
        }

        /// <summary>
        /// Gets or sets the texture model repository.
        /// Repository is used to load texture from file path.
        /// <para>
        /// You can customize the repository to provider your own implementation. Helixtoolkit will ask repository to load texture on-demand.
        /// </para>
        /// </summary>
        /// <value>
        /// The texture model repository.
        /// </value>
        public static ITextureModelRepository TextureModelRepository { set; get; } = new TextureModelRepository();
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid Guid
        {
            get;
        }
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public object Tag
        {
            get;
        }
        /// <summary>
        /// Gets the texture information loader.
        /// </summary>
        /// <value>
        /// The texture information loader.
        /// </value>
        public ITextureInfoLoader TextureInfoLoader
        {
            get;
        } = NullLoader.Null;


        /// <summary>
        /// Provides interface for user defined texture loader. 
        /// HelixToolkit will call <see cref="ITextureInfoLoader.Load(Guid)"/> to start loading content into GPU.
        /// HelixToolkit will call <see cref="ITextureInfoLoader.Complete(Guid, TextureInfo, bool)"/> once content has been loaded into GPU.
        /// </summary>
        /// <param name="contentId">The content identifier.</param>
        /// <param name="loader">The loader.</param>
        public TextureModel(Guid contentId, ITextureInfoLoader loader)
        {
            Guid = contentId;
            TextureInfoLoader = loader ?? NullLoader.Null;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="stream">The compressed texture stream. Supports Jpg, Bmp, Png, Tiff, DDS, Wmp</param>
        /// <param name="autoCloseStream">Close the stream after being uploaded into GPU. 
        /// Use with caution since the stream cannot be re-used after being closed.</param>
        public TextureModel(Stream stream, bool autoCloseStream = false)
            : this(Guid.NewGuid(), new StreamLoader(stream, autoCloseStream))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="colorArray">The color array.</param>
        /// <param name="width">The width.</param>
        /// <exception cref="ArgumentException">Width = {width} does not equal to the Color Array Length {colorArray.Length}</exception>
        public TextureModel(Color4[] colorArray, int width)
            : this(Guid.NewGuid(), new Color4ArrayLoader(colorArray, width))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="colorArray">The color array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentException">Width * Height = {width * height} does not equal to the Color Array Length {colorArray.Length}</exception>
        public TextureModel(Color4[] colorArray, int width, int height)
            : this(Guid.NewGuid(), new Color4ArrayLoader(colorArray, width, height))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="colorArray">The color array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <exception cref="ArgumentException">Width * Height * Depth = {width * height * depth} does not equal to the Color Array Length {colorArray.Length}</exception>
        public TextureModel(Color4[] colorArray, int width, int height, int depth)
            : this(Guid.NewGuid(), new Color4ArrayLoader(colorArray, width, height, depth))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="data">The image data.</param>
        /// <param name="pixelFormat">The image pixel format.</param>
        /// <param name="width">The width.</param>
        /// <exception cref="ArgumentException">Width = {width} does not correspond to the Data Array Length {data.Length}</exception>
        public TextureModel(byte[] data, Format pixelFormat, int width)
            : this(Guid.NewGuid(), new ByteArrayLoader(data, pixelFormat, width))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="data">The image data.</param>
        /// <param name="pixelFormat">The image pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentException">Width * Height = {width * height} does not correspond to the Data Array Length {data.Length}</exception>
        public TextureModel(byte[] data, Format pixelFormat, int width, int height)
            : this(Guid.NewGuid(), new ByteArrayLoader(data, pixelFormat, width, height))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="data">The image data.</param>
        /// <param name="pixelFormat">The image pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <exception cref="ArgumentException">Width * Height * Depth = {width * height * depth} does not correspond to the Data Array Length {data.Length}</exception>
        public TextureModel(byte[] data, Format pixelFormat, int width, int height, int depth)
            : this(Guid.NewGuid(), new ByteArrayLoader(data, pixelFormat, width, height, depth))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="data">The image data.</param>
        /// <param name="pixelFormat">The image pixel format.</param>
        /// <param name="width">The width.</param>
        /// <exception cref="ArgumentException">Width = {width} does not correspond to the Data Length</exception>
        public TextureModel(IntPtr data, Format pixelFormat, int width)
            : this(Guid.NewGuid(), new RawDataLoader(data, pixelFormat, width))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="data">The image data.</param>
        /// <param name="pixelFormat">The image pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentException">Width * Height = {width * height} does not correspond to the Data Length</exception>
        public TextureModel(IntPtr data, Format pixelFormat, int width, int height)
            : this(Guid.NewGuid(), new RawDataLoader(data, pixelFormat, width, height))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="data">The image data.</param>
        /// <param name="pixelFormat">The image pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <exception cref="ArgumentException">Width * Height * Depth = {width * height * depth} does not correspond to the Data Length</exception>
        public TextureModel(IntPtr data, Format pixelFormat, int width, int height, int depth)
            : this(Guid.NewGuid(), new RawDataLoader(data, pixelFormat, width, height, depth))
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="textureFile">The texture file.</param>
        public TextureModel(string textureFile)
            : this(Guid.NewGuid(), new TextureFileLoader(textureFile))
        {

        }
        /// <summary>
        /// Creates texture model from specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static TextureModel Create(Stream stream)
        {
            return TextureModelRepository != null ?
                TextureModelRepository.Create(stream) : new TextureModel(stream);
        }
        /// <summary>
        /// Creates texture model from specified texture path.
        /// </summary>
        /// <param name="texturePath">The texture path.</param>
        /// <returns></returns>
        public static TextureModel Create(string texturePath)
        {
            return TextureModelRepository != null ?
               TextureModelRepository.Create(texturePath) : new TextureModel(texturePath);
        }
        /// <summary>
        /// Loads the texture info.
        /// </summary>
        /// <returns></returns>
        public TextureInfo Load()
        {
            return TextureInfoLoader.Load(Guid);
        }
        /// <summary>
        /// Completes loading
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="succ">if set to <c>true</c> [succ].</param>
        public void Complete(TextureInfo info, bool succ)
        {
            TextureInfoLoader.Complete(Guid, info, succ);
        }
        /// <summary>
        /// Performs an implicit conversion from <see cref="Stream"/> to <see cref="TextureModel"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator TextureModel(Stream stream)
        {
            return TextureModelRepository != null ?
                TextureModelRepository.Create(stream) : new TextureModel(stream);
        }
    }

    public enum TextureDataType
    {
        None,
        Stream,
        ByteArray,
        Color4,
        RawPointer,
    }

    /// <summary>
    /// Stream texture data.
    /// </summary>
    public sealed class TextureInfo
    {
        private static readonly byte[] emptyBytes = new byte[0];
        private static readonly Color4[] emptyColor4 = new Color4[0];

        public static TextureInfo Null
        {
            get;
        } = new TextureInfo();

        public TextureDataType DataType
        {
            get;
        } = TextureDataType.None;

        /// <summary>
        /// The texture stream
        /// </summary>
        public Stream Texture
        {
            get;
        } = Stream.Null;
        /// <summary>
        /// Gets the texture raw bytes.
        /// </summary>
        /// <value>
        /// The texture raw.
        /// </value>
        public byte[] TextureRaw
        {
            get;
        } = emptyBytes;
        /// <summary>
        /// Gets the color4 array.
        /// </summary>
        /// <value>
        /// The color4 array.
        /// </value>
        public Color4[] Color4Array
        {
            get;
        } = emptyColor4;
        /// <summary>
        /// Gets the raw pointer.
        /// </summary>
        /// <value>
        /// The raw pointer.
        /// </value>
        public IntPtr RawPointer
        {
            get;
        } = IntPtr.Zero;

        /// <summary>
        /// Whether the texture is a compressed data format, such as Jepg, Png, DDS.
        /// Set <see cref="IsCompressed"/>= false for such as Color4[] array content.
        /// </summary>
        public bool IsCompressed
        {
            get;
        }
        /// <summary>
        /// The pixel format. Only used by non-compressed texture data.
        /// </summary>
        public Format PixelFormat
        {
            get;
        }
        /// <summary>
        /// The texture width. Only used by non-compressed texture data.
        /// </summary>
        public int Width
        {
            get;
        }
        /// <summary>
        /// The height. Only used by non-compressed texture data.
        /// </summary>
        public int Height
        {
            get;
        }

        public int Depth
        {
            get;
        }
        /// <summary>
        /// The generate mip maps automatically after being loaded.
        /// </summary>
        public bool GenerateMipMaps
        {
            get;
        }
        /// <summary>
        /// Gets the texture dimension.
        /// </summary>
        /// <value>
        /// The dimension.
        /// </value>
        public int Dimension
        {
            get;
        }
        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public object Tag
        {
            get;
        }

        private TextureInfo()
        {
        }
        /// <summary>
        /// Create <see cref="TextureInfo"/> with a compressed texture stream
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentNullException">Texture cannot be null.</exception>
        public TextureInfo(Stream texture, bool generateMipMaps = true)
        {
            Texture = texture ?? throw new ArgumentNullException("Texture cannot be null.");
            DataType = TextureDataType.Stream;
            IsCompressed = true;
            GenerateMipMaps = generateMipMaps;
        }
        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 1D texture stream
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentNullException">Texture cannot be null.</exception>
        /// <exception cref="ArgumentException">Height cannot be zero.</exception>
        public TextureInfo(Stream texture, Format pixelFormat, int width, bool generateMipMaps = true)
        {
            Texture = texture ?? throw new ArgumentNullException("Texture cannot be null.");
            Width = width == 0 ? throw new ArgumentException("Height cannot be zero.") : width;
            DataType = TextureDataType.Stream;
            IsCompressed = false;
            PixelFormat = pixelFormat;
            GenerateMipMaps = generateMipMaps;
            Dimension = 1;
        }
        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 2D texture stream
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentException">Height cannot be zero.</exception>
        public TextureInfo(Stream texture, Format pixelFormat, int width, int height, bool generateMipMaps = true)
            : this(texture, pixelFormat, width, generateMipMaps)
        {
            Height = height == 0 ? throw new ArgumentException("Height cannot be zero.") : height;
            Dimension = 2;
        }
        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 3D texture stream
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentOutOfRangeException">Depth cannot be zero.</exception>
        public TextureInfo(Stream texture, Format pixelFormat, int width, int height, int depth, bool generateMipMaps = true)
            : this(texture, pixelFormat, width, height, generateMipMaps)
        {
            Depth = depth == 0 ? throw new ArgumentOutOfRangeException("Depth cannot be zero.") : depth;
            Dimension = 3;
        }
        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 1D texture data
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentNullException">Texture cannot be null.</exception>
        /// <exception cref="ArgumentException">Height cannot be zero.</exception>
        public TextureInfo(byte[] texture, Format pixelFormat, int width, bool generateMipMaps = true)
        {
            TextureRaw = texture ?? throw new ArgumentNullException("Texture cannot be null.");
            Width = width == 0 ? throw new ArgumentException("Height cannot be zero.") : width;
            DataType = TextureDataType.ByteArray;
            IsCompressed = false;
            PixelFormat = pixelFormat;
            GenerateMipMaps = generateMipMaps;
            Dimension = 1;
        }
        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 2D texture data
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentException">Height cannot be zero.</exception>
        public TextureInfo(byte[] texture, Format pixelFormat, int width, int height, bool generateMipMaps = true)
            : this(texture, pixelFormat, width, generateMipMaps)
        {
            Height = height == 0 ? throw new ArgumentException("Height cannot be zero.") : height;
            Dimension = 2;
        }
        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 3D texture data
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentOutOfRangeException">Depth cannot be zero.</exception>
        public TextureInfo(byte[] texture, Format pixelFormat, int width, int height, int depth, bool generateMipMaps = true)
            : this(texture, pixelFormat, width, height, generateMipMaps)
        {
            Depth = depth == 0 ? throw new ArgumentOutOfRangeException("Depth cannot be zero.") : depth;
            Dimension = 3;
        }

        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 1D color4 data
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentNullException">Texture cannot be null.</exception>
        /// <exception cref="ArgumentException">Height cannot be zero.</exception>
        public TextureInfo(Color4[] texture, bool generateMipMaps = true)
        {
            Color4Array = texture ?? throw new ArgumentNullException("Texture cannot be null.");
            Width = texture.Length;
            DataType = TextureDataType.Color4;
            IsCompressed = false;
            GenerateMipMaps = generateMipMaps;
            Dimension = 1;
            PixelFormat = Format.R32G32B32A32_Float;
        }

        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 1D color4 data
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="width">The width.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentNullException">Texture cannot be null.</exception>
        /// <exception cref="ArgumentException">Height cannot be zero.</exception>
        public TextureInfo(Color4[] texture, int width, bool generateMipMaps = true)
            : this(texture, generateMipMaps)
        {
            Width = width == 0 ? throw new ArgumentException("Height cannot be zero.") : width;
        }
        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 2D color4 data
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentException">Height cannot be zero.</exception>
        public TextureInfo(Color4[] texture, int width, int height, bool generateMipMaps = true)
            : this(texture, width, generateMipMaps)
        {
            Height = height == 0 ? throw new ArgumentException("Height cannot be zero.") : height;
            Dimension = 2;
        }
        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 3D color4 data
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentOutOfRangeException">Depth cannot be zero.</exception>
        public TextureInfo(Color4[] texture, int width, int height, int depth, bool generateMipMaps = true)
            : this(texture, width, height, generateMipMaps)
        {
            Depth = depth == 0 ? throw new ArgumentOutOfRangeException("Depth cannot be zero.") : depth;
            Dimension = 3;
        }

        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 1D texture raw pointer
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentNullException">Texture cannot be null.</exception>
        /// <exception cref="ArgumentException">Height cannot be zero.</exception>
        public TextureInfo(IntPtr texture, Format pixelFormat, int width, bool generateMipMaps = true)
        {
            RawPointer = texture == IntPtr.Zero ? throw new ArgumentNullException("Texture cannot be null.") : texture;
            Width = width == 0 ? throw new ArgumentException("Height cannot be zero.") : width;
            DataType = TextureDataType.RawPointer;
            IsCompressed = false;
            PixelFormat = pixelFormat;
            GenerateMipMaps = generateMipMaps;
            Dimension = 1;
        }
        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 2D texture raw pointer
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentException">Height cannot be zero.</exception>
        public TextureInfo(IntPtr texture, Format pixelFormat, int width, int height, bool generateMipMaps = true)
            : this(texture, pixelFormat, width, generateMipMaps)
        {
            Height = height == 0 ? throw new ArgumentException("Height cannot be zero.") : height;
            Dimension = 2;
        }
        /// <summary>
        /// Create <see cref="TextureInfo"/> with a non-compressed 3D texture raw pointer
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="generateMipMaps">if set to <c>true</c> [generate mip maps].</param>
        /// <exception cref="ArgumentOutOfRangeException">Depth cannot be zero.</exception>
        public TextureInfo(IntPtr texture, Format pixelFormat, int width, int height, int depth, bool generateMipMaps = true)
            : this(texture, pixelFormat, width, height, generateMipMaps)
        {
            Depth = depth == 0 ? throw new ArgumentOutOfRangeException("Depth cannot be zero.") : depth;
            Dimension = 3;
        }
    }
}
