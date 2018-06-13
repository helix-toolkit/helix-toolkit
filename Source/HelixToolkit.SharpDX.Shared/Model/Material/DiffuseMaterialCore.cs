/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    public sealed class DiffuseMaterialCore : PhongMaterialCore
    {
        public override IEffectMaterialVariables CreateMaterialVariables(IEffectsManager manager)
        {
            return new TextureSharedPhongMaterialVariables(DefaultPassNames.Diffuse, manager, this);
        }
    }

    public sealed class ViewCubeMaterialCore : PhongMaterialCore
    {
        public override IEffectMaterialVariables CreateMaterialVariables(IEffectsManager manager)
        {
            return new TextureSharedPhongMaterialVariables(DefaultPassNames.ViewCube, manager, this);
        }
    }
}
