using HelixToolkit.SharpDX.Render;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
///
/// </summary>
public sealed class ImmutableBufferProxy : BufferProxyBase, IElementsBufferProxy
{
    /// <summary>
    ///
    /// </summary>
    public ResourceOptionFlags OptionFlags
    {
        private set; get;
    }
    public ResourceUsage Usage { private set; get; } = ResourceUsage.Immutable;

    public CpuAccessFlags CpuAccess { private set; get; } = CpuAccessFlags.None;
    /// <summary>
    ///
    /// </summary>
    /// <param name="structureSize"></param>
    /// <param name="bindFlags"></param>
    /// <param name="optionFlags"></param>
    /// <param name="usage"></param>
    public ImmutableBufferProxy(int structureSize, BindFlags bindFlags, ResourceOptionFlags optionFlags = ResourceOptionFlags.None, ResourceUsage usage = ResourceUsage.Immutable)
        : base(structureSize, bindFlags)
    {
        OptionFlags = optionFlags;
        Usage = usage;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmutableBufferProxy"/> class.
    /// </summary>
    /// <param name="structureSize">Size of the structure.</param>
    /// <param name="bindFlags">The bind flags.</param>
    /// <param name="cpuAccess">The cpu access.</param>
    /// <param name="optionFlags">The option flags.</param>
    /// <param name="usage">The usage.</param>
    public ImmutableBufferProxy(int structureSize, BindFlags bindFlags,
        CpuAccessFlags cpuAccess,
        ResourceOptionFlags optionFlags = ResourceOptionFlags.None,
        ResourceUsage usage = ResourceUsage.Immutable)
        : base(structureSize, bindFlags)
    {
        OptionFlags = optionFlags;
        Usage = usage;
        CpuAccess = cpuAccess;
    }

    /// <summary>
    /// <see cref="IElementsBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, IList{T}, int)"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <param name="count"></param>
    public void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count) where T : unmanaged
    {
        UploadDataToBuffer<T>(context, data, count, 0);
    }

    /// <summary>
    /// <see cref="IElementsBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, IList{T}, int, int, int)"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <param name="data"></param>
    /// <param name="count"></param>
    /// <param name="offset"></param>
    /// <param name="minBufferCount">This is not being used in ImmutableBuffer</param>
    public void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count, int offset,
        int minBufferCount = default) where T : unmanaged
    {
        RemoveAndDispose(ref buffer);
        ElementCount = count;
        if (count == 0)
        {
            return;
        }
        var buffdesc = new BufferDescription()
        {
            BindFlags = this.BindFlags,
            CpuAccessFlags = CpuAccess,
            OptionFlags = this.OptionFlags,
            SizeInBytes = StructureSize * count,
            StructureByteStride = StructureSize,
            Usage = Usage
        };
        buffer = Buffer.Create(context, data.GetArrayByType(), buffdesc);
    }

    /// <summary>
    /// Uploads the data to buffer using data pointer.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="data">The data pointer.</param>
    /// <param name="countByBytes">The count by bytes.</param>
    /// <param name="offsetByBytes">The offset by bytes.</param>
    /// <param name="minBufferCountByBytes">The minimum buffer count by bytes.</param>
    public unsafe void UploadDataToBuffer(DeviceContextProxy context, System.IntPtr data, int countByBytes, int offsetByBytes, int minBufferCountByBytes = default)
    {
        RemoveAndDispose(ref buffer);
        ElementCount = countByBytes / StructureSize;
        if (countByBytes == 0)
        {
            return;
        }
        var buffdesc = new BufferDescription()
        {
            BindFlags = this.BindFlags,
            CpuAccessFlags = CpuAccess,
            OptionFlags = this.OptionFlags,
            SizeInBytes = countByBytes,
            StructureByteStride = StructureSize,
            Usage = Usage
        };
        buffer = new Buffer(context, data, buffdesc);
    }
    /// <summary>
    /// Creates the buffer with size of count * structure size.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="count">The element count.</param>
    public void CreateBuffer(DeviceContextProxy context, int count)
    {
        RemoveAndDispose(ref buffer);
        ElementCount = count;
        if (count == 0)
        {
            return;
        }
        var buffdesc = new BufferDescription()
        {
            BindFlags = this.BindFlags,
            CpuAccessFlags = CpuAccess,
            OptionFlags = this.OptionFlags,
            SizeInBytes = StructureSize * count,
            StructureByteStride = StructureSize,
            Usage = Usage
        };
        buffer = new Buffer(context, buffdesc);
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref buffer);
        base.OnDispose(disposeManagedResources);
    }
}
