/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.DXGI;
using System;
using System.IO;
using System.Runtime.CompilerServices;

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
    /// Texture model, used to support both compressed texture and uncompressed texture.
    /// <para>For compress textures, only provide their memory stream.</para>
    /// <para>For uncompressed textures, provides either data as byte[] or Color4[] with width/height and proper <see cref="Format"/></para>
    /// </summary>
    [System.ComponentModel.TypeConverter(typeof(StreamToTextureModelConverter))]
    public sealed class TextureModel
    {
        private static ITextureModelRepository texRepo = new TextureModelRepository();
        public static void SetRepository(ITextureModelRepository repository)
        {
            texRepo = repository;
        }

        public Guid Guid { get; } = Guid.NewGuid();

        private Stream compressedStream = null;
        /// <summary>
        /// The compressed stream
        /// </summary>
        public Stream CompressedStream
        {
            private set
            {
                compressedStream = value;
            }
            get
            {
                if (compressedStream != null && compressedStream.CanRead)
                {
                    return compressedStream;
                }
                if (!String.IsNullOrEmpty(FilePath))
                {
                    var texStream = texRepo.Load(FilePath);
                    compressedStream = texStream.Stream;
                    CanAutoCloseStream = texStream.AutoCloseAfterLoading;
                    return compressedStream;
                }
                return null;
            }
        }
        /// <summary>
        /// Indicate whether this texture is compressed. Use <see cref="CompressedStream"/> if is compressed, otherwise use <see cref="NonCompressedData"/>
        /// </summary>
        public readonly bool IsCompressed = true;

        /// <summary>
        /// Gets or sets a value indicating whether this stream can be closed after loading texture into the buffer.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this stream can be closed; otherwise, <c>false</c>.
        /// </value>
        public bool CanAutoCloseStream
        {
            set; get;
        } = false;

        /// <summary>
        /// The uncompressed data
        /// </summary>
        public Color4[] NonCompressedData;

        public string FilePath
        {
            get;
        } = String.Empty;

        /// <summary>
        /// The uncompressed format
        /// </summary>
        public readonly Format UncompressedFormat;
        /// <summary>
        /// The width. Only for uncompressed
        /// </summary>
        public readonly int Width;
        /// <summary>
        /// The height. Only for uncompressed
        /// </summary>
        public readonly int Height;
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        public TextureModel()
        {

        }

        public TextureModel(string filePath)
        {
            if (!String.IsNullOrEmpty(filePath))
            {
                FilePath = filePath;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="stream">The compressed texture stream. Supports Jpg, Bmp, Png, Tiff, DDS, Wmp</param>
        public TextureModel(Stream stream)
        {
            CompressedStream = stream;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureModel"/> class.
        /// </summary>
        /// <param name="colorArray">The color array.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentException">Width * Height = {width * height} does not equal to the Color Array Length {colorArray.Length}</exception>
        public TextureModel(Color4[] colorArray, int width, int height)
        {
            if(width * height != colorArray.Length)
            {
                throw new ArgumentException($"Width * Height = {width * height} does not equal to the Color Array Length {colorArray.Length}");
            }
            NonCompressedData = colorArray;               
            IsCompressed = false;
            UncompressedFormat = Format.R32G32B32A32_Float;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="CompressedStream"/> to <see cref="TextureModel"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator TextureModel(Stream stream)
        {
            return texRepo.Create(stream);
        }
        /// <summary>
        /// Performs an explicit conversion from <see cref="TextureModel"/> to <see cref="Stream"/>.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator Stream(TextureModel model)
        {
            return model?.CompressedStream;
        }
    }
}
