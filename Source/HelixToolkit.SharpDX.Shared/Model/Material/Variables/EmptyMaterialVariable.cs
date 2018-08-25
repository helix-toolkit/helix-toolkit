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
    using Utilities;
    using Render;
    using Shaders;
    public sealed class EmptyMaterialVariable : MaterialVariable
    {
        public static readonly EmptyMaterialVariable EmptyVariable = new EmptyMaterialVariable();

        public EmptyMaterialVariable() : base(null, null, null)
        {

        }

        protected override bool CanUpdateMaterial()
        {
            return false;
        }

        protected override bool OnBindMaterialTextures(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            return false;
        }

        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return ShaderPass.NullPass;
        }

        protected override void UpdateInternalVariables(DeviceContextProxy context)
        {
        }

        protected override void WriteMaterialDataToConstantBuffer(global::SharpDX.DataStream cbStream)
        {
        }

        public override void Draw(DeviceContextProxy deviceContext, IElementsBufferProxy indexBuffer, IElementsBufferModel instanceModel)
        {
        }

        public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
        {
            return ShaderPass.NullPass;
        }

        public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
        {
            return ShaderPass.NullPass;
        }
    }
}
