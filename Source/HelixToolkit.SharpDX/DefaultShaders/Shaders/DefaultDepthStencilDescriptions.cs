using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
public static class DefaultDepthStencilDescriptions
{
    /// <summary>
    /// The DSS depth less
    /// </summary>
    public readonly static DepthStencilStateDescription DSSDepthLess = new()
    {
        IsDepthEnabled = true,
        DepthWriteMask = DepthWriteMask.All,
        DepthComparison = Comparison.Less,
        IsStencilEnabled = false
    };
    /// <summary>
    /// The DSS depth less equal
    /// </summary>
    public readonly static DepthStencilStateDescription DSSDepthLessEqual = new()
    {
        IsDepthEnabled = true,
        DepthWriteMask = DepthWriteMask.All,
        DepthComparison = Comparison.LessEqual,
        IsStencilEnabled = false
    };
    /// <summary>
    /// The DSS less no write
    /// </summary>
    public readonly static DepthStencilStateDescription DSSLessNoWrite = new()
    {
        IsDepthEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.Less,
        IsStencilEnabled = false
    };
    /// <summary>
    /// The DSS less equal no write
    /// </summary>
    public readonly static DepthStencilStateDescription DSSLessEqualNoWrite = new()
    {
        IsDepthEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.LessEqual,
        IsStencilEnabled = false
    };
    /// <summary>
    /// The DSS greater no write
    /// </summary>
    public readonly static DepthStencilStateDescription DSSGreaterNoWrite = new()
    {
        IsDepthEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.Greater
    };
    /// <summary>
    /// The DSS equal no write
    /// </summary>
    public readonly static DepthStencilStateDescription DSSEqualNoWrite = new()
    {
        IsDepthEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.Equal
    };
    /// <summary>
    /// The DSS clip plane backface
    /// </summary>
    public readonly static DepthStencilStateDescription DSSClipPlaneBackface = new()
    {
        IsDepthEnabled = true,
        IsStencilEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.Less,
        StencilWriteMask = 0xFF,
        StencilReadMask = 0,
        BackFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Replace,
            Comparison = Comparison.Always,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Keep
        },
        FrontFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Keep,
            Comparison = Comparison.Never,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Keep
        }
    };

    /// <summary>
    /// The DSS mesh outline pass1
    /// </summary>
    public readonly static DepthStencilStateDescription DSSMeshOutlineP1 = new()
    {
        IsDepthEnabled = false,
        IsStencilEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.Always,
        StencilWriteMask = 0xFF,
        StencilReadMask = 0,
        BackFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Replace,
            Comparison = Comparison.Always,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Keep
        },
        FrontFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Replace,
            Comparison = Comparison.Always,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Keep
        }
    };

    /// <summary>
    /// The DSS mesh outline pass1
    /// </summary>
    public readonly static DepthStencilStateDescription DSSEffectMeshXRayP1 = new()
    {
        IsDepthEnabled = false,
        IsStencilEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.Always,
        StencilWriteMask = 0xFF,
        StencilReadMask = 0,
        BackFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Keep,
            Comparison = Comparison.Never,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Keep
        },
        FrontFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Increment,
            Comparison = Comparison.Always,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Keep
        }
    };

    public readonly static DepthStencilStateDescription DSSEffectMeshXRayGridP1 = new()
    {
        IsDepthEnabled = false,
        IsStencilEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.Always,
        StencilWriteMask = 0xFF,
        StencilReadMask = 0,
        BackFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Keep,
            Comparison = Comparison.Never,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Keep
        },
        FrontFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Replace,
            Comparison = Comparison.Always,
            DepthFailOperation = StencilOperation.Zero,
            FailOperation = StencilOperation.Zero
        }
    };
    public readonly static DepthStencilStateDescription DSSEffectMeshXRayGridP2 = new()
    {
        IsDepthEnabled = true,
        IsStencilEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.LessEqual,
        StencilWriteMask = 0xFF,
        StencilReadMask = 0,
        BackFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Keep,
            Comparison = Comparison.Never,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Keep
        },
        FrontFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Zero,
            Comparison = Comparison.Equal,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Zero
        }
    };
    public readonly static DepthStencilStateDescription DSSEffectMeshXRayGridP3 = new()
    {
        IsDepthEnabled = false,
        IsStencilEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.NotEqual,
        StencilWriteMask = 0,
        StencilReadMask = 0xFF,
        BackFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Keep,
            Comparison = Comparison.Never,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Keep
        },
        FrontFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Keep,
            Comparison = Comparison.Equal,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Keep
        }
    };
    /// <summary>
    /// The DSS mesh outline pass1
    /// </summary>
    public readonly static DepthStencilStateDescription DSSEffectMeshXRayP2 = new()
    {
        IsDepthEnabled = true,
        IsStencilEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.Greater,
        StencilWriteMask = 0,
        StencilReadMask = 0xFF,
        BackFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Keep,
            Comparison = Comparison.Never,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Keep
        },
        FrontFace = new DepthStencilOperationDescription()
        {
            PassOperation = StencilOperation.Keep,
            Comparison = Comparison.Equal,
            DepthFailOperation = StencilOperation.Keep,
            FailOperation = StencilOperation.Keep
        }
    };
    /// <summary>
    /// The DSS clip plane fill quad
    /// </summary>
    public readonly static DepthStencilStateDescription DSSOutlineFillQuad = new()
    {
        IsDepthEnabled = false,
        IsStencilEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.Always,
        FrontFace = new DepthStencilOperationDescription()
        {
            FailOperation = StencilOperation.Keep,
            DepthFailOperation = StencilOperation.Keep,
            PassOperation = StencilOperation.Keep,
            Comparison = Comparison.Equal
        },
        BackFace = new DepthStencilOperationDescription()
        {
            Comparison = Comparison.Never,
            FailOperation = StencilOperation.Keep,
            DepthFailOperation = StencilOperation.Keep,
            PassOperation = StencilOperation.Keep
        },
        StencilReadMask = 0xFF,
        StencilWriteMask = 0
    };

    /// <summary>
    /// The DSS clip plane fill quad
    /// </summary>
    public readonly static DepthStencilStateDescription DSSClipPlaneFillQuad = new()
    {
        IsDepthEnabled = false,
        IsStencilEnabled = true,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.Less,
        FrontFace = new DepthStencilOperationDescription()
        {
            FailOperation = StencilOperation.Keep,
            DepthFailOperation = StencilOperation.Keep,
            PassOperation = StencilOperation.Keep,
            Comparison = Comparison.Equal
        },
        BackFace = new DepthStencilOperationDescription()
        {
            Comparison = Comparison.Never,
            FailOperation = StencilOperation.Keep,
            DepthFailOperation = StencilOperation.Keep,
            PassOperation = StencilOperation.Keep
        },
        StencilReadMask = 0xFF,
        StencilWriteMask = 0
    };
    /// <summary>
    /// The DSS depth always no stencil
    /// </summary>
    public readonly static DepthStencilStateDescription DSSDepthAlwaysNoStencil = new()
    {
        IsDepthEnabled = true,
        DepthComparison = Comparison.Always,
        IsStencilEnabled = false,
    };
    /// <summary>
    /// The DSS no depth no stencil
    /// </summary>
    public readonly static DepthStencilStateDescription DSSNoDepthNoStencil = new()
    {
        IsDepthEnabled = false,
        IsStencilEnabled = false,
        DepthWriteMask = DepthWriteMask.Zero,
        DepthComparison = Comparison.Always,
        FrontFace = new DepthStencilOperationDescription()
        {
            FailOperation = StencilOperation.Keep,
            DepthFailOperation = StencilOperation.Keep,
            PassOperation = StencilOperation.Keep,
            Comparison = Comparison.Always
        },
        BackFace = new DepthStencilOperationDescription()
        {
            Comparison = Comparison.Always,
            FailOperation = StencilOperation.Keep,
            DepthFailOperation = StencilOperation.Keep,
            PassOperation = StencilOperation.Keep
        },
        StencilReadMask = 0,
        StencilWriteMask = 0
    };

    public readonly static DepthStencilStateDescription DSSVolumeBackFace = new()
    {
        IsDepthEnabled = true,
        DepthWriteMask = DepthWriteMask.All,
        DepthComparison = Comparison.Less,
        IsStencilEnabled = true,
        FrontFace = new DepthStencilOperationDescription()
        {
            FailOperation = StencilOperation.Keep,
            DepthFailOperation = StencilOperation.Keep,
            PassOperation = StencilOperation.Zero,
            Comparison = Comparison.Always
        },
        BackFace = new DepthStencilOperationDescription()
        {
            Comparison = Comparison.Always,
            FailOperation = StencilOperation.Keep,
            DepthFailOperation = StencilOperation.Keep,
            PassOperation = StencilOperation.Zero
        },
        StencilReadMask = 0x00,
        StencilWriteMask = 0xFF
    };

    public readonly static DepthStencilStateDescription DSSVolumeFrontFace = new()
    {
        IsDepthEnabled = true,
        DepthWriteMask = DepthWriteMask.All,
        DepthComparison = Comparison.Less,
        IsStencilEnabled = true,
        FrontFace = new DepthStencilOperationDescription()
        {
            FailOperation = StencilOperation.Keep,
            DepthFailOperation = StencilOperation.Keep,
            PassOperation = StencilOperation.Keep,
            Comparison = Comparison.Equal
        },
        BackFace = new DepthStencilOperationDescription()
        {
            Comparison = Comparison.Never,
            FailOperation = StencilOperation.Keep,
            DepthFailOperation = StencilOperation.Keep,
            PassOperation = StencilOperation.Zero
        },
        StencilReadMask = 0xFF,
        StencilWriteMask = 0x00
    };
}

