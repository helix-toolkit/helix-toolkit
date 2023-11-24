using SharpDX.Direct3D11;
using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Shaders;

[DataContract(Name = @"BlendStateDataContract")]
public struct BlendStateDataContract
{
    [DataMember(Name = @"AlphaToCoverageEnable")]
    public bool AlphaToCoverageEnable
    {
        set; get;
    }
    [DataMember(Name = @"IndependentBlendEnable")]
    public bool IndependentBlendEnable
    {
        set; get;
    }
    [DataMember(Name = @"RenderTarget")]
    public RenderTargetBlendDataContract[] RenderTarget
    {
        set; get;
    }

    public BlendStateDataContract(BlendStateDescription desc)
    {
        AlphaToCoverageEnable = desc.AlphaToCoverageEnable;
        IndependentBlendEnable = desc.IndependentBlendEnable;
        RenderTarget = new RenderTargetBlendDataContract[desc.RenderTarget.Length];
        for (var i = 0; i < desc.RenderTarget.Length; ++i)
        {
            RenderTarget[i] = new RenderTargetBlendDataContract(desc.RenderTarget[i]);
        }
    }

    public BlendStateDescription ToBlendStateDescription()
    {
        var desc = new BlendStateDescription()
        {
            AlphaToCoverageEnable = AlphaToCoverageEnable,
            IndependentBlendEnable = IndependentBlendEnable,
        };
        for (var i = 0; i < desc.RenderTarget.Length; ++i)
        {
            desc.RenderTarget[i] = RenderTarget[i].ToRenderTargetBlendDescription();
        }
        return desc;
    }
}
