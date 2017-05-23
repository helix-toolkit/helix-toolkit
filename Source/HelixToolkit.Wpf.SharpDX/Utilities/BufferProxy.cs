using SDX11 = SharpDX.Direct3D11;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Utilities
{
    public class DynamicBufferProxy<T> : BufferProxyBase<T> where T : struct
    {
        private readonly BindFlags bindFlags;
        private readonly ResourceOptionFlags optionFlags = ResourceOptionFlags.None;
        private readonly int structureSize;
        public int StructureSize { get { return structureSize; } }
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
                var buffdesc = new BufferDescription()
                {
                    BindFlags = this.bindFlags,
                    CpuAccessFlags = CpuAccessFlags.Write,
                    OptionFlags = this.optionFlags,
                    SizeInBytes = structureSize * data.Count,
                    StructureByteStride = structureSize,
                    Usage = ResourceUsage.Dynamic
                };
                Disposer.RemoveAndDispose(ref buffer);
                buffer = SDX11.Buffer.Create(context.Device, data.ToArray(), buffdesc);
            }
            else
            {
                DataStream stream;
                context.MapSubresource(this.buffer, MapMode.WriteDiscard, MapFlags.None, out stream);
                stream.Position = 0;
                stream.WriteRange(data.ToArray(), 0, data.Count);
                context.UnmapSubresource(this.buffer, 0);
                stream.Dispose();
            }
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

        public override void UploadDataToBuffer(DeviceContext context, ref T data)
        {
            context.UpdateSubresource(ref data, buffer);
        }

        public override void UploadDataToBuffer(DeviceContext context, IList<T> data)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class BufferProxyBase<T> : IDisposable where T : struct  
    {
        protected SDX11.Buffer buffer;

        public SDX11.Buffer Buffer { get { return buffer; } }

        public abstract void UploadDataToBuffer(DeviceContext context, IList<T> data);

        public abstract void UploadDataToBuffer(DeviceContext context, ref T data);

        public void Dispose()
        {
            Disposer.RemoveAndDispose(ref buffer);
        }
    }
}
