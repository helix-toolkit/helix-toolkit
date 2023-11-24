using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Shaders;

[DataContract]
public sealed class ConstantBufferMapping
{
    [DataMember]
    public int Slot
    {
        set; get;
    }

    [DataMember]
    public ConstantBufferDescription Description
    {
        set; get;
    }

    public ConstantBufferMapping(int slot, ConstantBufferDescription description)
    {
        Slot = slot;
        Description = description;
    }

    public static ConstantBufferMapping Create(int slot, ConstantBufferDescription description)
    {
        return new ConstantBufferMapping(slot, description);
    }

    public ConstantBufferMapping Clone()
    {
        return new ConstantBufferMapping(this.Slot, this.Description.Clone());
    }
}
