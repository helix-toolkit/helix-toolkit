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

    public interface IConstantBufferPool
    {
        Device Device { get; }
        IBufferProxy Register(ConstantBufferDescription description);
        IBufferProxy Register(string name, int structSize);
    }

    public class ConstantBufferPool : BufferPool<string, ConstantBufferDescription>, IConstantBufferPool
    {
        public ConstantBufferPool(Device device)
            :base(device)
        {

        }
        protected override IBufferProxy CreateBuffer(ConstantBufferDescription description)
        {
            return description.CreateBuffer();
        }

        protected override string GetKey(ConstantBufferDescription description)
        {
            return description.Name + description.StructSize;
        }

        public IBufferProxy Register(string name, int structSize)
        {
            return Register(new ConstantBufferDescription(name, structSize));
        }
    }
}
