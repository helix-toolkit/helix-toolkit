/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.Direct3D11;
using System;

using SDX11 = SharpDX.Direct3D11;
#if !NETFX_CORE

namespace HelixToolkit.Wpf.SharpDX.Utilities
#else
namespace HelixToolkit.UWP.Utilities
#endif
{
    using Shaders;
    using System.Runtime.CompilerServices;
    using Render;

    /// <summary>
    ///
    /// </summary>
    public sealed class ConstantBufferProxy : BufferProxyBase
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
            CpuAccessFlags cpuAccessFlags = CpuAccessFlags.None, ResourceOptionFlags optionFlags = ResourceOptionFlags.None,
            ResourceUsage usage = ResourceUsage.Default, int strideSize = 0)
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
        /// <see cref="ConstantBufferProxy.CreateBuffer(Device)"/>
        /// </summary>
        /// <param name="device"></param>
        public void CreateBuffer(Device device)
        {
            RemoveAndDispose(ref buffer);
            buffer = Collect(new SDX11.Buffer(device, bufferDesc));
        }

        /// <summary>
        /// <see cref="ConstantBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, ref T)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UploadDataToBuffer<T>(DeviceContextProxy context, ref T data) where T : struct
        {
            UploadDataToBuffer<T>(context, ref data, 0);
        }

        /// <summary>
        /// <see cref="ConstantBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, ref T, int)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UploadDataToBuffer<T>(DeviceContextProxy context, ref T data, int offset) where T : struct
        {
            if (bufferDesc.Usage == ResourceUsage.Dynamic)
            {
                context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                using (stream)
                {
                    stream.Write(data);                   
                }
                context.UnmapSubresource(buffer, 0);
            }
            else
            {
                context.UpdateSubresource(ref data, buffer);
            }
        }

        /// <summary>
        /// <see cref="ConstantBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, T[], int)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UploadDataToBuffer<T>(DeviceContextProxy context, T[] data, int count) where T : struct
        {
            UploadDataToBuffer<T>(context, data, count, 0);
        }

        /// <summary>
        /// <see cref="ConstantBufferProxy.UploadDataToBuffer{T}(DeviceContextProxy, T[], int, int)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="offset"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UploadDataToBuffer<T>(DeviceContextProxy context, T[] data, int count, int offset) where T : struct
        {
            if (bufferDesc.Usage == ResourceUsage.Dynamic)
            {
                context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                using (stream)
                {
                    stream.WriteRange(data, offset, count);                   
                }
                context.UnmapSubresource(buffer, 0);
            }
            else
            {
                context.UpdateSubresource(data, buffer);
            }
        }

        /// <summary>
        /// <see cref="ConstantBufferProxy.UploadDataToBuffer(DeviceContextProxy, Action{DataStream})"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="writeFuc"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UploadDataToBuffer(DeviceContextProxy context, Action<DataStream> writeFuc)
        {
            if (bufferDesc.Usage == ResourceUsage.Dynamic)
            {
                context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
                using (stream)
                {
                    writeFuc?.Invoke(stream);                    
                }
                context.UnmapSubresource(buffer, 0);
            }
            else
            {
#if DEBUG
                throw new Exception("Constant buffer must be dynamic to use this function.");
#endif
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DataStream Map(DeviceContextProxy context)
        {
            context.MapSubresource(buffer, 0, MapMode.WriteDiscard, MapFlags.None, out DataStream stream);
            return stream;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unmap(DeviceContextProxy context)
        {
            context.UnmapSubresource(buffer, 0);
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
            return proxy?.buffer;
        }
    }
}