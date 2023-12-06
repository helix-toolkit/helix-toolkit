using SharpDX.Direct3D11;
using SharpDX;
using System.Runtime.CompilerServices;
using Buffer = SharpDX.Direct3D11.Buffer;
using HelixToolkit.SharpDX.Render;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
///
/// </summary>
public class DynamicBufferProxy : BufferProxyBase, IElementsBufferProxy
{
    public readonly bool CanOverwrite = false;
    public readonly bool LazyResize = true;
    /// <summary>
    ///
    /// </summary>
    public ResourceOptionFlags OptionFlags
    {
        private set; get;
    }
    /// <summary>
    /// Gets the capacity in bytes.
    /// </summary>
    /// <value>
    /// The capacity.
    /// </value>
    public int Capacity
    {
        private set; get;
    }
    /// <summary>
    /// Gets the capacity used in bytes.
    /// </summary>
    /// <value>
    /// The capacity used.
    /// </value>
    public int CapacityUsed
    {
        private set; get;
    }

    public CpuAccessFlags CpuAccess { private set; get; } = CpuAccessFlags.Write;
    /// <summary>
    ///
    /// </summary>
    /// <param name="structureSize"></param>
    /// <param name="bindFlags"></param>
    /// <param name="optionFlags"></param>
    /// <param name="lazyResize">If existing data size is smaller than buffer size, reuse existing. Otherwise create a new buffer with exact same size</param>
    public DynamicBufferProxy(int structureSize, BindFlags bindFlags,
        ResourceOptionFlags optionFlags = ResourceOptionFlags.None, bool lazyResize = true)
        : base(structureSize, bindFlags)
    {
        CanOverwrite = (bindFlags & (BindFlags.VertexBuffer | BindFlags.IndexBuffer)) != 0;
        this.OptionFlags = optionFlags;
        LazyResize = lazyResize;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicBufferProxy"/> class.
    /// </summary>
    /// <param name="structureSize">Size of the structure.</param>
    /// <param name="bindFlags">The bind flags.</param>
    /// <param name="optionFlags">The option flags.</param>
    /// <param name="lazyResize">if set to <c>true</c> [lazy resize].</param>
    /// <param name="canOverWrite">if set to <c>true</c> [can over write].</param>
    public DynamicBufferProxy(int structureSize, BindFlags bindFlags, bool canOverWrite,
        ResourceOptionFlags optionFlags = ResourceOptionFlags.None, bool lazyResize = true)
        : base(structureSize, bindFlags)
    {
        CanOverwrite = canOverWrite;
        this.OptionFlags = optionFlags;
        LazyResize = lazyResize;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicBufferProxy"/> class.
    /// </summary>
    /// <param name="structureSize">Size of the structure.</param>
    /// <param name="bindFlags">The bind flags.</param>
    /// <param name="canOverWrite">if set to <c>true</c> [can over write].</param>
    /// <param name="cpuAccess">The cpu access.</param>
    /// <param name="optionFlags">The option flags.</param>
    /// <param name="lazyResize">if set to <c>true</c> [lazy resize].</param>
    public DynamicBufferProxy(int structureSize, BindFlags bindFlags, bool canOverWrite,
        CpuAccessFlags cpuAccess,
        ResourceOptionFlags optionFlags = ResourceOptionFlags.None, bool lazyResize = true)
        : base(structureSize, bindFlags)
    {
        CanOverwrite = canOverWrite;
        this.OptionFlags = optionFlags;
        LazyResize = lazyResize;
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
    /// <param name="count">Data Count</param>
    /// <param name="offset"></param>
    /// <param name="minBufferCount">Used to create a dynamic buffer with size of Max(count, minBufferCount).</param>
    public void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count, int offset,
        int minBufferCount = default) where T : unmanaged
    {
        ElementCount = count;
        var newSizeInBytes = StructureSize * count;
        if (count == 0)
        {
            return;
        }
        EnsureBufferCapacity(context, ElementCount, minBufferCount);
        var mapMode = MapMode.WriteNoOverwrite;
        if (CapacityUsed + newSizeInBytes <= Capacity && !context.IsDeferred && CanOverwrite)
        {
            Offset = CapacityUsed;
            CapacityUsed += newSizeInBytes;
        }
        else
        {
            mapMode = MapMode.WriteDiscard;
            Offset = CapacityUsed = 0;
        }
        var dataArray = data.GetArrayByType();
        var dataBox = context.MapSubresource(this.buffer, 0, mapMode, MapFlags.None);
        if (dataBox is not null)
        {
            UnsafeHelper.Write(dataBox.Value.DataPointer + Offset, dataArray, offset, count);
            context.UnmapSubresource(this.buffer, 0);
        }
    }

    /// <summary>
    /// Uploads the data pointer to buffer. 
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="data">The data.</param>
    /// <param name="byteCount">The count by bytes.</param>
    /// <param name="byteOffset">The offset by bytes.</param>
    /// <param name="minBufferSizeByBytes">The minimum buffer count by bytes.</param>
    public unsafe void UploadDataToBuffer(DeviceContextProxy context, System.IntPtr data, int byteCount,
        int byteOffset, int minBufferSizeByBytes = default)
    {
        ElementCount = byteCount / StructureSize;
        var newSizeInBytes = byteCount;
        if (byteCount == 0)
        {
            return;
        }
        EnsureBufferCapacity(context, ElementCount, minBufferSizeByBytes / StructureSize);
        var mapMode = MapMode.WriteNoOverwrite;
        if (CapacityUsed + newSizeInBytes <= Capacity && !context.IsDeferred && CanOverwrite)
        {
            Offset = CapacityUsed;
            CapacityUsed += newSizeInBytes;
        }
        else
        {
            mapMode = MapMode.WriteDiscard;
            Offset = CapacityUsed = 0;
        }
        var dataBox = context.MapSubresource(this.buffer, 0, mapMode, MapFlags.None);
        if (dataBox is not null)
        {
            UnsafeHelper.Write(dataBox.Value.DataPointer + Offset, data, byteOffset, byteCount);
            context.UnmapSubresource(this.buffer, 0);
        }
    }

    /// <summary>
    /// Ensures the buffer capacity is enough.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="count">The count.</param>
    /// <param name="minSizeCount">The minimum size count.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureBufferCapacity(DeviceContextProxy context, int count, int minSizeCount)
    {
        var bytes = count * StructureSize;
        if (buffer == null || Capacity < bytes || (!LazyResize && Capacity != bytes))
        {
            Initialize(context, count, minSizeCount);
        }
    }

    /// <summary>
    /// Maps the buffer. Make sure to call <see cref="EnsureBufferCapacity(DeviceContextProxy, int, int)"/> to make sure buffer has enough space
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="action">The action.</param>
    public void MapBuffer(DeviceContextProxy context, System.Action<DataBox> action)
    {
        var dataBox = context.MapSubresource(this.buffer, 0, MapMode.WriteDiscard, MapFlags.None);
        if (dataBox is not null)
        {
            action(dataBox.Value);
            context.UnmapSubresource(this.buffer, 0);
        }

        Offset = CapacityUsed = 0;
    }
    /// <summary>
    /// Initializes the specified device.
    /// </summary>
    /// <param name="device">The device.</param>
    /// <param name="count">The count.</param>
    /// <param name="minBufferCount">The minimum buffer count.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Initialize(Device device, int count, int minBufferCount = default)
    {
        RemoveAndDispose(ref buffer);
        var buffdesc = new BufferDescription()
        {
            BindFlags = this.BindFlags,
            CpuAccessFlags = CpuAccess,
            OptionFlags = this.OptionFlags,
            SizeInBytes = StructureSize * System.Math.Max(count, minBufferCount),
            StructureByteStride = StructureSize,
            Usage = ResourceUsage.Dynamic
        };
        Capacity = buffdesc.SizeInBytes;
        CapacityUsed = 0;
        buffer = new Buffer(device, buffdesc);
        OnBufferChanged(buffer);
    }

    /// <summary>
    /// Creates the buffer with size of count * structure size.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="count">The element count.</param>
    public void CreateBuffer(DeviceContextProxy context, int count)
    {
        Initialize(context, count);
    }

    protected virtual void OnBufferChanged(Buffer newBuffer)
    {
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref buffer);
        base.OnDispose(disposeManagedResources);
    }
}
