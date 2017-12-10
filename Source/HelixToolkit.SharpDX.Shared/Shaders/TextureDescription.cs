using System;
using System.Runtime.Serialization;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    [DataContract]
    public class TextureDescription : ICloneable
    {

        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public ShaderStage ShaderType;

        public TextureDescription() { }

        public TextureDescription(string name, ShaderStage shaderType)
        {
            Name = name;
            ShaderType = shaderType;
        }

        public TextureMapping CreateMapping(int slot)
        {
            return new TextureMapping(slot, this);
        }

        public object Clone()
        {
            return new TextureDescription(this.Name, this.ShaderType);
        }
    }

    [DataContract]
    public class TextureMapping : ICloneable
    {
        [DataMember]
        public int Slot { set; get; }
        [DataMember]
        public TextureDescription Description { set; get; }

        public TextureMapping(int slot, TextureDescription description)
        {
            Slot = slot;
            Description = description;
        }

        public object Clone()
        {
            return new TextureMapping(this.Slot, (TextureDescription)this.Description.Clone());
        }
    }
}
