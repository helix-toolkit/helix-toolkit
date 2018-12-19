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
                    SupportedFormats = temp.GetSupportedExportFormats().ToArray();
                }

                var builder = new StringBuilder();
                foreach (var s in SupportedFormats)
                {
                    builder.Append($"{s.Description} (*.{s.FileExtension})|*.{ s.FileExtension }|");
                }
                SupportedFormatsString = builder.ToString(0, builder.Length - 1);
            }
            #region Properties
            /// <summary>
            ///     Gets the supported formats.
            /// </summary>
            /// <value>
            ///     The supported formats.
            /// </value>
            public static ExportFormatDescription[] SupportedFormats { get; }

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

            protected readonly Dictionary<Geometry3D, uint> geometryCollection = new Dictionary<Geometry3D, uint>();
            protected readonly Dictionary<MaterialCore, uint> materialCollection = new Dictionary<MaterialCore, uint>();
            protected readonly Dictionary<ulong, MeshInfo> meshInfos = new Dictionary<ulong, MeshInfo>();
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
                    if(!exporter.ExportFile(scene, filePath, formatId))
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

            private Scene CreateScene(HxScene.SceneNode root)
            {
                CollectAllGeometriesAndMaterials(root);
                var scene = new Scene();
                //Adds material and meshes into the assimp scene
                foreach(var material in materialCollection.OrderBy(x=>x.Value))
                {
                    scene.Materials.Add(OnCreateAssimpMaterial(material.Key));
                }
                foreach(var mesh in meshInfos.OrderBy(x=>x.Value.MeshIndex))
                {
                    scene.Meshes.Add(mesh.Value.AssimpMesh);
                }
                scene.RootNode = ConstructAssimpNode(root, null);
                return scene;
            }

            private Node ConstructAssimpNode(HxScene.SceneNode current, Node parent)
            {
                var node = new Node(current.Name, parent);

                if(current is HxScene.GroupNodeBase group)
                {
                    foreach(var s in group.Items)
                    {
                        if(s is HxScene.GeometryNode geo)
                        {
                            if(geometryCollection.TryGetValue(geo.Geometry, out var meshIndex))
                            {
                                node.MeshIndices.Add((int)meshIndex);
                            }
                        }
                        else if(s is HxScene.GroupNodeBase)
                        {
                            ConstructAssimpNode(s, node);
                        }
                    }
                }
                return node;
            }

            private void CollectAllGeometriesAndMaterials(HxScene.SceneNode root)
            {
                // Collect all geometries and materials
                foreach(var node in root.Traverse())
                {
                    if(GetMaterialFromNode(node, out var material) && !materialCollection.ContainsKey(material))
                    {
                        materialCollection.Add(material, (uint)materialCollection.Count);
                    }
                    if (GetGeometryFromNode(node, out var geometry) && !geometryCollection.ContainsKey(geometry))
                    {
                        geometryCollection.Add(geometry, (uint)geometryCollection.Count);
                    }
                }
                // Create Mesh Material Pair
                foreach (var node in root.Traverse())
                {
                    if(node is HxScene.GeometryNode geo)
                    {
                        var info = CreateMeshInfo(geo);
                        if (!meshInfos.ContainsKey(info.MaterialMeshKey))
                        {
                            meshInfos.Add(info.MaterialMeshKey, info);
                        }
                    }
                }
            }

            protected virtual void Clear()
            {
                geometryCollection.Clear();
                materialCollection.Clear();
                meshInfos.Clear();
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
