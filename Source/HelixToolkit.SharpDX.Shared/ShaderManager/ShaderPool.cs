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
        using Shaders;
        using Utilities;
        /// <summary>
        /// Pool to store and share shaders. Do not dispose shader object externally.
        /// </summary>
        public sealed class ShaderPool : ReferenceCountedDictionaryPool<byte[], ShaderBase, ShaderDescription>
        {
            /// <summary>
            /// Gets or sets the constant buffer pool.
            /// </summary>
            /// <value>
            /// The constant buffer pool.
            /// </value>
            public IConstantBufferPool ConstantBufferPool
            {
                private set; get;
            }

            private readonly Device device;
            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderPool"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="cbPool">The cb pool.</param>
            public ShaderPool(Device device, IConstantBufferPool cbPool)
                : base(false)
            {
                ConstantBufferPool = cbPool;
                this.device = device;
            }

            protected override bool CanCreate(ref byte[] key, ref ShaderDescription argument)
            {
                return key != null && key.Length > 0;
            }

            protected override ShaderBase OnCreate(ref byte[] key, ref ShaderDescription description)
            {
                return description.ByteCode == null ?
                    Constants.GetNullShader(description.ShaderType) : description.CreateShader(device, ConstantBufferPool);
            }
        }
        /// <summary>
        /// Pool to store and share shader layouts. Do not dispose layout object externally.
        /// </summary>
        public sealed class LayoutPool : ReferenceCountedDictionaryPool<byte[], InputLayoutProxy, InputLayoutDescription>
        {
            private readonly Device device;
            /// <summary>
            /// Initializes a new instance of the <see cref="LayoutPool"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="logger"></param>
            public LayoutPool(Device device)
                : base(false)
            {
                this.device = device;
            }

            protected override bool CanCreate(ref byte[] key, ref InputLayoutDescription argument)
            {
                return key != null && key.Length > 0;
            }

            protected override InputLayoutProxy OnCreate(ref byte[] key, ref InputLayoutDescription description)
            {
                return new InputLayoutProxy(device, description.ShaderByteCode, description.InputElements);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public class ShaderPoolManager : DisposeObject, IShaderPoolManager
        {
            private readonly ShaderPool[] shaderPools = new ShaderPool[Constants.NumShaderStages];
            private LayoutPool layoutPool;
            /// <summary>
            /// Initializes a new instance of the <see cref="ShaderPoolManager"/> class.
            /// </summary>
            /// <param name="device">The device.</param>
            /// <param name="cbPool">The cb pool.</param>
            public ShaderPoolManager(Device device, IConstantBufferPool cbPool)
            {
                shaderPools[Constants.VertexIdx] = new ShaderPool(device, cbPool);
                shaderPools[Constants.DomainIdx] = new ShaderPool(device, cbPool);
                shaderPools[Constants.HullIdx] = new ShaderPool(device, cbPool);
                shaderPools[Constants.GeometryIdx] = new ShaderPool(device, cbPool);
                shaderPools[Constants.PixelIdx] = new ShaderPool(device, cbPool);
                shaderPools[Constants.ComputeIdx] = new ShaderPool(device, cbPool);
                layoutPool = new LayoutPool(device);
            }
            /// <summary>
            /// Registers the shader.
            /// </summary>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            public ShaderBase RegisterShader(ShaderDescription description)
            {
                if (description == null)
                {
                    return null;
                }
                return shaderPools[description.ShaderType.ToIndex()].TryCreateOrGet(description.ByteCode, description, out var shader)
                    ? shader
                    : null;
            }
            /// <summary>
            /// Registers the input layout.
            /// </summary>
            /// <param name="description">The description.</param>
            /// <returns></returns>
            public InputLayoutProxy RegisterInputLayout(InputLayoutDescription description)
            {
                if (description == null)
                {
                    return null;
                }
                return layoutPool.TryCreateOrGet(description.ShaderByteCode, description, out var inputLayout) ? inputLayout : null;
            }
            /// <summary>
            /// Called when [dispose].
            /// </summary>
            /// <param name="disposeManagedResources">if set to <c>true</c> [dispose managed resources].</param>
            protected override void OnDispose(bool disposeManagedResources)
            {
                for (var i = 0; i < shaderPools.Length; ++i)
                {
                    RemoveAndDispose(ref shaderPools[i]);
                }
                RemoveAndDispose(ref layoutPool);
                base.OnDispose(disposeManagedResources);
            }
        }
    }
}
