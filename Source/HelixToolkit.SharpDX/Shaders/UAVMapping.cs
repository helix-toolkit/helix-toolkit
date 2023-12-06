using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Shaders;

[DataContract]
public sealed class UAVMapping
{
    [DataMember]
    public int Slot
    {
        set; get;
    }
    [DataMember]
    public UAVDescription Description
    {
        set; get;
    }

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
