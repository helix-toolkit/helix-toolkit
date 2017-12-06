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
        public int Slot { set; get; }
        [DataMember]
        public string Name { set; get; }
        [DataMember]
        public ShaderStage ShaderType;

        public TextureDescription() { }

        public TextureDescription(int slot, string name, ShaderStage shaderType)
        {
            Slot = slot;
            Name = name;
            ShaderType = shaderType;
        }
    }
}
