/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using Shaders;
    /// <summary>
    /// Default shader technique manager, includes all internal shaders
    /// </summary>
    public class DefaultShaderTechniqueManager : ShaderTechniqueManager
    {
        protected override IList<TechniqueDescription> LoadTechniqueDescriptions()
        {
            var renderBlinn = new TechniqueDescription(DefaultRenderTechniqueNames.Blinn)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshDefault, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhong
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshTriTessellation)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshTessellation,
                            DefaultHullShaderDescriptions.HSMeshTessellation,
                            DefaultDomainShaderDescriptions.DSMeshTessellation,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhong
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshOutline)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshXRay,
                            DefaultPSShaderDescriptions.PSMeshXRay
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOverlayBlending,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshXRay)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshXRay,
                            DefaultPSShaderDescriptions.PSMeshXRay
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOverlayBlending,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSGreaterNoWrite
                    }
                }
            };

            var renderColors = new TechniqueDescription(DefaultRenderTechniqueNames.Colors)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshDefault, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshVertColor
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshTriTessellation)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshTessellation,
                            DefaultHullShaderDescriptions.HSMeshTessellation,
                            DefaultDomainShaderDescriptions.DSMeshTessellation,
                            DefaultPSShaderDescriptions.PSMeshVertColor
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                }
            };

            var renderNormals = new TechniqueDescription(DefaultRenderTechniqueNames.Normals)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshDefault, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshVertNormal
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshTriTessellation)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshTessellation,
                            DefaultHullShaderDescriptions.HSMeshTessellation,
                            DefaultDomainShaderDescriptions.DSMeshTessellation,
                            DefaultPSShaderDescriptions.PSMeshVertNormal
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                }
            };

            var renderPositions = new TechniqueDescription(DefaultRenderTechniqueNames.Positions)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshDefault, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshVertPosition
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshTriTessellation)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshTessellation,
                            DefaultHullShaderDescriptions.HSMeshTessellation,
                            DefaultDomainShaderDescriptions.DSMeshTessellation,
                            DefaultPSShaderDescriptions.PSMeshVertPosition
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                }
            };


            var renderBlinnInstancing = new TechniqueDescription(DefaultRenderTechniqueNames.InstancingBlinn)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshInstancing, DefaultInputLayout.VSInputInstancing),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhong
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshTriTessellation)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancingTessellation,
                            DefaultHullShaderDescriptions.HSMeshTessellation,
                            DefaultDomainShaderDescriptions.DSMeshTessellation,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhong
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                }
            };

            var renderBoneSkinning = new TechniqueDescription(DefaultRenderTechniqueNames.BoneSkinBlinn)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshBoneSkinning, DefaultInputLayout.VSInputBoneSkinning),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBoneSkinning,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhong
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshTriTessellation)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBoneSkinningTessellation,
                            DefaultHullShaderDescriptions.HSMeshTessellation,
                            DefaultDomainShaderDescriptions.DSMeshTessellation,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhong
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
            };

            var renderPoint = new TechniqueDescription(DefaultRenderTechniqueNames.Points)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSPoint, DefaultInputLayout.VSInputPoint),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSPoint,
                            DefaultGSShaderDescriptions.GSPoint,
                            DefaultPSShaderDescriptions.PSPoint
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
            };

            var renderLine = new TechniqueDescription(DefaultRenderTechniqueNames.Lines)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSPoint, DefaultInputLayout.VSInputPoint),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSPoint,
                            DefaultGSShaderDescriptions.GSLine,
                            DefaultPSShaderDescriptions.PSLine
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
            };

            var renderBillboardText = new TechniqueDescription(DefaultRenderTechniqueNames.BillboardText)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSBillboard, DefaultInputLayout.VSInputBillboard),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSBillboardText,
                            DefaultGSShaderDescriptions.GSBillboard,
                            DefaultPSShaderDescriptions.PSBillboardText
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
            };
            var renderBillboardInstancing = new TechniqueDescription(DefaultRenderTechniqueNames.BillboardInstancing)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSBillboardInstancing, DefaultInputLayout.VSInputBillboardInstancing),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSBillboardInstancing,
                            DefaultGSShaderDescriptions.GSBillboard,
                            DefaultPSShaderDescriptions.PSBillboardText
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
                }
            };

            var renderMeshBlinnClipPlane = new TechniqueDescription(DefaultRenderTechniqueNames.CrossSection)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshDefault, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshClipPlane
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Backface)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshClipBackface
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSClipPlaneBackface
                    },
                    new ShaderPassDescription(DefaultPassNames.ScreenQuad)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSScreenQuad,
                            DefaultPSShaderDescriptions.PSMeshClipScreenQuad
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSNormal,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSClipPlaneFillQuad
                    },
                }
            };

            var renderParticle = new TechniqueDescription(DefaultRenderTechniqueNames.ParticleStorm)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSParticle, DefaultInputLayout.VSInputParticle),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultParticlePassNames.Insert)
                    {
                        ShaderList = new[]
                        {
                            DefaultComputeShaderDescriptions.CSParticleInsert
                        }
                    },
                    new ShaderPassDescription(DefaultParticlePassNames.Update)
                    {
                        ShaderList = new[]
                        {
                            DefaultComputeShaderDescriptions.CSParticleUpdate
                        }
                    },
                    new ShaderPassDescription(DefaultParticlePassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSParticle,
                            DefaultGSShaderDescriptions.GSParticle,
                            DefaultPSShaderDescriptions.PSParticle
                        },
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite,
                        RasterStateDescription = DefaultRasterDescriptions.RSSolidNoMSAA
                    }
                }
            };

            return new List<TechniqueDescription>
            {
                renderBlinn,
                renderBlinnInstancing,
                renderBoneSkinning,
                renderPoint,
                renderLine,
                renderBillboardText,
                renderBillboardInstancing,
                renderNormals,
                renderColors,
                renderPositions,
                renderMeshBlinnClipPlane,
                renderParticle,
            };
        }
    }
}
