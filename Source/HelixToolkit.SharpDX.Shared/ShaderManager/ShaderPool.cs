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

    public class ShaderPool : ResourcePoolBase<byte[], IShader, ShaderDescription>
    {
        public IConstantBufferPool ConstantBufferPool { private set; get; }
        public ShaderPool(Device device, IConstantBufferPool cbPool)
            :base(device)
        {
            ConstantBufferPool = cbPool;
        }

        protected override byte[] GetKey(ref ShaderDescription description)
        {
            return description.ByteCode;
        }

        protected override IShader Create(Device device, ref ShaderDescription description)
        {
            return description.ByteCode == null ? new NullShader(description.ShaderType) : description.CreateShader(device, ConstantBufferPool);
        }
    }

    public class LayoutPool : ResourcePoolBase<byte[], InputLayout, KeyValuePair<byte[], InputElement[]>>
    {
        public LayoutPool(Device device)
            :base(device)
        { }

        protected override InputLayout Create(Device device, ref KeyValuePair<byte[], InputElement[]> description)
        {
            return description.Key == null || description.Value == null ? null : new InputLayout(Device, description.Key, description.Value);
        }

        protected override byte[] GetKey(ref KeyValuePair<byte[], InputElement[]> description)
        {
            return description.Key;
        }
    }

    public class ShaderPoolManager : DisposeObject, IShaderPoolManager
    {
        private readonly Dictionary<ShaderStage, ShaderPool> shaderPools = new Dictionary<ShaderStage, ShaderPool>();
        private readonly LayoutPool layoutPool;

        public ShaderPoolManager(Device device, IConstantBufferPool cbPool)
        {
            shaderPools.Add(ShaderStage.Vertex, Collect(new ShaderPool(device, cbPool)));
            shaderPools.Add(ShaderStage.Domain, Collect(new ShaderPool(device, cbPool)));
            shaderPools.Add(ShaderStage.Hull, Collect(new ShaderPool(device, cbPool)));
            shaderPools.Add(ShaderStage.Geometry, Collect(new ShaderPool(device, cbPool)));
            shaderPools.Add(ShaderStage.Pixel, Collect(new ShaderPool(device, cbPool)));
            shaderPools.Add(ShaderStage.Compute, Collect(new ShaderPool(device, cbPool)));
            layoutPool = Collect(new LayoutPool(device));
        }

        public IShader RegisterShader(ShaderDescription description)
        {
            return shaderPools[description.ShaderType].Register(description);
        }

        public InputLayout RegisterInputLayout(InputLayoutDescription description)
        {
            return layoutPool.Register(description.Description);
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            shaderPools.Clear();
            base.Dispose(disposeManagedResources);
        }       
    }
}
