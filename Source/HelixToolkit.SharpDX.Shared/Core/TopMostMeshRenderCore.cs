/*
The MIT License (MIT)
Copyright (c) 2021 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
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
        using Shaders;
        using Render;
        using Components;
        /// <summary>
        /// Clears the depth buffer and reset global transform.
        /// </summary>
        public class TopMostMeshRenderCore: RenderCore
        {
            public TopMostMeshRenderCore() : base(RenderType.ScreenSpaced)
            {
            }
            
            public override void Render(RenderContext context, DeviceContextProxy deviceContext)
            {
                if (RenderType != RenderType.ScreenSpaced)
                {
                    return;
                }
                deviceContext.GetDepthStencilView(out DepthStencilView dsView);
                if (dsView == null)
                {
                    return;
                }

                deviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Depth, 1f, 0);
                dsView.Dispose();
                context.RestoreGlobalTransform();
                context.UpdatePerFrameData(true, false, deviceContext);
                deviceContext.SetViewport(context.Viewport.X, context.Viewport.Y, context.Viewport.Width, context.Viewport.Height);
                deviceContext.SetScissorRectangle((int)context.Viewport.X, (int)context.Viewport.Y,
                    (int)context.Viewport.Width, (int)context.Viewport.Height);
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                return true;
            }
        }
    }
}
