/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
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
    using Core;
    using global::SharpDX.Direct3D11;

    /// <summary>
    /// 
    /// </summary>
    public class DefaultRenderHost : DX11RenderHostBase
    {
        /// <summary>
        /// The pending renderables
        /// </summary>
        protected readonly List<IRenderable> pendingRenderables = new List<IRenderable>();
        /// <summary>
        /// The pending render cores
        /// </summary>
        protected readonly List<IRenderCore> pendingRenderCores = new List<IRenderCore>();
        /// <summary>
        /// The render parameter
        /// </summary>
        protected RenderParameter renderParameter;

        private Task asyncTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderHost"/> class.
        /// </summary>
        public DefaultRenderHost() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderHost"/> class.
        /// </summary>
        /// <param name="createRenderer">The create renderer.</param>
        public DefaultRenderHost(Func<Device, IRenderer> createRenderer) : base(createRenderer)
        {

        }

        /// <summary>
        /// Creates the render buffer.
        /// </summary>
        /// <returns></returns>
        protected override IDX11RenderBufferProxy CreateRenderBuffer()
        {
            return new DX11RenderBufferProxy(Device);
        }

        /// <summary>
        /// <see cref="DX11RenderHostBase.PreRender"/>
        /// </summary>
        protected override void PreRender()
        {
            base.PreRender();
            pendingRenderables.Clear();
            pendingRenderCores.Clear();
            pendingRenderables.AddRange(renderer.UpdateSceneGraph(RenderContext, Viewport.Renderables));
            pendingRenderCores.AddRange(pendingRenderables.Select(x => x.RenderCore));
            asyncTask = Task.Factory.StartNew(() => {
                renderer.UpdateNotRenderParallel(pendingRenderables);
            });
        }

        /// <summary>
        /// <see cref="DX11RenderHostBase.OnRender(TimeSpan)"/>
        /// </summary>
        /// <param name="time">The time.</param>
        protected override void OnRender(TimeSpan time)
        {
            var renderParameter = CreateParameter(RenderBuffer);
            renderer.UpdateGlobalVariables(RenderContext, Viewport.Renderables, ref renderParameter);
            renderer.RenderScene(RenderContext, pendingRenderCores, ref renderParameter);
        }

        /// <summary>
        /// Creates the render parameter.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns></returns>
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

        /// <summary>
        /// <see cref="DX11RenderHostBase.PostRender"/>
        /// </summary>
        protected override void PostRender()
        {
            asyncTask?.Wait();
        }

        /// <summary>
        /// Called when [render2 d].
        /// </summary>
        /// <param name="time">The time.</param>
        protected override void OnRender2D(TimeSpan time)
        {
            
        }
    }
}
