using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX;
using System.Runtime.Serialization;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
[DataContract(Name = @"ShaderPassDescription")]
public sealed class ShaderPassDescription
{
    /// <summary>
    /// Pass Name
    /// </summary>
    [DataMember(Name = @"Name")]
    public string Name
    {
        set; get;
    } = string.Empty;

    /// <summary>
    /// Shaders for this technique
    /// </summary>
    [DataMember(Name = @"ShaderList")]
    public IList<ShaderDescription>? ShaderList
    {
        set; get;
    }

    /// <summary>
    /// 
    /// </summary>
    public BlendStateDescription? BlendStateDescription { set; get; } = null;

    /// <summary>
    /// Only used for data serialization
    /// </summary>
    [DataMember(Name = @"BlendStateDescSerialization")]
    public BlendStateDataContract? BlendStateDescSerialization
    {
        set
        {
            if (value == null)
            {
                BlendStateDescription = null;
            }
            else
            {
                BlendStateDescription = ((BlendStateDataContract)value).ToBlendStateDescription();
            }
        }
        get
        {
            if (BlendStateDescription == null)
            {
                return null;
            }
            else
            {
                return new BlendStateDataContract((BlendStateDescription)BlendStateDescription);
            }
        }
    }
    /// <summary>
    /// Gets or sets the color of the blend.
    /// </summary>
    /// <value>
    /// The color of the blend.
    /// </value>
    [DataMember(Name = @"BlendFactor")]
    public Color4 BlendFactor { set; get; } = Color4.White;
    /// <summary>
    /// Gets or sets the blend sample mask.
    /// </summary>
    /// <value>
    /// The sample mask.
    /// </value>
    [DataMember(Name = @"SampleMask")]
    public int SampleMask { set; get; } = -1;
    /// <summary>
    /// Gets or sets the stencil reference.
    /// </summary>
    /// <value>
    /// The stencil reference.
    /// </value>
    [DataMember(Name = @"StencilRef")]
    public int StencilRef { set; get; } = 0;
    /// <summary>
    /// Gets or sets the topology. This is optional. Used if topology is different from vertex buffer topology
    /// </summary>
    /// <value>
    /// The topology.
    /// </value>
    [DataMember(Name = @"Topology")]
    public PrimitiveTopology Topology { set; get; } = PrimitiveTopology.Undefined;

    /// <summary>
    /// 
    /// </summary>
    public DepthStencilStateDescription? DepthStencilStateDescription { set; get; } = null;

    /// <summary>
    /// Only used for data serialization
    /// </summary>
    [DataMember(Name = @"DepthStencilStateDescSerialization")]
    public DepthStencilStateDataContract? DepthStencilStateDescSerialization
    {
        set
        {
            if (value == null)
            {
                DepthStencilStateDescription = null;
            }
            else
            {
                DepthStencilStateDescription = ((DepthStencilStateDataContract)value).ToDepthStencilStateDescription();
            }
        }
        get
        {
            if (DepthStencilStateDescription == null)
            {
                return null;
            }
            else
            {
                return new DepthStencilStateDataContract((DepthStencilStateDescription)DepthStencilStateDescription);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public RasterizerStateDescription? RasterStateDescription { set; get; } = null;

    /// <summary>
    /// Only used for data serialization
    /// </summary>
    /// <value>
    /// The rasterizer state data contract.
    /// </value>
    [DataMember(Name = @"RasterizerStateDescSerialization")]
    public RasterizerStateDataContract? RasterizerStateDescSerialization
    {
        set
        {
            if (value == null)
            {
                RasterStateDescription = null;
            }
            else
            {
                RasterStateDescription = ((RasterizerStateDataContract)value).ToRasterizerStateDescription();
            }
        }
        get
        {
            if (RasterStateDescription == null)
            {
                return null;
            }
            else
            {
                return new RasterizerStateDataContract((RasterizerStateDescription)RasterStateDescription);
            }
        }
    }

    /// <summary>
    /// Input Layout
    /// </summary>
    [DataMember(Name = @"InputLayoutDescription")]
    public InputLayoutDescription? InputLayoutDescription { set; get; } = null;

    public ShaderPassDescription()
    {
    }

    public ShaderPassDescription(string name)
    {
        Name = name;
    }
}
