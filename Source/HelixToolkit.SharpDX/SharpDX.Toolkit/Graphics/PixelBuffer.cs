using SharpDX.DXGI;

namespace SharpDX.Toolkit.Graphics;

/// <summary>
/// An unmanaged buffer of pixels.
/// </summary>
public sealed class PixelBuffer
{
    private int width;

    private int height;

    private DXGI.Format format;

    private int rowStride;

    private int bufferStride;

    private readonly IntPtr dataPointer;

    private int pixelSize;

    /// <summary>
    /// True when RowStride == sizeof(pixelformat) * width
    /// </summary>
    private bool isStrictRowStride;

    /// <summary>
    /// Initializes a new instance of the <see cref="PixelBuffer" /> struct.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="format">The format.</param>
    /// <param name="rowStride">The row pitch.</param>
    /// <param name="bufferStride">The slice pitch.</param>
    /// <param name="dataPointer">The pixels.</param>
    public PixelBuffer(int width, int height, Format format, int rowStride, int bufferStride, IntPtr dataPointer)
    {
        if (dataPointer == IntPtr.Zero)
            throw new ArgumentException("Pointer cannot be equal to IntPtr.Zero", nameof(dataPointer));

        this.width = width;
        this.height = height;
        this.format = format;
        this.rowStride = rowStride;
        this.bufferStride = bufferStride;
        this.dataPointer = dataPointer;
        this.pixelSize = FormatHelper.SizeOfInBytes(this.format);
        this.isStrictRowStride = (pixelSize * width) == rowStride;
    }

    /// <summary>
    /// Gets the width.
    /// </summary>
    /// <value>The width.</value>
    public int Width
    {
        get
        {
            return width;
        }
    }

    /// <summary>
    /// Gets the height.
    /// </summary>
    /// <value>The height.</value>
    public int Height
    {
        get
        {
            return height;
        }
    }

    /// <summary>
    /// Gets the format (this value can be changed)
    /// </summary>
    /// <value>The format.</value>
    public Format Format
    {
        get
        {
            return format;
        }
        set
        {
            if (PixelSize != (int)FormatHelper.SizeOfInBytes(value))
            {
                throw new ArgumentException(string.Format("Format [{0}] doesn't have same pixel size in bytes than current format [{1}]", value, format));
            }
            format = value;
        }
    }

    /// <summary>
    /// Gets the pixel size in bytes.
    /// </summary>
    /// <value>The pixel size in bytes.</value>
    public int PixelSize
    {
        get
        {
            return this.pixelSize;
        }
    }

    /// <summary>
    /// Gets the row stride in number of bytes.
    /// </summary>
    /// <value>The row stride in number of bytes.</value>
    public int RowStride
    {
        get
        {
            return this.rowStride;
        }
    }

    /// <summary>
    /// Gets the total size in bytes of this pixel buffer.
    /// </summary>
    /// <value>The size in bytes of the pixel buffer.</value>
    public int BufferStride
    {
        get
        {
            return this.bufferStride;
        }
    }

    /// <summary>
    /// Gets the pointer to the pixel buffer.
    /// </summary>
    /// <value>The pointer to the pixel buffer.</value>
    public IntPtr DataPointer
    {
        get
        {
            return this.dataPointer;
        }
    }

