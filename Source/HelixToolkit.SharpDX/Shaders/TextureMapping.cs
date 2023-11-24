using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
[DataContract]
public sealed class TextureMapping
{
    [DataMember]
    public int Slot
    {
        set; get;
    }
    [DataMember]
    public TextureDescription Description
    {
        set; get;
    }

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