/// <summary>
/// 
/// </summary>
public static class DefaultRasterDescriptions
{
    /// <summary>
    /// The solid no msaa RasterizerState
    /// </summary>
    public readonly static RasterizerStateDescription RSSolidNoMSAA = new()
    {
        FillMode = FillMode.Solid,
        CullMode = CullMode.Back,
        DepthBias = -5,
        DepthBiasClamp = -10,
        SlopeScaledDepthBias = +0,
        IsFrontCounterClockwise = true,
        IsMultisampleEnabled = false,
        IsAntialiasedLineEnabled = false,
    };

    /// <summary>
    /// The skybox RasterizerState
    /// </summary>
    public readonly static RasterizerStateDescription RSSkybox = new()
    {
        FillMode = FillMode.Solid,
        CullMode = CullMode.None,
        DepthBias = 0,
        DepthBiasClamp = 0,
        SlopeScaledDepthBias = +0,
        IsFrontCounterClockwise = true,
        IsMultisampleEnabled = false,
        IsAntialiasedLineEnabled = false,
        IsDepthClipEnabled = false
    };

    public readonly static RasterizerStateDescription RSSkyDome = new()
    {
        FillMode = FillMode.Solid,
        CullMode = CullMode.None,
        DepthBias = 0,
        DepthBiasClamp = 0,
        SlopeScaledDepthBias = +0,
        IsFrontCounterClockwise = false,
        IsMultisampleEnabled = false,
        IsAntialiasedLineEnabled = false,
        IsDepthClipEnabled = false
    };

