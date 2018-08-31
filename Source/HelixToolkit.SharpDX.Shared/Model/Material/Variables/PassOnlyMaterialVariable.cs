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
    using Render;
    using Shaders;
    /// <summary>
    /// 
    /// </summary>
    public sealed class PassOnlyMaterialVariable : MaterialVariable
    {
        public ShaderPass MaterialPass { get; }

        public ShaderPass ShadowPass { get; }

        public ShaderPass WireframePass { get; }

        private readonly string passName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PassOnlyMaterialVariable"/> class.
        /// </summary>
        /// <param name="passName">Name of the pass.</param>
        /// <param name="technique">The technique.</param>
        /// <param name="shadowPassName">Name of the shadow pass.</param>
        /// <param name="wireframePassName">Name of the wireframe pass.</param>
        public PassOnlyMaterialVariable(string passName, IRenderTechnique technique, string shadowPassName = DefaultPassNames.ShadowPass, string wireframePassName = DefaultPassNames.Wireframe)
            : base(technique.EffectsManager, technique, DefaultMeshConstantBufferDesc, null)
        {
            this.passName = passName;
            MaterialPass = technique[passName];
            ShadowPass = technique[shadowPassName];
            WireframePass = technique[wireframePassName];
        }

        public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            return true;
        }

        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return MaterialPass;
        }
        public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
        {
            return ShadowPass;
        }

        public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
        {
            return WireframePass;
        }

        public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
        {
            DrawIndexed(deviceContext, bufferModel.IndexBuffer.ElementCount, instanceCount);
        }
    }
}
