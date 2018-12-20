/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Assimp;
using Assimp.Unmanaged;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Linq;

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
    using Model;
    using HxAnimations = Animations;
    using HxScene = Model.Scene;
    namespace Assimp
    {
        public partial class Exporter
        {
            protected virtual bool GetMaterialFromNode(HxScene.SceneNode node, out MaterialCore material)
            {
                if (node is HxScene.MaterialGeometryNode geo)
                {
                    material = geo.Material;
                    return true;
                }
                else
                {
                    material = null;
                    return false;
                }
            }

            protected virtual void AddProperties(PhongMaterialCore phong, global::Assimp.Material assimpMaterial)
            {
                assimpMaterial.ShadingMode = ShadingMode.Blinn;
                assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_DIFFUSE_BASE, phong.DiffuseColor.ToAssimpColor4D()));
                assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_SPECULAR_BASE, phong.SpecularColor.ToAssimpColor4D()));
                assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_REFLECTIVE_BASE, phong.ReflectiveColor.ToAssimpColor4D()));
                assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_EMISSIVE_BASE, phong.EmissiveColor.ToAssimpColor4D()));
                assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_AMBIENT_BASE, phong.AmbientColor.ToAssimpColor4D()));
                assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.SHININESS_BASE, phong.SpecularShininess));
                
                if(phong.DiffuseColor.Alpha < 1f)
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.OPACITY_BASE, phong.DiffuseColor.Alpha));
                }
                if (phong.DiffuseMap != null && !string.IsNullOrEmpty(phong.DiffuseMapFilePath))
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.TEXTURE_BASE, phong.DiffuseMapFilePath, TextureType.Diffuse, 0));
                    assimpMaterial.AddProperty(new MaterialProperty(
                        AiMatKeys.GetFullTextureName(AiMatKeys.MAPPINGMODE_U_BASE, TextureType.Diffuse, 0),
                        (int)ToAssimpAddressMode(phong.DiffuseMapSampler.AddressU)));
                    assimpMaterial.AddProperty(new MaterialProperty(
                        AiMatKeys.GetFullTextureName(AiMatKeys.MAPPINGMODE_V_BASE, TextureType.Diffuse, 0),
                        (int)ToAssimpAddressMode(phong.DiffuseMapSampler.AddressV)));
                }
                if (phong.EmissiveMap != null && !string.IsNullOrEmpty(phong.EmissiveMapFilePath))
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.TEXTURE_BASE, phong.EmissiveMapFilePath, TextureType.Emissive, 0));
                }
                if (phong.NormalMap != null && !string.IsNullOrEmpty(phong.NormalMapFilePath))
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.TEXTURE_BASE, phong.NormalMapFilePath, TextureType.Normals, 0));
                }
                if (phong.DisplacementMap != null && !string.IsNullOrEmpty(phong.DisplacementMapFilePath))
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.TEXTURE_BASE, phong.DisplacementMapFilePath, TextureType.Displacement, 0));
                }
                if (phong.SpecularColorMap != null && !string.IsNullOrEmpty(phong.SpecularColorMapFilePath))
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.TEXTURE_BASE, phong.SpecularColorMapFilePath, TextureType.Specular, 0));
                }
                if (phong.DiffuseAlphaMap != null && !string.IsNullOrEmpty(phong.DiffuseAlphaMapFilePath))
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.TEXTURE_BASE, phong.DiffuseAlphaMapFilePath, TextureType.Opacity, 0));
                }
                if (phong.UVTransform.HasUVTransform)
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.UVTRANSFORM_BASE, phong.UVTransform.ToArray()));
                }
            }
            protected virtual void AddProperties(PBRMaterialCore pbr, global::Assimp.Material assimpMaterial)
            {
                assimpMaterial.ShadingMode = ShadingMode.Fresnel;
                assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_DIFFUSE_BASE, pbr.AlbedoColor.ToAssimpColor4D()));
                assimpMaterial.AddProperty(new MaterialProperty(GLTFMatKeys.AI_MATKEY_GLTF_BASECOLOR_FACTOR, pbr.AlbedoColor.ToAssimpColor4D()));
                assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_EMISSIVE_BASE, pbr.EmissiveColor.ToAssimpColor4D()));
                assimpMaterial.AddProperty(new MaterialProperty(GLTFMatKeys.AI_MATKEY_GLTF_METALLIC_FACTOR, pbr.MetallicFactor));
                assimpMaterial.AddProperty(new MaterialProperty(GLTFMatKeys.AI_MATKEY_GLTF_ROUGHNESS_FACTOR, pbr.RoughnessFactor));
                if (pbr.AmbientOcclusionFactor != 1)
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_AMBIENT, new Color4D(pbr.AmbientOcclusionFactor)));
                }
                if(pbr.ReflectanceFactor != 0)
                {
                    assimpMaterial.AddProperty(new MaterialProperty(GLTFMatKeys.AI_MATKEY_GLTF_PBRSPECULARGLOSSINESS, true));
                    assimpMaterial.AddProperty(new MaterialProperty(GLTFMatKeys.AI_MATKEY_GLTF_PBRSPECULARGLOSSINESS_GLOSSINESS_FACTOR, pbr.ReflectanceFactor));
                }
                if(pbr.AlbedoColor.Alpha < 1)
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.OPACITY, pbr.AlbedoColor.Alpha));
                }
                if(pbr.AlbedoMap != null && !string.IsNullOrEmpty(pbr.AlbedoMapFilePath))
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.TEXTURE_BASE, pbr.AlbedoMapFilePath, TextureType.Diffuse, 0));
                    assimpMaterial.AddProperty(new MaterialProperty(
                        AiMatKeys.GetFullTextureName(AiMatKeys.MAPPINGMODE_U_BASE, TextureType.Diffuse, 0),
                        (int)ToAssimpAddressMode(pbr.SurfaceMapSampler.AddressU)));
                    assimpMaterial.AddProperty(new MaterialProperty(
                        AiMatKeys.GetFullTextureName(AiMatKeys.MAPPINGMODE_V_BASE, TextureType.Diffuse, 0),
                        (int)ToAssimpAddressMode(pbr.SurfaceMapSampler.AddressV)));
                }
                if (pbr.RMAMap != null && !string.IsNullOrEmpty(pbr.RMAMapFilePath))
                {
                    assimpMaterial.AddProperty(new MaterialProperty(GLTFMatKeys.AI_MATKEY_GLTF_METALLICROUGHNESSAO_TEXTURE, 
                        pbr.RMAMapFilePath, TextureType.Unknown, 0));
                }
                if (pbr.EmissiveMap != null && !string.IsNullOrEmpty(pbr.EmissiveMapFilePath))
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.TEXTURE_BASE, pbr.EmissiveMapFilePath, TextureType.Emissive, 0));
                }
                if (pbr.NormalMap != null && !string.IsNullOrEmpty(pbr.NormalMapFilePath))
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.TEXTURE_BASE, pbr.NormalMapFilePath, TextureType.Normals, 0));
                }
                if (pbr.DisplacementMap != null && !string.IsNullOrEmpty(pbr.DisplacementMapFilePath))
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.TEXTURE_BASE, pbr.DisplacementMapFilePath, TextureType.Displacement, 0));
                }
                if (pbr.IrradianceMap != null && !string.IsNullOrEmpty(pbr.IrradianceMapFilePath))
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.TEXTURE_BASE, pbr.IrradianceMapFilePath, TextureType.Lightmap, 0));
                }
                if (pbr.UVTransform.HasUVTransform)
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.UVTRANSFORM_BASE, pbr.UVTransform.ToArray()));
                }
            }

            protected virtual void AddProperties(DiffuseMaterialCore diffuse, global::Assimp.Material assimpMaterial)
            {
                assimpMaterial.ShadingMode = ShadingMode.Gouraud;
                assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_DIFFUSE_BASE, diffuse.DiffuseColor.ToAssimpColor4D()));
            }

            protected virtual global::Assimp.Material OnCreateAssimpMaterial(MaterialCore material)
            {
                var assimpMaterial = new global::Assimp.Material() { Name = string.IsNullOrEmpty(material.Name) ? "MAT" : material.Name };
                if(material is PhongMaterialCore phong)
                {
                    AddProperties(phong, assimpMaterial);
                }
                else if(material is PBRMaterialCore pbr)
                {
                    AddProperties(pbr, assimpMaterial);                  
                }
                else if(material is DiffuseMaterialCore diffuse)
                {
                    AddProperties(diffuse, assimpMaterial);
                }
                else if(material is ColorMaterialCore vColor)
                {
                    assimpMaterial.ShadingMode = ShadingMode.Flat;
                }
                else if(material is LineMaterialCore line)
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_DIFFUSE_BASE, line.LineColor.ToAssimpColor4D()));
                }
                else if(material is PointMaterialCore point)
                {
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_DIFFUSE_BASE, point.PointColor.ToAssimpColor4D()));
                }
                return assimpMaterial;
            }

            private static TextureWrapMode ToAssimpAddressMode(TextureAddressMode mode)
            {
                switch (mode)
                {
                    case TextureAddressMode.Clamp:
                        return TextureWrapMode.Clamp;
                    case TextureAddressMode.Mirror:
                        return TextureWrapMode.Mirror;
                    case TextureAddressMode.Wrap:
                        return TextureWrapMode.Wrap;
                    default:
                        return TextureWrapMode.Wrap;
                }
            }
        }
    }
}
