using Assimp;
using Assimp.Configs;
using SharpDX;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    namespace Assimp
    {
        /// <summary>
        /// 
        /// </summary>
        public enum MaterialType
        {
            Auto,
            BlinnPhong,
            PBR,
            Diffuse,
            VertexColor,
            Normal,
            Position,
        }
        /// <summary>
        /// 
        /// </summary>
        public class ImporterConfiguration
        {
            /// <summary>
            /// Force to use material type. Default is Auto
            /// </summary>
            public MaterialType ImportMaterialType = MaterialType.Auto;
            /// <summary>
            /// The enable parallel processing, such as converting Assimp meshes into HelixToolkit meshes
            /// </summary>
            public bool EnableParallelProcessing = false;
            /// <summary>
            /// The default post process steps for Assimp Importer. <see cref="PostProcessSteps.FlipUVs"/> must be used for DirectX texture sampling
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
            /// The assimp property configuration
            /// </summary>
            public PropertyConfig[] AssimpPropertyConfig = null;
            /// <summary>
            /// The external context. Can be use to do more customized configuration for Assimp Importer
            /// </summary>
            public AssimpContext ExternalContext = null;
            /// <summary>
            /// The ai matkey GLTF metallic, roughness, ambient occlusion texture
            /// </summary>
            public string AI_MATKEY_GLTF_METALLICROUGHNESSAO_TEXTURE = @"$tex.file";
            /// <summary>
            /// The ai matkey GLTF metallic factor for PBR material
            /// </summary>
            public string AI_MATKEY_GLTF_METALLIC_FACTOR = @"$mat.gltf.pbrMetallicRoughness.metallicFactor";
            /// <summary>
            /// The ai matkey GLTF roughness factor for PBR material
            /// </summary>
            public string AI_MATKEY_GLTF_ROUGHNESS_FACTOR = @"$mat.gltf.pbrMetallicRoughness.roughnessFactor";
            /// <summary>
            /// The ai matkey GLTF basecolor factor for PBR material
            /// </summary>
            public string AI_MATKEY_GLTF_BASECOLOR_FACTOR = @"$mat.gltf.pbrMetallicRoughness.baseColorFactor";
        }

        /// <summary>
        /// 
        /// </summary>
        public class Importer
        {
            /// <summary>
            /// 
            /// </summary>
            public sealed class MeshInfo
            {
                /// <summary>
                /// The mesh type
                /// </summary>
                public PrimitiveType Type;
                /// <summary>
                /// The Assimp mesh
                /// </summary>
                public Mesh AssimpMesh;
                /// <summary>
                /// The Helix mesh
                /// </summary>
                public Geometry3D Mesh;
                /// <summary>
                /// The material index
                /// </summary>
                public int MaterialIndex;
                /// <summary>
                /// Initializes a new instance of the <see cref="MeshInfo"/> class.
                /// </summary>
                public MeshInfo()
                {

                }
                /// <summary>
                /// Initializes a new instance of the <see cref="MeshInfo"/> class.
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
            /// 
            /// </summary>
            public sealed class HelixScene
            {
                /// <summary>
                /// The meshes
                /// </summary>
                public MeshInfo[] Meshes;
                /// <summary>
                /// The materials
                /// </summary>
                public Tuple<global::Assimp.Material, Model.MaterialCore>[] Materials;
            }
            /// <summary>
            /// Gets the supported formats.
            /// </summary>
            /// <value>
            /// The supported formats.
            /// </value>
            public static string[] SupportedFormats { get; }

            public static string SupportedFormatsString { get; }

            private ConcurrentDictionary<string, Stream> textureDict = new ConcurrentDictionary<string, Stream>();

            private const string ToUpperDictString = @"..\";

            public ImporterConfiguration Configuration { set; get; } = new ImporterConfiguration();

            private string filePath = "";

            static Importer()
            {
                using (var temp = new AssimpContext())
                {
                    SupportedFormats = temp.GetSupportedImportFormats();
                }
                var builder = new StringBuilder();
                foreach(var s in SupportedFormats)
                {
                    builder.Append("*");
                    builder.Append(s);
                    builder.Append(";");
                }
                SupportedFormatsString = builder.ToString();
            }

            /// <summary>
            /// Loads the specified file path.
            /// </summary>
            /// <param name="filePath">The file path.</param>
            /// <param name="config">The configuration.</param>
            /// <returns></returns>
            public HxScene.SceneNode Load(string filePath, ImporterConfiguration config)
            {
                Configuration = config;
                return Load(filePath);
            }

            /// <summary>
            /// Loads the specified file path.
            /// </summary>
            /// <param name="filePath">The file path.</param>
            /// <param name="parallelLoad">if set to <c>true</c> [parallel load].</param>
            /// <param name="postprocessSteps">The postprocess steps.</param>
            /// <param name="configs">The configs.</param>
            /// <returns></returns>
            public HxScene.SceneNode Load(string filePath, bool parallelLoad, PostProcessSteps postprocessSteps, params PropertyConfig[] configs)
            {
                Configuration.EnableParallelProcessing = parallelLoad;
                Configuration.AssimpPostProcessSteps = postprocessSteps;
                return Load(filePath);
            }

            /// <summary>
            /// Loads the specified file path.
            /// </summary>
            /// <param name="filePath">The file path.</param>
            /// <returns></returns>
            public HxScene.SceneNode Load(string filePath)
            {
                this.filePath = filePath;
                AssimpContext importer = null;
                bool useExtern = false;
                if (Configuration.ExternalContext != null)
                {
                    importer = Configuration.ExternalContext;
                    useExtern = true;
                }
                else
                {
                    importer = new AssimpContext();
                }
                textureDict.Clear();
                try
                {
                    if (!useExtern && Configuration.AssimpPropertyConfig != null)
                    {
                        foreach (var config in Configuration.AssimpPropertyConfig)
                        {
                            importer.SetConfig(config);
                        }
                    }
                    importer.SetConfig(new FloatPropertyConfig(Configuration.AI_MATKEY_GLTF_METALLIC_FACTOR, 0f));
                    importer.SetConfig(new FloatPropertyConfig(Configuration.AI_MATKEY_GLTF_ROUGHNESS_FACTOR, 0f));

                    var assimpScene = importer.ImportFile(filePath, Configuration.AssimpPostProcessSteps);
                    if (assimpScene == null)
                    {
                        return null;
                    }

                    if (!assimpScene.HasMeshes)
                    {
                        return new HxScene.GroupNode();
                    }

                    return ConstructHelixScene(assimpScene.RootNode, ToHelixScene(assimpScene, Configuration.EnableParallelProcessing));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (!useExtern)
                    {
                        importer.Dispose();
                    }
                }
            }

            private HelixScene ToHelixScene(Scene scene, bool parallel)
            {
                var s = new HelixScene
                {
                    Meshes = new MeshInfo[scene.MeshCount],
                    Materials = new Tuple<global::Assimp.Material, Model.MaterialCore>[scene.MaterialCount]
                };
                if (scene.HasMeshes)
                {
                    if (parallel)
                    {
                        Parallel.ForEach(scene.Meshes, (mesh, state, index) =>
                        {
                            s.Meshes[index] = ToHelixGeometry(mesh);
                        });
                    }
                    else
                    {
                        for (int i = 0; i < scene.MeshCount; ++i)
                        {
                            s.Meshes[i] = ToHelixGeometry(scene.Meshes[i]);
                        }
                    }
                }
                if (scene.HasMaterials)
                {
                    if (parallel)
                    {
                        Parallel.ForEach(scene.Materials, (material, state, index) => 
                        {
                            s.Materials[index] = ToHelixMaterial(material);
                        });
                    }
                    else
                    {
                        for (int i = 0; i < scene.MaterialCount; ++i)
                        {
                            s.Materials[i] = ToHelixMaterial(scene.Materials[i]);
                        }
                    }
                }
                return s;
            }


            private HxScene.SceneNode ConstructHelixScene(Node node, HelixScene scene)
            {
                if (node.HasChildren || node.MeshCount > 1)
                {
                    var group = new HxScene.GroupNode
                    {
                        Name = string.IsNullOrEmpty(node.Name) ? nameof(HxScene.GroupNode) : node.Name,
                        ModelMatrix = node.Transform.ToSharpDXMatrix()
                    };
                    foreach (var c in node.Children)
                    {
                        group.AddChildNode(ConstructHelixScene(c, scene));
                    }
                    foreach (var idx in node.MeshIndices)
                    {
                        var mesh = scene.Meshes[idx];
                        group.AddChildNode(ToHxMesh(mesh, scene));
                    }
                    return group;
                }
                else if (node.MeshCount == 1)
                {
                    return ToHxMesh(scene.Meshes[node.MeshIndices[0]], scene);
                }
                else
                {
                    return null;
                }
            }
            /// <summary>
            /// To the hx mesh nodes.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <param name="scene">The scene.</param>
            /// <returns></returns>
            /// <exception cref="System.NotSupportedException">Mesh Type {mesh.Type}</exception>
            protected virtual HxScene.SceneNode ToHxMesh(MeshInfo mesh, HelixScene scene)
            {
                switch (mesh.Type)
                {
                    case PrimitiveType.Triangle:
                        var material = scene.Materials[mesh.MaterialIndex];
                        return new HxScene.MeshNode()
                        {
                            Name = string.IsNullOrEmpty(mesh.AssimpMesh.Name) ? nameof(HxScene.MeshNode) : mesh.AssimpMesh.Name,
                            Geometry = mesh.Mesh,
                            Material = scene.Materials[mesh.MaterialIndex].Item2
                        };
                    case PrimitiveType.Line:
                        var lnode = new HxScene.LineNode()
                        {
                            Name = string.IsNullOrEmpty(mesh.AssimpMesh.Name) ? nameof(HxScene.LineNode) : mesh.AssimpMesh.Name,
                            Geometry = mesh.Mesh
                        };
                        var lmaterial = new Model.LineMaterialCore(); //Must create separate line material
                        lnode.Material = lmaterial;
                        var ml = scene.Materials[mesh.MaterialIndex].Item2;
                        if (ml is Model.DiffuseMaterialCore diffuse)
                        {
                            lmaterial.LineColor = diffuse.DiffuseColor;                         
                        }
                        return lnode;
                    case PrimitiveType.Point:
                        var pnode = new HxScene.PointNode()
                        {
                            Name = string.IsNullOrEmpty(mesh.AssimpMesh.Name) ? nameof(HxScene.PointNode) : mesh.AssimpMesh.Name,
                            Geometry = mesh.Mesh
                        };
                        var pmaterial = new Model.PointMaterialCore(); //Must create separate point material
                        pnode.Material = pmaterial;
                        var pm = scene.Materials[mesh.MaterialIndex].Item2;
                        if(pm is Model.DiffuseMaterialCore diffuse1)
                        {
                            pmaterial.PointColor = diffuse1.DiffuseColor;
                        }
                        return pnode;
                    default:
                        throw new NotSupportedException($"Mesh Type {mesh.Type} does not supported");
                }
            }

            private MeshInfo ToHelixGeometry(Mesh mesh)
            {
                switch (mesh.PrimitiveType)
                {
                    case PrimitiveType.Triangle:
                        return new MeshInfo(PrimitiveType.Triangle, mesh, ToHelixMesh(mesh), mesh.MaterialIndex);
                    case PrimitiveType.Point:
                        return new MeshInfo(PrimitiveType.Point, mesh, ToHelixPoint(mesh), mesh.MaterialIndex);
                    case PrimitiveType.Line:
                        return new MeshInfo(PrimitiveType.Line, mesh, ToHelixLine(mesh), mesh.MaterialIndex);
                    default:
                        throw new NotSupportedException($"MeshType : {mesh.PrimitiveType} does not supported");
                }
            }
            /// <summary>
            /// To the helix mesh.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <returns></returns>
            protected virtual MeshGeometry3D ToHelixMesh(Mesh mesh)
            {
                var hVertices = new Vector3Collection(mesh.Vertices.Select(x => x.ToSharpDXVector3()));
                var builder = new MeshBuilder(false, false);
                builder.Positions.AddRange(hVertices);
                for (int i=0; i < mesh.FaceCount; ++i)
                {
                    if (!mesh.Faces[i].HasIndices)
                    {
                        continue;
                    }
                    if (mesh.Faces[i].IndexCount == 3)
                    {
                        builder.AddTriangle(mesh.Faces[i].Indices);
                    }
                    else if (mesh.Faces[i].IndexCount == 4)
                    {
                        builder.AddTriangleFan(mesh.Faces[i].Indices);
                    }
                    else
                    {
                        //builder.AddPolygonByTriangulation(mesh.Faces[i].Indices);
                    }
                }
                
                var hMesh = new MeshGeometry3D() { Positions = hVertices, Indices = builder.TriangleIndices };
                if (mesh.HasNormals)
                {
                    hMesh.Normals = new Vector3Collection(mesh.Normals.Select(x => x.ToSharpDXVector3()));
                }
                if (mesh.HasTangentBasis)
                {
                    hMesh.Tangents = new Vector3Collection(mesh.Tangents.Select(x => x.ToSharpDXVector3()));
                    hMesh.BiTangents = new Vector3Collection(mesh.BiTangents.Select(x => x.ToSharpDXVector3()));
                }
                if (mesh.HasVertexColors(0))
                {
                    hMesh.Colors = new Color4Collection(mesh.VertexColorChannels[0].Select(x => new global::SharpDX.Color4(x.R, x.G, x.B, x.A)));
                }
                if (mesh.HasTextureCoords(0))
                {
                    hMesh.TextureCoordinates = new Vector2Collection(mesh.TextureCoordinateChannels[0].Select(x => x.ToSharpDXVector2()));
                }
                hMesh.UpdateBounds();
                hMesh.UpdateOctree();
                return hMesh;
            }
            /// <summary>
            /// To the helix point.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <returns></returns>
            protected virtual PointGeometry3D ToHelixPoint(Mesh mesh)
            {
                var hVertices = new Vector3Collection(mesh.Vertices.Select(x => x.ToSharpDXVector3()));
                var hMesh = new PointGeometry3D() { Positions = hVertices };
                return hMesh;
            }
            /// <summary>
            /// To the helix line.
            /// </summary>
            /// <param name="mesh">The mesh.</param>
            /// <returns></returns>
            protected virtual LineGeometry3D ToHelixLine(Mesh mesh)
            {
                var hVertices = new Vector3Collection(mesh.Vertices.Select(x => x.ToSharpDXVector3()));
                var hIndices = new IntCollection(mesh.Faces.SelectMany(x => x.Indices));
                var hMesh = new LineGeometry3D() { Positions = hVertices, Indices = hIndices };
                if (mesh.HasVertexColors(0))
                {
                    hMesh.Colors = new Color4Collection(mesh.VertexColorChannels[0].Select(x => new global::SharpDX.Color4(x.R, x.G, x.B, x.A)));
                }
                return hMesh;
            }
            /// <summary>
            /// To the phong material.
            /// </summary>
            /// <param name="material">The material.</param>
            /// <returns></returns>
            protected virtual Model.PhongMaterialCore OnCreatePhongMaterial(global::Assimp.Material material)
            {
                var phong = new Model.PhongMaterialCore
                {
                    AmbientColor = material.HasColorAmbient ? material.ColorAmbient.ToSharpDXColor4() : Color.Black,
                    DiffuseColor = material.HasColorDiffuse ? material.ColorDiffuse.ToSharpDXColor4() : Color.Black,
                    EmissiveColor = material.HasColorEmissive ? material.ColorEmissive.ToSharpDXColor4() : Color.Black,
                    ReflectiveColor = material.HasColorReflective ? material.ColorReflective.ToSharpDXColor4() : Color.Black,
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
                {
                    phong.NormalMap = LoadTexture(material.TextureNormal.FilePath);
                }
                else if (material.HasTextureHeight)
                {
                    phong.NormalMap = LoadTexture(material.TextureHeight.FilePath);
                }
                if (material.HasTextureSpecular)
                {
                    phong.SpecularColorMap = LoadTexture(material.TextureSpecular.FilePath);
                }
                if (material.HasTextureDisplacement)
                {
                    phong.DisplacementMap = LoadTexture(material.TextureDisplacement.FilePath);
                }
                if (material.HasBumpScaling)
                {
                    phong.DisplacementMapScaleMask = new global::SharpDX.Vector4(material.BumpScaling, material.BumpScaling, material.BumpScaling, 0);
                }
                if (material.HasTextureOpacity)
                {
                    phong.DiffuseAlphaMap = LoadTexture(material.TextureOpacity.FilePath);
                }
                return phong;
            }
            /// <summary>
            /// To the PBR material.
            /// </summary>
            /// <param name="material">The material.</param>
            /// <returns></returns>
            protected virtual Model.PBRMaterialCore OnCreatePBRMaterial(global::Assimp.Material material)
            {
                var pbr = new Model.PBRMaterialCore()
                {
                    AlbedoColor = material.HasColorDiffuse ? material.ColorDiffuse.ToSharpDXColor4() : Color.Black,
                    EmissiveColor = material.HasColorEmissive ? material.ColorEmissive.ToSharpDXColor4() : Color.Black,
                    MetallicFactor = material.Shininess,// Used this for now, not sure which to use
                    ReflectanceFactor = material.ShininessStrength,// Used this for now, not sure which to use
                    RoughnessFactor = material.Reflectivity,// Used this for now, not sure which to use
                };
                if (material.HasNonTextureProperty(Configuration.AI_MATKEY_GLTF_BASECOLOR_FACTOR))
                {
                    pbr.AlbedoColor = material.GetNonTextureProperty(Configuration.AI_MATKEY_GLTF_BASECOLOR_FACTOR).GetColor4DValue().ToSharpDXColor4();
                }
                if(material.HasNonTextureProperty(Configuration.AI_MATKEY_GLTF_METALLIC_FACTOR))
                {
                    pbr.MetallicFactor = material.GetNonTextureProperty(Configuration.AI_MATKEY_GLTF_METALLIC_FACTOR).GetFloatValue();
                }
                if(material.HasNonTextureProperty(Configuration.AI_MATKEY_GLTF_ROUGHNESS_FACTOR))
                {
                    pbr.RoughnessFactor = material.GetNonTextureProperty(Configuration.AI_MATKEY_GLTF_METALLIC_FACTOR).GetFloatValue();
                }
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
                {
                    pbr.NormalMap = LoadTexture(material.TextureNormal.FilePath);
                }
                else if (material.HasTextureHeight)
                {
                    pbr.NormalMap = LoadTexture(material.TextureHeight.FilePath);
                }
                if (material.HasProperty(Configuration.AI_MATKEY_GLTF_METALLICROUGHNESSAO_TEXTURE, TextureType.Unknown, 0))
                {
                    var t = material.GetProperty(Configuration.AI_MATKEY_GLTF_METALLICROUGHNESSAO_TEXTURE, TextureType.Unknown, 0);
                    pbr.RMAMap = LoadTexture(t.GetStringValue());
                }
                else if (material.HasTextureSpecular)
                {
                    pbr.RMAMap = LoadTexture(material.TextureSpecular.FilePath);
                }
                if (material.HasTextureDisplacement)
                {
                    pbr.DisplacementMap = LoadTexture(material.TextureDisplacement.FilePath);
                }
                if (material.HasBumpScaling)
                {
                    pbr.DisplacementMapScaleMask = new global::SharpDX.Vector4(material.BumpScaling, material.BumpScaling, material.BumpScaling, 0);
                }
                if (material.HasTextureLightMap)
                {
                    pbr.IrradianceMap = LoadTexture(material.TextureLightMap.FilePath);
                }
                return pbr;
            }

            /// <summary>
            /// To the helix material.
            /// </summary>
            /// <param name="material">The material.</param>
            /// <returns></returns>
            /// <exception cref="System.NotSupportedException">Shading Mode {material.ShadingMode}</exception>
            protected virtual Tuple<global::Assimp.Material, Model.MaterialCore> ToHelixMaterial(global::Assimp.Material material)
            {
                Model.MaterialCore core = null;
                if (!material.HasShadingMode)
                {
                    if(material.HasNonTextureProperty(Configuration.AI_MATKEY_GLTF_METALLIC_FACTOR)
                        || material.HasNonTextureProperty(Configuration.AI_MATKEY_GLTF_ROUGHNESS_FACTOR)
                        || material.HasNonTextureProperty(Configuration.AI_MATKEY_GLTF_BASECOLOR_FACTOR))
                    {
                        var pbr = OnCreatePBRMaterial(material);
                        return new Tuple<global::Assimp.Material, Model.MaterialCore>(material, pbr);
                    }
                    else
                    {
                        var phong = OnCreatePhongMaterial(material);
                        return new Tuple<global::Assimp.Material, Model.MaterialCore>(material, phong);
                    }
                }
                var mode = material.ShadingMode;
                if(Configuration.ImportMaterialType != MaterialType.Auto)
                {
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
                        var diffuse = new Model.DiffuseMaterialCore()
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
                        {
                            diffuse.DiffuseMap = LoadTexture(material.TextureDiffuse.FilePath);
                        }
                        if(material.ShadingMode == ShadingMode.Flat)
                        {
                            diffuse.EnableUnLit = true;
                        }
                        core = diffuse;
                        break;
                    case ShadingMode.Flat:
                        core = new Model.ColorMaterialCore();
                        break;
                    default:
                        switch (Configuration.ImportMaterialType)
                        {
                            case MaterialType.Position:
                                core = new Model.PositionMaterialCore();
                                break;
                            case MaterialType.Normal:
                                core = new Model.NormalMaterialCore();
                                break;
                            default:
                                throw new NotSupportedException($"Shading Mode {material.ShadingMode} does not supported.");
                        }
                        break;
                }
                if (core != null)
                {
                    core.Name = material.Name;
                }
                return new Tuple<global::Assimp.Material, Model.MaterialCore>(material, core);
            }


            private static global::SharpDX.Direct3D11.TextureAddressMode ToDXAddressMode(TextureWrapMode mode)
            {
                switch (mode)
                {
                    case TextureWrapMode.Clamp:
                        return global::SharpDX.Direct3D11.TextureAddressMode.Clamp;
                    case TextureWrapMode.Mirror:
                        return global::SharpDX.Direct3D11.TextureAddressMode.Mirror;
                    case TextureWrapMode.Wrap:
                        return global::SharpDX.Direct3D11.TextureAddressMode.Wrap;
                    default:
                        return global::SharpDX.Direct3D11.TextureAddressMode.Wrap;
                }
            }

            private Stream LoadTexture(string path)
            {
                if (textureDict.TryGetValue(path, out var s))
                {
                    return s;
                }
                else
                {
                    var texture = OnLoadTexture(path);
                    if (texture != null)
                    {
                        textureDict.TryAdd(path, texture);
                    }
                    return texture;
                }
            }
            /// <summary>
            /// Called when [load texture].
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            protected virtual Stream OnLoadTexture(string path)
            {
                var dict = Path.GetDirectoryName(filePath);
                var p = Path.GetFullPath(Path.Combine(dict, path));
                if (!File.Exists(p) && path.StartsWith(ToUpperDictString))
                {
                    path.Remove(0, ToUpperDictString.Length);
                    p = Path.GetFullPath(Path.Combine(dict, path));
                }
                var texture = LoadFileToStream(p);               
                return texture;
            }

            private static Stream LoadFileToStream(string path)
            {             
                if (!File.Exists(path))
                {
                    return null;
                }
                using (var v = File.OpenRead(path))
                {
                    var m = new MemoryStream();
                    v.CopyTo(m);
                    return m;
                }
            }
        }
    }
}
