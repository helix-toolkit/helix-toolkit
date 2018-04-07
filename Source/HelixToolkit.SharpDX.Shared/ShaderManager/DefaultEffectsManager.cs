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
    using HelixToolkit.Logger;
    using Shaders;
    /// <summary>
    /// Default shader technique manager, includes all internal shaders
    /// </summary>
    public class DefaultEffectsManager : EffectsManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEffectsManager"/> class.
        /// </summary>
        /// <param name="adapterIndex">Index of the adapter.</param>
        public DefaultEffectsManager(int adapterIndex) : base(adapterIndex)
        {
            AddDefaultTechniques();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEffectsManager"/> class.
        /// </summary>
        /// <param name="adapterIndex">Index of the adapter.</param>
        /// <param name="externallogger">The externallogger.</param>
        public DefaultEffectsManager(int adapterIndex, ILogger externallogger): base(adapterIndex, externallogger)
        {
            AddDefaultTechniques();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEffectsManager"/> class.
        /// </summary>
        public DefaultEffectsManager() : base()
        {
            AddDefaultTechniques();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEffectsManager"/> class.
        /// </summary>
        /// <param name="externallogger">The externallogger.</param>
        public DefaultEffectsManager(ILogger externallogger) : base(externallogger)
        {
            AddDefaultTechniques();
        }

        private void AddDefaultTechniques()
        {
            foreach(var technique in LoadTechniqueDescriptions())
            {
                AddTechnique(technique);
            }
        }
        /// <summary>
        /// Loads the technique descriptions.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TechniqueDescription> LoadTechniqueDescriptions()
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshOutline)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshXRay
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOverlayBlending,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite
                    },
                    new ShaderPassDescription(DefaultPassNames.ShadowPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshShadow,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Wireframe)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSMeshWireframe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectOutlineP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSMeshOutlineQuadStencil
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSMeshOutlineP1,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayP1,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayP2)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSEffectMeshXRay
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOverlayBlending,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayP2
                    },
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.ShadowPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshShadow,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Wireframe)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSMeshWireframe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,
                    }
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.ShadowPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshShadow,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Wireframe)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSMeshWireframe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,
                    }
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.ShadowPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshShadow,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Wireframe)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSMeshWireframe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,
                    }
                }
            };

            var renderDiffuseMap = new TechniqueDescription(DefaultRenderTechniqueNames.Diffuse)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshDefault, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshDiffuseMap
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshTriTessellation)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshTessellation,
                            DefaultHullShaderDescriptions.HSMeshTessellation,
                            DefaultDomainShaderDescriptions.DSMeshTessellation,
                            DefaultPSShaderDescriptions.PSMeshDiffuseMap
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.ShadowPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshShadow,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Wireframe)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSMeshWireframe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectOutlineP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSMeshOutlineQuadStencil
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSMeshOutlineP1,
                    },
                }
            };

            var renderViewCube = new TechniqueDescription(DefaultRenderTechniqueNames.ViewCube)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshDefault, DefaultInputLayout.VSInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshViewCube
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    }
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.ShadowPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshShadow,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Wireframe)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSMeshWireframe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectOutlineP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSMeshOutlineQuadStencil
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSMeshOutlineP1,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayP1,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayP2)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultPSShaderDescriptions.PSEffectMeshXRay
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOverlayBlending,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayP2
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.ShadowPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBoneSkinningShadow,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Wireframe)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSBoneSkinningWireframe,
                            DefaultPSShaderDescriptions.PSMeshWireframe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectOutlineP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSBoneSkinningWireframe,
                            DefaultPSShaderDescriptions.PSMeshOutlineQuadStencil
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSMeshOutlineP1,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSBoneSkinningWireframe,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayP1,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayP2)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBoneSkinning,
                            DefaultPSShaderDescriptions.PSEffectMeshXRay
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOverlayBlending,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayP2
                    },
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.ShadowPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSPointShadow,
                            DefaultGSShaderDescriptions.GSPoint,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectOutlineP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSPoint,
                            DefaultGSShaderDescriptions.GSPoint,
                            DefaultPSShaderDescriptions.PSMeshOutlineQuadStencil
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSMeshOutlineP1,
                    },
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.ShadowPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSPointShadow,
                            DefaultGSShaderDescriptions.GSLine,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectOutlineP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSPoint,
                            DefaultGSShaderDescriptions.GSLine,
                            DefaultPSShaderDescriptions.PSMeshOutlineQuadStencil
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSMeshOutlineP1,
                    },
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },                    
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
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhong
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Backface)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSClipPlaneFillQuad
                    },
                    new ShaderPassDescription(DefaultPassNames.Wireframe)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSMeshWireframe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectOutlineP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSMeshOutlineQuadStencil
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSMeshOutlineP1,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayP1,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayP2)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSEffectMeshXRay
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOverlayBlending,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayP2
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

            var renderSkybox = new TechniqueDescription(DefaultRenderTechniqueNames.Skybox)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSSkybox, DefaultInputLayout.VSInputSkybox),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSSkybox,
                            DefaultPSShaderDescriptions.PSSkybox
                        },
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,    
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        RasterStateDescription = DefaultRasterDescriptions.RSSkybox                   
                    },                   
                }
            };
            #region Post Effects
            var meshOutlineBlurPostEffect = new TechniqueDescription(DefaultRenderTechniqueNames.PostEffectMeshOutlineBlur)
            {
                InputLayoutDescription = InputLayoutDescription.EmptyInputLayout,
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.ScreenQuad)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSMeshOutlineScreenQuad
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSOutlineFillQuad,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectBlurVertical)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSEffectFullScreenBlurVertical
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectBlurHorizontal)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSEffectFullScreenBlurHorizontal
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshOutline)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSMeshOutlineQuadFinal
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline
                    },
                }
            };

            var meshBorderHighlightPostEffect = new TechniqueDescription(DefaultRenderTechniqueNames.PostEffectMeshBorderHighlight)
            {
                InputLayoutDescription = InputLayoutDescription.EmptyInputLayout,
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.ScreenQuad)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSMeshOutlineScreenQuad
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSOutlineFillQuad,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectBlurVertical)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSEffectMeshBorderHighlight
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshOutline)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSMeshOutlineQuadFinal
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline
                    },
                }
            };

            var bloomPostEffect = new TechniqueDescription(DefaultRenderTechniqueNames.PostEffectBloom)
            {
                InputLayoutDescription = InputLayoutDescription.EmptyInputLayout,
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.ScreenQuad)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSEffectBloomExtract
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline
                    },
                    new ShaderPassDescription(DefaultPassNames.ScreenQuadCopy)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSMeshOutlineQuadFinal
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectBlurVertical)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSEffectBloomVerticalBlur
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectBlurHorizontal)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSEffectBloomHorizontalBlur
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshOutline)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSEffectBloomCombine
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.AdditiveBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline
                    },
                }
            };
            #endregion

