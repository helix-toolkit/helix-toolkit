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
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new DiffuseMaterialVariables(DefaultPassNames.Diffuse, manager, technique, this);
        }
    }

    public sealed class ViewCubeMaterialCore : PhongMaterialCore
    {
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new DiffuseMaterialVariables(DefaultPassNames.ViewCube, manager, technique, this);
        }
    }
}
