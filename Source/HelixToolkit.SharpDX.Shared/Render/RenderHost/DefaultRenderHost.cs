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
        /// The pending render cores
        /// </summary>
        protected readonly List<IRenderCore2D> pendingRenderCores2D = new List<IRenderCore2D>();


        private Task asyncTask;

        private Task layoutUpdate2DTask;

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
            return new DX11Texture2DRenderBufferProxy(EffectsManager);
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
            pendingRenderCores.AddRange(pendingRenderables.Select(x => x.RenderCore).Where(x => !x.IsEmpty));
            asyncTask = Task.Factory.StartNew(() =>
            {
                renderer.UpdateNotRenderParallel(pendingRenderables);
            });

            var d2dRoot = Viewport.D2DRenderables.FirstOrDefault();
            if (d2dRoot != null)
            {
                layoutUpdate2DTask = Task.Factory.StartNew(() => 
                {
                    d2dRoot.Measure(new Vector2((float)ActualWidth, (float)ActualHeight));
                    d2dRoot.Arrange(new RectangleF(0, 0, (float)ActualWidth, (float)ActualHeight));
                });
            }
        }

        /// <summary>
        /// <see cref="DX11RenderHostBase.OnRender(TimeSpan)"/>
        /// </summary>
        /// <param name="time">The time.</param>
        protected override void OnRender(TimeSpan time)
        {
            var renderParameter = new RenderParameter()
            {
                RenderTargetView = ColorBufferView,
                DepthStencilView = DepthStencilBufferView,
                ScissorRegion = new Rectangle(0, 0, RenderBuffer.TargetWidth , RenderBuffer.TargetHeight),
                ViewportRegion = new ViewportF(0, 0, RenderBuffer.TargetWidth, RenderBuffer.TargetHeight)
            };
            renderer.UpdateGlobalVariables(RenderContext, Viewport.Renderables, ref renderParameter);
            renderer.RenderScene(RenderContext, pendingRenderCores, ref renderParameter);
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
            layoutUpdate2DTask?.Wait();
            foreach(var item in Viewport.D2DRenderables)
            {
                item.Render(RenderContext2D);
            }
            //pendingRenderCores2D.Clear();
            //pendingRenderCores2D.AddRange(renderer.UpdateSceneGraph2D(RenderContext2D, Viewport.D2DRenderables).Select(x=>x.RenderCore));
            //var renderParameter2D = new RenderParameter2D() { RenderTarget = RenderBuffer.D2DTarget.D2DTarget };
            //renderer.RenderScene2D(RenderContext2D, pendingRenderCores2D, ref renderParameter2D);
        }
    }
}
