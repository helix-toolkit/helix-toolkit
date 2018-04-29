#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using Model;
    public static class MaterialExtension
    {
        public static PhongMaterial ConvertToPhongMaterial(this PhongMaterialCore core)
        {
            return new PhongMaterial()
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