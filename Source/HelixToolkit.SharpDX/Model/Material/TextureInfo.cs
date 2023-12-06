using SharpDX.DXGI;
using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Stream texture data.
/// </summary>
public sealed class TextureInfo
{
    private static readonly byte[] emptyBytes = Array.Empty<byte>();
    private static readonly Color4[] emptyColor4 = Array.Empty<Color4>();

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
    public object? Tag
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
    public TextureInfo(Stream? texture, bool generateMipMaps = true)
    {
        Texture = texture ?? throw new ArgumentNullException(nameof(texture), "Texture cannot be null.");
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
    public TextureInfo(Stream? texture, Format pixelFormat, int width, bool generateMipMaps = true)
    {
        Texture = texture ?? throw new ArgumentNullException(nameof(texture), "Texture cannot be null.");
        Width = width == 0 ? throw new ArgumentException(nameof(width), "Height cannot be zero.") : width;
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
    public TextureInfo(Stream? texture, Format pixelFormat, int width, int height, bool generateMipMaps = true)
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
    public TextureInfo(Stream? texture, Format pixelFormat, int width, int height, int depth, bool generateMipMaps = true)
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
    public TextureInfo(byte[]? texture, Format pixelFormat, int width, bool generateMipMaps = true)
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
    public TextureInfo(byte[]? texture, Format pixelFormat, int width, int height, bool generateMipMaps = true)
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
    public TextureInfo(byte[]? texture, Format pixelFormat, int width, int height, int depth, bool generateMipMaps = true)
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
    public TextureInfo(Color4[]? texture, bool generateMipMaps = true)
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
    public TextureInfo(Color4[]? texture, int width, bool generateMipMaps = true)
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
    public TextureInfo(Color4[]? texture, int width, int height, bool generateMipMaps = true)
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
    public TextureInfo(Color4[]? texture, int width, int height, int depth, bool generateMipMaps = true)
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
