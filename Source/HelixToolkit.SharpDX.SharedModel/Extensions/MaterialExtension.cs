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