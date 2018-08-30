/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    /// <summary>
    /// 
    /// </summary>
    public sealed class EmptyRenderCore : RenderCore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyRenderCore"/> class.
        /// </summary>
        public EmptyRenderCore() : base(RenderType.None)
        {
        }

        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        public override void Render(RenderContext context, DeviceContextProxy deviceContext)
        {

        }

        public override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            return true;
        }
    }
}
