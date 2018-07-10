#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using Model;
    using System;

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
                DiffuseAlphaMapSampler = core.DiffuseAlphaMapSampler,
                DiffuseMap = core.DiffuseMap,
                DiffuseMapSampler = core.DiffuseMapSampler,
                DisplacementMap = core.DisplacementMap,
                DisplacementMapSampler = core.DisplacementMapSampler,
                NormalMap = core.NormalMap,
                NormalMapSampler = core.NormalMapSampler,
                DisplacementMapScaleMask = core.DisplacementMapScaleMask,
                Name = core.Name,
            };
        }

        public static Material ConvertToMaterial(this MaterialCore core)
        {
            if(core is PhongMaterialCore p)
            {
                return p.ConvertToPhongMaterial();
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
            targetMaterial.DiffuseAlphaMapSampler = material.DiffuseAlphaMapSampler;
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
            targetMaterial.NormalMapSampler = material.NormalMapSampler;
            targetMaterial.ReflectiveColor = material.ReflectiveColor;
            targetMaterial.SpecularColor = material.SpecularColor;
            targetMaterial.SpecularShininess = material.SpecularShininess;
            targetMaterial.RenderDiffuseAlphaMap = material.RenderDiffuseAlphaMap;
            targetMaterial.RenderDiffuseMap = material.RenderDiffuseMap;
            targetMaterial.RenderDisplacementMap = material.RenderDisplacementMap;
            targetMaterial.RenderNormalMap = material.RenderNormalMap;
        }

        public static void AssignTo(this DiffuseMaterial material, DiffuseMaterial targetMaterial)
        {
            targetMaterial.DiffuseColor = material.DiffuseColor;
            targetMaterial.DiffuseMap = material.DiffuseMap;
            targetMaterial.DiffuseMapSampler = material.DiffuseMapSampler;
        }
    }
}


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
}