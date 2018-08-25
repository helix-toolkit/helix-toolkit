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
    using Shaders;
    using Render;
    /// <summary>
    /// 
    /// </summary>
    public sealed class PassOnlyMaterialVariable : MaterialVariable
    {
        public ShaderPass MaterialPass { private set; get; }

        public override string DefaultShaderPassName { set; get; }

        private readonly string passName;
        public PassOnlyMaterialVariable(string passName, IRenderTechnique technique)
            : base(technique.EffectsManager, technique, DefaultMeshConstantBufferDesc)
        {
            this.passName = passName;
            MaterialPass = technique[passName];
        }

        protected override bool OnBindMaterialTextures(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            return true;
        }

        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return MaterialPass;
        }

        protected override void UpdateInternalVariables(DeviceContextProxy context)
        {
        }

        protected override void WriteMaterialDataToConstantBuffer(global::SharpDX.DataStream cbStream)
        {
        }
    }
}
