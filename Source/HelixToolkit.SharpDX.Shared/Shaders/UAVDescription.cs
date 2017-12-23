using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    public class UAVDescription
    {
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public ShaderStage ShaderType;

        public UAVDescription() { }

        public UAVDescription(string name, ShaderStage shaderType)
        {
            Name = name;
            ShaderType = shaderType;
        }

        public UAVMapping CreateMapping(int slot)
        {
            return new UAVMapping(slot, this);
        }

        public UAVDescription Clone()
        {
            return new UAVDescription(this.Name, this.ShaderType);
        }
    }

    [DataContract]
    public class UAVMapping
    {
        [DataMember]
        public int Slot { set; get; }
        [DataMember]
        public UAVDescription Description { set; get; }

        public UAVMapping(int slot, UAVDescription description)
        {
            Slot = slot;
            Description = description;
        }

        public UAVMapping Clone()
        {
            return new UAVMapping(this.Slot, this.Description.Clone());
        }
    }
}
