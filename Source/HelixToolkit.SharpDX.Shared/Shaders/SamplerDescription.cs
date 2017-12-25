using System;
using System.Runtime.Serialization;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    [DataContract]
    public class SamplerDescription
    {

        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public ShaderStage ShaderType;

        public SamplerDescription() { }

        public SamplerDescription(string name, ShaderStage shaderType)
        {
            Name = name;
            ShaderType = shaderType;
        }

        public SamplerMapping CreateMapping(int slot)
        {
            return new SamplerMapping(slot, this);
        }

        public SamplerDescription Clone()
        {
            return new SamplerDescription(this.Name, this.ShaderType);
        }
    }

    [DataContract]
    public class SamplerMapping
    {
        [DataMember]
        public int Slot { set; get; }
        [DataMember]
        public SamplerDescription Description { set; get; }

        public SamplerMapping(int slot, SamplerDescription description)
        {
            Slot = slot;
            Description = description;
        }

        public SamplerMapping Clone()
        {
            return new SamplerMapping(this.Slot, this.Description.Clone());
        }
    }
}
