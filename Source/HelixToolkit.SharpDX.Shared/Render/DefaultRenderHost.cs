/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Wpf.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX.Core2D;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    public class DefaultRenderHost : DX11RenderHostBase
    {
        protected readonly List<IRenderable> pendingRenderables = new List<IRenderable>();
        protected readonly List<IRenderCore> pendingRenderCores = new List<IRenderCore>();
        protected RenderParameter renderParameter;
        private Task asyncTask;

        protected override ID2DTarget CreateD2DTarget()
        {
            return new D2DControlWrapper();
        }

        protected override IDX11RenderBufferProxy CreateRenderBuffer()
        {
            return new DX11RenderBufferProxy(Device);
        }

        protected override IRenderer CreateRenderer()
        {
            return new CommonRenderer(Device);
        }

        protected override void OnInitializeBuffers(IDX11RenderBufferProxy buffer, ID2DTarget d2dTarget, IRenderer renderer)
        {           
            var texture = buffer.Initialize((int)ActualWidth, (int)ActualHeight, MSAA);
            d2dTarget.Initialize(texture);
        }

        protected override void PreRender()
        {
            base.PreRender();
            pendingRenderables.Clear();
            pendingRenderCores.Clear();
            pendingRenderables.AddRange(renderer.UpdateSceneGraph(RenderContext, Viewport.Renderables));
            pendingRenderCores.AddRange(pendingRenderables.Select(x => x.RenderCore));
            asyncTask = Task.Factory.StartNew(() => { renderer.UpdateNotRenderParallel(pendingRenderables); });
            renderer.UpdateGlobalVariables(RenderContext, Viewport.Renderables, ref renderParameter);
        }

        protected override void OnRender(TimeSpan time)
        {
            var renderParameter = CreateParameter(renderBuffer);
            renderer.RenderScene(RenderContext, renderer.ImmediateContext, pendingRenderCores, ref renderParameter);
        }

        protected static RenderParameter CreateParameter(IDX11RenderBufferProxy buffer)
        {
            return new RenderParameter()
            {
                RenderTargetView = buffer.ColorBufferView,
                DepthStencilView = buffer.DepthStencilBufferView,
                ScissorRegion = new Rectangle(0, 0, buffer.TargetWidth, buffer.TargetHeight),
                ViewportRegion = new ViewportF(0, 0, buffer.TargetWidth, buffer.TargetHeight)
            };
        }

        protected override void PostRender()
        {
            asyncTask?.Wait();
        }

        protected override void OnRender2D(TimeSpan time)
        {
            
        }
    }
}
