/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Assimp;
using Assimp.Configs;
using SharpDX;
using SharpDX.Direct3D11;
using System;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using HelixToolkit.Logger;
    using Model;

    namespace Assimp
    {
        /// <summary>
        /// </summary>
        public enum MaterialType
        {
            /// <summary>
            ///     Automatic determine material type
            /// </summary>
            Auto,

            /// <summary>
            ///     The blinn phong
            /// </summary>
            BlinnPhong,

            /// <summary>
            ///     The PBR
            /// </summary>
            PBR,

            /// <summary>
            ///     The diffuse
            /// </summary>
            Diffuse,

            /// <summary>
            ///     The vertex color
            /// </summary>
            VertexColor,

            /// <summary>
            ///     The normal
            /// </summary>
            Normal,

            /// <summary>
            ///     The position
            /// </summary>
            Position
        }

        /// <summary>
        /// </summary>
        public class ImporterConfiguration
        {
            /// <summary>
            ///     The default post process steps for Assimp Importer. <see cref="PostProcessSteps.FlipUVs" /> must be used for
            ///     DirectX texture sampling
            /// </summary>
            public PostProcessSteps AssimpPostProcessSteps = 
                PostProcessSteps.Triangulate
                | PostProcessSteps.JoinIdenticalVertices
                | PostProcessSteps.FindDegenerates
                | PostProcessSteps.SortByPrimitiveType
                | PostProcessSteps.RemoveRedundantMaterials
                | PostProcessSteps.FlipUVs;

            /// <summary>
            ///     The assimp property configuration
            /// </summary>
            public PropertyConfig[] AssimpPropertyConfig = null;

            /// <summary>
            ///     The cull mode
            /// </summary>
            public CullMode CullMode = CullMode.None;

            /// <summary>
            ///     The enable parallel processing, such as converting Assimp meshes into HelixToolkit meshes
            /// </summary>
            public bool EnableParallelProcessing;

            /// <summary>
            ///     The external context. Can be use to do more customized configuration for Assimp Importer
            /// </summary>
            public AssimpContext ExternalContext = null;

            /// <summary>
            ///     Force cull mode for all imported meshes. Otherwise automatically set cull mode according to the materials.
            /// </summary>
            public bool ForceCullMode = false;

            /// <summary>
            /// Ignores emissive color during importing.
            /// </summary>
            public bool IgnoreEmissiveColor = false;

            /// <summary>
            /// Ignores the ambient color during importing.
            /// </summary>
            public bool IgnoreAmbientColor = false;

            /// <summary>
            ///     Force to use material type. Default is Auto
            /// </summary>
            public MaterialType ImportMaterialType = MaterialType.Auto;
            /// <summary>
            /// Import animations
            /// </summary>
            public bool ImportAnimations = true;
            /// <summary>
            /// The create skeleton mesh for bone skinning
            /// </summary>
            public bool CreateSkeletonForBoneSkinningMesh = false;
            /// <summary>
            /// The skeleton material
            /// </summary>
            public MaterialCore SkeletonMaterial = new Model.DiffuseMaterialCore() { DiffuseColor = Color.Red };
            /// <summary>
            /// The skeleton effects such as xray effects
            /// </summary>
            public string SkeletonEffects = "EffectSkeletonGrid";
            /// <summary>
            /// The skeleton size scale
            /// </summary>
            public float SkeletonSizeScale = 0.1f;
            /// <summary>
            /// The adds post effect for skeleton
            /// </summary>
            public bool AddsPostEffectForSkeleton = true;
            /// <summary>
            /// The flip triangle winding order during import
            /// </summary>
            public bool FlipWindingOrder = false;

            private ILogger logger = new DebugLogger();
            /// <summary>
            /// Gets or sets the logger.
            /// </summary>
            /// <value>
            /// The logger.
            /// </value>
            public ILogger Logger
            {
                set
                {
                    logger = value;
                    if (logger == null)
                    {
                        logger = new DebugLogger();
                    }
                }
                get { return logger; }
            }
            /// <summary>
            /// The global scale for model
            /// </summary>
            public float GlobalScale = 1f;
            /// <summary>
            /// The tickes per second. Only used when file does not contains tickes per second for animation.
            /// </summary>
            public float TickesPerSecond = 25f;
            /// <summary>
            /// Indicate if source model transform matrix column major. Note: Most of software exported model defaults to be column major in transform matrix
            /// </summary>
            public bool IsSourceMatrixColumnMajor = true;

            /// <summary>
            /// The build octree automatically during loading.
            /// </summary>
            public bool BuildOctree = true;

            public ITexturePathResolver TexturePathResolver;
            /// <summary>
            /// Initializes a new instance of the <see cref="ImporterConfiguration"/> class.
            /// </summary>
            public ImporterConfiguration()
            {
#if WINDOWS_UWP
                TexturePathResolver = new UWPTextureLoader();
#else
                TexturePathResolver = new DefaultTexturePathResolver();
#endif
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public class ExportConfiguration
        {
            /// <summary>
            /// The post processing
            /// </summary>
            public PostProcessSteps PostProcessing =
                PostProcessSteps.FlipUVs;

            /// <summary>
            ///     The assimp property configuration
            /// </summary>
            public PropertyConfig[] AssimpPropertyConfig = null;

            /// <summary>
            ///     The enable parallel processing, such as converting Assimp meshes into HelixToolkit meshes
            /// </summary>
            public bool EnableParallelProcessing;

            /// <summary>
            ///     The external context. Can be use to do more customized configuration for Assimp Importer
            /// </summary>
            public AssimpContext ExternalContext = null;

            /// <summary>
            /// The global scale for model
            /// </summary>
            public float GlobalScale = 1f;
            /// <summary>
            /// The tickes per second. Only used when file does not contains tickes per second for animation.
            /// </summary>
            public float TickesPerSecond = 25f;

            private ILogger logger = new DebugLogger();
            /// <summary>
            /// Gets or sets the logger.
            /// </summary>
            /// <value>
            /// The logger.
            /// </value>
            public ILogger Logger
            {
                set
                {
                    logger = value;
                    if (logger == null)
                    {
                        logger = new DebugLogger();
                    }
                }
                get { return logger; }
            }

            /// <summary>
            /// The flip triangle winding order during import
            /// </summary>
            public bool FlipWindingOrder = false;

            /// <summary>
            /// Convert transform matrix to column major. Note: Most of software exported model defaults to be column major in transform matrix
            /// </summary>
            public bool ToSourceMatrixColumnMajor = true;
        }

        /// <summary>
        ///
        /// </summary>
        [Flags]
        public enum ErrorCode
        {
            None = 0,
            Failed = 1,
            Succeed = 1<<1,
            DuplicateNodeName = 1<<2,
            FileTypeNotSupported = 1<<3,
            NonUniformAnimationKeyDoesNotSupported = 1<<4
        }
    }
}
