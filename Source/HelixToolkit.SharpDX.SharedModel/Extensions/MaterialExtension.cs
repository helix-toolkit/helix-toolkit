using System;
#if !NETFX_CORE && !WINUI
#if COREWPF
using HelixToolkit.SharpDX.Core.Model;
#endif
namespace HelixToolkit.Wpf.SharpDX
#elif WINUI
using HelixToolkit.SharpDX.Core.Model;
namespace HelixToolkit.WinUI
#else
namespace HelixToolkit.UWP
#endif
{
    using Model;

    public static class MaterialExtension
    {
        public static PhongMaterial ConvertToPhongMaterial(this PhongMaterialCore core)
        {
            return core == null ? null : new PhongMaterial()
            {
                DiffuseColor = core.DiffuseColor,
                AmbientColor = core.AmbientColor,
                EmissiveColor = core.EmissiveColor,
                SpecularColor = core.SpecularColor,
                ReflectiveColor = core.ReflectiveColor,
                SpecularShininess = core.SpecularShininess,
                DiffuseAlphaMap = core.DiffuseAlphaMap,
                DiffuseMap = core.DiffuseMap,
                EmissiveMap = core.EmissiveMap,
                SpecularColorMap = core.SpecularColorMap,
                RenderDiffuseMap = core.RenderDiffuseMap,
                RenderDiffuseAlphaMap = core.RenderDiffuseAlphaMap,
                RenderDisplacementMap = core.RenderDisplacementMap,
                RenderEnvironmentMap = core.RenderEnvironmentMap,
                RenderNormalMap = core.RenderNormalMap,
                RenderShadowMap = core.RenderShadowMap,
                RenderSpecularColorMap = core.RenderSpecularColorMap,
                RenderEmissiveMap = core.RenderEmissiveMap,
                EnableAutoTangent = core.EnableAutoTangent,
                DiffuseMapSampler = core.DiffuseMapSampler,
                DisplacementMap = core.DisplacementMap,
                DisplacementMapSampler = core.DisplacementMapSampler,
                NormalMap = core.NormalMap,
                DisplacementMapScaleMask = core.DisplacementMapScaleMask,
                Name = core.Name,
                UVTransform = core.UVTransform,
                EnableTessellation = core.EnableTessellation,
                MaxDistanceTessellationFactor = core.MaxDistanceTessellationFactor,
                MaxTessellationDistance = core.MaxTessellationDistance,
                MinDistanceTessellationFactor = core.MinDistanceTessellationFactor,
                MinTessellationDistance = core.MinTessellationDistance,
            };
        }

        public static PBRMaterial ConvertToPBRMaterial(this PBRMaterialCore core)
        {
            return core == null ? null : new PBRMaterial()
            {
                AlbedoColor = core.AlbedoColor,
                MetallicFactor = core.MetallicFactor,
                RoughnessFactor = core.RoughnessFactor,
                AlbedoMap = core.AlbedoMap,
                NormalMap = core.NormalMap,
                EmissiveMap = core.EmissiveMap,
                RoughnessMetallicMap = core.RoughnessMetallicMap,
                AmbientOcculsionMap = core.AmbientOcculsionMap,
                IrradianceMap = core.IrradianceMap,
                DisplacementMap = core.DisplacementMap,
                SurfaceMapSampler = core.SurfaceMapSampler,
                IBLSampler = core.IBLSampler,
                DisplacementMapSampler = core.DisplacementMapSampler,
                AmbientOcclusionFactor = core.AmbientOcclusionFactor,
                ClearCoatRoughness = core.ClearCoatRoughness,
                ClearCoatStrength = core.ClearCoatStrength,
                EmissiveColor = core.EmissiveColor,
                EnableAutoTangent = core.EnableAutoTangent,
                Name = core.Name,
                ReflectanceFactor = core.ReflectanceFactor,
                RenderAlbedoMap = core.RenderAlbedoMap,
                RenderDisplacementMap = core.RenderDisplacementMap,
                RenderEmissiveMap = core.RenderEmissiveMap,
                RenderEnvironmentMap = core.RenderEnvironmentMap,
                RenderIrradianceMap = core.RenderIrradianceMap,
                RenderNormalMap = core.RenderNormalMap,
                RenderRoughnessMetallicMap = core.RenderRoughnessMetallicMap,
                RenderAmbientOcclusionMap = core.RenderAmbientOcclusionMap,
                RenderShadowMap = core.RenderShadowMap,

                DisplacementMapScaleMask = core.DisplacementMapScaleMask,
                UVTransform = core.UVTransform,

                EnableTessellation = core.EnableTessellation,
                MaxDistanceTessellationFactor = core.MaxDistanceTessellationFactor,
                MinDistanceTessellationFactor = core.MinDistanceTessellationFactor,
                MaxTessellationDistance = core.MaxTessellationDistance,
                MinTessellationDistance = core.MinTessellationDistance,
            };
        }

        public static Material ConvertToMaterial(this MaterialCore core)
        {
            if (core is PhongMaterialCore p)
            {
                return p.ConvertToPhongMaterial();
            }
            else if (core is PBRMaterialCore pbr)
            {
                return pbr.ConvertToPBRMaterial();
            }
            else
            {
                throw new NotSupportedException($"Current material core to material conversion has not been supported yet.");
            }
        }

        public static void AssignTo(this PhongMaterial material, PhongMaterial targetMaterial)
        {
            targetMaterial.AmbientColor = material.AmbientColor;
            targetMaterial.DiffuseAlphaMap = material.DiffuseAlphaMap;
            targetMaterial.DiffuseColor = material.DiffuseColor;
            targetMaterial.DiffuseMap = material.DiffuseMap;
            targetMaterial.DiffuseMapSampler = material.DiffuseMapSampler;
            targetMaterial.DisplacementMap = material.DisplacementMap;
            targetMaterial.DisplacementMapSampler = material.DisplacementMapSampler;
            targetMaterial.DisplacementMapScaleMask = material.DisplacementMapScaleMask;
            targetMaterial.EmissiveColor = material.EmissiveColor;
            targetMaterial.EnableTessellation = material.EnableTessellation;
            targetMaterial.MaxTessellationDistance = material.MaxTessellationDistance;
            targetMaterial.MaxDistanceTessellationFactor = material.MaxDistanceTessellationFactor;
            targetMaterial.MinTessellationDistance = material.MinTessellationDistance;
            targetMaterial.MinDistanceTessellationFactor = material.MinDistanceTessellationFactor;
            targetMaterial.NormalMap = material.NormalMap;
            targetMaterial.EmissiveMap = material.EmissiveMap;
            targetMaterial.SpecularColorMap = material.SpecularColorMap;
            targetMaterial.ReflectiveColor = material.ReflectiveColor;
            targetMaterial.SpecularColor = material.SpecularColor;
            targetMaterial.SpecularShininess = material.SpecularShininess;
            targetMaterial.RenderDiffuseAlphaMap = material.RenderDiffuseAlphaMap;
            targetMaterial.RenderDiffuseMap = material.RenderDiffuseMap;
            targetMaterial.RenderDisplacementMap = material.RenderDisplacementMap;
            targetMaterial.RenderNormalMap = material.RenderNormalMap;
            targetMaterial.RenderSpecularColorMap = material.RenderSpecularColorMap;
            targetMaterial.RenderEmissiveMap = material.RenderEmissiveMap;
            targetMaterial.EnableAutoTangent = material.EnableAutoTangent;
            targetMaterial.UVTransform = material.UVTransform;
        }

        public static void AssignTo(this DiffuseMaterial material, DiffuseMaterial targetMaterial)
        {
            targetMaterial.DiffuseColor = material.DiffuseColor;
            targetMaterial.DiffuseMap = material.DiffuseMap;
            targetMaterial.DiffuseMapSampler = material.DiffuseMapSampler;
            targetMaterial.UVTransform = material.UVTransform;
        }
    }
}

#if !COREWPF && !WINUI
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    public partial class PhongMaterialCore
    {
        public static implicit operator PhongMaterial(PhongMaterialCore core)
        {
            return MaterialExtension.ConvertToPhongMaterial(core);
        }
    }
    public partial class PBRMaterialCore
    {
        public static implicit operator PBRMaterial(PBRMaterialCore core)
        {
            return MaterialExtension.ConvertToPBRMaterial(core);
        }
    }
}
#endif