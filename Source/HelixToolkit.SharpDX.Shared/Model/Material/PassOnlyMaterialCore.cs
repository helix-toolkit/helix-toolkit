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
    /// <summary>
    /// 
    /// </summary>
    public sealed class NormalMaterialCore : MaterialCore
    {
        public static readonly NormalMaterialCore Core = new NormalMaterialCore();

        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new PassOnlyMaterialVariable(DefaultPassNames.Normals, technique);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class ColorMaterialCore : MaterialCore
    {
        public static readonly ColorMaterialCore Core = new ColorMaterialCore();
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new PassOnlyMaterialVariable(DefaultPassNames.Colors, technique);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class PositionMaterialCore : MaterialCore
    {
        public static readonly PositionMaterialCore Core = new PositionMaterialCore();
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new PassOnlyMaterialVariable(DefaultPassNames.Positions, technique);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class NormalVectorMaterialCore : MaterialCore
    {
        public static readonly NormalVectorMaterialCore Core = new NormalVectorMaterialCore();
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new PassOnlyMaterialVariable(DefaultPassNames.NormalVector, technique);
        }
    }
}
