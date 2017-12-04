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

    public interface IBufferProxy<T> : IBufferProxy where T : struct
    {
        void UploadDataToBuffer(DeviceContext context, ref T data);
        void UploadDataToBuffer(DeviceContext context, IList<T> data);
        void CreateBuffer(Device device);
        void CreateBufferFromDataArray(Device context, IList<T> data);
        void CreateBufferFromDataArray(Device context, IList<T> data, int count);
    }

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
    }

    public class ImmutableBufferProxy<T> : DynamicBufferProxy<T> where T : struct
    {
        public ImmutableBufferProxy(int structureSize, BindFlags bindFlags, ResourceOptionFlags optionFlags = ResourceOptionFlags.None) 
            : base(structureSize, bindFlags, optionFlags)
        {
        }

        public override void UploadDataToBuffer(DeviceContext context, IList<T> data, int length)
        {
            CreateBufferFromDataArray(context.Device, data, length);
        }

        public override void CreateBufferFromDataArray(Device device, IList<T> data, int length)
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

    public class DynamicBufferProxy<T> : BufferProxyBase<T> where T : struct
    {
        public ResourceOptionFlags OptionFlags { private set; get; }

        public DynamicBufferProxy(int structureSize, BindFlags bindFlags, ResourceOptionFlags optionFlags = ResourceOptionFlags.None)
            :base(structureSize, bindFlags)
        {
            this.OptionFlags = optionFlags;
        }

        public virtual void UploadDataToBuffer(DeviceContext context, IList<T> data, int length)
        {
            Count = length;
            if (buffer == null || buffer.Description.SizeInBytes < StructureSize * length)
            {
                CreateBufferFromDataArray(context.Device, data, length);
            }
            else
            {
                DataStream stream;
                context.MapSubresource(this.buffer, MapMode.WriteDiscard, MapFlags.None, out stream);
                stream.Position = 0;
                stream.WriteRange(data.GetArrayByType(), 0, length);
                context.UnmapSubresource(this.buffer, 0);
                stream.Dispose();
            }
        }

        public override void CreateBufferFromDataArray(Device device, IList<T> data, int length)
        {
            Disposer.RemoveAndDispose(ref buffer);
            var buffdesc = new BufferDescription()
            {
                BindFlags = this.BindFlags,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = this.OptionFlags,
                SizeInBytes = StructureSize * length,
                StructureByteStride = StructureSize,
                Usage = ResourceUsage.Dynamic
            };           
            buffer = SDX11.Buffer.Create(device, data.GetArrayByType(), buffdesc);
            Count = length;
        }

        public override void CreateBufferFromDataArray(Device context, IList<T> data)
        {
            CreateBufferFromDataArray(context, data, data.Count);
        }
        public override void UploadDataToBuffer(DeviceContext context, IList<T> data)
        {
            UploadDataToBuffer(context, data, data.Count);
        }

        public override void UploadDataToBuffer(DeviceContext context, ref T data)
        {
            UploadDataToBuffer(context, new[] { data }, 1);
        }

        public override void CreateBuffer(Device device)
        {
            throw new ArgumentException("Method does not supported.");
        }
    }

    public class ConstantBufferProxy<T> : BufferProxyBase<T> where T : struct
    {
        private readonly BufferDescription bufferDesc;
        public ConstantBufferProxy(int structSize, BindFlags bindFlags = BindFlags.ConstantBuffer, 
            CpuAccessFlags cpuAccessFlags = CpuAccessFlags.None, ResourceOptionFlags optionFlags = ResourceOptionFlags.None, ResourceUsage usage = ResourceUsage.Default)
            :base(structSize, bindFlags)
        {
            bufferDesc = new BufferDescription()
            {
                SizeInBytes = structSize,
                BindFlags = bindFlags,
                CpuAccessFlags = cpuAccessFlags,
                OptionFlags = optionFlags,
                Usage = usage,
                StructureByteStride = 0
            };
        }

        public override void UploadDataToBuffer(DeviceContext context, ref T data)
        {
            context.UpdateSubresource(ref data, buffer);
        }

        public override void CreateBuffer(Device device)
        {
            buffer = new SDX11.Buffer(device, bufferDesc);
        }

        public override void CreateBufferFromDataArray(Device context, IList<T> data)
        {
            throw new ArgumentException("Constant Buffer does not support data array.");
        }
        public override void CreateBufferFromDataArray(Device context, IList<T> data, int count)
        {
            throw new ArgumentException("Constant Buffer does not support data array.");
        }
        public override void UploadDataToBuffer(DeviceContext context, IList<T> data)
        {
            throw new ArgumentException("Constant Buffer does not support data array.");
        }
    }

    public abstract class BufferProxyBase<T> : IBufferProxy<T> where T : struct  
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

        public abstract void UploadDataToBuffer(DeviceContext context, ref T data);

        public abstract void UploadDataToBuffer(DeviceContext context, IList<T> data);

        public abstract void CreateBuffer(Device device);

        public abstract void CreateBufferFromDataArray(Device context, IList<T> data);
        public abstract void CreateBufferFromDataArray(Device context, IList<T> data, int count);
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
