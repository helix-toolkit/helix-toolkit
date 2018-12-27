/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Assimp;
using Assimp.Unmanaged;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Concurrent;
using System.IO;
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
    using Model;
    using System.Collections.Generic;
    using System.Threading;

    namespace Assimp
    {
        public partial class Importer
        {
            private readonly ConcurrentDictionary<string, Stream> textureDict =
                new ConcurrentDictionary<string, Stream>();
            /// <summary>
            ///     To the phong material.
            /// </summary>
            /// <param name="material">The material.</param>
            /// <returns></returns>
            protected virtual PhongMaterialCore OnCreatePhongMaterial(global::Assimp.Material material)
            {
                var phong = new PhongMaterialCore
                {
                    AmbientColor = material.ColorAmbient.ToSharpDXColor4(),
                    DiffuseColor = material.ColorDiffuse.ToSharpDXColor4(),
                    SpecularColor = material.ColorSpecular.ToSharpDXColor4(),
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
                    phong.DiffuseMapFilePath = material.TextureDiffuse.FilePath;
                    var desc = Shaders.DefaultSamplers.LinearSamplerClampAni1;
                    desc.AddressU = ToDXAddressMode(material.TextureDiffuse.WrapModeU);
                    desc.AddressV = ToDXAddressMode(material.TextureDiffuse.WrapModeV);
                    phong.DiffuseMapSampler = desc;
                }

                if (material.HasTextureNormal)
                {
                    phong.NormalMap = LoadTexture(material.TextureNormal.FilePath);
                    phong.NormalMapFilePath = material.TextureNormal.FilePath;
                }
                else if (material.HasTextureHeight)
                {
                    phong.NormalMap = LoadTexture(material.TextureHeight.FilePath);
                    phong.NormalMapFilePath = material.TextureHeight.FilePath;
                }
                if (material.HasTextureSpecular)
                {
                    phong.SpecularColorMap = LoadTexture(material.TextureSpecular.FilePath);
                    phong.SpecularColorMapFilePath = material.TextureSpecular.FilePath;
                }
                if (material.HasTextureDisplacement)
                {
                    phong.DisplacementMap = LoadTexture(material.TextureDisplacement.FilePath);
                    phong.DisplacementMapFilePath = material.TextureDisplacement.FilePath;
                }

                if (material.HasTextureOpacity)
                {
                    phong.DiffuseAlphaMap = LoadTexture(material.TextureOpacity.FilePath);
                    phong.DiffuseAlphaMapFilePath = material.TextureOpacity.FilePath;
                }
                if (material.HasTextureEmissive)
                {
                    phong.EmissiveMap = LoadTexture(material.TextureEmissive.FilePath);
                    phong.EmissiveMapFilePath = material.TextureEmissive.FilePath;
                }

                if (material.HasNonTextureProperty(AiMatKeys.UVTRANSFORM_BASE))
                {
                    var values = material.GetNonTextureProperty(AiMatKeys.UVTRANSFORM_BASE).GetFloatArrayValue();
                    if (values != null && values.Length == 5)
                    {
                        phong.UVTransform = new UVTransform(values[0], new Vector2(values[1], values[2]), new Vector2(values[3], values[4]));
                    }
                }
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
                };
                if (material.HasNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_BASECOLOR_FACTOR))
                {
                    pbr.AlbedoColor = material.GetNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_BASECOLOR_FACTOR)
                       .GetColor4DValue().ToSharpDXColor4();
                }
                if (material.HasNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_METALLIC_FACTOR))
                {
                    pbr.MetallicFactor = material.GetNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_METALLIC_FACTOR)
                       .GetFloatValue();
                }
                if (material.HasColorAmbient)
                {
                    pbr.AmbientOcclusionFactor = material.ColorAmbient.R;
                }
                if (material.HasNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_ROUGHNESS_FACTOR))
                {
                    pbr.RoughnessFactor = material.GetNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_METALLIC_FACTOR)
                        .GetFloatValue();
                }
                else if(material.HasColorSpecular && material.HasShininess)
                {
                    //Ref https://github.com/assimp/assimp/blob/master/code/glTF2Exporter.cpp
                    float specularIntensity = material.ColorSpecular.R * 0.2125f 
                        + material.ColorSpecular.G * 0.7154f + material.ColorSpecular.B * 0.0721f;
                    float normalizedShininess = (float)Math.Sqrt(material.Shininess / 1000);
                    normalizedShininess = Math.Min(Math.Max(normalizedShininess, 0), 1f);
                    normalizedShininess *= specularIntensity;
                    pbr.RoughnessFactor = 1 - normalizedShininess;
                }
                if(material.HasNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_PBRSPECULARGLOSSINESS))
                {
                    var hasGlossiness = material.GetNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_PBRSPECULARGLOSSINESS).GetBooleanValue();
                    if (hasGlossiness)
                    {
                        if(material.HasNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_PBRSPECULARGLOSSINESS_GLOSSINESS_FACTOR))
                        {
                            pbr.ReflectanceFactor = material.GetNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_PBRSPECULARGLOSSINESS_GLOSSINESS_FACTOR).GetFloatValue();
                        }
                        else if(material.HasShininess)
                        {
                            pbr.ReflectanceFactor = material.Shininess / 1000;
                        }
                    }
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
                    pbr.AlbedoMapFilePath = material.TextureDiffuse.FilePath;
                    var desc = Shaders.DefaultSamplers.LinearSamplerClampAni1;
                    desc.AddressU = ToDXAddressMode(material.TextureDiffuse.WrapModeU);
                    desc.AddressV = ToDXAddressMode(material.TextureDiffuse.WrapModeV);
                    pbr.SurfaceMapSampler = desc;
                }

                if (material.HasTextureNormal)
                {
                    pbr.NormalMap = LoadTexture(material.TextureNormal.FilePath);
                    pbr.NormalMapFilePath = material.TextureNormal.FilePath;
                }
                else if (material.HasTextureHeight)
                {
                    pbr.NormalMap = LoadTexture(material.TextureHeight.FilePath);
                    pbr.NormalMapFilePath = material.TextureHeight.FilePath;
                }
                if (material.HasProperty(GLTFMatKeys.AI_MATKEY_GLTF_METALLICROUGHNESSAO_TEXTURE, TextureType.Unknown, 0))
                {
                    var t = material.GetProperty(GLTFMatKeys.AI_MATKEY_GLTF_METALLICROUGHNESSAO_TEXTURE,
                        TextureType.Unknown, 0);
                    pbr.RMAMap = LoadTexture(t.GetStringValue());
                    pbr.RMAMapFilePath = t.GetStringValue();
                }
                else if (material.HasTextureSpecular)
                {
                    pbr.RMAMap = LoadTexture(material.TextureSpecular.FilePath);
                    pbr.RMAMapFilePath = material.TextureSpecular.FilePath;
                }

                if (material.HasTextureDisplacement)
                {
                    pbr.DisplacementMap = LoadTexture(material.TextureDisplacement.FilePath);
                    pbr.DisplacementMapFilePath = material.TextureDisplacement.FilePath;
                }
                if (material.HasTextureLightMap)
                {
                    pbr.IrradianceMap = LoadTexture(material.TextureLightMap.FilePath);
                    pbr.IrradianceMapFilePath = material.TextureLightMap.FilePath;
                }
                if (material.HasTextureEmissive)
                {
                    pbr.EmissiveMap = LoadTexture(material.TextureEmissive.FilePath);
                    pbr.EmissiveMapFilePath = material.TextureEmissive.FilePath;
                }
                if(material.HasNonTextureProperty(AiMatKeys.UVTRANSFORM_BASE))
                {
                    var values = material.GetNonTextureProperty(AiMatKeys.UVTRANSFORM_BASE).GetFloatArrayValue();
                    if(values != null && values.Length == 5)
                    {
                        pbr.UVTransform = new UVTransform(values[0], new Vector2(values[1], values[2]), new Vector2(values[3], values[4]));
                    }
                }
                return pbr;
            }

            /// <summary>
            ///     To the helix material.
            /// </summary>
            /// <param name="material">The material.</param>
            /// <returns></returns>
            /// <exception cref="System.NotSupportedException">Shading Mode {material.ShadingMode}</exception>
            protected virtual KeyValuePair<global::Assimp.Material, MaterialCore> OnCreateHelixMaterial(
                global::Assimp.Material material)
            {
                MaterialCore core = null;
                if (!material.HasShadingMode)
                {
                    if (material.HasNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_METALLIC_FACTOR)
                        || material.HasNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_ROUGHNESS_FACTOR)
                        || material.HasNonTextureProperty(GLTFMatKeys.AI_MATKEY_GLTF_BASECOLOR_FACTOR))
                    {
                        material.ShadingMode = ShadingMode.Fresnel;
                    }
                    else if(material.HasColorSpecular || material.HasColorDiffuse || material.HasTextureDiffuse)
                    {
                        material.ShadingMode = ShadingMode.Blinn;
                    }
                    else
                    {
                        material.ShadingMode = ShadingMode.Gouraud;
                    }
                }

                var mode = material.ShadingMode;
                if (Configuration.ImportMaterialType != MaterialType.Auto)
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
                            mode = ShadingMode.Fresnel;
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
                        {
                            diffuse.DiffuseMap = LoadTexture(material.TextureDiffuse.FilePath);
                            diffuse.DiffuseMapFilePath = material.TextureDiffuse.FilePath;
                        }
                        if (material.ShadingMode == ShadingMode.Flat)
                        {
                            diffuse.EnableUnLit = true;
                        }
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
                                Log(HelixToolkit.Logger.LogLevel.Warning, $"Shading Mode is not supported:{material.ShadingMode}");
                                core = new DiffuseMaterialCore() { DiffuseColor = Color.Red, EnableUnLit = true };
                                break;
                        }
                        break;
                }

                if (core != null)
                    core.Name = string.IsNullOrEmpty(material.Name) ? $"Material_{Interlocked.Increment(ref MaterialIndexForNoName)}" : material.Name;
                return new KeyValuePair<global::Assimp.Material, MaterialCore>(material, core);
            }

            /// <summary>
            ///     Called when [load texture].
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns></returns>
            protected virtual Stream OnLoadTexture(string path)
            {
                try
                {
                    var dict = Path.GetDirectoryName(this.path);
                    if (string.IsNullOrEmpty(dict))
                    {
                        dict = Directory.GetCurrentDirectory();
                    }
                    var p = Path.GetFullPath(Path.Combine(dict, path));
                    if (!File.Exists(p))
                        p = HandleTexturePathNotFound(dict, path);
                    if (!File.Exists(p))
                    {
                        Log(HelixToolkit.Logger.LogLevel.Warning, $"Load Texture Failed. Texture Path = {path}.");
                        return null;
                    }
                    return LoadFileToStream(p);
                }
                catch(Exception ex)
                {
                    Log(HelixToolkit.Logger.LogLevel.Warning, $"Load Texture Exception. Texture Path = {path}. Exception: {ex.Message}");
                }
                return null;
            }
            /// <summary>
            /// Handles the texture path not found. Override to provide your own handling
            /// </summary>
            /// <param name="dir">The dir.</param>
            /// <param name="texturePath">The texture path.</param>
            /// <returns></returns>
            protected virtual string HandleTexturePathNotFound(string dir, string texturePath)
            {
                //If file not found in texture path dir, try to find the file in the same dir as the model file
                if (texturePath.StartsWith(ToUpperDictString))
                {
                    var t = texturePath.Remove(0, ToUpperDictString.Length);
                    var p = Path.GetFullPath(Path.Combine(dir, t));
                    if (File.Exists(p))
                        return p;
                }

                //If still not found, try to go one upper level and find
                var upper = Directory.GetParent(dir).FullName;
                try
                {
                    upper = Path.GetFullPath(upper + texturePath);
                }
                catch (NotSupportedException ex)
                {
                    Log(HelixToolkit.Logger.LogLevel.Warning, $"Exception: {ex}");
                }
                if (File.Exists(upper))
                    return upper;
                var fileName = Path.GetFileName(texturePath);
                var currentPath = Path.Combine(dir, fileName);
                if (File.Exists(currentPath))
                {
                    return currentPath;
                }
                return "";
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
        }
    }
}
