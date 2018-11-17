/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using System.IO;
using System.Runtime.CompilerServices;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Utilities;
    public sealed class SSAOCore : RenderCore
    {
        private Texture2DDescription ssaoTextureDesc = new Texture2DDescription()
        {
            CpuAccessFlags = CpuAccessFlags.None,
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
            Format = global::SharpDX.DXGI.Format.R16_Float,
            SampleDescription = new global::SharpDX.DXGI.SampleDescription(1, 0),
            OptionFlags = ResourceOptionFlags.None,
            Usage = ResourceUsage.Default,
            ArraySize = 1,
            MipLevels = 1,
        };

        private ShaderResourceViewProxy ssaoView;
        private int width, height;

        public SSAOCore():base(RenderType.PreProc)
        {

        }

        public override void Render(RenderContext context, DeviceContextProxy deviceContext)
        {
            EnsureTextureResources((int)context.ActualWidth, (int)context.ActualHeight, deviceContext);
            var ds = context.RenderHost.RenderBuffer.FullResDepthStencilPool.Get(global::SharpDX.DXGI.Format.D32_Float);
            var rt0 = context.RenderHost.RenderBuffer.FullResRenderTargetPool.Get(global::SharpDX.DXGI.Format.R16G16B16A16_Float);
            deviceContext.ClearDepthStencilView(ds, DepthStencilClearFlags.Depth, 1, 0);
            deviceContext.ClearRenderTargetView(rt0, new Color4(0,0,0,0));
            deviceContext.SetRenderTarget(ds, rt0);
            for(int i = 0; i < context.RenderHost.PerFrameOpaqueNodes.Count; ++i)
            {

            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureTextureResources(int w, int h, DeviceContextProxy deviceContext)
        {
            if(w != width || h != height)
            {
                RemoveAndDispose(ref ssaoView);
                width = w;
                height = h;
                if(width > 10 && height > 0)
                {
                    ssaoTextureDesc.Width = width;
                    ssaoTextureDesc.Height = height;
                    ssaoView = Collect(new ShaderResourceViewProxy(deviceContext, ssaoTextureDesc));
                    ssaoView.CreateTextureView();
                    ssaoView.CreateRenderTargetView();
                }
            }
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            width = height = 0;
            return true;
        }
    }
}
