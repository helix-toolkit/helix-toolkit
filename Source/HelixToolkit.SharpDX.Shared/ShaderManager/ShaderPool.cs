/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Text;
using global::SharpDX.Direct3D11;
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
    namespace ShaderManager
    {
        using HelixToolkit.Logger;
        using Shaders;
        /// <summary>
        /// Pool to store and share shaders. Do not dispose shader object externally.
        /// </summary>
        public sealed class ShaderPool : LongLivedResourcePoolBase<byte[], ShaderBase, ShaderDescription>
        {
            /// <summary>
            /// Gets or sets the constant buffer pool.
            /// </summary>
            /// <value>
            /// The constant buffer pool.
            /// </value>
            public IConstantBufferPool ConstantBufferPool { private set; get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderPool"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="cbPool">The cb pool.</param>
            /// <param name="logger">The logger.</param>
            public ShaderPool(Device device, IConstantBufferPool cbPool, LogWrapper logger)
                :base(device, logger)
            {
                ConstantBufferPool = cbPool;
            }
            /// <summary>
            /// Gets the key.
            /// </summary>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override byte[] GetKey(ref ShaderDescription description)
            {
                return description.ByteCode;
            }
            /// <summary>
            /// Creates the specified device.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override ShaderBase Create(Device device, ref ShaderDescription description)
            {
                return description.ByteCode == null ? Constants.GetNullShader(description.ShaderType) : description.CreateShader(device, ConstantBufferPool, logger);
            }
        }
        /// <summary>
        /// Pool to store and share shader layouts. Do not dispose layout object externally.
        /// </summary>
        public sealed class LayoutPool : LongLivedResourcePoolBase<byte[], InputLayout, KeyValuePair<byte[], InputElement[]>>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LayoutPool"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="logger"></param>
            public LayoutPool(Device device, LogWrapper logger)
                :base(device, logger)
            { }
            /// <summary>
            /// Creates the specified device.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override InputLayout Create(Device device, ref KeyValuePair<byte[], InputElement[]> description)
            {
                return description.Key == null || description.Value == null ? null : new InputLayout(Device, description.Key, description.Value);
            }
            /// <summary>
            /// Gets the key.
            /// </summary>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            protected override byte[] GetKey(ref KeyValuePair<byte[], InputElement[]> description)
            {
                return description.Key;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public class ShaderPoolManager : DisposeObject, IShaderPoolManager
        {
            private readonly ShaderPool[] shaderPools = new ShaderPool[Constants.NumShaderStages];
            private readonly LayoutPool layoutPool;
            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderPoolManager"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="cbPool">The cb pool.</param>
            /// <param name="logger"></param>
            public ShaderPoolManager(Device device, IConstantBufferPool cbPool, LogWrapper logger)
            {
                shaderPools[Constants.VertexIdx] = Collect(new ShaderPool(device, cbPool, logger));
                shaderPools[Constants.DomainIdx] = Collect(new ShaderPool(device, cbPool, logger));
                shaderPools[Constants.HullIdx] = Collect(new ShaderPool(device, cbPool, logger));
                shaderPools[Constants.GeometryIdx] = Collect(new ShaderPool(device, cbPool, logger));
                shaderPools[Constants.PixelIdx] = Collect(new ShaderPool(device, cbPool, logger));
                shaderPools[Constants.ComputeIdx] = Collect(new ShaderPool(device, cbPool, logger));
                layoutPool = Collect(new LayoutPool(device, logger));
            }
            /// <summary>
            /// Registers the shader.
            /// </summary>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            public ShaderBase RegisterShader(ShaderDescription description)
            {
                return description == null ? null : shaderPools[description.ShaderType.ToIndex()].Register(description);
            }
            /// <summary>
            /// Registers the input layout.
            /// </summary>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            public InputLayout RegisterInputLayout(InputLayoutDescription description)
            {
                return description == null ? null : layoutPool.Register(description.Description);
            }
            /// <summary>
            /// Called when [dispose].
            /// </summary>
            /// <param name="disposeManagedResources">if set to <c>true</c> [dispose managed resources].</param>
            protected override void OnDispose(bool disposeManagedResources)
            {
                for(int i=0; i < shaderPools.Length; ++i)
                {
                    shaderPools[i] = null;
                }
                base.OnDispose(disposeManagedResources);
            }       
        }
    }
    

}