    /// <summary>
    /// Copies this pixel buffer to a destination pixel buffer.
    /// </summary>
    /// <param name="pixelBuffer">The destination pixel buffer.</param>
    /// <remarks>
    /// The destination pixel buffer must have exactly the same dimensions (width, height) and format than this instance.
    /// Destination buffer can have different row stride.
    /// </remarks>
    public unsafe void CopyTo(PixelBuffer pixelBuffer)
    {
        // Check that buffers are identical
        if (this.Width != pixelBuffer.Width
            || this.Height != pixelBuffer.Height
            || PixelSize != FormatHelper.SizeOfInBytes(pixelBuffer.Format))
        {
            throw new ArgumentException("Invalid destination pixelBufferArray. Mush have same Width, Height and Format", nameof(pixelBuffer));
        }

        // If buffers have same size, than we can copy it directly
        if (this.BufferStride == pixelBuffer.BufferStride)
        {
            Utilities.CopyMemory(pixelBuffer.DataPointer, this.DataPointer, this.BufferStride);
        }
        else
        {
            var srcPointer = (byte*)this.DataPointer;
            var dstPointer = (byte*)pixelBuffer.DataPointer;
            var rowStride = Math.Min(RowStride, pixelBuffer.RowStride);

            // Copy per scanline
            for (var i = 0; i < Height; i++)
            {
                Utilities.CopyMemory(new IntPtr(dstPointer), new IntPtr(srcPointer), rowStride);
                srcPointer += this.RowStride;
                dstPointer += pixelBuffer.RowStride;
            }
        }
    }

    /// <summary>
    /// Gets the pixel value at a specified position.
    /// </summary>
    /// <typeparam name="T">Type of the pixel data</typeparam>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <returns>The pixel value.</returns>
    /// <remarks>
    /// Caution, this method doesn't check bounding.
    /// </remarks>
    public unsafe T GetPixel<T>(int x, int y) where T : unmanaged
    {
        return Utilities.Read<T>(new IntPtr(((byte*)this.DataPointer + RowStride * y + x * PixelSize)));
    }

    /// <summary>
    /// Gets the pixel value at a specified position.
    /// </summary>
    /// <typeparam name="T">Type of the pixel data</typeparam>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="value">The pixel value.</param>
    /// <remarks>
    /// Caution, this method doesn't check bounding.
    /// </remarks>
    public unsafe void SetPixel<T>(int x, int y, T value) where T : unmanaged
    {
        Utilities.Write(new IntPtr((byte*)this.DataPointer + RowStride * y + x * PixelSize), ref value);
    }

    /// <summary>
    /// Gets scanline pixels from the buffer.
    /// </summary>
    /// <typeparam name="T">Type of the pixel data</typeparam>
    /// <param name="yOffset">The y line offset.</param>
    /// <returns>Scanline pixels from the buffer</returns>
    /// <exception cref="System.ArgumentException">If the sizeof(T) is an invalid size</exception>
    /// <remarks>
    /// This method is working on a row basis. The <paramref name="yOffset"/> is specifying the first row to get 
    /// the pixels from.
    /// </remarks>
    public T[] GetPixels<T>(int yOffset = 0) where T : unmanaged
    {
        var sizeOfOutputPixel = Utilities.SizeOf<T>();
        var totalSize = Width * Height * pixelSize;
        if ((totalSize % sizeOfOutputPixel) != 0)
            throw new ArgumentException(string.Format("Invalid sizeof(T), not a multiple of current size [{0}]in bytes ", totalSize));

        var buffer = new T[totalSize / sizeOfOutputPixel];
        GetPixels(buffer, yOffset);
        return buffer;
    }

    /// <summary>
    /// Gets scanline pixels from the buffer.
    /// </summary>
    /// <typeparam name="T">Type of the pixel data</typeparam>
    /// <param name="pixels">An allocated scanline pixel buffer</param>
    /// <param name="yOffset">The y line offset.</param>
    /// <returns>Scanline pixels from the buffer</returns>
    /// <exception cref="System.ArgumentException">If the sizeof(T) is an invalid size</exception>
    /// <remarks>
    /// This method is working on a row basis. The <paramref name="yOffset"/> is specifying the first row to get 
    /// the pixels from.
    /// </remarks>
    public void GetPixels<T>(T[] pixels, int yOffset = 0) where T : unmanaged
    {
        GetPixels(pixels, yOffset, 0, pixels.Length);
    }

