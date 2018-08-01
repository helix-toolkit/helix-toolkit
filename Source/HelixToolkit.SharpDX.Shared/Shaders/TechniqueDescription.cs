/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Runtime.Serialization;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public sealed class TechniqueDescription
    {
        /// <summary>
        /// Technique Name
        /// </summary>
        [DataMember(Name= @"Name")]
        public string Name { set; get; }

        /// <summary>
        /// Input Layout
        /// </summary>
        [DataMember(Name = @"InputLayoutDescription")]
        public InputLayoutDescription InputLayoutDescription { set; get; }
        /// <summary>
        /// Gets or sets the pass descriptions.
        /// </summary>
        /// <value>
        /// The pass descriptions.
        /// </value>
        [DataMember(Name = @"PassDescriptions")]
        public IList<ShaderPassDescription> PassDescriptions { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether this technique is null technique.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is null; otherwise, <c>false</c>.
        /// </value>
        public bool IsNull { set; get; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TechniqueDescription"/> class.
        /// </summary>
        public TechniqueDescription() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TechniqueDescription"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TechniqueDescription(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TechniqueDescription"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="inputLayout">The input layout.</param>
        public TechniqueDescription(string name, InputLayoutDescription inputLayout)
            : this(name)
        {
            InputLayoutDescription = inputLayout;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TechniqueDescription"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="inputLayout">The input layout.</param>
        /// <param name="shaderPasses">The shader passes.</param>
        public TechniqueDescription(string name, InputLayoutDescription inputLayout, IList<ShaderPassDescription> shaderPasses)
            : this(name, inputLayout)
        {
            PassDescriptions = shaderPasses;
        }
    }

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
        public string Name { set; get; }
        /// <summary>
        /// Shaders for this technique
        /// </summary>
        [DataMember(Name = @"ShaderList")]
        public IList<ShaderDescription> ShaderList { set; get; }

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
        public InputLayoutDescription InputLayoutDescription { set; get; } = null;

        public ShaderPassDescription() { }

        public ShaderPassDescription(string name)
        {
            Name = name;
        }
    }

    #region Serializable descriptions
    [DataContract(Name = @"DepthStencilOperationDataContract")]
    public struct DepthStencilOperationDataContract
    {
        [DataMember(Name = @"FailOperation")]
        public int FailOperation { set; get; }
        [DataMember(Name = @"DepthFailOperation")]
        public int DepthFailOperation { set; get; }
        [DataMember(Name = @"PassOperation")]
        public int PassOperation { set; get; }
        [DataMember(Name = @"Comparison")]
        public int Comparison { set; get; }

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

    [DataContract(Name = @"DepthStencilStateDataContract")]
    public struct DepthStencilStateDataContract
    {
        [DataMember(Name = @"IsDepthEnabled")]
        public bool IsDepthEnabled { set; get; }
        [DataMember(Name = @"DepthWriteMask")]
        public int DepthWriteMask { set; get; }
        [DataMember(Name = @"DepthComparison")]
        public int DepthComparison { set; get; }
        [DataMember(Name = @"IsStencilEnabled")]
        public bool IsStencilEnabled { set; get; }
        [DataMember(Name = @"StencilReadMask")]
        public byte StencilReadMask { set; get; }
        [DataMember(Name = @"StencilWriteMask")]
        public byte StencilWriteMask { set; get; }
        [DataMember(Name = @"FrontFace")]
        public DepthStencilOperationDataContract FrontFace { set; get; }
        [DataMember(Name = @"BackFace")]
        public DepthStencilOperationDataContract BackFace { set; get; }

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

    [DataContract(Name = @"RasterizerStateDataContract")]
    public struct RasterizerStateDataContract
    {
        [DataMember(Name = @"FillMode")]
        public int FillMode { set; get; }
        [DataMember(Name = @"CullMode")]
        public int CullMode { set; get; }
        [DataMember(Name = @"IsFrontCounterClockwise")]
        public bool IsFrontCounterClockwise { set; get; }
        [DataMember(Name = @"DepthBias")]
        public int DepthBias { set; get; }
        [DataMember(Name = @"DepthBiasClamp")]
        public float DepthBiasClamp { set; get; }
        [DataMember(Name = @"SlopeScaledDepthBias")]
        public float SlopeScaledDepthBias { set; get; }
        [DataMember(Name = @"IsDepthClipEnabled")]
        public bool IsDepthClipEnabled { set; get; }
        [DataMember(Name = @"IsScissorEnabled")]
        public bool IsScissorEnabled { set; get; }
        [DataMember(Name = @"IsMultisampleEnabled")]
        public bool IsMultisampleEnabled { set; get; }
        [DataMember(Name = @"IsAntialiasedLineEnabled")]
        public bool IsAntialiasedLineEnabled { set; get; }

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

    [DataContract(Name = @"BlendStateDataContract")]
    public struct BlendStateDataContract
    {
        [DataMember(Name = @"AlphaToCoverageEnable")]
        public bool AlphaToCoverageEnable { set; get; }
        [DataMember(Name = @"IndependentBlendEnable")]
        public bool IndependentBlendEnable { set; get; }
        [DataMember(Name = @"RenderTarget")]
        public RenderTargetBlendDataContract[] RenderTarget { set; get; }

        public BlendStateDataContract(BlendStateDescription desc)
        {
            AlphaToCoverageEnable = desc.AlphaToCoverageEnable;
            IndependentBlendEnable = desc.IndependentBlendEnable;
            RenderTarget = new RenderTargetBlendDataContract[desc.RenderTarget.Length];
            for (int i = 0; i < desc.RenderTarget.Length; ++i)
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
            for(int i=0; i < desc.RenderTarget.Length; ++i)
            {
                desc.RenderTarget[i] = RenderTarget[i].ToRenderTargetBlendDescription();
            }
            return desc;
        }
    }

    [DataContract(Name = @"RenderTargetBlendDataContract")]
    public struct RenderTargetBlendDataContract
    {
        [DataMember(Name = @"IsBlendEnabled")]
        public bool IsBlendEnabled { set; get; }
        [DataMember(Name = @"SourceBlend")]
        public int SourceBlend { set; get; }
        [DataMember(Name = @"DestinationBlend")]
        public int DestinationBlend { set; get; }
        [DataMember(Name = @"BlendOperation")]
        public int BlendOperation { set; get; }
        [DataMember(Name = @"SourceAlphaBlend")]
        public int SourceAlphaBlend { set; get; }
        [DataMember(Name = @"DestinationAlphaBlend")]
        public int DestinationAlphaBlend { set; get; }
        [DataMember(Name = @"AlphaBlendOperation")]
        public int AlphaBlendOperation { set; get; }
        [DataMember(Name = @"RenderTargetWriteMask")]
        public int RenderTargetWriteMask { set; get; }

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
    #endregion
}
