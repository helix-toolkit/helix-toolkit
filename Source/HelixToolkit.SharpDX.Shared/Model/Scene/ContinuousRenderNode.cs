/*
The MIT License(MIT)
Copyright(c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System.Collections.Generic;

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
    using Core;
    using Render;
    namespace Model.Scene
    {
        /// <summary>
        /// Use this node to keep update rendering in each frame.
        /// <para>Default behavior for render host is lazy rendering. 
        /// Only property changes trigger a render inside render loop. 
        /// However, sometimes user want to keep updating rendering for each frame such as doing shader animation using TimeStamp.
        /// Use this node to invalidate rendering and keep render host busy.</para>
        /// </summary>
        public sealed class ContinuousRenderNode : SceneNode
        {
            protected override RenderCore OnCreateRenderCore()
            {
                return new InvalidRendererCore();
            }

            protected override bool CanHitTest(RenderContext context)
            {
                return false;
            }

            protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
            {
                return false;
            }

            private sealed class InvalidRendererCore : RenderCore
            {
                public InvalidRendererCore() : base(RenderType.PostProc) { }

                public override void Render(RenderContext context, DeviceContextProxy deviceContext)
                {
                    RaiseInvalidateRender();
                }

                protected override bool OnAttach(IRenderTechnique technique)
                {
                    return true;
                }
            }
        }
    }
}
