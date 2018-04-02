/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.ShaderManager
#else
namespace HelixToolkit.UWP.ShaderManager
#endif
{
    using global::SharpDX.Direct3D11;
    using Shaders;
    /// <summary>
    /// Pool to store and share shaders. Do not dispose shader object externally.
    /// </summary>
    public sealed class ShaderPool : ResourcePoolBase<byte[], ShaderBase, ShaderDescription>
    {
        /// <summary>
        /// Gets or sets the constant buffer pool.
        /// </summary>
        /// <value>
        /// The constant buffer pool.
        /// </value>
        public IConstantBufferPool ConstantBufferPool { private set; get; }
        public ShaderPool(Device device, IConstantBufferPool cbPool)
            :base(device)
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
            return description.ByteCode == null ? NullShader.GetNullShader(description.ShaderType) : description.CreateShader(device, ConstantBufferPool);
        }
    }
    /// <summary>
    /// Pool to store and share shader layouts. Do not dispose layout object externally.
    /// </summary>
    public sealed class LayoutPool : ResourcePoolBase<byte[], InputLayout, KeyValuePair<byte[], InputElement[]>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPool"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public LayoutPool(Device device)
            :base(device)
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
        public ShaderPoolManager(Device device, IConstantBufferPool cbPool)
        {
            shaderPools[ShaderStage.Vertex.ToIndex()] = Collect(new ShaderPool(device, cbPool));
            shaderPools[ShaderStage.Domain.ToIndex()] = Collect(new ShaderPool(device, cbPool));
            shaderPools[ShaderStage.Hull.ToIndex()] = Collect(new ShaderPool(device, cbPool));
            shaderPools[ShaderStage.Geometry.ToIndex()] = Collect(new ShaderPool(device, cbPool));
            shaderPools[ShaderStage.Pixel.ToIndex()] = Collect(new ShaderPool(device, cbPool));
            shaderPools[ShaderStage.Compute.ToIndex()] = Collect(new ShaderPool(device, cbPool));
            layoutPool = Collect(new LayoutPool(device));
        }
        /// <summary>
        /// Registers the shader.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public ShaderBase RegisterShader(ShaderDescription description)
        {
            return shaderPools[description.ShaderType.ToIndex()].Register(description);
        }
        /// <summary>
        /// Registers the input layout.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public InputLayout RegisterInputLayout(InputLayoutDescription description)
        {
            return layoutPool.Register(description.Description);
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
