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
        public int Slot { set; get; }
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

        public ConstantBufferDescription(string name, int structSize, int slot, int strideSize=0)
        {
            Name = name;
            StructSize = structSize;
            Slot = slot;
            StrideSize = strideSize;
        }

        public IBufferProxy CreateBuffer()
        {
            return new ConstantBufferProxy(this);
        }
    }
}
