using SharpDX.Direct3D11;
using System;
using System.Runtime.Serialization;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using Utilities;

    [DataContract]
    public class ConstantBufferDescription
    {
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public int StructSize { set; get; }
        [DataMember]
        public int StrideSize { set; get; }
        [DataMember]
        public BindFlags BindFlags { set; get; } = BindFlags.ConstantBuffer;
        [DataMember]
        public CpuAccessFlags CpuAccessFlags { set; get; } = CpuAccessFlags.Write;
        [DataMember]
        public ResourceOptionFlags OptionFlags { set; get; } = ResourceOptionFlags.None;
        [DataMember]
        public ResourceUsage Usage { set; get; } = ResourceUsage.Dynamic;
        //[DataMember]
        //public Type StructType { set; get; }

        public ConstantBufferDescription() { }

        public ConstantBufferDescription(string name, int structSize, int strideSize=0)
        {
            Name = name;
            StructSize = structSize;
            StrideSize = strideSize;
        }

        public IBufferProxy CreateBuffer()
        {
            return new ConstantBufferProxy(this);
        }

        public ConstantBufferMapping CreateMapping(int slot)
        {
            return new ConstantBufferMapping(slot, this);
        }
    }

    [DataContract]
    public class ConstantBufferMapping
    {
        [DataMember]
        public int Slot { set; get; }
        [DataMember]
        public ConstantBufferDescription Description { set; get; }

        public ConstantBufferMapping(int slot, ConstantBufferDescription description)
        {
            Slot = slot;
            Description = description;
        }

        public static ConstantBufferMapping Create(int slot, ConstantBufferDescription description)
        {
            return new ConstantBufferMapping(slot, description);
        }
    }
}
