/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SDX11 = SharpDX.Direct3D11;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using SharpDX;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    using Extensions;
    using Shaders;

    public interface IBufferProxy : IDisposable
    {
        /// <summary>
        /// Raw Buffer
        /// </summary>
        SDX11.Buffer Buffer { get; }
        /// <summary>
        /// Element Size
        /// </summary>
        int StructureSize { get; }
        /// <summary>
        /// Element count
        /// </summary>
        int Count { get; }
        /// <summary>
        /// Buffer offset
        /// </summary>
        int Offset { set; get; }
        /// <summary>
        /// Buffer binding flag
        /// </summary>
        BindFlags BindFlags { get; }

        void CreateBuffer(Device device);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        void UploadDataToBuffer<T>(DeviceContext context, ref T data, int offset) where T : struct;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        void UploadDataToBuffer<T>(DeviceContext context, ref T data) where T : struct;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        void UploadDataToBuffer<T>(DeviceContext context, IList<T> data) where T : struct;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        void UploadDataToBuffer<T>(DeviceContext context, IList<T> data, int count) where T : struct;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        void UploadDataToBuffer<T>(DeviceContext context, IList<T> data, int count, int offset) where T : struct;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        void CreateBufferFromDataArray<T>(Device context, IList<T> data) where T : struct;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        void CreateBufferFromDataArray<T>(Device context, IList<T> data, int count) where T : struct;
    }

    public class ImmutableBufferProxy : DynamicBufferProxy
    {
        public ImmutableBufferProxy(int structureSize, BindFlags bindFlags, ResourceOptionFlags optionFlags = ResourceOptionFlags.None) 
            : base(structureSize, bindFlags, optionFlags)
        {
        }

        public override void UploadDataToBuffer<T>(DeviceContext context, IList<T> data, int count, int offset)
        {
            CreateBufferFromDataArray(context.Device, data, count);
        }

        public override void CreateBufferFromDataArray<T>(Device device, IList<T> data, int length)
        {
            Disposer.RemoveAndDispose(ref buffer);
            var buffdesc = new BufferDescription()
            {
                BindFlags = this.BindFlags,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = this.OptionFlags,
                SizeInBytes = StructureSize * length,
                StructureByteStride = StructureSize,
                Usage = ResourceUsage.Immutable
            };
            buffer = SDX11.Buffer.Create(device, data.GetArrayByType(), buffdesc);
            Count = length;
        }
    }

    public class DynamicBufferProxy : BufferProxyBase
    {
        public ResourceOptionFlags OptionFlags { private set; get; }

        public DynamicBufferProxy(int structureSize, BindFlags bindFlags, ResourceOptionFlags optionFlags = ResourceOptionFlags.None)
            :base(structureSize, bindFlags)
        {
            this.OptionFlags = optionFlags;
        }

        public override void UploadDataToBuffer<T>(DeviceContext context, IList<T> data, int count, int offset)
        {
            Count = count;
            if (buffer == null || buffer.Description.SizeInBytes < StructureSize * count)
            {
                CreateBufferFromDataArray(context.Device, data, count);
            }
            else
            {
                DataStream stream;
                context.MapSubresource(this.buffer, MapMode.WriteDiscard, MapFlags.None, out stream);
                using (stream)
                {
                    stream.WriteRange(data.GetArrayByType(), offset, count);
                    context.UnmapSubresource(this.buffer, 0);
                }
            }
        }

        public override void CreateBufferFromDataArray<T>(Device device, IList<T> data, int count)
        {
            Disposer.RemoveAndDispose(ref buffer);
            var buffdesc = new BufferDescription()
            {
                BindFlags = this.BindFlags,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = this.OptionFlags,
                SizeInBytes = StructureSize * count,
                StructureByteStride = StructureSize,
                Usage = ResourceUsage.Dynamic
            };           
            buffer = SDX11.Buffer.Create(device, data.GetArrayByType(), buffdesc);
            Count = count;
        }

        public override void UploadDataToBuffer<T>(DeviceContext context, ref T data, int offset)
        {
            UploadDataToBuffer(context, new[] { data }, 1, offset);
        }

        public override void CreateBuffer(Device device)
        {
            throw new ArgumentException("Method does not supported.");
        }
    }

    public class ConstantBufferProxy : BufferProxyBase
    {
        private readonly BufferDescription bufferDesc;
        public ConstantBufferProxy(int structSize, BindFlags bindFlags = BindFlags.ConstantBuffer, 
            CpuAccessFlags cpuAccessFlags = CpuAccessFlags.None, ResourceOptionFlags optionFlags = ResourceOptionFlags.None, ResourceUsage usage = ResourceUsage.Default, int strideSize = 0)
            :base(structSize, bindFlags)
        {
            if (structSize % 16 != 0)
            {
                throw new ArgumentException("Constant buffer struct size must be multiple of 16 bytes");
            }
            bufferDesc = new BufferDescription()
            {
                SizeInBytes = structSize,
                BindFlags = bindFlags,
                CpuAccessFlags = cpuAccessFlags,
                OptionFlags = optionFlags,
                Usage = usage,
                StructureByteStride = strideSize
            };
        }

        public ConstantBufferProxy(ConstantBufferDescription description)
            :base(description.StructSize, description.BindFlags)
        {
            if (description.StructSize % 16 != 0)
            {
                throw new ArgumentException("Constant buffer struct size must be multiple of 16 bytes");
            }
            bufferDesc = new BufferDescription()
            {
                SizeInBytes = description.StructSize,
                BindFlags = description.BindFlags,
                CpuAccessFlags = description.CpuAccessFlags, 
                OptionFlags = description.OptionFlags,
                Usage = description.Usage,
                StructureByteStride = description.StrideSize
            };
        }

        public override void UploadDataToBuffer<T>(DeviceContext context, ref T data, int offset)
        {
            if (buffer.Description.Usage == ResourceUsage.Dynamic)
            {
                DataStream stream;
                context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None, out stream);
                using (stream)
                {
                    stream.Write(data);
                    context.UnmapSubresource(buffer, 0);
                }
            }
            else
            {
                context.UpdateSubresource(ref data, buffer);
            }
        }

        public override void CreateBuffer(Device device)
        {
            buffer = new SDX11.Buffer(device, bufferDesc);
        }

        public override void UploadDataToBuffer<T>(DeviceContext context, IList<T> data, int count, int offset)
        {
            if (buffer.Description.Usage == ResourceUsage.Dynamic)
            {
                DataStream stream;
                context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None, out stream);
                using (stream)
                {
                    stream.WriteRange(data.GetArrayByType(), offset, count);
                    context.UnmapSubresource(buffer, 0);
                }
            }
            else
            {
                context.UpdateSubresource(data.GetArrayByType(), buffer);
            }
        }

        public override void CreateBufferFromDataArray<T>(Device context, IList<T> data, int count)
        {
            throw new ArgumentException("Constant Buffer does not support this function.");
        }
    }

    public abstract class BufferProxyBase : IBufferProxy
    {
        protected SDX11.Buffer buffer;
        public int StructureSize { get; private set; }
        public int Count { get; protected set; } = 0;
        public int Offset { get; set; } = 0;
        public SDX11.Buffer Buffer { get { return buffer; } }
        public BindFlags BindFlags { private set; get; }       

        public BufferProxyBase(int structureSize, BindFlags bindFlags)
        {
            StructureSize = structureSize;
            BindFlags = bindFlags;
        }
        /// <summary>
        /// <see cref="IBufferProxy.UploadDataToBuffer{T}(DeviceContext, ref T, int)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        public abstract void UploadDataToBuffer<T>(DeviceContext context, ref T data, int offset) where T : struct;
        /// <summary>
        /// <see cref="IBufferProxy.UploadDataToBuffer{T}(DeviceContext, IList{T}, int, int)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="offset"></param>
        public abstract void UploadDataToBuffer<T>(DeviceContext context, IList<T> data, int count, int offset) where T : struct;
        /// <summary>
        /// <see cref="IBufferProxy.CreateBuffer(Device)"/>
        /// </summary>
        /// <param name="device"></param>
        public abstract void CreateBuffer(Device device);
        /// <summary>
        /// <see cref="IBufferProxy.CreateBufferFromDataArray{T}(Device, IList{T}, int)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        public abstract void CreateBufferFromDataArray<T>(Device context, IList<T> data, int count) where T : struct;
        /// <summary>
        /// <see cref="IBufferProxy.UploadDataToBuffer{T}(DeviceContext, IList{T})"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        public void UploadDataToBuffer<T>(DeviceContext context, IList<T> data) where T : struct
        {
            UploadDataToBuffer<T>(context, data, data.Count);
        }

        /// <summary>
        /// <see cref="IBufferProxy.UploadDataToBuffer{T}(DeviceContext, IList{T}, int)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        public void UploadDataToBuffer<T>(DeviceContext context, IList<T> data, int count) where T : struct
        {
            UploadDataToBuffer<T>(context, data, count, 0);
        }

        /// <summary>
        /// <see cref="IBufferProxy.UploadDataToBuffer{T}(DeviceContext, ref T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        public void UploadDataToBuffer<T>(DeviceContext context, ref T data) where T : struct
        {
            UploadDataToBuffer<T>(context, ref data, 0);
        }

        /// <summary>
        /// <see cref="IBufferProxy.CreateBufferFromDataArray{T}(Device, IList{T})"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        public void CreateBufferFromDataArray<T>(Device context, IList<T> data) where T : struct
        {
            CreateBufferFromDataArray<T>(context, data, data.Count);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Disposer.RemoveAndDispose(ref buffer);
        }
    }

    public sealed class UAVBufferViewProxy : IDisposable
    {
        private SDX11.Buffer buffer;
        public UnorderedAccessView uav;
        public ShaderResourceView srv;

        /// <summary>
        /// Get UnorderedAccessView
        /// </summary>
        public UnorderedAccessView UAV { get { return uav; } }

        /// <summary>
        /// Get ShaderResourceView
        /// </summary>
        public ShaderResourceView SRV { get { return srv; } }

        public UAVBufferViewProxy(Device device, ref BufferDescription bufferDesc, ref UnorderedAccessViewDescription uavDesc, ref ShaderResourceViewDescription srvDesc)
        {
            buffer = new SDX11.Buffer(device, bufferDesc);
            srv = new ShaderResourceView(device, buffer);
            uav = new UnorderedAccessView(device, buffer, uavDesc);
        }

        public void CopyCount(DeviceContext device, SDX11.Buffer destBuffer, int offset)
        {
            device.CopyStructureCount(destBuffer, offset, UAV);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disposer.RemoveAndDispose(ref uav);
                    Disposer.RemoveAndDispose(ref srv);
                    Disposer.RemoveAndDispose(ref buffer);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BufferViewProxy() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
