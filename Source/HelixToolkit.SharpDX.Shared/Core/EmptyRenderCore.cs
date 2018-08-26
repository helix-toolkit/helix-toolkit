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
    public sealed class EmptyRenderCore : RenderCoreBase<int>
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
        protected override void OnRender(RenderContext context, DeviceContextProxy deviceContext)
        {

        }

        public override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        /// <summary>
        /// Called when [update per model structure].
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected override void OnUpdatePerModelStruct(ref int model, RenderContext context)
        {
           
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            return true;
        }
    }
}
