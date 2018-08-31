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
    /// <summary>
    /// 
    /// </summary>
    public sealed class EmptyMaterialVariable : MaterialVariable
    {
        public static readonly EmptyMaterialVariable EmptyVariable = new EmptyMaterialVariable();
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyMaterialVariable"/> class.
        /// </summary>
        public EmptyMaterialVariable() : base(null, null, null, null)
        {

        }
        /// <summary>
        /// Binds the material resources.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="shaderPass">The shader pass.</param>
        /// <returns></returns>
        public override bool BindMaterialResources(RenderContext context, DeviceContextProxy deviceContext, ShaderPass shaderPass)
        {
            return false;
        }
        /// <summary>
        /// Gets the pass.
        /// </summary>
        /// <param name="renderType">Type of the render.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override ShaderPass GetPass(RenderType renderType, RenderContext context)
        {
            return ShaderPass.NullPass;
        }

        protected override void UpdateInternalVariables(DeviceContextProxy context)
        {
        }
        /// <summary>
        /// Draws the specified device context.
        /// </summary>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="bufferModel">The buffer model.</param>
        /// <param name="instanceCount">The instance count.</param>
        public override void Draw(DeviceContextProxy deviceContext, IAttachableBufferModel bufferModel, int instanceCount)
        {
        }
        /// <summary>
        /// Gets the shadow pass.
        /// </summary>
        /// <param name="renderType">Type of the render.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override ShaderPass GetShadowPass(RenderType renderType, RenderContext context)
        {
            return ShaderPass.NullPass;
        }
        /// <summary>
        /// Gets the wireframe pass.
        /// </summary>
        /// <param name="renderType">Type of the render.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override ShaderPass GetWireframePass(RenderType renderType, RenderContext context)
        {
            return ShaderPass.NullPass;
        }
    }
}
