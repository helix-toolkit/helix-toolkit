/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Core
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

            protected override bool OnAttach(IRenderTechnique technique)
            {
                return true;
            }
        }
    }

}
