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
    public sealed class DepthPrepassCore : RenderCore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepthPrepassCore"/> class.
        /// </summary>
        public DepthPrepassCore() : base(RenderType.PreProc)
        {
        }

        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        public override void Render(RenderContext context, DeviceContextProxy deviceContext)
        {
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
                    core.RenderCustom(context, deviceContext);
                }
            }
        }

        public sealed override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public sealed override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            return true;
        }
    }
}