    public readonly static RasterizerStateDescription RSOutline = new()
    {
        FillMode = FillMode.Solid,
        CullMode = CullMode.None,
        DepthBias = 0,
        DepthBiasClamp = 0,
        SlopeScaledDepthBias = +0,
        IsFrontCounterClockwise = true,
        IsMultisampleEnabled = false,
        IsAntialiasedLineEnabled = false,
    };

    public readonly static RasterizerStateDescription RSPlaneGrid = new()
    {
        FillMode = FillMode.Solid,
        CullMode = CullMode.None,
        DepthBias = 10,
        DepthBiasClamp = 1000,
        SlopeScaledDepthBias = 0,
        IsFrontCounterClockwise = true,
        IsMultisampleEnabled = false,
        IsAntialiasedLineEnabled = false,
        IsDepthClipEnabled = true,
        IsScissorEnabled = true
    };

    public readonly static RasterizerStateDescription RSSpriteCW = new()
    {
        FillMode = FillMode.Solid,
        CullMode = CullMode.None,
        DepthBias = 0,
        DepthBiasClamp = 0,
        SlopeScaledDepthBias = 0,
        IsFrontCounterClockwise = false,
        IsMultisampleEnabled = false,
        IsAntialiasedLineEnabled = false,
        IsDepthClipEnabled = false,
        IsScissorEnabled = true
    };

    public readonly static RasterizerStateDescription RSVolume = new()
    {
        FillMode = FillMode.Solid,
        CullMode = CullMode.None,
        DepthBias = 0,
        DepthBiasClamp = 0,
        SlopeScaledDepthBias = +0,
        IsFrontCounterClockwise = true,
        IsMultisampleEnabled = false,
        IsAntialiasedLineEnabled = false,
    };

    public readonly static RasterizerStateDescription RSVolumeCubeFront = new()
    {
        FillMode = FillMode.Solid,
        CullMode = CullMode.Back,
        DepthBias = 0,
        DepthBiasClamp = 0,
        SlopeScaledDepthBias = +0,
        IsFrontCounterClockwise = true,
        IsMultisampleEnabled = false,
        IsAntialiasedLineEnabled = false,
    };

    public readonly static RasterizerStateDescription RSVolumeCubeBack = new()
    {
        FillMode = FillMode.Solid,
        CullMode = CullMode.Front,
        DepthBias = 0,
        DepthBiasClamp = 0,
        SlopeScaledDepthBias = +0,
        IsFrontCounterClockwise = true,
        IsMultisampleEnabled = false,
        IsAntialiasedLineEnabled = false,
    };

    /// <summary>
    /// The screen duplication RasterizerState
    /// </summary>
    public readonly static RasterizerStateDescription RSScreenDuplication = new()
    {
        FillMode = FillMode.Solid,
        CullMode = CullMode.None,
        DepthBias = 0,
        DepthBiasClamp = 0,
        SlopeScaledDepthBias = +0,
        IsFrontCounterClockwise = true,
        IsMultisampleEnabled = false,
        IsAntialiasedLineEnabled = false,
        IsScissorEnabled = true
    };
}
