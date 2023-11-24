using SharpDX.Direct3D11;
using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Shaders;

[DataContract(Name = @"DepthStencilStateDataContract")]
public struct DepthStencilStateDataContract
{
    [DataMember(Name = @"IsDepthEnabled")]
    public bool IsDepthEnabled
    {
        set; get;
    }
    [DataMember(Name = @"DepthWriteMask")]
    public int DepthWriteMask
    {
        set; get;
    }
    [DataMember(Name = @"DepthComparison")]
    public int DepthComparison
    {
        set; get;
    }
    [DataMember(Name = @"IsStencilEnabled")]
    public bool IsStencilEnabled
    {
        set; get;
    }
    [DataMember(Name = @"StencilReadMask")]
    public byte StencilReadMask
    {
        set; get;
    }
    [DataMember(Name = @"StencilWriteMask")]
    public byte StencilWriteMask
    {
        set; get;
    }
    [DataMember(Name = @"FrontFace")]
    public DepthStencilOperationDataContract FrontFace
    {
        set; get;
    }
    [DataMember(Name = @"BackFace")]
    public DepthStencilOperationDataContract BackFace
    {
        set; get;
    }

    public DepthStencilStateDataContract(DepthStencilStateDescription desc)
    {
        IsDepthEnabled = desc.IsDepthEnabled;
        IsStencilEnabled = desc.IsStencilEnabled;
        DepthWriteMask = (int)desc.DepthWriteMask;
        DepthComparison = (int)desc.DepthComparison;
        StencilReadMask = desc.StencilReadMask;
        StencilWriteMask = desc.StencilWriteMask;
        FrontFace = new DepthStencilOperationDataContract(desc.FrontFace);
        BackFace = new DepthStencilOperationDataContract(desc.BackFace);
    }

    public DepthStencilStateDescription ToDepthStencilStateDescription()
    {
        return new DepthStencilStateDescription()
        {
            IsDepthEnabled = IsDepthEnabled,
            DepthWriteMask = (DepthWriteMask)DepthWriteMask,
            DepthComparison = (Comparison)DepthComparison,
            IsStencilEnabled = IsStencilEnabled,
            StencilReadMask = StencilReadMask,
            StencilWriteMask = StencilWriteMask,
            FrontFace = FrontFace.ToDepthStencilOperationDescription(),
            BackFace = BackFace.ToDepthStencilOperationDescription()
        };
    }
}
