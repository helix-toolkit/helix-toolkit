/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;

using SDX11 = SharpDX.Direct3D11;
#if !NETFX_CORE

namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    using Extensions;
    using Render;

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
        void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count) where T : struct;

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="offset"></param>
        /// <param name="minBufferCount">Used to initialize a buffer which size is Max(count, minBufferCount). Only used in dynamic buffer.</param>
        void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count, int offset, int minBufferCount = default(int)) where T : struct;

        /// <summary>
        /// <see cref="DisposeObject.DisposeAndClear"/>
        /// </summary>
        void DisposeAndClear();
    }

    /// <summary>
    ///
    /// </summary>
    public sealed class ImmutableBufferProxy : BufferProxyBase, IElementsBufferProxy
    {
        /// <summary>
        ///
        /// </summary>
        public ResourceOptionFlags OptionFlags { private set; get; }
        public ResourceUsage Usage { private set; get; } = ResourceUsage.Immutable;
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
        /// <see cref="IElementsBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, IList{T}, int)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        public void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count) where T : struct
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
        public void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count, int offset, int minBufferCount = default(int)) where T : struct
        {
            RemoveAndDispose(ref buffer);
            ElementCount = count;
            if(count == 0)
            {
                return;
            }
            var buffdesc = new BufferDescription()
            {
                BindFlags = this.BindFlags,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = this.OptionFlags,
                SizeInBytes = StructureSize * count,
                StructureByteStride = StructureSize,
                Usage = Usage
            };
            buffer = Collect(Buffer.Create(context, data.GetArrayByType(), buffdesc));
        }
    }

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
        public ResourceOptionFlags OptionFlags { private set; get; }
        /// <summary>
        /// Gets the capacity in bytes.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public int Capacity { private set; get; }
        /// <summary>
        /// Gets the capacity used in bytes.
        /// </summary>
        /// <value>
        /// The capacity used.
        /// </value>
        public int CapacityUsed { private set; get; }
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
        /// <see cref="IElementsBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, IList{T}, int)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        public void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count) where T : struct
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
        /// <param name="minBufferCount">Used to create a dynamic buffer with size of Max(count, minBufferCount).</param>
        public void UploadDataToBuffer<T>(DeviceContextProxy context, IList<T> data, int count, int offset, int minBufferCount = default(int)) where T : struct
        {
            ElementCount = count;
            int newSizeInBytes = StructureSize * count;
            if (count == 0)
            {
                return;
            }
            else if (buffer == null || Capacity < newSizeInBytes || (!LazyResize && Capacity != newSizeInBytes))
            {
                Initialize(context, data, count, offset, minBufferCount);
            }
            if(CapacityUsed + newSizeInBytes <= Capacity && !context.IsDeferred && CanOverwrite)
            {
                Offset = CapacityUsed;
                context.MapSubresource(this.buffer, MapMode.WriteNoOverwrite, MapFlags.None, out DataStream stream);
                using (stream)
                {
                    stream.Position = Offset;
                    stream.WriteRange(data.GetArrayByType(), offset, count);                    
                }
                context.UnmapSubresource(this.buffer, 0);
                CapacityUsed += newSizeInBytes;
            }
            else
            {
                context.MapSubresource(this.buffer, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                using (stream)
                {
                    stream.WriteRange(data.GetArrayByType(), offset, count);
                }
                context.UnmapSubresource(this.buffer, 0);
                Offset = CapacityUsed = 0;
            }
        }

        /// <summary>
        /// Initializes the specified device.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="device">The device.</param>
        /// <param name="data">The data.</param>
        /// <param name="count">The count.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="minBufferCount">The minimum buffer count.</param>
        public void Initialize<T>(Device device, IList<T> data, int count, int offset = default(int), int minBufferCount = default(int)) where T : struct
        {
            RemoveAndDispose(ref buffer);
            var buffdesc = new BufferDescription()
            {
                BindFlags = this.BindFlags,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = this.OptionFlags,
                SizeInBytes = StructureSize * System.Math.Max(count, minBufferCount),
                StructureByteStride = StructureSize,
                Usage = ResourceUsage.Dynamic
            };
            Capacity = buffdesc.SizeInBytes;
            CapacityUsed = 0;
            buffer = Collect(new Buffer(device, buffdesc));
            OnBufferChanged(buffer);
        }

        protected virtual void OnBufferChanged(Buffer newBuffer) { }
    }

    public sealed class StructuredBufferProxy : DynamicBufferProxy
    {
        private ShaderResourceViewProxy srv;
        public ShaderResourceViewProxy SRV
        {
            get { return srv; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredBufferProxy"/> class.
        /// </summary>
        /// <param name="structureSize">Size of the structure.</param>
        /// <param name="lazyResize">If existing data size is smaller than buffer size, reuse existing. Otherwise create a new buffer with exact same size</param>
        public StructuredBufferProxy(int structureSize, bool lazyResize = true) :
            base(structureSize, BindFlags.ShaderResource, ResourceOptionFlags.BufferStructured, lazyResize)
        {

        }

        protected override void OnBufferChanged(Buffer newBuffer)
        {
            RemoveAndDispose(ref srv);
            srv = Collect(new ShaderResourceViewProxy(newBuffer.Device, newBuffer));
            srv.CreateTextureView();
        }

        public static implicit operator ShaderResourceViewProxy(StructuredBufferProxy proxy)
        {
            return proxy.srv;
        }

        public static implicit operator ShaderResourceView(StructuredBufferProxy proxy)
        {
            return proxy.srv;
        }
    }
}