#if !NETFX_CORE
            var renderScreenDup = new TechniqueDescription(DefaultRenderTechniqueNames.ScreenDuplication)
            {
                InputLayoutDescription = InputLayoutDescription.EmptyInputLayout,
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSScreenDup,
                            DefaultPSShaderDescriptions.PSScreenDup
                        },
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        RasterStateDescription = DefaultRasterDescriptions.RSScreenDuplication
                    },
                    new ShaderPassDescription(DefaultPassNames.ScreenQuad)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSScreenDupCursor,
                            DefaultPSShaderDescriptions.PSScreenDup
                        },
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        BlendStateDescription = DefaultBlendStateDescriptions.BSScreenDupCursorBlend,
                        RasterStateDescription = DefaultRasterDescriptions.RSScreenDuplication
                    }
                }
            };
#endif
            yield return renderBlinn;
            yield return renderBlinnInstancing;
            yield return renderBoneSkinning;
            yield return renderPoint;
            yield return renderLine;
            yield return renderBillboardText;
            yield return renderBillboardInstancing;
            yield return renderNormals;
            yield return renderColors;
            yield return renderPositions;
            yield return renderDiffuseMap;
            yield return renderViewCube;
            yield return renderMeshBlinnClipPlane;
            yield return renderParticle;
            yield return renderSkybox;
            yield return meshOutlineBlurPostEffect;
            yield return meshBorderHighlightPostEffect;
            yield return bloomPostEffect;
#if !NETFX_CORE
            yield return renderScreenDup;
#endif
        }
    }
}
