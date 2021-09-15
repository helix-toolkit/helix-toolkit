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
            private readonly ConstantBufferComponent globalTransformCB;
            public TopMostMeshRenderCore() : base(RenderType.ScreenSpaced)
            {
                globalTransformCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes)));
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
                var globalTrans = context.GlobalTransform;
                globalTransformCB.Upload(deviceContext, ref globalTrans);
                deviceContext.SetViewport(0, 0, context.ActualWidth, context.ActualHeight);
                deviceContext.SetScissorRectangle(0, 0, (int)context.ActualWidth, (int)context.ActualHeight);
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                return true;
            }
        }
    }
}
