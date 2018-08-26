/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using Model;


    /// <summary>
    /// 
    /// </summary>
    public static class DefaultBlendStateDescriptions
    {
        public readonly static BlendStateDescription BSAlphaBlend;
        public readonly static BlendStateDescription BSSourceAlways;
        public readonly static BlendStateDescription NoBlend;
        public readonly static BlendStateDescription BSOverlayBlending;
        public readonly static BlendStateDescription AdditiveBlend;
        public readonly static BlendStateDescription BSScreenDupCursorBlend;
        public readonly static BlendStateDescription BSOITBlend = new BlendStateDescription() { IndependentBlendEnable = true };
        public readonly static BlendStateDescription BSMeshOITBlendQuad;

        static DefaultBlendStateDescriptions()
        {
            BSAlphaBlend.RenderTarget[0] = new RenderTargetBlendDescription()
            {
                AlphaBlendOperation = BlendOperation.Add,
                BlendOperation = BlendOperation.Add,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.InverseSourceAlpha,

                SourceAlphaBlend = BlendOption.SourceAlpha,
                DestinationAlphaBlend = BlendOption.DestinationAlpha,
                IsBlendEnabled = true,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };

            BSSourceAlways.RenderTarget[0] = new RenderTargetBlendDescription()
            {
                AlphaBlendOperation = BlendOperation.Add,
                BlendOperation = BlendOperation.Add,
                DestinationBlend = BlendOption.Zero,
                SourceBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.Zero,
                SourceAlphaBlend = BlendOption.One,
                IsBlendEnabled = false,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };

            NoBlend.RenderTarget[0] = new RenderTargetBlendDescription() { IsBlendEnabled = false };
            BSOverlayBlending.RenderTarget[0] = new RenderTargetBlendDescription()
            {
                IsBlendEnabled = true,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.One,
                BlendOperation = BlendOperation.Add,
                SourceAlphaBlend = BlendOption.Zero,
                DestinationAlphaBlend = BlendOption.One,
                AlphaBlendOperation = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };

            AdditiveBlend.RenderTarget[0] = new RenderTargetBlendDescription()
            {
                IsBlendEnabled = true,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.One,
                BlendOperation = BlendOperation.Add,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.One,
                AlphaBlendOperation = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };

            BSScreenDupCursorBlend.RenderTarget[0] = new RenderTargetBlendDescription()
            {
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                BlendOperation = BlendOperation.Add,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.Zero,
                AlphaBlendOperation = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.All,
                IsBlendEnabled = true
            };

            BSOITBlend.RenderTarget[0] = new RenderTargetBlendDescription()
            {
                IsBlendEnabled = true,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.One,
                BlendOperation = BlendOperation.Add,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.One,
                AlphaBlendOperation = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };
            BSOITBlend.RenderTarget[1] = new RenderTargetBlendDescription()
            {
                IsBlendEnabled = true,
                SourceBlend = BlendOption.Zero,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                BlendOperation = BlendOperation.Add,
                SourceAlphaBlend = BlendOption.Zero,
                DestinationAlphaBlend = BlendOption.InverseSourceAlpha,
                AlphaBlendOperation = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.Alpha
            };

            BSMeshOITBlendQuad.RenderTarget[0] = new RenderTargetBlendDescription()
            {
                IsBlendEnabled = true,
                SourceBlend = BlendOption.InverseSourceAlpha,
                DestinationBlend = BlendOption.SourceAlpha,
                BlendOperation = BlendOperation.Add,
                SourceAlphaBlend = BlendOption.InverseSourceAlpha,
                DestinationAlphaBlend = BlendOption.DestinationAlpha,
                AlphaBlendOperation = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.Red | ColorWriteMaskFlags.Green | ColorWriteMaskFlags.Blue
            };
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class DefaultDepthStencilDescriptions
    {
        /// <summary>
        /// The DSS depth less
        /// </summary>
        public readonly static DepthStencilStateDescription DSSDepthLess = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            DepthWriteMask = DepthWriteMask.All,
            DepthComparison = Comparison.Less,
            IsStencilEnabled = false
        };
        /// <summary>
        /// The DSS depth less equal
        /// </summary>
        public readonly static DepthStencilStateDescription DSSDepthLessEqual = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            DepthWriteMask = DepthWriteMask.All,
            DepthComparison = Comparison.LessEqual,
            IsStencilEnabled = false
        };
        /// <summary>
        /// The DSS less no write
        /// </summary>
        public readonly static DepthStencilStateDescription DSSLessNoWrite = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            DepthWriteMask = DepthWriteMask.Zero,
            DepthComparison = Comparison.Less,
            IsStencilEnabled = false
        };
        /// <summary>
        /// The DSS less equal no write
        /// </summary>
        public readonly static DepthStencilStateDescription DSSLessEqualNoWrite = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            DepthWriteMask = DepthWriteMask.Zero,
            DepthComparison = Comparison.LessEqual,
            IsStencilEnabled = false
        };
        /// <summary>
        /// The DSS greater no write
        /// </summary>
        public readonly static DepthStencilStateDescription DSSGreaterNoWrite = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            DepthWriteMask = DepthWriteMask.Zero,
            DepthComparison = Comparison.Greater
        };
        /// <summary>
        /// The DSS equal no write
        /// </summary>
        public readonly static DepthStencilStateDescription DSSEqualNoWrite = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            DepthWriteMask = DepthWriteMask.Zero,
            DepthComparison = Comparison.Equal
        };
        /// <summary>
        /// The DSS clip plane backface
        /// </summary>
        public readonly static DepthStencilStateDescription DSSClipPlaneBackface = new DepthStencilStateDescription()
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
        public readonly static DepthStencilStateDescription DSSMeshOutlineP1 = new DepthStencilStateDescription()
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
                DepthFailOperation = StencilOperation.Keep,
                FailOperation = StencilOperation.Keep
            }
        };

        /// <summary>
        /// The DSS mesh outline pass1
        /// </summary>
        public readonly static DepthStencilStateDescription DSSEffectMeshXRayP1 = new DepthStencilStateDescription()
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

        public readonly static DepthStencilStateDescription DSSEffectMeshXRayGridP1 = new DepthStencilStateDescription()
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
        public readonly static DepthStencilStateDescription DSSEffectMeshXRayGridP2 = new DepthStencilStateDescription()
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
        public readonly static DepthStencilStateDescription DSSEffectMeshXRayGridP3 = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            IsStencilEnabled = true,
            DepthWriteMask = DepthWriteMask.All,
            DepthComparison = Comparison.Less,
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
        public readonly static DepthStencilStateDescription DSSEffectMeshXRayP2 = new DepthStencilStateDescription()
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
        public readonly static DepthStencilStateDescription DSSOutlineFillQuad = new DepthStencilStateDescription()
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
        public readonly static DepthStencilStateDescription DSSClipPlaneFillQuad = new DepthStencilStateDescription()
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
        public readonly static DepthStencilStateDescription DSSDepthAlwaysNoStencil = new DepthStencilStateDescription()
        {
            IsDepthEnabled = true,
            DepthComparison = Comparison.Always,
            IsStencilEnabled = false,
        };
        /// <summary>
        /// The DSS no depth no stencil
        /// </summary>
        public readonly static DepthStencilStateDescription DSSNoDepthNoStencil = new DepthStencilStateDescription()
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
    }

    /// <summary>
    /// 
    /// </summary>
    public static class DefaultRasterDescriptions
    {
        /// <summary>
        /// The solid no msaa RasterizerState
        /// </summary>
        public readonly static RasterizerStateDescription RSSolidNoMSAA = new RasterizerStateDescription()
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
        public readonly static RasterizerStateDescription RSSkybox = new RasterizerStateDescription()
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

        public readonly static RasterizerStateDescription RSSkyDome = new RasterizerStateDescription()
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

        public readonly static RasterizerStateDescription RSOutline = new RasterizerStateDescription()
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

        public readonly static RasterizerStateDescription RSPlaneGrid = new RasterizerStateDescription()
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
#if !NETFX_CORE        
        /// <summary>
        /// The screen duplication RasterizerState
        /// </summary>
        public readonly static RasterizerStateDescription RSScreenDuplication = new RasterizerStateDescription()
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
#endif
    }
}