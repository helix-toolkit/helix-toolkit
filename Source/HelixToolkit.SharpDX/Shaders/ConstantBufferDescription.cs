using HelixToolkit.SharpDX.Utilities;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Shaders;

public sealed class ConstantBufferDescription
{
    public string Name
    {
        set; get;
    }

    public int StructSize
    {
        set; get;
    }

    public int StrideSize
    {
        set; get;
    }

    public BindFlags BindFlags { set; get; } = BindFlags.ConstantBuffer;

    public CpuAccessFlags CpuAccessFlags { set; get; } = CpuAccessFlags.Write;

    public ResourceOptionFlags OptionFlags { set; get; } = ResourceOptionFlags.None;

    public ResourceUsage Usage { set; get; } = ResourceUsage.Dynamic;

    public ShaderStage Stage
    {
        set; get;
    }

    public int Slot
    {
        set; get;
    }

    public List<ConstantBufferVariable> Variables { get; } = new List<ConstantBufferVariable>();

    public ConstantBufferDescription(ConstantBuffer buffer)
    {
        Name = buffer.Description.Name;
        StructSize = StrideSize = buffer.Description.Size;
        Variables = new List<ConstantBufferVariable>();
        for (var i = 0; i < buffer.Description.VariableCount; ++i)
        {
            var variable = buffer.GetVariable(i);
            Variables.Add(new ConstantBufferVariable() { Name = variable.Description.Name, Size = variable.Description.Size, StartOffset = variable.Description.StartOffset });
        }
    }

    public ConstantBufferDescription(string name, int structSize, int strideSize = 0)
    {
        Name = name;
        StructSize = structSize;
        StrideSize = strideSize;
    }

    public ConstantBufferProxy CreateBuffer()
    {
        return new ConstantBufferProxy(this);
    }

    public ConstantBufferMapping CreateMapping(int slot)
    {
        return new ConstantBufferMapping(slot, this);
    }

    public ConstantBufferDescription Clone()
    {
        return new ConstantBufferDescription(Name, StructSize, StrideSize)
        {
            BindFlags = this.BindFlags,
            CpuAccessFlags = this.CpuAccessFlags,
            OptionFlags = this.OptionFlags,
            Usage = this.Usage
        };
    }
}
