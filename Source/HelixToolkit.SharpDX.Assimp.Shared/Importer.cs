/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Assimp;
using Assimp.Configs;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
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
    using HxAnimations = Animations;
    using HxScene = Model.Scene;

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
            ///     The ai matkey GLTF basecolor factor for PBR material
            /// </summary>
            public string AI_MATKEY_GLTF_BASECOLOR_FACTOR = @"$mat.gltf.pbrMetallicRoughness.baseColorFactor";

            /// <summary>
            ///     The ai matkey GLTF metallic factor for PBR material
            /// </summary>
            public string AI_MATKEY_GLTF_METALLIC_FACTOR = @"$mat.gltf.pbrMetallicRoughness.metallicFactor";

            /// <summary>
            ///     The ai matkey GLTF metallic, roughness, ambient occlusion texture
            /// </summary>
            public string AI_MATKEY_GLTF_METALLICROUGHNESSAO_TEXTURE = @"$tex.file";

            /// <summary>
            ///     The ai matkey GLTF roughness factor for PBR material
            /// </summary>
            public string AI_MATKEY_GLTF_ROUGHNESS_FACTOR = @"$mat.gltf.pbrMetallicRoughness.roughnessFactor";

            /// <summary>
            ///     The default post process steps for Assimp Importer. <see cref="PostProcessSteps.FlipUVs" /> must be used for
            ///     DirectX texture sampling
            /// </summary>
            public PostProcessSteps AssimpPostProcessSteps =
                PostProcessSteps.GenerateNormals
                | PostProcessSteps.Triangulate
                | PostProcessSteps.TransformUVCoords
                | PostProcessSteps.CalculateTangentSpace
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
            ///     The ignore emissive color
            /// </summary>
            public bool IgnoreEmissiveColor = false;

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
            /// Initializes a new instance of the <see cref="ImporterConfiguration"/> class.
            /// </summary>
            public ImporterConfiguration()
            {

            }
        }

        /// <summary>
        ///
        /// </summary>
        [Flags]
        public enum ErrorCode
        {        
            None = 0,
            Failed = 1,
            Succeed = 2,
            DuplicateNodeName = 4,
            FileTypeNotSupported = 8,
            NonUniformAnimationKeyDoesNotSupported = 16
        }

        /// <summary>
        /// </summary>
        public partial class Importer
        {
            private const string ToUpperDictString = @"..\";
            private string path = "";

            static Importer()
            {
                using (var temp = new AssimpContext())
                {
                    SupportedFormats = temp.GetSupportedImportFormats();
                }

                var builder = new StringBuilder();
                foreach (var s in SupportedFormats)
                {
                    builder.Append("*");
                    builder.Append(s);
                    builder.Append(";");
                }

                SupportedFormatsString = builder.ToString();
            }
            #region Properties
            /// <summary>
            ///     Gets the supported formats.
            /// </summary>
            /// <value>
            ///     The supported formats.
            /// </value>
            public static string[] SupportedFormats { get; }

            /// <summary>
            ///     Gets the supported formats string.
            /// </summary>
            /// <value>
            ///     The supported formats string.
            /// </value>
            public static string SupportedFormatsString { get; }

            private ImporterConfiguration configuration = new ImporterConfiguration();
            /// <summary>
            ///     Gets or sets the configuration.
            /// </summary>
            /// <value>
            ///     The configuration.
            /// </value>
            public ImporterConfiguration Configuration
            {
                set
                {
                    configuration = value;
                    if (value == null)
                    {
                        configuration = new ImporterConfiguration();
                    }
                }
                get
                {
                    return configuration;
                }
            }

            /// <summary>
            ///     Gets all the loaded scene nodes order by preorder traverse.
            /// </summary>
            /// <value>
            ///     The scene nodes.
            /// </value>
            public List<HxScene.SceneNode> SceneNodes { get; } = new List<HxScene.SceneNode>();
            /// <summary>
            /// Gets the animations.
            /// </summary>
            /// <value>
            /// The animations.
            /// </value>
            public List<Animations.Animation> Animations { get; } = new List<HxAnimations.Animation>();
            /// <summary>
            /// Gets or sets the error code.
            /// </summary>
            /// <value>
            /// The error code.
            /// </value>
            public ErrorCode ErrorCode { protected set; get; }
            /// <summary>
            /// Gets the logger.
            /// </summary>
            /// <value>
            /// The logger.
            /// </value>
            public ILogger Logger { get => configuration.Logger; }
            #endregion

            #region Public Methods
            /// <summary>
            ///     Loads the model specified file path.
            /// </summary>
            /// <param name="filePath">The file path.</param>
            /// <param name="config">The configuration.</param>
            /// <returns></returns>
            public HelixToolkitScene Load(string filePath, ImporterConfiguration config)
            {
                Configuration = config;
                return Load(filePath);
            }

            /// <summary>
            ///     Loads the model specified file path.
            /// </summary>
            /// <param name="filePath">The file path.</param>
            /// <param name="parallelLoad">if set to <c>true</c> [parallel load].</param>
            /// <param name="postprocessSteps">The postprocess steps.</param>
            /// <param name="configs">The configs.</param>
            /// <returns></returns>
            public HelixToolkitScene Load(string filePath, bool parallelLoad, PostProcessSteps postprocessSteps,
                params PropertyConfig[] configs)
            {
                Configuration.EnableParallelProcessing = parallelLoad;
                Configuration.AssimpPostProcessSteps = postprocessSteps;
                return Load(filePath);
            }

            /// <summary>
            ///     Loads the model specified file path.
            /// </summary>
            /// <param name="filePath">The file path.</param>
            /// <returns></returns>
            public HelixToolkitScene Load(string filePath)
            {
                if (Load(filePath, out var root).HasFlag(ErrorCode.Succeed))
                    return root;
                return null;
            }

            /// <summary>
            ///     Loads the model by specified file path.
            /// </summary>
            /// <param name="filePath">The file path.</param>
            /// <param name="scene">The loaded scene.</param>
            /// <returns></returns>
            /// <exception cref="System.Exception"></exception>
            public ErrorCode Load(string filePath, out HelixToolkitScene scene)
            {
                this.path = filePath;
                ErrorCode = ErrorCode.None;
                AssimpContext importer = null;
                var useExtern = false;
                if (Configuration.ExternalContext != null)
                {
                    importer = Configuration.ExternalContext;
                    useExtern = true;
                }
                else
                {
                    importer = new AssimpContext();
                }

                Clear();
                scene = null;
                try
                {
                    if (!useExtern && Configuration.AssimpPropertyConfig != null)
                        foreach (var config in Configuration.AssimpPropertyConfig)
                            importer.SetConfig(config);
                    importer.Scale = configuration.GlobalScale;
                    var fileName = Path.GetExtension(filePath);
                    if (!importer.IsImportFormatSupported(fileName))
                    {
                        ErrorCode |= ErrorCode.FileTypeNotSupported;
                        return ErrorCode;
                    }
                    var postProcess = configuration.AssimpPostProcessSteps;
                    if (configuration.FlipWindingOrder)
                    {
                        postProcess |= PostProcessSteps.FlipWindingOrder;
                    }
                    var assimpScene = importer.ImportFile(filePath, postProcess);
                    if (assimpScene == null)
                    {
                        ErrorCode |= ErrorCode.Failed;
                        return ErrorCode;
                    }

                    if (!assimpScene.HasMeshes)
                    {
                        scene = new HelixToolkitScene(new HxScene.GroupNode());
                        ErrorCode = ErrorCode.Succeed;
                        return ErrorCode.Succeed;
                    }

                    var internalScene = ToHelixScene(assimpScene, Configuration.EnableParallelProcessing);
                    scene = new HelixToolkitScene(ConstructHelixScene(assimpScene.RootNode, internalScene));
                    ErrorCode |= ProcessSceneNodes(scene.Root);
                    if (ErrorCode.HasFlag(ErrorCode.Failed))
                        return ErrorCode;
                    if (Configuration.ImportAnimations)
                    {
                        LoadAnimations(internalScene);
                        scene.Animations = Animations.ToArray();
                        if (Configuration.CreateSkeletonForBoneSkinningMesh
                            && Configuration.AddsPostEffectForSkeleton)
                        {
                            (scene.Root as HxScene.GroupNode).AddChildNode(new HxScene.NodePostEffectXRayGrid()
                            { EffectName = Configuration.SkeletonEffects });
                        }
                    }
                    if(!ErrorCode.HasFlag(ErrorCode.Failed))
                        ErrorCode |= ErrorCode.Succeed;
                    return ErrorCode;
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, ex.Message);
                    ErrorCode = ErrorCode.Failed;
                    return ErrorCode;
                }
                finally
                {
                    if (!useExtern)
                        importer.Dispose();
                }
            }

            #endregion

            #region Protected Methods            
            /// <summary>
            /// Clears this instance.
            /// </summary>
            protected virtual void Clear()
            {
                textureDict.Clear();
                SceneNodes.Clear();
                Animations.Clear();
            }
            /// <summary>
            /// Processes the scene nodes.
            /// </summary>
            /// <param name="root">The root.</param>
            /// <returns></returns>
            protected virtual ErrorCode ProcessSceneNodes(HxScene.SceneNode root)
            {
                if (root == null) return ErrorCode.Failed;
                SceneNodes.Add(root);
                SceneNodes.AddRange(root.Items.PreorderDFT(n => { return true; }));
                return ErrorCode.Succeed;
            }
            #endregion

            #region Private Methods
            private HelixInternalScene ToHelixScene(Scene scene, bool parallel)
            {
                var s = new HelixInternalScene
                {
                    AssimpScene = scene,
                    Meshes = new MeshInfo[scene.MeshCount],
                    Materials = new Tuple<global::Assimp.Material, MaterialCore>[scene.MaterialCount]
                };
                Parallel.Invoke(() =>
                    {
                        if (scene.HasMeshes)
                        {
                            if (parallel)
                            {
                                Parallel.ForEach(scene.Meshes,
                                        (mesh, state, index) => { s.Meshes[index] = ToHelixGeometry(mesh); });
                            }
                            else
                            {
                                for (var i = 0; i < scene.MeshCount; ++i)
                                {
                                    s.Meshes[i] = ToHelixGeometry(scene.Meshes[i]);
                                }
                            }
                        }
                    },
                    () =>
                    {
                        if (scene.HasMaterials)
                        {
                            for (var i = 0; i < scene.MaterialCount; ++i)
                            {
                                s.Materials[i] = ToHelixMaterial(scene.Materials[i]);
                            }
                        }
                    });
                return s;
            }

            private HxScene.SceneNode ConstructHelixScene(Node node, HelixInternalScene scene)
            {
                var group = new HxScene.GroupNode
                {
                    Name = string.IsNullOrEmpty(node.Name) ? nameof(HxScene.GroupNode) : node.Name,
                    ModelMatrix = node.Transform.ToSharpDXMatrix()
                };
                if (node.HasChildren)
                {
                    foreach (var c in node.Children)
                    {
                        group.AddChildNode(ConstructHelixScene(c, scene));
                    }
                }
                if (node.HasMeshes)
                {
                    foreach (var idx in node.MeshIndices)
                    {
                        var mesh = scene.Meshes[idx];
                        var hxNode = ToHxMeshNode(mesh, scene, Matrix.Identity);
                        group.AddChildNode(hxNode);
                        if(hxNode is HxScene.BoneSkinMeshNode skinNode && Configuration.CreateSkeletonForBoneSkinningMesh)
                        {
                            var skeleton = skinNode.CreateSkeletonNode(Configuration.SkeletonMaterial,
                                Configuration.SkeletonEffects, Configuration.SkeletonSizeScale);
                            skeleton.Name = "HxSK_" + skinNode.Name;
                            group.AddChildNode(skeleton);
                        }
                    }
                }
                return group;
            }
            /// <summary>
            /// Logs the specified level.
            /// </summary>
            /// <typeparam name="Type">The type of the ype.</typeparam>
            /// <param name="level">The level.</param>
            /// <param name="msg">The MSG.</param>
            /// <param name="caller">The caller.</param>
            /// <param name="sourceLineNumber">The source line number.</param>
            protected void Log<Type>(LogLevel level, Type msg, [CallerMemberName]string caller = "", [CallerLineNumber] int sourceLineNumber = 0)
            {
                Logger.Log(level, msg, nameof(EffectsManager), caller, sourceLineNumber);
            }
            #endregion

            #region Inner Classes

            /// <summary>
            /// </summary>
            protected sealed class HelixInternalScene
            {
                /// <summary>
                /// The animations
                /// </summary>
                public List<HxAnimations.Animation> Animations;

                /// <summary>
                /// The assimp scene
                /// </summary>
                public Scene AssimpScene;

                /// <summary>
                ///     The materials
                /// </summary>
                public Tuple<global::Assimp.Material, MaterialCore>[] Materials;

                /// <summary>
                ///     The meshes
                /// </summary>
                public MeshInfo[] Meshes;
            }

            #endregion
        }
    }
}