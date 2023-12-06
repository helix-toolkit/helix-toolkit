using SharpDX.Direct3D11;
using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Shaders;

[DataContract(Name = @"RasterizerStateDataContract")]
public struct RasterizerStateDataContract
{
    [DataMember(Name = @"FillMode")]
    public int FillMode
    {
        set; get;
    }
    [DataMember(Name = @"CullMode")]
    public int CullMode
    {
        set; get;
    }
    [DataMember(Name = @"IsFrontCounterClockwise")]
    public bool IsFrontCounterClockwise
    {
        set; get;
    }
    [DataMember(Name = @"DepthBias")]
    public int DepthBias
    {
        set; get;
    }
    [DataMember(Name = @"DepthBiasClamp")]
    public float DepthBiasClamp
    {
        set; get;
    }
    [DataMember(Name = @"SlopeScaledDepthBias")]
    public float SlopeScaledDepthBias
    {
        set; get;
    }
    [DataMember(Name = @"IsDepthClipEnabled")]
    public bool IsDepthClipEnabled
    {
        set; get;
    }
    [DataMember(Name = @"IsScissorEnabled")]
    public bool IsScissorEnabled
    {
        set; get;
    }
    [DataMember(Name = @"IsMultisampleEnabled")]
    public bool IsMultisampleEnabled
    {
        set; get;
    }
    [DataMember(Name = @"IsAntialiasedLineEnabled")]
    public bool IsAntialiasedLineEnabled
    {
        set; get;
    }

    public RasterizerStateDescription ToRasterizerStateDescription()
    {
        return new RasterizerStateDescription()
        {
            FillMode = (FillMode)FillMode,
            CullMode = (CullMode)CullMode,
            IsFrontCounterClockwise = IsFrontCounterClockwise,
            DepthBias = DepthBias,
            DepthBiasClamp = DepthBiasClamp,
            SlopeScaledDepthBias = SlopeScaledDepthBias,
            IsDepthClipEnabled = IsDepthClipEnabled,
            IsScissorEnabled = IsScissorEnabled,
            IsMultisampleEnabled = IsMultisampleEnabled,
            IsAntialiasedLineEnabled = IsAntialiasedLineEnabled
        };
    }

    public RasterizerStateDataContract(RasterizerStateDescription desc)
    {
        FillMode = (int)desc.FillMode;
        CullMode = (int)desc.CullMode;
        DepthBias = desc.DepthBias;
        IsFrontCounterClockwise = desc.IsFrontCounterClockwise;
        DepthBiasClamp = desc.DepthBiasClamp;
        SlopeScaledDepthBias = desc.SlopeScaledDepthBias;
        IsDepthClipEnabled = desc.IsDepthClipEnabled;
        IsScissorEnabled = desc.IsScissorEnabled;
        IsMultisampleEnabled = desc.IsMultisampleEnabled;
        IsAntialiasedLineEnabled = desc.IsAntialiasedLineEnabled;
    }
}
