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
        public ImmutableBufferProxy(int structureSize, BindFlags bindFlags, ResourceOptionFlags optionFlags = ResourceOptionFlags.None) : base(structureSize, bindFlags, optionFlags)
        {
        }

        public override void UploadDataToBuffer(DeviceContext context, ref T data)
        {
            throw new NotImplementedException();
        }

        public override void UploadDataToBuffer(DeviceContext context, IList<T> data)
        {
            CreateBufferFromDataArray(context.Device, data);
        }

        public override void CreateBufferFromDataArray(Device device, IList<T> data)
        {
            var buffdesc = new BufferDescription()
            {
                BindFlags = this.bindFlags,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = this.optionFlags,
                SizeInBytes = structureSize * data.Count,
                StructureByteStride = structureSize,
                Usage = ResourceUsage.Immutable
            };
            Disposer.RemoveAndDispose(ref buffer);
            buffer = SDX11.Buffer.Create(device, data.GetArrayByType(), buffdesc);
        }
    }

    public class DynamicBufferProxy<T> : BufferProxyBase<T> where T : struct
    {
        protected readonly BindFlags bindFlags;
        protected readonly ResourceOptionFlags optionFlags = ResourceOptionFlags.None;
        protected readonly int structureSize;
        public override int StructureSize { get { return structureSize; } }
        public DynamicBufferProxy(int structureSize, BindFlags bindFlags, ResourceOptionFlags optionFlags = ResourceOptionFlags.None)
        {
            this.bindFlags = bindFlags;
            this.optionFlags = optionFlags;
            this.structureSize = structureSize;
        }

        public override void UploadDataToBuffer(DeviceContext context, ref T data)
        {
            throw new NotImplementedException();
        }

        public override void UploadDataToBuffer(DeviceContext context, IList<T> data)
        {
            if (buffer == null || buffer.Description.SizeInBytes < structureSize * data.Count)
            {
                CreateBufferFromDataArray(context.Device, data);
            }
            else
            {
                DataStream stream;
                context.MapSubresource(this.buffer, MapMode.WriteDiscard, MapFlags.None, out stream);
                stream.Position = 0;
                stream.WriteRange(data.GetArrayByType(), 0, data.Count);
                context.UnmapSubresource(this.buffer, 0);
                stream.Dispose();
            }
        }

        public override void CreateBufferFromDataArray(Device device, IList<T> data)
        {
            Disposer.RemoveAndDispose(ref buffer);
            var buffdesc = new BufferDescription()
            {
                BindFlags = this.bindFlags,
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = this.optionFlags,
                SizeInBytes = structureSize * data.Count,
                StructureByteStride = structureSize,
                Usage = ResourceUsage.Dynamic
            };           
            buffer = SDX11.Buffer.Create(device, data.GetArrayByType(), buffdesc);
        }
    }

    public class ConstantBufferProxy<T> : BufferProxyBase<T> where T : struct
    {
        public ConstantBufferProxy(Device device, int structSize)
        {
            buffer = new SDX11.Buffer(device, new BufferDescription()
            {
                SizeInBytes = structSize,
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default,
                StructureByteStride = 0
            });
        }

        public override void CreateBufferFromDataArray(Device context, IList<T> data)
        {
            throw new NotImplementedException();
        }

        public override void UploadDataToBuffer(DeviceContext context, ref T data)
        {
            context.UpdateSubresource(ref data, buffer);
        }

        public override void UploadDataToBuffer(DeviceContext context, IList<T> data)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class BufferProxyBase<T> : IBufferProxy where T : struct  
    {
        protected SDX11.Buffer buffer;
        public virtual int StructureSize { get; }
        public SDX11.Buffer Buffer { get { return buffer; } }

        public abstract void UploadDataToBuffer(DeviceContext context, IList<T> data);
        public abstract void CreateBufferFromDataArray(Device device, IList<T> data);
        public abstract void UploadDataToBuffer(DeviceContext context, ref T data);

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
