using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using Assimp.Configs;
using SharpDX;
using SharpDX.Direct3D11;
using Animation = Assimp.Animation;
using TextureType = Assimp.TextureType;

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
    using HxScene = Model.Scene;
    using HxAnimations = Animations;
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
        }

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
        /// Scene for importer output
        /// </summary>
        public class HelixToolkitScene
        {
            public HxScene.SceneNode Root { set; get; }
            public IList<HxAnimations.Animation> Animations { set; get; }
            /// <summary>
            /// Initializes a new instance of the <see cref="HelixScene"/> class.
            /// </summary>
            /// <param name="root">The root.</param>
            public HelixToolkitScene(HxScene.SceneNode root)
            {
                Root = root;
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="HelixScene"/> class.
            /// </summary>
            /// <param name="root">The root.</param>
            /// <param name="animations">The animations.</param>
            public HelixToolkitScene(HxScene.SceneNode root, IList<HxAnimations.Animation> animations = null)
            {
                Root = root;
                Animations = animations.ToArray();
            }
        }

        /// <summary>
        /// </summary>
        public class Importer
        {
            private const string ToUpperDictString = @"..\";
            private string path = "";

            private readonly ConcurrentDictionary<string, Stream> textureDict =
                new ConcurrentDictionary<string, Stream>();

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

            /// <summary>
            ///     Gets or sets the configuration.
            /// </summary>
            /// <value>
            ///     The configuration.
            /// </value>
            public ImporterConfiguration Configuration { set; get; } = new ImporterConfiguration();

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

            public ErrorCode ErrorCode { private set; get; }
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
                if (Load(filePath, out var root) == ErrorCode.Succeed)
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
                    importer.SetConfig(new FloatPropertyConfig(Configuration.AI_MATKEY_GLTF_METALLIC_FACTOR, 0f));
                    importer.SetConfig(new FloatPropertyConfig(Configuration.AI_MATKEY_GLTF_ROUGHNESS_FACTOR, 0f));
                    var fileName = Path.GetExtension(filePath);
                    if (!importer.IsImportFormatSupported(fileName))
                    {
                        ErrorCode |= ErrorCode.FileTypeNotSupported;
                        return ErrorCode;
                    }

                    var assimpScene = importer.ImportFile(filePath, Configuration.AssimpPostProcessSteps);
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
                    LoadAnimations(internalScene);
                    scene.Animations = Animations.ToArray();
                    if(!ErrorCode.HasFlag(ErrorCode.Failed))
                        ErrorCode |= ErrorCode.Succeed;
                    return ErrorCode;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (!useExtern) importer.Dispose();
                }
            }

            #endregion

            #region Protected Methods

            protected virtual void Clear()
            {
                textureDict.Clear();
                SceneNodes.Clear();
                Animations.Clear();
            }

            protected virtual ErrorCode ProcessSceneNodes(HxScene.SceneNode root)
            {
                if (root == null) return ErrorCode.Failed;
                SceneNodes.Add(root);
                SceneNodes.AddRange(root.Items.PreorderDFT(n => { return true; }));
                return ErrorCode.Succeed;
            }

            protected virtual ErrorCode ProcessNodeAnimation(NodeAnimationChannel channel, double ticksPerSecond, out FastList<HxAnimations.Keyframe1> list)
            {
                var posCount = channel.PositionKeyCount;
                var rotCount = channel.RotationKeyCount;
                var scaleCount = channel.ScalingKeyCount;
                if (posCount != rotCount || rotCount != scaleCount)
                {
                    list = null;
                    ErrorCode |= ErrorCode.NonUniformAnimationKeyDoesNotSupported;
                    return ErrorCode.NonUniformAnimationKeyDoesNotSupported;
                }

                var ret = new FastList<HxAnimations.Keyframe1>(posCount);
                for (var i = 0; i < posCount; ++i)
                    ret.Add(new HxAnimations.Keyframe1
                    {
                        Time = (float)(channel.PositionKeys[i].Time / ticksPerSecond),
                        Position = channel.PositionKeys[i].Value.ToSharpDXVector3(),
                        Rotation = channel.RotationKeys[i].Value.ToSharpDXQuaternion(),
                        Scale = channel.ScalingKeys[i].Value.ToSharpDXVector3()
                    });
                list = ret;
                return ErrorCode.Succeed;
            }

            /// <summary>
            ///     To the hx mesh nodes.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <param name="scene">The scene.</param>
            /// <param name="transform"></param>
            /// <returns></returns>
            /// <exception cref="System.NotSupportedException">Mesh Type {mesh.Type}</exception>
            protected virtual HxScene.SceneNode ToHxMesh(MeshInfo mesh, HelixInternalScene scene, Matrix transform)
            {
                switch (mesh.Type)
                {
                    case PrimitiveType.Triangle:
                        var material = scene.Materials[mesh.MaterialIndex];
                        var cullMode = material.Item1.HasTwoSided && material.Item1.IsTwoSided
                            ? CullMode.Back
                            : CullMode.None;
                        if (Configuration.ForceCullMode) cullMode = Configuration.CullMode;
                        var fillMode = material.Item1.HasWireFrame && material.Item1.IsWireFrameEnabled
                            ? FillMode.Wireframe
                            : FillMode.Solid;
                        return new HxScene.MeshNode
                        {
                            Name = string.IsNullOrEmpty(mesh.AssimpMesh.Name)
                                ? nameof(HxScene.MeshNode)
                                : mesh.AssimpMesh.Name,
                            Geometry = mesh.Mesh,
                            Material = material.Item2,
                            ModelMatrix = transform,
                            CullMode = cullMode,
                            FillMode = fillMode
                        };
                    case PrimitiveType.Line:
                        var lnode = new HxScene.LineNode
                        {
                            Name = string.IsNullOrEmpty(mesh.AssimpMesh.Name)
                                ? nameof(HxScene.LineNode)
                                : mesh.AssimpMesh.Name,
                            Geometry = mesh.Mesh,
                            ModelMatrix = transform
                        };
                        var lmaterial = new LineMaterialCore(); //Must create separate line material
                        lnode.Material = lmaterial;
                        var ml = scene.Materials[mesh.MaterialIndex].Item2;
                        if (ml is DiffuseMaterialCore diffuse) lmaterial.LineColor = diffuse.DiffuseColor;
                        return lnode;
                    case PrimitiveType.Point:
                        var pnode = new HxScene.PointNode
                        {
                            Name = string.IsNullOrEmpty(mesh.AssimpMesh.Name)
                                ? nameof(HxScene.PointNode)
                                : mesh.AssimpMesh.Name,
                            Geometry = mesh.Mesh,
                            ModelMatrix = transform
                        };
                        var pmaterial = new PointMaterialCore(); //Must create separate point material
                        pnode.Material = pmaterial;
                        var pm = scene.Materials[mesh.MaterialIndex].Item2;
                        if (pm is DiffuseMaterialCore diffuse1) pmaterial.PointColor = diffuse1.DiffuseColor;
                        return pnode;
                    default:
                        throw new NotSupportedException($"Mesh Type {mesh.Type} does not supported");
                }
            }

            /// <summary>
            ///     To the helix mesh.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <returns></returns>
            protected virtual MeshGeometry3D ToHelixMesh(Mesh mesh)
            {
                var hVertices = new Vector3Collection(mesh.Vertices.Select(x => x.ToSharpDXVector3()));
                var builder = new MeshBuilder(false, false);
                builder.Positions.AddRange(hVertices);
                for (var i = 0; i < mesh.FaceCount; ++i)
                {
                    if (!mesh.Faces[i].HasIndices) continue;
                    if (mesh.Faces[i].IndexCount == 3)
                        builder.AddTriangle(mesh.Faces[i].Indices);
                    else if (mesh.Faces[i].IndexCount == 4) builder.AddTriangleFan(mesh.Faces[i].Indices);
                }

                var hMesh = new MeshGeometry3D { Positions = hVertices, Indices = builder.TriangleIndices };
                if (mesh.HasNormals)
                    hMesh.Normals = new Vector3Collection(mesh.Normals.Select(x => x.ToSharpDXVector3()));
                if (mesh.HasTangentBasis)
                {
                    hMesh.Tangents = new Vector3Collection(mesh.Tangents.Select(x => x.ToSharpDXVector3()));
                    hMesh.BiTangents = new Vector3Collection(mesh.BiTangents.Select(x => x.ToSharpDXVector3()));
                }

                if (mesh.HasVertexColors(0))
                    hMesh.Colors =
                        new Color4Collection(mesh.VertexColorChannels[0].Select(x => new Color4(x.R, x.G, x.B, x.A)));
                if (mesh.HasTextureCoords(0))
                    hMesh.TextureCoordinates =
                        new Vector2Collection(mesh.TextureCoordinateChannels[0].Select(x => x.ToSharpDXVector2()));
                hMesh.UpdateBounds();
                hMesh.UpdateOctree();
                return hMesh;
            }

            protected virtual BoneSkinnedMeshGeometry3D ToHelixMeshWithBones(Mesh mesh)
            {
                var m = ToHelixMesh(mesh);
                var vertBoneIds = new FastList<BoneIds>(Enumerable.Repeat(new BoneIds(), m.Positions.Count));
                var vertBoneInternal = vertBoneIds.GetInternalArray();
                var accumArray = new int[m.Positions.Count];
                var boneMesh = new BoneSkinnedMeshGeometry3D(m)
                {
                    VertexBoneIds = vertBoneIds,
                    Bones = new FastList<HxAnimations.Bone>(mesh.BoneCount),
                    BoneNames = new FastList<string>(mesh.BoneCount)
                };
                for (var j = 0; j < mesh.BoneCount; ++j)
                {
                    if (mesh.Bones[j].HasVertexWeights)
                        for (var i = 0; i < mesh.Bones[j].VertexWeightCount; ++i)
                        {
                            var vWeight = mesh.Bones[j].VertexWeights[i];
                            var currIdx = accumArray[vWeight.VertexID]++;
                            ref var id = ref vertBoneInternal[vWeight.VertexID];
                            switch (currIdx)
                            {
                                case 0:
                                    id.Bone1 = j;
                                    id.Weights.X = vWeight.Weight;
                                    break;
                                case 1:
                                    id.Bone2 = j;
                                    id.Weights.Y = vWeight.Weight;
                                    break;
                                case 2:
                                    id.Bone3 = j;
                                    id.Weights.Z = vWeight.Weight;
                                    break;
                                case 3:
                                    id.Bone4 = j;
                                    id.Weights.W = vWeight.Weight;
                                    break;
                            }
                        }

                    boneMesh.Bones.Add(new HxAnimations.Bone
                    {
                        BindPose = mesh.Bones[j].OffsetMatrix.ToSharpDXMatrix(),
                        InvBindPose = mesh.Bones[j].OffsetMatrix.ToSharpDXMatrix().Inverted()
                    });
                    boneMesh.BoneNames.Add(mesh.Bones[j].Name);
                }

                return boneMesh;
            }

            /// <summary>
            ///     To the helix point.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <returns></returns>
            protected virtual PointGeometry3D ToHelixPoint(Mesh mesh)
            {
                var hVertices = new Vector3Collection(mesh.Vertices.Select(x => x.ToSharpDXVector3()));
                var hMesh = new PointGeometry3D { Positions = hVertices };
                return hMesh;
            }

            /// <summary>
            ///     To the helix line.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <returns></returns>
            protected virtual LineGeometry3D ToHelixLine(Mesh mesh)
            {
                var hVertices = new Vector3Collection(mesh.Vertices.Select(x => x.ToSharpDXVector3()));
                var hIndices = new IntCollection(mesh.Faces.SelectMany(x => x.Indices));
                var hMesh = new LineGeometry3D { Positions = hVertices, Indices = hIndices };
                if (mesh.HasVertexColors(0))
                    hMesh.Colors =
                        new Color4Collection(mesh.VertexColorChannels[0].Select(x => new Color4(x.R, x.G, x.B, x.A)));
                return hMesh;
            }

            /// <summary>
            ///     To the phong material.
            /// </summary>
            /// <param name="material">The material.</param>
            /// <returns></returns>
            protected virtual PhongMaterialCore OnCreatePhongMaterial(global::Assimp.Material material)
            {
                var phong = new PhongMaterialCore
                {
                    AmbientColor = material.HasColorAmbient ? material.ColorAmbient.ToSharpDXColor4() : Color.Black,
                    DiffuseColor = material.HasColorDiffuse ? material.ColorDiffuse.ToSharpDXColor4() : Color.Black,
                    EmissiveColor = material.HasColorEmissive ? material.ColorEmissive.ToSharpDXColor4() : Color.Black,
                    ReflectiveColor = material.HasColorReflective
                        ? material.ColorReflective.ToSharpDXColor4()
                        : Color.Black,
                    SpecularShininess = material.Shininess
                };
                if (material.HasOpacity)
                {
                    var c = phong.DiffuseColor;
                    c.Alpha = material.Opacity;
                    phong.DiffuseColor = c;
                }

                if (material.HasTextureDiffuse)
                {
                    phong.DiffuseMap = LoadTexture(material.TextureDiffuse.FilePath);
                    var desc = Shaders.DefaultSamplers.LinearSamplerClampAni1;
                    desc.AddressU = ToDXAddressMode(material.TextureDiffuse.WrapModeU);
                    desc.AddressV = ToDXAddressMode(material.TextureDiffuse.WrapModeV);
                    phong.DiffuseMapSampler = desc;
                }

                if (material.HasTextureNormal)
                    phong.NormalMap = LoadTexture(material.TextureNormal.FilePath);
                else if (material.HasTextureHeight) phong.NormalMap = LoadTexture(material.TextureHeight.FilePath);
                if (material.HasTextureSpecular)
                    phong.SpecularColorMap = LoadTexture(material.TextureSpecular.FilePath);
                if (material.HasTextureDisplacement)
                    phong.DisplacementMap = LoadTexture(material.TextureDisplacement.FilePath);
                if (material.HasBumpScaling)
                    phong.DisplacementMapScaleMask = new Vector4(material.BumpScaling, material.BumpScaling,
                        material.BumpScaling, 0);
                if (material.HasTextureOpacity) phong.DiffuseAlphaMap = LoadTexture(material.TextureOpacity.FilePath);
                return phong;
            }

            /// <summary>
            ///     To the PBR material.
            /// </summary>
            /// <param name="material">The material.</param>
            /// <returns></returns>
            protected virtual PBRMaterialCore OnCreatePBRMaterial(global::Assimp.Material material)
            {
                var pbr = new PBRMaterialCore
                {
                    AlbedoColor = material.HasColorDiffuse ? material.ColorDiffuse.ToSharpDXColor4() : Color.Black,
                    EmissiveColor = material.HasColorEmissive && !Configuration.IgnoreEmissiveColor
                        ? material.ColorEmissive.ToSharpDXColor4()
                        : Color.Black,
                    ReflectanceFactor = material.HasReflectivity ? material.Reflectivity : 0
                };
                if (material.HasNonTextureProperty(Configuration.AI_MATKEY_GLTF_BASECOLOR_FACTOR))
                    pbr.AlbedoColor = material.GetNonTextureProperty(Configuration.AI_MATKEY_GLTF_BASECOLOR_FACTOR)
                        .GetColor4DValue().ToSharpDXColor4();
                if (material.HasNonTextureProperty(Configuration.AI_MATKEY_GLTF_METALLIC_FACTOR))
                    pbr.MetallicFactor = material.GetNonTextureProperty(Configuration.AI_MATKEY_GLTF_METALLIC_FACTOR)
                        .GetFloatValue();
                if (material.HasNonTextureProperty(Configuration.AI_MATKEY_GLTF_ROUGHNESS_FACTOR))
                    pbr.RoughnessFactor = material.GetNonTextureProperty(Configuration.AI_MATKEY_GLTF_METALLIC_FACTOR)
                        .GetFloatValue();
                if (material.HasOpacity)
                {
                    var c = pbr.AlbedoColor;
                    c.Alpha = material.Opacity;
                    pbr.AlbedoColor = c;
                }

                if (material.HasTextureDiffuse)
                {
                    pbr.AlbedoMap = LoadTexture(material.TextureDiffuse.FilePath);
                    var desc = Shaders.DefaultSamplers.LinearSamplerClampAni1;
                    desc.AddressU = ToDXAddressMode(material.TextureDiffuse.WrapModeU);
                    desc.AddressV = ToDXAddressMode(material.TextureDiffuse.WrapModeV);
                    pbr.SurfaceMapSampler = desc;
                }

                if (material.HasTextureNormal)
                    pbr.NormalMap = LoadTexture(material.TextureNormal.FilePath);
                else if (material.HasTextureHeight) pbr.NormalMap = LoadTexture(material.TextureHeight.FilePath);
                if (material.HasProperty(Configuration.AI_MATKEY_GLTF_METALLICROUGHNESSAO_TEXTURE, TextureType.Unknown,
                    0))
                {
                    var t = material.GetProperty(Configuration.AI_MATKEY_GLTF_METALLICROUGHNESSAO_TEXTURE,
                        TextureType.Unknown, 0);
                    pbr.RMAMap = LoadTexture(t.GetStringValue());
                }
                else if (material.HasTextureSpecular)
                {
                    pbr.RMAMap = LoadTexture(material.TextureSpecular.FilePath);
                }

                if (material.HasTextureDisplacement)
                    pbr.DisplacementMap = LoadTexture(material.TextureDisplacement.FilePath);
                if (material.HasBumpScaling)
                    pbr.DisplacementMapScaleMask = new Vector4(material.BumpScaling, material.BumpScaling,
                        material.BumpScaling, 0);
                if (material.HasTextureLightMap) pbr.IrradianceMap = LoadTexture(material.TextureLightMap.FilePath);
                return pbr;
            }

            /// <summary>
            ///     To the helix material.
            /// </summary>
            /// <param name="material">The material.</param>
            /// <returns></returns>
            /// <exception cref="System.NotSupportedException">Shading Mode {material.ShadingMode}</exception>
            protected virtual Tuple<global::Assimp.Material, MaterialCore> ToHelixMaterial(
                global::Assimp.Material material)
            {
                MaterialCore core = null;
                if (!material.HasShadingMode)
                {
                    if (material.HasNonTextureProperty(Configuration.AI_MATKEY_GLTF_METALLIC_FACTOR)
                        || material.HasNonTextureProperty(Configuration.AI_MATKEY_GLTF_ROUGHNESS_FACTOR)
                        || material.HasNonTextureProperty(Configuration.AI_MATKEY_GLTF_BASECOLOR_FACTOR))
                    {
                        var pbr = OnCreatePBRMaterial(material);
                        return new Tuple<global::Assimp.Material, MaterialCore>(material, pbr);
                    }

                    var phong = OnCreatePhongMaterial(material);
                    return new Tuple<global::Assimp.Material, MaterialCore>(material, phong);
                }

                var mode = material.ShadingMode;
                if (Configuration.ImportMaterialType != MaterialType.Auto)
                    switch (Configuration.ImportMaterialType)
                    {
                        case MaterialType.BlinnPhong:
                            mode = ShadingMode.Blinn;
                            break;
                        case MaterialType.Diffuse:
                            mode = ShadingMode.Flat;
                            break;
                        case MaterialType.PBR:
                            mode = ShadingMode.CookTorrance;
                            break;
                        case MaterialType.VertexColor:
                            mode = ShadingMode.Flat;
                            break;
                        case MaterialType.Normal:
                            break;
                        case MaterialType.Position:
                            break;
                    }
                switch (material.ShadingMode)
                {
                    case ShadingMode.Blinn:
                    case ShadingMode.Phong:
                    case ShadingMode.None:
                        core = OnCreatePhongMaterial(material);
                        break;
                    case ShadingMode.CookTorrance:
                    case ShadingMode.Fresnel:
                    case ShadingMode.OrenNayar:
                        core = OnCreatePBRMaterial(material);
                        break;
                    case ShadingMode.Gouraud:
                        var diffuse = new DiffuseMaterialCore
                        {
                            DiffuseColor = material.ColorDiffuse.ToSharpDXColor4()
                        };
                        if (material.HasOpacity)
                        {
                            var c = diffuse.DiffuseColor;
                            c.Alpha = material.Opacity;
                            diffuse.DiffuseColor = c;
                        }

                        if (material.HasTextureDiffuse)
                            diffuse.DiffuseMap = LoadTexture(material.TextureDiffuse.FilePath);
                        if (material.ShadingMode == ShadingMode.Flat) diffuse.EnableUnLit = true;
                        core = diffuse;
                        break;
                    case ShadingMode.Flat:
                        core = new ColorMaterialCore();
                        break;
                    default:
                        switch (Configuration.ImportMaterialType)
                        {
                            case MaterialType.Position:
                                core = new PositionMaterialCore();
                                break;
                            case MaterialType.Normal:
                                core = new NormalMaterialCore();
                                break;
                            default:
                                throw new NotSupportedException(
                                    $"Shading Mode {material.ShadingMode} does not supported.");
                        }

                        break;
                }

                if (core != null) core.Name = material.Name;
                return new Tuple<global::Assimp.Material, MaterialCore>(material, core);
            }

            /// <summary>
            ///     Called when [load texture].
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            protected virtual Stream OnLoadTexture(string path)
            {
                var dict = Path.GetDirectoryName(this.path);
                var p = Path.GetFullPath(Path.Combine(dict, path));
                if (!File.Exists(p)) p = HandleTexturePathNotFound(dict, path);
                if (!File.Exists(p)) return null;
                return LoadFileToStream(p);
            }

            protected virtual string HandleTexturePathNotFound(string dir, string texturePath)
            {
                //If file not found in texture path dir, try to find the file in the same dir as the model file
                if (texturePath.StartsWith(ToUpperDictString))
                {
                    texturePath.Remove(0, ToUpperDictString.Length);
                    var p = Path.GetFullPath(Path.Combine(dir, texturePath));
                    if (File.Exists(p)) return p;
                }

                //If still not found, try to go one upper level and find
                var upper = Directory.GetParent(dir).FullName;
                upper = Path.GetFullPath(upper + texturePath);
                if (File.Exists(upper)) return upper;

                return "";
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
                                Parallel.ForEach(scene.Meshes,
                                    (mesh, state, index) => { s.Meshes[index] = ToHelixGeometry(mesh); });
                            else
                                for (var i = 0; i < scene.MeshCount; ++i)
                                    s.Meshes[i] = ToHelixGeometry(scene.Meshes[i]);
                        }
                    },
                    () =>
                    {
                        if (scene.HasMaterials)
                            for (var i = 0; i < scene.MaterialCount; ++i)
                                s.Materials[i] = ToHelixMaterial(scene.Materials[i]);
                    });
                return s;
            }

            private ErrorCode LoadAnimations(HelixInternalScene scene)
            {
                var dict = new Dictionary<string, HxScene.SceneNode>(SceneNodes.Count);
                foreach (var node in SceneNodes)
                    if (!dict.ContainsKey(node.Name))
                        dict.Add(node.Name, node);

                foreach (var mesh in scene.Meshes.Where(x => x.Mesh is BoneSkinnedMeshGeometry3D)
                    .Select(x => x.Mesh as BoneSkinnedMeshGeometry3D))
                    if (mesh.Bones != null && mesh.BoneNames != null && mesh.Bones.Count == mesh.BoneNames.Count)
                        for (var i = 0; i < mesh.Bones.Count; ++i)
                            if (dict.TryGetValue(mesh.BoneNames[i], out var s))
                            {
                                var b = mesh.Bones[i];
                                b.ParentNode = s.Parent;
                                b.Node = s;
                                mesh.Bones[i] = b;
                            }

                if (scene.AssimpScene.HasAnimations)
                {
                    var animationList = new List<HxAnimations.Animation>(scene.AssimpScene.AnimationCount);
                    if (Configuration.EnableParallelProcessing)
                        Parallel.ForEach(scene.AssimpScene.Animations, ani =>
                        {
                            if (LoadAnimation(ani, dict, out var hxAni) == ErrorCode.Succeed)
                                lock (animationList)
                                {
                                    animationList.Add(hxAni);
                                }
                        });
                    else
                        foreach (var ani in scene.AssimpScene.Animations)
                            if (LoadAnimation(ani, dict, out var hxAni) == ErrorCode.Succeed)
                                animationList.Add(hxAni);
                    scene.Animations = animationList;
                    Animations.AddRange(animationList);
                }
                return ErrorCode.Succeed;
            }

            private ErrorCode LoadAnimation(Animation ani, Dictionary<string, HxScene.SceneNode> dict,
                out HxAnimations.Animation hxAni)
            {
                hxAni = new HxAnimations.Animation(HxAnimations.AnimationType.Node)
                {
                    StartTime = 0,
                    EndTime = (float)(ani.DurationInTicks / ani.TicksPerSecond),
                    Name = ani.Name,
                    NodeAnimationCollection = new List<HxAnimations.NodeAnimation>(ani.NodeAnimationChannelCount)
                };

                if (ani.HasNodeAnimations)
                {
                    var code = ErrorCode.None;
                    foreach (var key in ani.NodeAnimationChannels)
                        if (dict.TryGetValue(key.NodeName, out var node))
                        {
                            var nAni = new HxAnimations.NodeAnimation
                            {
                                Node = node
                            };
                            code = ProcessNodeAnimation(key, ani.TicksPerSecond, out var keyframes);
                            if (code == ErrorCode.Succeed)
                            {
                                nAni.KeyFrames = keyframes;
                                hxAni.NodeAnimationCollection.Add(nAni);
                            }
                            else
                            {
                                break;
                            }
                        }

                    return code;
                }

                return ErrorCode.Failed;
            }
            
            private HxScene.SceneNode ConstructHelixScene(Node node, HelixInternalScene scene)
            {
                var group = new HxScene.GroupNode
                {
                    Name = string.IsNullOrEmpty(node.Name) ? nameof(HxScene.GroupNode) : node.Name,
                    ModelMatrix = node.Transform.ToSharpDXMatrix()
                };
                if (node.HasChildren)
                    foreach (var c in node.Children)
                        group.AddChildNode(ConstructHelixScene(c, scene));
                if (node.HasMeshes)
                    foreach (var idx in node.MeshIndices)
                    {
                        var mesh = scene.Meshes[idx];
                        group.AddChildNode(ToHxMesh(mesh, scene, Matrix.Identity));
                    }

                return group;
                //if (node.HasChildren || node.MeshCount > 1)
                //{
                //    var group = new HxScene.GroupNode
                //    {
                //        Name = string.IsNullOrEmpty(node.Name) ? nameof(HxScene.GroupNode) : node.Name,
                //        ModelMatrix = node.Transform.ToSharpDXMatrix()
                //    };
                //    foreach (var c in node.Children)
                //    {
                //        group.AddChildNode(ConstructHelixScene(c, scene));
                //    }
                //    foreach (var idx in node.MeshIndices)
                //    {
                //        var mesh = scene.Meshes[idx];
                //        group.AddChildNode(ToHxMesh(mesh, scene, Matrix.Identity));
                //    }
                //    return group;
                //}
                //else if (node.MeshCount == 1)
                //{
                //    return ToHxMesh(scene.Meshes[node.MeshIndices[0]], scene, node.Transform.ToSharpDXMatrix());
                //}
                //else
                //{
                //    return null;
                //}
            }

            private MeshInfo ToHelixGeometry(Mesh mesh)
            {
                switch (mesh.PrimitiveType)
                {
                    case PrimitiveType.Triangle:
                        if (mesh.HasBones)
                            return new MeshInfo(PrimitiveType.Triangle, mesh, ToHelixMeshWithBones(mesh),
                                mesh.MaterialIndex);
                        else
                            return new MeshInfo(PrimitiveType.Triangle, mesh, ToHelixMesh(mesh), mesh.MaterialIndex);
                    case PrimitiveType.Point:
                        return new MeshInfo(PrimitiveType.Point, mesh, ToHelixPoint(mesh), mesh.MaterialIndex);
                    case PrimitiveType.Line:
                        return new MeshInfo(PrimitiveType.Line, mesh, ToHelixLine(mesh), mesh.MaterialIndex);
                    default:
                        throw new NotSupportedException($"MeshType : {mesh.PrimitiveType} does not supported");
                }
            }            

            private static TextureAddressMode ToDXAddressMode(TextureWrapMode mode)
            {
                switch (mode)
                {
                    case TextureWrapMode.Clamp:
                        return TextureAddressMode.Clamp;
                    case TextureWrapMode.Mirror:
                        return TextureAddressMode.Mirror;
                    case TextureWrapMode.Wrap:
                        return TextureAddressMode.Wrap;
                    default:
                        return TextureAddressMode.Wrap;
                }
            }

            private Stream LoadTexture(string path)
            {
                if (textureDict.TryGetValue(path, out var s))
                {
                    return s;
                }

                var texture = OnLoadTexture(path);
                if (texture != null) textureDict.TryAdd(path, texture);
                return texture;
            }

            private static Stream LoadFileToStream(string path)
            {
                if (!File.Exists(path)) return null;
                using (var v = File.OpenRead(path))
                {
                    var m = new MemoryStream();
                    v.CopyTo(m);
                    return m;
                }
            }
            #endregion

            #region Inner Classes

            /// <summary>
            /// </summary>
            protected sealed class MeshInfo
            {
                /// <summary>
                ///     The Assimp mesh
                /// </summary>
                public Mesh AssimpMesh;

                /// <summary>
                ///     The material index
                /// </summary>
                public int MaterialIndex;

                /// <summary>
                ///     The Helix mesh
                /// </summary>
                public Geometry3D Mesh;

                /// <summary>
                ///     The mesh type
                /// </summary>
                public PrimitiveType Type;

                /// <summary>
                ///     Initializes a new instance of the <see cref="MeshInfo" /> class.
                /// </summary>
                public MeshInfo()
                {
                }

                /// <summary>
                ///     Initializes a new instance of the <see cref="MeshInfo" /> class.
                /// </summary>
                /// <param name="type">The type.</param>
                /// <param name="assimpMesh">The assimp mesh.</param>
                /// <param name="mesh">The mesh.</param>
                /// <param name="materialIndex">Index of the material.</param>
                public MeshInfo(PrimitiveType type, Mesh assimpMesh, Geometry3D mesh, int materialIndex)
                {
                    Type = type;
                    Mesh = mesh;
                    AssimpMesh = assimpMesh;
                    MaterialIndex = materialIndex;
                }
            }

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