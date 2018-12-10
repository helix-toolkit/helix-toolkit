using System;
using System.Collections.Generic;
using System.Text;
using Assimp;
using Assimp.Configs;
using System.Linq;
using System.IO;

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
        public class Importer
        {
            public sealed class MeshInfo
            {
                public PrimitiveType Type;
                public Mesh AssimpMesh;
                public Geometry3D Mesh;
                public int MaterialIndex;
                public MeshInfo()
                {

                }

                public MeshInfo(PrimitiveType type, Mesh assimpMesh, Geometry3D mesh, int materialIndex)
                {
                    Type = type;
                    Mesh = mesh;
                    MaterialIndex = materialIndex;
                }
            }

            public sealed class HelixScene
            {
                public MeshInfo[] Meshes;
                public Tuple<Material, Model.MaterialCore>[] Materials;
            }

            private Dictionary<string, Stream> textureDict = new Dictionary<string, Stream>();

            private string filePath = "";

            public HxScene.SceneNode Load(string filePath)
            {
                return Load(filePath, PostProcessSteps.GenerateNormals, null);
            }

            public HxScene.SceneNode Load(string filePath, PostProcessSteps postprocessSteps, params PropertyConfig[] configs)
            {
                this.filePath = filePath;
                var importer = new AssimpContext();
                if (configs != null)
                {
                    foreach (var config in configs)
                    {
                        importer.SetConfig(config);
                    }
                }

                var assimpScene = importer.ImportFile(filePath, postprocessSteps);
                if (assimpScene == null)
                {
                    return null;
                }

                if (!assimpScene.HasMeshes)
                {
                    return new HxScene.GroupNode();
                }

                return ConstructHelixScene(assimpScene.RootNode, ToHelixScene(assimpScene));
            }

            private HelixScene ToHelixScene(Scene scene)
            {
                var s = new HelixScene
                {
                    Meshes = new MeshInfo[scene.MeshCount],
                    Materials = new Tuple<Material, Model.MaterialCore>[scene.MaterialCount]
                };
                if (scene.HasMeshes)
                {
                    for (int i = 0; i < scene.MeshCount; ++i)
                    {
                        s.Meshes[i] = ToHelixGeometry(scene.Meshes[i]);
                    }
                }
                if (scene.HasMaterials)
                {
                    for (int i = 0; i < scene.MaterialCount; ++i)
                    {
                        s.Materials[i] = ToHelixMaterial(scene.Materials[i]);
                    }
                }
                return s;
            }


            private static HxScene.SceneNode ConstructHelixScene(Node node, HelixScene scene)
            {
                var group = new HxScene.GroupNode
                {
                    ModelMatrix = node.Transform.ToSharpDXMatrix()
                };
                if (node.HasMeshes)
                {
                    foreach (var idx in node.MeshIndices)
                    {
                        var mesh = scene.Meshes[idx];
                        switch (mesh.Type)
                        {
                            case PrimitiveType.Triangle:
                                var material = scene.Materials[mesh.MaterialIndex];
                                group.AddChildNode(new HxScene.MeshNode() { Geometry = mesh.Mesh, Material = scene.Materials[mesh.MaterialIndex].Item2 });
                                break;
                            case PrimitiveType.Line:
                                group.AddChildNode(new HxScene.LineNode() { Geometry = mesh.Mesh, Material = scene.Materials[mesh.MaterialIndex].Item2 });
                                break;
                            case PrimitiveType.Point:
                                group.AddChildNode(new HxScene.PointNode() { Geometry = mesh.Mesh, Material = scene.Materials[mesh.MaterialIndex].Item2 });
                                break;
                            default:
                                throw new NotSupportedException($"Mesh Type {mesh.Type} does not supported");
                        }
                    }                 
                }
                if (node.HasChildren)
                {
                    foreach(var c in node.Children)
                    {
                        group.AddChildNode(ConstructHelixScene(c, scene));
                    }                                       
                }
                return group;
            }

            public static MeshInfo ToHelixGeometry(Mesh mesh)
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

            private static MeshGeometry3D ToHelixMesh(Mesh mesh)
            {
                var hVertices = new Vector3Collection(mesh.Vertices.Select(x => x.ToSharpDXVector3()));
                var hIndices = new IntCollection(mesh.Faces.SelectMany(x => x.Indices));
                var hMesh = new MeshGeometry3D() { Positions = hVertices, Indices = hIndices };
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
                return hMesh;
            }

            private static PointGeometry3D ToHelixPoint(Mesh mesh)
            {
                var hVertices = new Vector3Collection(mesh.Vertices.Select(x => x.ToSharpDXVector3()));
                var hMesh = new PointGeometry3D() { Positions = hVertices };
                return hMesh;
            }

            private static LineGeometry3D ToHelixLine(Mesh mesh)
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

            private Tuple<Material, Model.MaterialCore> ToHelixMaterial(Material material)
            {
                Model.MaterialCore core = null;
                if (!material.HasShadingMode)
                {
                    return new Tuple<Material, Model.MaterialCore>(material, new Model.ColorMaterialCore());
                }
                switch (material.ShadingMode)
                {
                    case ShadingMode.Blinn:
                    case ShadingMode.Phong:
                        var phong = new Model.PhongMaterialCore
                        {
                            AmbientColor = material.ColorAmbient.ToSharpDXColor4(),
                            DiffuseColor = material.ColorDiffuse.ToSharpDXColor4(),
                            EmissiveColor = material.ColorEmissive.ToSharpDXColor4(),
                            ReflectiveColor = material.ColorReflective.ToSharpDXColor4(),
                            SpecularShininess = material.Shininess
                        };
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
                        core = phong;
                        break;
                    case ShadingMode.None:
                        core = new Model.ColorMaterialCore();
                        break;
                    case ShadingMode.Fresnel:
                        var pbr = new Model.PBRMaterialCore()
                        {
                            AlbedoColor = material.ColorDiffuse.ToSharpDXColor4(),
                            EmissiveColor = material.ColorEmissive.ToSharpDXColor4(),
                            MetallicFactor = material.Shininess,// Used this for now, not sure which to use
                            ReflectanceFactor = material.ShininessStrength,// Used this for now, not sure which to use
                            RoughnessFactor = material.Reflectivity,// Used this for now, not sure which to use
                        };
                        if (material.HasColorDiffuse)
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
                        if (material.HasTextureSpecular)
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
                        core = pbr;
                        break;
                    case ShadingMode.Flat:
                    case ShadingMode.Gouraud:
                        var diffuse = new Model.DiffuseMaterialCore()
                        {
                            DiffuseColor = material.ColorDiffuse.ToSharpDXColor4()
                        };
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
                    default:
                        throw new NotSupportedException($"Shading Mode {material.ShadingMode} does not supported.");
                }

                return new Tuple<Material, Model.MaterialCore>(material, core);
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
                    var dict = Path.GetDirectoryName(filePath);
                    var p = Path.GetFullPath(Path.Combine(dict, path));
                    var texture = LoadFileToStream(p);
                    textureDict.Add(path, texture);
                    return texture;
                }
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
