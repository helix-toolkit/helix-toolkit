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
    using Shaders;
    /// <summary>
    /// 
    /// </summary>
    public interface IConstantBufferProxy : IBufferProxy
    {
        bool Initialized { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
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
        /// <param name="count"></param>
        void UploadDataToBuffer<T>(DeviceContext context, T[] data, int count) where T : struct;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="offset"></param>
        void UploadDataToBuffer<T>(DeviceContext context, T[] data, int count, int offset) where T : struct;
        /// <summary>
        /// Use external write function to upload data
        /// </summary>
        /// <param name="context"></param>
        /// <param name="writeFunc"></param>
        void UploadDataToBuffer(DeviceContext context, Action<DataStream> writeFunc);
        /// <summary>
        /// Disposes the buffer. Reuse the object
        /// </summary>
        void DisposeAndClear();
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConstantBufferProxy : BufferProxyBase, IConstantBufferProxy
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Initialized { get { return buffer != null; } }
        private BufferDescription bufferDesc;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="structSize"></param>
        /// <param name="bindFlags"></param>
        /// <param name="cpuAccessFlags"></param>
        /// <param name="optionFlags"></param>
        /// <param name="usage"></param>
        /// <param name="strideSize"></param>
        public ConstantBufferProxy(int structSize, BindFlags bindFlags = BindFlags.ConstantBuffer,
            CpuAccessFlags cpuAccessFlags = CpuAccessFlags.None, ResourceOptionFlags optionFlags = ResourceOptionFlags.None, ResourceUsage usage = ResourceUsage.Default, int strideSize = 0)
            : base(structSize, bindFlags)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        public ConstantBufferProxy(ConstantBufferDescription description)
            : base(description.StructSize, description.BindFlags)
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

        /// <summary>
        /// <see cref="IConstantBufferProxy.CreateBuffer(Device)"/>
        /// </summary>
        /// <param name="device"></param>
        public void CreateBuffer(Device device)
        {
            buffer = Collect(new SDX11.Buffer(device, bufferDesc));
        }

        /// <summary>
        /// <see cref="IConstantBufferProxy.UploadDataToBuffer{T}(DeviceContext, ref T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        public void UploadDataToBuffer<T>(DeviceContext context, ref T data) where T : struct
        {
            UploadDataToBuffer<T>(context, ref data, 0);
        }
        /// <summary>
        /// <see cref="IConstantBufferProxy.UploadDataToBuffer{T}(DeviceContext, ref T, int)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        public void UploadDataToBuffer<T>(DeviceContext context, ref T data, int offset) where T : struct
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
        /// <summary>
        /// <see cref="IConstantBufferProxy.UploadDataToBuffer{T}(DeviceContext, T[], int)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        public void UploadDataToBuffer<T>(DeviceContext context, T[] data, int count) where T : struct
        {
            UploadDataToBuffer<T>(context, data, count, 0);
        }
        /// <summary>
        /// <see cref="IConstantBufferProxy.UploadDataToBuffer{T}(DeviceContext, T[], int, int)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="offset"></param>
        public void UploadDataToBuffer<T>(DeviceContext context, T[] data, int count, int offset) where T : struct
        {
            if (buffer.Description.Usage == ResourceUsage.Dynamic)
            {
                DataStream stream;
                context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None, out stream);
                using (stream)
                {
                    stream.WriteRange(data, offset, count);
                    context.UnmapSubresource(buffer, 0);
                }
            }
            else
            {
                context.UpdateSubresource(data, buffer);
            }
        }
        /// <summary>
        /// <see cref="IConstantBufferProxy.UploadDataToBuffer(DeviceContext, Action{DataStream})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="writeFuc"></param>
        public void UploadDataToBuffer(DeviceContext context, Action<DataStream> writeFuc)
        {
            if (buffer.Description.Usage == ResourceUsage.Dynamic)
            {
                DataStream stream;
                context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None, out stream);
                using (stream)
                {
                    writeFuc?.Invoke(stream);
                    context.UnmapSubresource(buffer, 0);
                }
            }
            else
            {
                throw new Exception("Constant buffer must be dynamic to use this function.");
            }
        }


        /// <summary>
        /// Special function to recreate existing constant buffer to new size.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="structSize"></param>
        public void ResizeBuffer(Device device, int structSize)
        {
            if (structSize % 16 != 0)
            {
                throw new ArgumentException("Constant buffer struct size must be multiple of 16 bytes");
            }
            RemoveAndDispose(ref buffer);
            bufferDesc.SizeInBytes = structSize;            
            buffer = Collect(new SDX11.Buffer(device, bufferDesc));
        }

        public static implicit operator SDX11.Buffer(ConstantBufferProxy proxy)
        {
            return proxy == null ? null : proxy.buffer;
        }
    }
}
