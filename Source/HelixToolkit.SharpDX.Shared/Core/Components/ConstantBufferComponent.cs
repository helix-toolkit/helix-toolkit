/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System.Runtime.CompilerServices;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Core.Components
    {
        using Render;
        using Shaders;
        using System;
        using Utilities;
        /// <summary>
        /// 
        /// </summary>
        public sealed class ConstantBufferComponent : CoreComponent
        {
            private ConstantBufferProxy modelConstBuffer;
            /// <summary>
            /// Gets or sets the model constant buffer.
            /// </summary>
            /// <value>
            /// The model constant buffer.
            /// </value>
            public ConstantBufferProxy ModelConstBuffer => modelConstBuffer;
            private readonly ConstantBufferDescription bufferDesc;
            private int storageId = -1;
            private ArrayStorage storage;
            private bool IsValid = false;
            private readonly object lck = new object();
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstantBufferComponent"/> class.
            /// </summary>
            /// <param name="desc">The desc.</param>
            public ConstantBufferComponent(ConstantBufferDescription desc)
            {
                bufferDesc = desc;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="ConstantBufferComponent"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="structSize">Size of the structure.</param>
            public ConstantBufferComponent(string name, int structSize)
            {
                bufferDesc = new ConstantBufferDescription(name, structSize);
            }

            protected override void OnAttach(IRenderTechnique technique)
            {
                lock (lck)
                {
                    if (bufferDesc != null)
                    {
                        modelConstBuffer = technique.ConstantBufferPool.Register(bufferDesc);
                        storage = technique.EffectsManager.StructArrayPool.Register(bufferDesc.StructSize);
                        storageId = storage.GetId();
                    }
                    IsValid = bufferDesc != null && ModelConstBuffer != null;
                }
            }

            protected override void OnDetach()
            {
                lock (lck)
                {
                    RemoveAndDispose(ref modelConstBuffer);
                    storage.ReleaseId(storageId);
                    RemoveAndDispose(ref storage);
                    storageId = -1;
                    IsValid = false;
                }
            }

            /// <summary>
            /// Uploads the specified device context. This uploads internal byte buffer only.
            /// </summary>
            /// <param name="deviceContext">The device context.</param>
            public bool Upload(DeviceContextProxy deviceContext)
            {
                lock (lck)
                {
                    if (IsValid && IsAttached)
                    {
                        var array = storage.GetArray();
                        var off = storage.GetOffSet(storageId);
                        ModelConstBuffer.UploadDataToBuffer(deviceContext, array, ModelConstBuffer.StructureSize, off);
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>
            /// Uploads the specified device context. This function writes a external struct and writes remains byte buffer 
            /// by offset = input struct size/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="deviceContext">The device context.</param>
            /// <param name="data">The data.</param>
            /// <returns></returns>
            public bool Upload<T>(DeviceContextProxy deviceContext, ref T data) where T : unmanaged
            {
                lock (lck)
                {
                    if (IsValid && IsAttached)
                    {
                        var structSize = UnsafeHelper.SizeOf<T>();
                        if (ModelConstBuffer.Buffer.Description.SizeInBytes < structSize)
                        {
#if DEBUG
                            throw new ArgumentOutOfRangeException($"Try to write value out of range. StructureSize {structSize}" +
                                $" > Constant Buffer Size {ModelConstBuffer.StructureSize}");
#else
                            return false;
#endif
                        }
                        var box = ModelConstBuffer.Map(deviceContext);
                        unsafe
                        {
                            var pBuf = (byte*)box.DataPointer.ToPointer();
                            *(T*)pBuf = data;
                        }
                        ModelConstBuffer.Unmap(deviceContext);
                        return true;
                    }
                    return false;
                }
            }
            /// <summary>
            /// Writes the value into internal byte buffer
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="name">The variable name.</param>
            /// <param name="value">The value.</param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void WriteValueByName<T>(string name, T value) where T : unmanaged
            {
                if (IsValid && IsAttached)
                {
                    lock (lck)
                    {
                        if (IsValid && IsAttached)
                        {
                            if (ModelConstBuffer.TryGetVariableByName(name, out var variable))
                            {
                                if (UnsafeHelper.SizeOf<T>() > variable.Size)
                                {
                                    var structSize = UnsafeHelper.SizeOf<T>();
                                    throw new ArgumentException($"Input struct size {structSize} is larger than shader variable {variable.Name} size {variable.Size}");
                                }
                                if (!storage.Write(storageId, variable.StartOffset, ref value))
                                {
                                    throw new ArgumentException($"Failed to write value on {name}");
                                }
                            }
                            else
                            {
#if DEBUG
                                throw new ArgumentException($"Variable not found in constant buffer {bufferDesc.Name}. Variable = {name}");
#else
                                Technique.EffectsManager.Logger.Log(Logger.LogLevel.Warning, $"Variable not found in constant buffer {bufferDesc.Name}. Variable = {name}");
#endif
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Writes the value into internal byte buffer
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="value">The value.</param>
            /// <param name="offset">The offset.</param>
            public void WriteValue<T>(T value, int offset) where T : unmanaged
            {
                if (IsValid && IsAttached)
                {
                    lock (lck)
                    {
                        if (IsValid && IsAttached)
                        {
                            storage.Write(storageId, offset, ref value);
                        }
                    }
                }
            }

            public bool ReadValueByName<T>(string name, out T value) where T : unmanaged
            {
                var v = default(T);
                if (IsValid && IsAttached)
                {
                    lock (lck)
                    {
                        if (IsValid && IsAttached)
                        {
                            if (ModelConstBuffer.TryGetVariableByName(name, out var variable))
                            {                                
                                return storage.Read(storageId, variable.StartOffset, out value);
                            }
                            else
                            {
#if DEBUG
                                throw new ArgumentException($"Variable not found in constant buffer {bufferDesc.Name}. Variable = {name}");
#else
                                Technique.EffectsManager.Logger.Log(Logger.LogLevel.Warning, $"Variable not found in constant buffer {bufferDesc.Name}. Variable = {name}");
                                value = v;
                                return false;
#endif
                            }
                        }
                    }
                }
                value = v;
                return false;
            }

            public bool ReadValue<T>(int offset, out T value) where T : unmanaged
            {
                var v = default(T);
                if (IsValid && IsAttached)
                {
                    lock (lck)
                    {
                        if (IsValid && IsAttached)
                        {                           
                            return storage.Read(storageId, offset, out value);
                        }
                    }
                }
                value = v;
                return false;
            }
        }
    }
}
