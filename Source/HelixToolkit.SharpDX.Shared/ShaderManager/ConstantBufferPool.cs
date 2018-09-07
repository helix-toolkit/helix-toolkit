/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

#if !NETFX_CORE

namespace HelixToolkit.Wpf.SharpDX.ShaderManager
#else
namespace HelixToolkit.UWP.ShaderManager
#endif
{
    using global::SharpDX.Direct3D11;
    using HelixToolkit.Logger;
    using Shaders;
    using System;
    using Utilities;

    /// <summary>
    /// Pool to store and share constant buffers. Do not dispose constant buffer object externally.
    /// </summary>
    public interface IConstantBufferPool
    {
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        int Count { get; }
        /// <summary>
        /// Gets the device.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        Device Device { get; }

        /// <summary>
        /// Registers the specified description.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        ConstantBufferProxy Register(ConstantBufferDescription description);

        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="structSize">Size of the structure.</param>
        /// <returns></returns>
        ConstantBufferProxy Register(string name, int structSize);
    }

    /// <summary>
    /// Pool to store and share constant buffers. Do not dispose constant buffer object externally.
    /// </summary>
    public sealed class ConstantBufferPool : LongLivedResourcePoolBase<string, ConstantBufferProxy, ConstantBufferDescription>, IConstantBufferPool
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantBufferPool"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="logger"></param>
        public ConstantBufferPool(Device device, LogWrapper logger)
            : base(device, logger)
        {
        }

        /// <summary>
        /// Creates the specified device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        protected override ConstantBufferProxy Create(Device device, ref ConstantBufferDescription description)
        {
            var buffer = description.CreateBuffer();
            buffer.CreateBuffer(device);
            return buffer;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        protected override string GetKey(ref ConstantBufferDescription description)
        {
            return description.Name;
        }

        protected override void ErrorCheck(ConstantBufferProxy value, ref ConstantBufferDescription description)
        {
            if (value.StructureSize != description.StructSize)
            {
                throw new ArgumentException($"Constant buffer with same name is found but their size does not match.\n" +
                    $"Name: {description.Name}. Existing Size:{value.StructureSize}; New Size:{description.StructSize}.\n" +
                    $"Potential Causes: Different Constant buffer header has been used.\n" +
                    $"Please refer and update to latest HelixToolkit.SharpDX.ShaderBuilder.CommonBuffers.hlsl. Link: https://github.com/helix-toolkit/helix-toolkit");
            }
            if (description.Variables.Count > 0)
            {
                foreach(var variable in description.Variables)
                {
                    value.AddVariable(variable);
                }
            }
        }

        /// <summary>
        /// Registers the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="structSize">Size of the structure.</param>
        /// <returns></returns>
        public ConstantBufferProxy Register(string name, int structSize)
        {
            return Register(new ConstantBufferDescription(name, structSize));
        }
    }
}