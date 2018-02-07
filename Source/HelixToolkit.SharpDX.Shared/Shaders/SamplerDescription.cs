using System;
using System.Runtime.Serialization;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    [DataContract]
    public class SamplerMapping
    {
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public ShaderStage ShaderType;
        [DataMember]
        public int Slot { set; get; }

        public SamplerMapping() { }

        public SamplerMapping(int slot, string name, ShaderStage type)
        {
            Slot = slot;
            Name = name;
            ShaderType = type;
        }

        public SamplerMapping Clone()
        {
            return new SamplerMapping(this.Slot, this.Name, this.ShaderType);
        }
    }
}
