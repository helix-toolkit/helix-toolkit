/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Runtime.Serialization;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    [DataContract]
    public class TextureDescription
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

        public TextureDescription Clone()
        {
            return new TextureDescription(this.Name, this.ShaderType);
        }
    }

    [DataContract]
    public class TextureMapping 
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

        public TextureMapping Clone()
        {
            return new TextureMapping(this.Slot, this.Description.Clone());
        }
    }
}
