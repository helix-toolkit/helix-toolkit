/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Core;
    using Render;
    using Shaders;
    /// <summary>
    /// 
    /// </summary>
    public sealed class NormalMaterialCore : MaterialCore
    {
        public static readonly NormalMaterialCore Core = new NormalMaterialCore();

        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager)
        {
            return new PassOnlyMaterialVariable(DefaultPassNames.Normals);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class ColorMaterialCore : MaterialCore
    {
        public static readonly ColorMaterialCore Core = new ColorMaterialCore();
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager)
        {
            return new PassOnlyMaterialVariable(DefaultPassNames.Colors);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class PositionMaterialCore : MaterialCore
    {
        public static readonly PositionMaterialCore Core = new PositionMaterialCore();
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager)
        {
            return new PassOnlyMaterialVariable(DefaultPassNames.Positions);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class NormalVectorMaterialCore : MaterialCore
    {
        public static readonly NormalVectorMaterialCore Core = new NormalVectorMaterialCore();
        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager)
        {
            return new PassOnlyMaterialVariable(DefaultPassNames.NormalVector);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class PassOnlyMaterialVariable : MaterialVariable
    {
        public ShaderPass MaterialPass { private set; get; }

        public bool RenderDiffuseMap { set; get; }
        public bool RenderDiffuseAlphaMap { set; get; }
        public bool RenderNormalMap { set; get; }
        public bool RenderDisplacementMap { set; get; }
        public override bool RenderShadowMap { set; get; }
        public override bool RenderEnvironmentMap { set; get; }
        public override string DefaultShaderPassName { set; get; }

        private readonly string passName;
        public PassOnlyMaterialVariable(string passName) : base(null)
        {
            this.passName = passName;
        }

        public override bool Attach(IRenderTechnique technique)
        {
            if (base.Attach(technique))
            {
                MaterialPass = technique[passName];
                return !MaterialPass.IsNULL;
            }
            else
            {
                return false;
            }
        }

        protected override bool OnBindMaterialTextures(DeviceContextProxy context, ShaderPass shaderPass)
        {
            return true;
        }

        public override ShaderPass GetPass(MaterialGeometryRenderCore core, RenderContext context)
        {
            return MaterialPass;
        }

        protected override void AssignVariables(ref ModelStruct model)
        {
        }
    }
}
