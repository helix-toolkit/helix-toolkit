using SharpDX.Direct3D11;
using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Shaders;

[DataContract(Name = @"RenderTargetBlendDataContract")]
public struct RenderTargetBlendDataContract
{
    [DataMember(Name = @"IsBlendEnabled")]
    public bool IsBlendEnabled
    {
        set; get;
    }
    [DataMember(Name = @"SourceBlend")]
    public int SourceBlend
    {
        set; get;
    }
    [DataMember(Name = @"DestinationBlend")]
    public int DestinationBlend
    {
        set; get;
    }
    [DataMember(Name = @"BlendOperation")]
    public int BlendOperation
    {
        set; get;
    }
    [DataMember(Name = @"SourceAlphaBlend")]
    public int SourceAlphaBlend
    {
        set; get;
    }
    [DataMember(Name = @"DestinationAlphaBlend")]
    public int DestinationAlphaBlend
    {
        set; get;
    }
    [DataMember(Name = @"AlphaBlendOperation")]
    public int AlphaBlendOperation
    {
        set; get;
    }
    [DataMember(Name = @"RenderTargetWriteMask")]
    public int RenderTargetWriteMask
    {
        set; get;
    }

    public RenderTargetBlendDataContract(RenderTargetBlendDescription desc)
    {
        IsBlendEnabled = desc.IsBlendEnabled;
        SourceBlend = (int)desc.SourceBlend;
        DestinationBlend = (int)desc.DestinationBlend;
        BlendOperation = (int)desc.BlendOperation;
        SourceAlphaBlend = (int)desc.SourceAlphaBlend;
        DestinationAlphaBlend = (int)desc.DestinationAlphaBlend;
        AlphaBlendOperation = (int)desc.AlphaBlendOperation;
        RenderTargetWriteMask = (int)desc.RenderTargetWriteMask;
    }

    public RenderTargetBlendDescription ToRenderTargetBlendDescription()
    {
        return new RenderTargetBlendDescription()
        {
            IsBlendEnabled = IsBlendEnabled,
            SourceBlend = (BlendOption)SourceBlend,
            DestinationBlend = (BlendOption)DestinationBlend,
            BlendOperation = (BlendOperation)BlendOperation,
            SourceAlphaBlend = (BlendOption)SourceAlphaBlend,
            DestinationAlphaBlend = (BlendOption)DestinationAlphaBlend,
            AlphaBlendOperation = (BlendOperation)AlphaBlendOperation,
            RenderTargetWriteMask = (ColorWriteMaskFlags)RenderTargetWriteMask
        };
    }
}
