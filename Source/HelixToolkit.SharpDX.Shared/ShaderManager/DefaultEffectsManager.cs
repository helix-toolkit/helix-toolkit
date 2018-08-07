/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using global::SharpDX.Direct3D;
using HelixToolkit.Mathematics;
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
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Colors)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshVertColor
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Normals)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshVertNormal
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Positions)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshVertPosition
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Diffuse)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshDiffuseMap
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.ColorStripe1D)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshColorStripe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.ViewCube)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshViewCube
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.NormalVector)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultGSShaderDescriptions.GSMeshNormalVector,
                            DefaultPSShaderDescriptions.PSLineColor
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess,
                        Topology = PrimitiveTopology.PointList
                    },
                    new ShaderPassDescription(DefaultPassNames.OITPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhongOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite
                    },
                    new ShaderPassDescription(DefaultPassNames.DiffuseOIT)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSMeshDiffuseMapOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite
                    },
                    new ShaderPassDescription(DefaultPassNames.PreComputeMeshBoneSkinned)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBoneSkinnedBasic,
                            DefaultGSShaderDescriptions.GSMeshBoneSkinnedOut
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        Topology = PrimitiveTopology.PointList,
                        InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshBoneSkinningBasic, DefaultInputLayout.VSInputBoneSkinnedBasic),
                    },
                    new ShaderPassDescription(DefaultPassNames.DepthPrepass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
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
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual,
                        Topology = PrimitiveTopology.PatchListWith3ControlPoints
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshTriTessellationOIT)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshTessellation,
                            DefaultHullShaderDescriptions.HSMeshTessellation,
                            DefaultDomainShaderDescriptions.DSMeshTessellation,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhongOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite,
                        Topology = PrimitiveTopology.PatchListWith3ControlPoints
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
                        Topology = PrimitiveTopology.TriangleList
                    },
                    new ShaderPassDescription(DefaultPassNames.WireframeOITPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSMeshWireframeOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,
                        Topology = PrimitiveTopology.TriangleList
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
                        StencilRef = 1
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
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayP2,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayGridP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP1,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayGridP2)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP2,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayGridP3)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSEffectXRayGrid
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP3,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshDiffuseXRayGridP3)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshDefault,
                            DefaultPSShaderDescriptions.PSEffectDiffuseXRayGrid
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP3,
                        StencilRef = 1
                    },
                }
            };

            var renderBlinnBatched = new TechniqueDescription(DefaultRenderTechniqueNames.BlinnBatched)
            {
                InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshBatched, DefaultInputLayout.VSMeshBatchedInput),
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhong
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Colors)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSMeshVertColor
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Normals)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSMeshVertNormal
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Positions)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSMeshVertPosition
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Diffuse)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSMeshDiffuseMap
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.ColorStripe1D)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSMeshColorStripe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.NormalVector)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultGSShaderDescriptions.GSMeshNormalVector,
                            DefaultPSShaderDescriptions.PSLineColor
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess,
                        Topology = PrimitiveTopology.PointList
                    },
                    new ShaderPassDescription(DefaultPassNames.OITPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhongOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite
                    },
                    new ShaderPassDescription(DefaultPassNames.PreComputeMeshBoneSkinned)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBoneSkinnedBasic,
                            DefaultGSShaderDescriptions.GSMeshBoneSkinnedOut
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        Topology = PrimitiveTopology.PointList,
                        InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshBoneSkinningBasic, DefaultInputLayout.VSInputBoneSkinnedBasic),
                    },
                    new ShaderPassDescription(DefaultPassNames.DiffuseOIT)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSMeshDiffuseMapOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite
                    },
                    new ShaderPassDescription(DefaultPassNames.DepthPrepass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    //new ShaderPassDescription(DefaultPassNames.MeshTriTessellation)
                    //{
                    //    ShaderList = new[]
                    //    {
                    //        DefaultVSShaderDescriptions.VSMeshTessellation,
                    //        DefaultHullShaderDescriptions.HSMeshTessellation,
                    //        DefaultDomainShaderDescriptions.DSMeshTessellation,
                    //        DefaultPSShaderDescriptions.PSMeshBlinnPhong
                    //    },
                    //    BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                    //    DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual,
                    //    Topology = PrimitiveTopology.PatchListWith3ControlPoints
                    //},
                    //new ShaderPassDescription(DefaultPassNames.MeshTriTessellationOIT)
                    //{
                    //    ShaderList = new[]
                    //    {
                    //        DefaultVSShaderDescriptions.VSMeshTessellation,
                    //        DefaultHullShaderDescriptions.HSMeshTessellation,
                    //        DefaultDomainShaderDescriptions.DSMeshTessellation,
                    //        DefaultPSShaderDescriptions.PSMeshBlinnPhongOIT
                    //    },
                    //    BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                    //    DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite,
                    //    Topology = PrimitiveTopology.PatchListWith3ControlPoints
                    //},
                    new ShaderPassDescription(DefaultPassNames.MeshOutline)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSMeshXRay
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOverlayBlending,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite
                    },
                    new ShaderPassDescription(DefaultPassNames.ShadowPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatchedShadow,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess
                    },
                    new ShaderPassDescription(DefaultPassNames.Wireframe)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatchedWireframe,
                            DefaultPSShaderDescriptions.PSMeshWireframe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,
                        Topology = PrimitiveTopology.TriangleList
                    },
                    new ShaderPassDescription(DefaultPassNames.WireframeOITPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatchedWireframe,
                            DefaultPSShaderDescriptions.PSMeshWireframeOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,
                        Topology = PrimitiveTopology.TriangleList
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectOutlineP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatchedWireframe,
                            DefaultPSShaderDescriptions.PSMeshOutlineQuadStencil
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSMeshOutlineP1,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatchedWireframe,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayP1,
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayP2)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSEffectMeshXRay
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOverlayBlending,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayP2,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayGridP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatchedWireframe,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP1,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayGridP2)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatchedWireframe,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP2,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayGridP3)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSEffectXRayGrid
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP3,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshDiffuseXRayGridP3)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBatched,
                            DefaultPSShaderDescriptions.PSEffectDiffuseXRayGrid
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP3,
                        StencilRef = 1
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
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Colors)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultPSShaderDescriptions.PSMeshVertColor
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Normals)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultPSShaderDescriptions.PSMeshVertNormal
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Positions)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultPSShaderDescriptions.PSMeshVertPosition
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Diffuse)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultPSShaderDescriptions.PSMeshDiffuseMap
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.ColorStripe1D)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultPSShaderDescriptions.PSMeshColorStripe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.NormalVector)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultGSShaderDescriptions.GSMeshNormalVector,
                            DefaultPSShaderDescriptions.PSLineColor
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess,
                        Topology = PrimitiveTopology.PointList
                    },
                    new ShaderPassDescription(DefaultPassNames.PreComputeMeshBoneSkinned)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshBoneSkinnedBasic,
                            DefaultGSShaderDescriptions.GSMeshBoneSkinnedOut
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        Topology = PrimitiveTopology.PointList,
                        InputLayoutDescription = new InputLayoutDescription(DefaultVSShaderByteCodes.VSMeshBoneSkinningBasic, DefaultInputLayout.VSInputBoneSkinnedBasic),
                    },
                    new ShaderPassDescription(DefaultPassNames.OITPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhongOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite
                    },
                    new ShaderPassDescription(DefaultPassNames.DiffuseOIT)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultPSShaderDescriptions.PSMeshDiffuseMapOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite
                    },
                    new ShaderPassDescription(DefaultPassNames.DepthPrepass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
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
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual,
                        Topology = PrimitiveTopology.PatchListWith3ControlPoints
                    },
                    new ShaderPassDescription(DefaultPassNames.MeshTriTessellationOIT)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancingTessellation,
                            DefaultHullShaderDescriptions.HSMeshTessellation,
                            DefaultDomainShaderDescriptions.DSMeshTessellation,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhongOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite,
                        Topology = PrimitiveTopology.PatchListWith3ControlPoints
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
                        Topology = PrimitiveTopology.TriangleList
                    },
                    new ShaderPassDescription(DefaultPassNames.WireframeOITPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSMeshWireframeOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,
                        Topology = PrimitiveTopology.TriangleList
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
                        StencilRef = 1
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
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayP2,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayGridP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP1,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayGridP2)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshWireframe,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP2,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayGridP3)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultPSShaderDescriptions.PSEffectXRayGrid
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP3,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshDiffuseXRayGridP3)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshInstancing,
                            DefaultPSShaderDescriptions.PSEffectDiffuseXRayGrid
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP3,
                        StencilRef = 1
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
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.DepthPrepass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSPoint,
                            DefaultGSShaderDescriptions.GSPoint,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
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
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
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
                        StencilRef = 1
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
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.DepthPrepass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSPoint,
                            DefaultGSShaderDescriptions.GSLine,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
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
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
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
                        StencilRef = 1
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
                    new ShaderPassDescription(DefaultPassNames.OITPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSBillboardText,
                            DefaultGSShaderDescriptions.GSBillboard,
                            DefaultPSShaderDescriptions.PSBillboardTextOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite
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
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.OITPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSBillboardInstancing,
                            DefaultGSShaderDescriptions.GSBillboard,
                            DefaultPSShaderDescriptions.PSBillboardTextOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite
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
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Colors)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSMeshVertColor
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Normals)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSMeshVertNormal
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Positions)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSMeshVertPosition
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.Diffuse)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSMeshDiffuseMap
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.ColorStripe1D)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSMeshColorStripe
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLessEqual
                    },
                    new ShaderPassDescription(DefaultPassNames.NormalVector)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultGSShaderDescriptions.GSMeshNormalVector,
                            DefaultPSShaderDescriptions.PSLineColor
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSDepthLess,
                        Topology = PrimitiveTopology.PointList
                    },
                    new ShaderPassDescription(DefaultPassNames.OITPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhongOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite
                    },
                    new ShaderPassDescription(DefaultPassNames.DiffuseOIT)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSMeshDiffuseMapOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite
                    },
                    new ShaderPassDescription(DefaultPassNames.DepthPrepass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSShadow
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
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
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSClipPlaneBackface,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.ScreenQuad)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSScreenQuad,
                            DefaultPSShaderDescriptions.PSMeshClipScreenQuad
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSClipPlaneFillQuad,
                        StencilRef = 1,
                        Topology = PrimitiveTopology.TriangleStrip
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
                    new ShaderPassDescription(DefaultPassNames.WireframeOITPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSMeshWireframeOIT
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
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
                        StencilRef = 1
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
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayGridP1)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP1,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayGridP2)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSDepthStencilOnly
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.NoBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP2,
                        StencilRef = 1
                    },
                    new ShaderPassDescription(DefaultPassNames.EffectMeshXRayGridP3)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshClipPlane,
                            DefaultPSShaderDescriptions.PSEffectXRayGrid
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSEffectMeshXRayGridP3,
                        StencilRef = 1
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
                        RasterStateDescription = DefaultRasterDescriptions.RSSolidNoMSAA,
                        Topology = PrimitiveTopology.PointList
                    },
                    new ShaderPassDescription(DefaultPassNames.OITPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSParticle,
                            DefaultGSShaderDescriptions.GSParticle,
                            DefaultPSShaderDescriptions.PSParticleOIT
                        },
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite,
                        BlendStateDescription = DefaultBlendStateDescriptions.BSOITBlend,
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
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessEqualNoWrite,    
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        RasterStateDescription = DefaultRasterDescriptions.RSSkybox                   
                    },                   
                }
            };

            var meshOITQuad = new TechniqueDescription(DefaultRenderTechniqueNames.MeshOITQuad)
            {
                InputLayoutDescription = InputLayoutDescription.EmptyInputLayout,
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSMeshBlinnPhongOITQuad
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSMeshOITBlendQuad,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
                    }
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
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
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
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
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
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
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
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
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
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
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
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
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
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
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
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
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
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
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
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
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
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
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
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
                    },
                }
            };

            var fxaaPostEffect = new TechniqueDescription(DefaultRenderTechniqueNames.PostEffectFXAA)
            {
                InputLayoutDescription = InputLayoutDescription.EmptyInputLayout,
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.LumaPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSEffectLUMA
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
                    },
                    new ShaderPassDescription(DefaultPassNames.FXAAPass)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSMeshOutlineScreenQuad,
                            DefaultPSShaderDescriptions.PSEffectFXAA
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSSourceAlways,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSNoDepthNoStencil,
                        RasterStateDescription = DefaultRasterDescriptions.RSOutline,
                        Topology = PrimitiveTopology.TriangleStrip
                    },
                }
            };
            #endregion

            var planeGrid = new TechniqueDescription(DefaultRenderTechniqueNames.PlaneGrid)
            {
                InputLayoutDescription = InputLayoutDescription.EmptyInputLayout,
                PassDescriptions = new[]
                {
                    new ShaderPassDescription(DefaultPassNames.Default)
                    {
                        ShaderList = new[]
                        {
                            DefaultVSShaderDescriptions.VSPlaneGrid,
                            DefaultPSShaderDescriptions.PSPlaneGrid
                        },
                        BlendStateDescription = DefaultBlendStateDescriptions.BSAlphaBlend,
                        DepthStencilStateDescription = DefaultDepthStencilDescriptions.DSSLessNoWrite,
                        RasterStateDescription = DefaultRasterDescriptions.RSPlaneGrid,
                        Topology = PrimitiveTopology.TriangleStrip
                    }
                }
            };

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
                        RasterStateDescription = DefaultRasterDescriptions.RSScreenDuplication,
                        Topology = PrimitiveTopology.TriangleStrip
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
                        BlendFactor = new Color4(0,0,0,0),
                        RasterStateDescription = DefaultRasterDescriptions.RSScreenDuplication,
                        Topology = PrimitiveTopology.TriangleStrip
                    }
                }
            };
#endif
            yield return renderBlinn;
            yield return renderBlinnBatched;
            yield return renderBlinnInstancing;
            yield return renderPoint;
            yield return renderLine;
            yield return renderBillboardText;
            yield return renderBillboardInstancing;
            yield return renderMeshBlinnClipPlane;
            yield return renderParticle;
            yield return renderSkybox;
            yield return meshOutlineBlurPostEffect;
            yield return meshBorderHighlightPostEffect;
            yield return bloomPostEffect;
            yield return fxaaPostEffect;
            yield return meshOITQuad;
            yield return planeGrid;
#if !NETFX_CORE
            yield return renderScreenDup;
#endif
        }
    }
}
