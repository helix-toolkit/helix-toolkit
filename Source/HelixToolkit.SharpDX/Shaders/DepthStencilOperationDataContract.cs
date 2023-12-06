using SharpDX.Direct3D11;
using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Shaders;

[DataContract(Name = @"DepthStencilOperationDataContract")]
public struct DepthStencilOperationDataContract
{
    [DataMember(Name = @"FailOperation")]
    public int FailOperation
    {
        set; get;
    }
    [DataMember(Name = @"DepthFailOperation")]
    public int DepthFailOperation
    {
        set; get;
    }
    [DataMember(Name = @"PassOperation")]
    public int PassOperation
    {
        set; get;
    }
    [DataMember(Name = @"Comparison")]
    public int Comparison
    {
        set; get;
    }

    public DepthStencilOperationDescription ToDepthStencilOperationDescription()
    {
        return new DepthStencilOperationDescription()
        {
            FailOperation = (StencilOperation)FailOperation,
            DepthFailOperation = (StencilOperation)DepthFailOperation,
            PassOperation = (StencilOperation)PassOperation,
            Comparison = (Comparison)Comparison
        };
    }

    public DepthStencilOperationDataContract(DepthStencilOperationDescription desc)
    {
        FailOperation = (int)desc.FailOperation;
        DepthFailOperation = (int)desc.DepthFailOperation;
        PassOperation = (int)desc.PassOperation;
        Comparison = (int)desc.Comparison;
    }
}