    /// <summary>
    /// Gets scanline pixels from the buffer.
    /// </summary>
    /// <typeparam name="T">Type of the pixel data</typeparam>
    /// <param name="pixels">An allocated scanline pixel buffer</param>
    /// <param name="yOffset">The y line offset.</param>
    /// <param name="pixelIndex">Offset into the destination <paramref name="pixels"/> buffer.</param>
    /// <param name="pixelCount">Number of pixels to write into the destination <paramref name="pixels"/> buffer.</param>
    /// <exception cref="System.ArgumentException">If the sizeof(T) is an invalid size</exception>
    /// <remarks>
    /// This method is working on a row basis. The <paramref name="yOffset"/> is specifying the first row to get 
    /// the pixels from.
    /// </remarks>
    public unsafe void GetPixels<T>(T[] pixels, int yOffset, int pixelIndex, int pixelCount) where T : unmanaged
    {
        var pixelPointer = (byte*)this.DataPointer + yOffset * rowStride;
        if (isStrictRowStride)
        {
            Utilities.Read(new IntPtr(pixelPointer), pixels, 0, pixelCount);
        }
        else
        {
            var sizeOfOutputPixel = Utilities.SizeOf<T>() * pixelCount;
            var sizePerWidth = sizeOfOutputPixel / Width;
            var remainingPixels = sizeOfOutputPixel % Width;
            for (var i = 0; i < sizePerWidth; i++)
            {
                Utilities.Read(new IntPtr(pixelPointer), pixels, pixelIndex, Width);
                pixelPointer += rowStride;
                pixelIndex += Width;
            }
            if (remainingPixels > 0)
            {
                Utilities.Read(new IntPtr(pixelPointer), pixels, pixelIndex, remainingPixels);
            }
        }
    }

    /// <summary>
    /// Sets scanline pixels to the buffer.
    /// </summary>
    /// <typeparam name="T">Type of the pixel data</typeparam>
    /// <param name="sourcePixels">Source pixel buffer</param>
    /// <param name="yOffset">The y line offset.</param>
    /// <exception cref="System.ArgumentException">If the sizeof(T) is an invalid size</exception>
    /// <remarks>
    /// This method is working on a row basis. The <paramref name="yOffset"/> is specifying the first row to get 
    /// the pixels from.
    /// </remarks>
    public void SetPixels<T>(T[] sourcePixels, int yOffset = 0) where T : unmanaged
    {
        SetPixels(sourcePixels, yOffset, 0, sourcePixels.Length);
    }

    /// <summary>
    /// Sets scanline pixels to the buffer.
    /// </summary>
    /// <typeparam name="T">Type of the pixel data</typeparam>
    /// <param name="sourcePixels">Source pixel buffer</param>
    /// <param name="yOffset">The y line offset.</param>
    /// <param name="pixelIndex">Offset into the source <paramref name="sourcePixels"/> buffer.</param>
    /// <param name="pixelCount">Number of pixels to write into the source <paramref name="sourcePixels"/> buffer.</param>
    /// <exception cref="System.ArgumentException">If the sizeof(T) is an invalid size</exception>
    /// <remarks>
    /// This method is working on a row basis. The <paramref name="yOffset"/> is specifying the first row to get 
    /// the pixels from.
    /// </remarks>
    public unsafe void SetPixels<T>(T[] sourcePixels, int yOffset, int pixelIndex, int pixelCount) where T : unmanaged
    {
        var pixelPointer = (byte*)this.DataPointer + yOffset * rowStride;
        if (isStrictRowStride)
        {
            Utilities.Write(new IntPtr(pixelPointer), sourcePixels, 0, pixelCount);
        }
        else
        {
            var sizeOfOutputPixel = Utilities.SizeOf<T>() * pixelCount;
            var sizePerWidth = sizeOfOutputPixel / Width;
            var remainingPixels = sizeOfOutputPixel % Width;
            for (var i = 0; i < sizePerWidth; i++)
            {
                Utilities.Write(new IntPtr(pixelPointer), sourcePixels, pixelIndex, Width);
                pixelPointer += rowStride;
                pixelIndex += Width;
            }
            if (remainingPixels > 0)
            {
                Utilities.Write(new IntPtr(pixelPointer), sourcePixels, pixelIndex, remainingPixels);
            }
        }
    }
}