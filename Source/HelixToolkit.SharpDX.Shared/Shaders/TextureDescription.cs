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
    public enum TextureType
    {
        Texture, Structured, TextureBuffer
    }
    [DataContract]
    public sealed class TextureDescription
    {
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public ShaderStage ShaderType;
        [DataMember]
        public TextureType Type;

        public TextureDescription() { }

        public TextureDescription(string name, ShaderStage shaderType, TextureType type)
        {
            Name = name;
            ShaderType = shaderType;
            Type = type;
        }

        public TextureMapping CreateMapping(int slot)
        {
            return new TextureMapping(slot, this);
        }

        public TextureDescription Clone()
        {
            return new TextureDescription(this.Name, this.ShaderType, this.Type);
        }
    }

    [DataContract]
    public sealed class TextureMapping 
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
