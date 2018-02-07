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
    using Utilities;
    using Shaders;
    using global::SharpDX.Direct3D11;
    using System;

    public interface IConstantBufferPool
    {
        Device Device { get; }
        IConstantBufferProxy Register(ConstantBufferDescription description);
        IConstantBufferProxy Register(string name, int structSize);
    }

    public class ConstantBufferPool : ResourcePoolBase<string, IConstantBufferProxy, ConstantBufferDescription>, IConstantBufferPool
    {
        public ConstantBufferPool(Device device)
            :base(device)
        {

        }

        protected override IConstantBufferProxy Create(Device device, ref ConstantBufferDescription description)
        {
            var buffer = description.CreateBuffer();
            buffer.CreateBuffer(device);
            return buffer;
        }

        protected override string GetKey(ref ConstantBufferDescription description)
        {
            return description.Name + description.StructSize;
        }

        public IConstantBufferProxy Register(string name, int structSize)
        {
            return Register(new ConstantBufferDescription(name, structSize));
        }
    }
}
