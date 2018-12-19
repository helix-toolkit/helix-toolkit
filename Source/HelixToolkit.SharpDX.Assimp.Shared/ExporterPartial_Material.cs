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

            protected virtual global::Assimp.Material OnCreateAssimpMaterial(MaterialCore material)
            {
                var assimpMaterial = new global::Assimp.Material();
                if(material is PhongMaterialCore phong)
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
                }
                else if(material is PBRMaterialCore pbr)
                {
                    assimpMaterial.ShadingMode = ShadingMode.Fresnel;
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_DIFFUSE_BASE, pbr.AlbedoColor.ToAssimpColor4D()));
                    assimpMaterial.AddProperty(new MaterialProperty(GLTFMatKeys.AI_MATKEY_GLTF_BASECOLOR_FACTOR, pbr.AlbedoColor.ToAssimpColor4D()));
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_EMISSIVE_BASE, pbr.EmissiveColor.ToAssimpColor4D()));
                    if(pbr.ReflectanceFactor != 0)
                    {
                        assimpMaterial.AddProperty(new MaterialProperty(GLTFMatKeys.AI_MATKEY_GLTF_PBRSPECULARGLOSSINESS, true));
                        assimpMaterial.AddProperty(new MaterialProperty(GLTFMatKeys.AI_MATKEY_GLTF_PBRSPECULARGLOSSINESS_GLOSSINESS_FACTOR, pbr.ReflectanceFactor));
                    }
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_AMBIENT, pbr.AmbientOcclusionFactor));
                    
                }
                else if(material is DiffuseMaterialCore diffuse)
                {
                    assimpMaterial.ShadingMode = ShadingMode.Gouraud;
                    assimpMaterial.AddProperty(new MaterialProperty(AiMatKeys.COLOR_DIFFUSE_BASE, diffuse.DiffuseColor.ToAssimpColor4D()));
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
        }
    }
}
