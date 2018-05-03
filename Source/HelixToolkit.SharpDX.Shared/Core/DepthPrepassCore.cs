/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
//#define TEST

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    /// <summary>
    /// Do a depth prepass before rendering.
    /// <para>Must customize the DefaultEffectsManager and set DepthStencilState to DefaultDepthStencilDescriptions.DSSDepthEqualNoWrite in default ShaderPass from EffectsManager to achieve best performance.</para>
    /// </summary>
    public class DepthPrepassCore : RenderCoreBase<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepthPrepassCore"/> class.
        /// </summary>
        public DepthPrepassCore() : base(RenderType.PreProc)
        {
        }
        /// <summary>
        /// Gets the model constant buffer description.
        /// </summary>
        /// <returns></returns>
        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return null;
        }
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            context.IsCustomPass = true;
            context.CustomPassName = DefaultPassNames.DepthPrepass;
            for (int i = 0; i < context.RenderHost.PerFrameOpaqueNodes.Count; ++i)
            {
                var core = context.RenderHost.PerFrameOpaqueNodes[i];
                if (core.RenderType == RenderType.Opaque)
                {
                    var pass = core.EffectTechnique[DefaultPassNames.DepthPrepass];
                    if (pass.IsNULL)
                    {
                        continue;
                    }
                    pass.BindShader(deviceContext);
                    pass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
                    core.Render(context, deviceContext);
                }
            }
            context.IsCustomPass = false;
        }
        /// <summary>
        /// Called when [update per model structure].
        /// </summary>
        /// <param name="model">if set to <c>true</c> [model].</param>
        /// <param name="context">The context.</param>
        protected override void OnUpdatePerModelStruct(ref bool model, RenderContext context)
        {
        }
        /// <summary>
        /// Called when [upload per model constant buffers].
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void OnUploadPerModelConstantBuffers(global::SharpDX.Direct3D11.DeviceContext context)
        {
            
        }
    }
}
