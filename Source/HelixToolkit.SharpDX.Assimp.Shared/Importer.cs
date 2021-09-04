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
using System.Collections.ObjectModel;
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
        public partial class Importer : IDisposable
        {
            private string path = "";
            public static readonly string[] SupportedTextureFormats = new string[]
            {
                "bmp", "jpg", "jpeg", "png", "dds", "tiff", "wmp", "gif",
            };

            protected static readonly HashSet<string> SupportedTextureFormatDict;

            static Importer()
            {
                using (var temp = new AssimpContext())
                {
                    SupportedFormats = temp.GetSupportedImportFormats();
                }

                var builder = new StringBuilder();
                builder.Append($"All Supported |");
                foreach (var s in SupportedFormats)
                {
                    builder.Append($"*{ s };");
                }
                builder.Append($"|");
                foreach (var s in SupportedFormats)
                {
                    builder.Append($"(*{ s })|*{ s }|");
                }

                SupportedFormatsString = builder.ToString(0, builder.Length - 1);
                SupportedTextureFormatDict = new HashSet<string>(SupportedTextureFormats);
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

            private int MaterialIndexForNoName = 0;
            private int MeshIndexForNoName = 0;
            private readonly List<EmbeddedTexture> embeddedTextures = new List<EmbeddedTexture>();
            private readonly Dictionary<string, EmbeddedTexture> embeddedTextureDict = new Dictionary<string, EmbeddedTexture>();

            public event EventHandler<Exception> AssimpExceptionOccurred;
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
                    if (!importer.IsImportFormatSupported(Path.GetExtension(filePath)))
                    {
                        return ErrorCode.FileTypeNotSupported | ErrorCode.Failed;
                    }
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

                    return BuildScene(assimpScene, out scene);
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, ex.Message);
                    ErrorCode = ErrorCode.Failed;
                    AssimpExceptionOccurred?.Invoke(this, ex);
                    return ErrorCode;
                }
                finally
                {
                    if (!useExtern)
                        importer.Dispose();
                }
            }

            /// <summary>
            /// Converts HelixToolkit Scene directly from assimp scene. User is responsible for providing the assimp scene.
            /// </summary>
            /// <param name="assimpScene">The assimp scene.</param>
            /// <param name="helixScene">The helix scene.</param>
            /// <returns></returns>
            public ErrorCode ToHelixToolkitScene(Scene assimpScene, out HelixToolkitScene helixScene)
            {
                return BuildScene(assimpScene, out helixScene);
            }

            /// <summary>
            /// Loads the specified file stream. User must provider custom texture loader to load texture files.
            /// </summary>
            /// <param name="fileStream">The file stream.</param>
            /// <param name="filePath">The filePath. Used to load texture.</param>
            /// <param name="formatHint">The format hint.</param>
            /// <param name="texturePathResolver">The custom texture path resolver</param>
            /// <param name="scene">The scene.</param>
            /// <returns></returns>
            public ErrorCode Load(Stream fileStream, string filePath, string formatHint, out HelixToolkitScene scene, ITexturePathResolver texturePathResolver = null)
            {
                path = filePath;
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
                configuration.TexturePathResolver = texturePathResolver;
                Clear();
                scene = null;
                try
                {
                    if (!importer.IsImportFormatSupported(formatHint))
                    {
                        return ErrorCode.FileTypeNotSupported | ErrorCode.Failed;
                    }
                    if (!useExtern && Configuration.AssimpPropertyConfig != null)
                        foreach (var config in Configuration.AssimpPropertyConfig)
                            importer.SetConfig(config);
                    importer.Scale = configuration.GlobalScale;
                    var postProcess = configuration.AssimpPostProcessSteps;
                    if (configuration.FlipWindingOrder)
                    {
                        postProcess |= PostProcessSteps.FlipWindingOrder;
                    }
                    var assimpScene = importer.ImportFileFromStream(fileStream, postProcess, formatHint);
                    return BuildScene(assimpScene, out scene);
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, ex.Message);
                    ErrorCode = ErrorCode.Failed;
                    AssimpExceptionOccurred?.Invoke(this, ex);
                    return ErrorCode;
                }
                finally
                {
                    if (!useExtern)
                        importer.Dispose();
                }
            }

            /// <summary>
            /// Convert the assimp scene to Helix Scene.
            /// </summary>
            /// <param name="assimpScene">The assimp scene.</param>
            /// <param name="filePath">The filePath of the model. It is used for texture loading</param>
            /// <param name="texturePathResolver">Custom texture path resolver</param>
            /// <param name="scene">The scene.</param>
            /// <returns></returns>
            public ErrorCode Load(Scene assimpScene, string filePath, out HelixToolkitScene scene, ITexturePathResolver texturePathResolver = null)
            {
                path = filePath;
                Configuration.TexturePathResolver = texturePathResolver;
                return BuildScene(assimpScene, out scene);
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
                MeshIndexForNoName = 0;
                MaterialIndexForNoName = 0;
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
            private ErrorCode BuildScene(Scene assimpScene, out HelixToolkitScene scene)
            {
                scene = null;
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
                if (!ErrorCode.HasFlag(ErrorCode.Failed))
                    ErrorCode |= ErrorCode.Succeed;
                return ErrorCode;
            }

            private HelixInternalScene ToHelixScene(Scene scene, bool parallel)
            {
                var s = new HelixInternalScene
                {
                    AssimpScene = scene,
                    Meshes = new MeshInfo[scene.MeshCount],
                    Materials = new KeyValuePair<global::Assimp.Material, MaterialCore>[scene.MaterialCount]
                };
                Parallel.Invoke(() =>
                    {
                        if (scene.HasMeshes)
                        {
                            if (parallel)
                            {
                                Parallel.ForEach(scene.Meshes,
                                        (mesh, state, index) => { s.Meshes[index] = OnCreateHelixGeometry(mesh); });
                            }
                            else
                            {
                                for (var i = 0; i < scene.MeshCount; ++i)
                                {
                                    s.Meshes[i] = OnCreateHelixGeometry(scene.Meshes[i]);
                                }
                            }
                        }
                    },
                    () =>
                    {
                        if (scene.HasMaterials)
                        {
                            embeddedTextures.Clear();
                            embeddedTextureDict.Clear();
                            if (scene.HasTextures)
                            {
                                embeddedTextures.AddRange(scene.Textures);
                                for (int i = 0; i < embeddedTextures.Count; ++i)
                                {
                                    var key = embeddedTextures[i].Filename;
                                    if (string.IsNullOrEmpty(key))
                                    {
                                        key = "*" + i.ToString();
                                    }
                                    if (!embeddedTextureDict.ContainsKey(key))
                                    { embeddedTextureDict.Add(key, embeddedTextures[i]); }
                                }
                            }

                            for (var i = 0; i < scene.MaterialCount; ++i)
                            {
                                s.Materials[i] = OnCreateHelixMaterial(scene.Materials[i]);
                            }
                            embeddedTextures.Clear();
                            embeddedTextureDict.Clear();
                        }
                    });
                return s;
            }

            private HxScene.SceneNode ConstructHelixScene(Node node, HelixInternalScene scene)
            {
                var group = new HxScene.GroupNode
                {
                    Name = string.IsNullOrEmpty(node.Name) ? nameof(HxScene.GroupNode) : node.Name,
                    ModelMatrix = node.Transform.ToSharpDXMatrix(configuration.IsSourceMatrixColumnMajor)
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
                        var hxNode = OnCreateHxMeshNode(mesh, scene, Matrix.Identity);
                        group.AddChildNode(hxNode);
                    }
                }
                if(node.Metadata.Count > 0)
                {
                    group.Metadata = new Metadata();
                    foreach (var metadata in node.Metadata.ToHelixMetadata())
                    {
                        group.Metadata.Add(metadata.Key, metadata.Value);
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
                public KeyValuePair<global::Assimp.Material, MaterialCore>[] Materials;

                /// <summary>
                ///     The meshes
                /// </summary>
                public MeshInfo[] Meshes;
            }
            #endregion

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).
                        Clear();
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~Importer() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
        }
    }
}