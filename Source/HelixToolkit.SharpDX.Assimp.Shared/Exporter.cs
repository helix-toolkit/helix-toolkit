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
        public partial class Exporter
        {
            public class ExportConfiguration
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

            }
            private const string ToUpperDictString = @"..\";

            static Exporter()
            {
                using (var temp = new AssimpContext())
                {
                    SupportedFormats = temp.GetSupportedExportFormats().Select(x=>x.FormatId).ToArray();
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

            private ExportConfiguration configuration = new ExportConfiguration();
            /// <summary>
            /// Gets or sets the configuration.
            /// </summary>
            /// <value>
            /// The configuration.
            /// </value>
            public ExportConfiguration Configuration
            {
                set
                {
                    configuration = value;
                    if (value == null)
                    {
                        configuration = new ExportConfiguration();
                    }
                }
                get
                {
                    return configuration;
                }
            }

            public ILogger Logger { get => configuration.Logger; }

            private HxScene.SceneNode root;
            protected readonly Dictionary<Geometry3D, int> geometryCollection = new Dictionary<Geometry3D, int>();
            protected readonly Dictionary<MaterialCore, int> materialCollection = new Dictionary<MaterialCore, int>();
            
            #endregion
            public ErrorCode ExportToFile(string filePath, HxScene.SceneNode root, string formatId)
            {
                Clear();
                var scene = CreateScene(root);
                AssimpContext exporter = null;
                var useExtern = false;
                if (Configuration.ExternalContext != null)
                {
                    exporter = Configuration.ExternalContext;
                    useExtern = true;
                }
                else
                {
                    exporter = new AssimpContext();
                }

                try
                {
                    if(!exporter.ExportFile(scene.AssimpScene, filePath, formatId))
                    {
                        Log(LogLevel.Error, $"Export failed. FilePath: {filePath}; Format: {formatId}");
                        return ErrorCode.Failed;
                    }
                    return ErrorCode.Succeed;
                }
                catch(Exception ex)
                {
                    Log(LogLevel.Error, ex.Message);
                }
                finally
                {
                    if (!useExtern)
                    {
                        exporter.Dispose();
                    }
                }
                return ErrorCode.Failed;
            }

            private HelixInternalScene CreateScene(HxScene.SceneNode root)
            {
                CollectAllGeometriesAndMaterials(root);

                var scene = new HelixInternalScene
                {
                    AssimpScene = new Scene(),
                    Materials = new Tuple<global::Assimp.Material, MaterialCore>[materialCollection.Count],
                    Meshes = new MeshInfo[geometryCollection.Count]
                };
                return scene;
            }

            private void CollectAllGeometriesAndMaterials(HxScene.SceneNode root)
            {
                foreach(var node in root.Traverse())
                {
                    if(GetGeometryFromNode(node, out var geometry) && !geometryCollection.ContainsKey(geometry))
                    {
                        geometryCollection.Add(geometry, geometryCollection.Count);
                    }
                    if(GetMaterialFromNode(node, out var material) && !materialCollection.ContainsKey(material))
                    {
                        materialCollection.Add(material, materialCollection.Count);
                    }
                }
            }

            protected virtual void Clear()
            {
                geometryCollection.Clear();
                materialCollection.Clear();
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
