using HelixToolkit.SharpDX.Render;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
///
/// </summary>
public interface IElementsBufferProxy : IBufferProxy
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <param name="count"></param>
    void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count) where T : unmanaged;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <param name="count"></param>
    /// <param name="offset"></param>
    /// <param name="minBufferCount">Used to initialize a buffer which size is Max(count, minBufferCount). Only used in dynamic buffer.</param>
    void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count, int offset,
        int minBufferCount = default) where T : unmanaged;

    /// <summary>
    /// Uploads the data to buffer using data pointer.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="data">The data.</param>
    /// <param name="countByBytes">The count by bytes.</param>
    /// <param name="offsetByBytes">The offset by bytes.</param>
    /// <param name="minBufferCountByBytes">The minimum buffer count by bytes.</param>
    unsafe void UploadDataToBuffer(DeviceContextProxy context, System.IntPtr data, int countByBytes, int offsetByBytes,
        int minBufferCountByBytes = default);
    /// <summary>
    /// Creates the buffer with size = count * structure size;
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="count">The count.</param>
    void CreateBuffer(DeviceContextProxy context, int count);
    /// <summary>
    /// Dispose and clear internal buffers. Does not dispose this object.
    /// </summary>
    void DisposeAndClear();
}
