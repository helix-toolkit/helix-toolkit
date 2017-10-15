using SDX11 = SharpDX.Direct3D11;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using HelixToolkit.Wpf.SharpDX.Extensions;

namespace HelixToolkit.Wpf.SharpDX.Utilities
{
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
                BindFlags = this.bindFlags,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = this.optionFlags,
                SizeInBytes = StructureSize * length,
                StructureByteStride = StructureSize,
                Usage = ResourceUsage.Immutable
            };
            buffer = SDX11.Buffer.Create(device, data.GetArrayByType(), buffdesc);
        }
    }

    public class DynamicBufferProxy<T> : BufferProxyBase<T> where T : struct
    {
        protected readonly BindFlags bindFlags;
        protected readonly ResourceOptionFlags optionFlags = ResourceOptionFlags.None;

        public DynamicBufferProxy(int structureSize, BindFlags bindFlags, ResourceOptionFlags optionFlags = ResourceOptionFlags.None)
            :base(structureSize)
        {
            this.bindFlags = bindFlags;
            this.optionFlags = optionFlags;
        }

        public virtual void UploadDataToBuffer(DeviceContext context, IList<T> data, int length)
        {
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

        public virtual void CreateBufferFromDataArray(Device device, IList<T> data, int length)
        {
            Disposer.RemoveAndDispose(ref buffer);
            var buffdesc = new BufferDescription()
            {
                BindFlags = this.bindFlags,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = this.optionFlags,
                SizeInBytes = StructureSize * length,
                StructureByteStride = StructureSize,
                Usage = ResourceUsage.Dynamic
            };           
            buffer = SDX11.Buffer.Create(device, data.GetArrayByType(), buffdesc);
        }

        public void CreateBufferFromDataArray(Device context, IList<T> data)
        {
            CreateBufferFromDataArray(context, data, data.Count);
        }
        public void UploadDataToBuffer(DeviceContext context, IList<T> data)
        {
            UploadDataToBuffer(context, data, data.Count);
        }
    }

    public class ConstantBufferProxy<T> : BufferProxyBase<T> where T : struct
    {
        private readonly BufferDescription bufferDesc;
        public ConstantBufferProxy(int structSize, BindFlags bindFlags = BindFlags.ConstantBuffer, 
            CpuAccessFlags cpuAccessFlags = CpuAccessFlags.None, ResourceOptionFlags optionFlags = ResourceOptionFlags.None, ResourceUsage usage = ResourceUsage.Default)
            :base(structSize)
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

        public void UploadDataToBuffer(DeviceContext context, ref T data)
        {
            context.UpdateSubresource(ref data, buffer);
        }

        public void CreateBuffer(Device device)
        {
            buffer = new SDX11.Buffer(device, bufferDesc);
        }
    }

    public abstract class BufferProxyBase<T> : IBufferProxy where T : struct  
    {
        protected SDX11.Buffer buffer;
        public int StructureSize { get; private set; }
        public SDX11.Buffer Buffer { get { return buffer; } }

        public BufferProxyBase(int structureSize)
        {
            StructureSize = structureSize;
        }
        public void Dispose()
        {
            Disposer.RemoveAndDispose(ref buffer);
        }
    }

    public interface IBufferProxy : IDisposable
    {
        SDX11.Buffer Buffer { get; }
        int StructureSize { get; }
    }
}
