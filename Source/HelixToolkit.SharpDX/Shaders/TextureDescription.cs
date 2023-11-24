using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
[DataContract]
public sealed class TextureDescription
{
    [DataMember]
    public string Name
    {
        set; get;
    } = string.Empty;
    [DataMember]
    public ShaderStage ShaderType
    {
        set; get;
    }
    [DataMember]
    public TextureType Type
    {
        set; get;
    }

    public TextureDescription()
    {
    }

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
