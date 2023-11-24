using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Shaders;

public sealed class UAVDescription
{
    [DataMember]
    public string Name
    {
        set; get;
    } = string.Empty;
    [DataMember]
    public ShaderStage ShaderType;
    [DataMember]
    public UnorderedAccessViewType Type;

    public UAVDescription()
    {
    }

    public UAVDescription(string name, ShaderStage shaderType, UnorderedAccessViewType type)
    {
        Name = name;
        ShaderType = shaderType;
        Type = type;
    }

    public UAVMapping CreateMapping(int slot)
    {
        return new UAVMapping(slot, this);
    }

    public UAVDescription Clone()
    {
        return new UAVDescription(this.Name, this.ShaderType, this.Type);
    }
}
