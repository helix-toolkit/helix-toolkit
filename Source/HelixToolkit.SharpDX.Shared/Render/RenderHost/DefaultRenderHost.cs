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
    using HelixToolkit.Logger;
    using System.Runtime.CompilerServices;

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


        private Task asyncTask;
        private Task getTriangleCountTask;
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
            Logger.Log(LogLevel.Information, "DX11Texture2DRenderBufferProxy", nameof(DefaultRenderHost));
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
                renderer?.UpdateNotRenderParallel(pendingRenderables);
            });
            if ((ShowRenderDetail & RenderDetail.TriangleInfo) == RenderDetail.TriangleInfo)
            {
                getTriangleCountTask = Task.Factory.StartNew(() =>
                {
                    int count = 0;
                    foreach(var core in pendingRenderCores)
                    {
                        if (core is IGeometryRenderCore c)
                        {
                            if(c.GeometryBuffer != null && c.GeometryBuffer.Geometry != null && c.GeometryBuffer.Geometry.Indices != null)
                                count += c.GeometryBuffer.Geometry.Indices.Count / 3;
                        }
                    }
                    RenderStatistics.NumTriangles = count;
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
                RenderTargetView = RenderTargetBufferView,
                DepthStencilView = DepthStencilBufferView,
                ScissorRegion = new Rectangle(0, 0, RenderBuffer.TargetWidth, RenderBuffer.TargetHeight),
                ViewportRegion = new ViewportF(0, 0, RenderBuffer.TargetWidth, RenderBuffer.TargetHeight),
                RenderLight = RenderConfiguration.RenderLights,
                UpdatePerFrameData = RenderConfiguration.UpdatePerFrameData
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
            asyncTask = null;
            getTriangleCountTask?.Wait();
            getTriangleCountTask = null;
        }

        /// <summary>
        /// Called when [render2 d].
        /// </summary>
        /// <param name="time">The time.</param>
        protected override void OnRender2D(TimeSpan time)
        {
            var d2dRoot = Viewport.D2DRenderables.FirstOrDefault();
            bool renderD2D = false;
            if (d2dRoot != null && d2dRoot.Items.Count() > 0 && RenderConfiguration.RenderD2D)
            {
                renderD2D = true;
                d2dRoot.Measure(new Size2F((float)ActualWidth, (float)ActualHeight));
                d2dRoot.Arrange(new RectangleF(0, 0, (float)ActualWidth, (float)ActualHeight));
            }                
            if(!renderD2D && ShowRenderDetail == RenderDetail.None)
            {
                return;
            }
            renderer.UpdateSceneGraph2D(RenderContext2D, Viewport.D2DRenderables);      
            if (ShowRenderDetail != RenderDetail.None)
            {
                getTriangleCountTask?.Wait();
                RenderStatistics.NumModel3D = pendingRenderables.Count;
                RenderStatistics.NumCore3D = pendingRenderCores.Count;
            }
            foreach (var item in Viewport.D2DRenderables)
            {
                item.Render(RenderContext2D);
            }   
            //Draw bitmap cache to render target
            RenderContext2D.PushRenderTarget(D2DTarget.D2DTarget, false);
            if (renderD2D || ShowRenderDetail != RenderDetail.None)
            {
                foreach (var item in Viewport.D2DRenderables)
                {
                    item.RenderBitmapCache(RenderContext2D);
                }
            }
            RenderContext2D.PopRenderTarget();
        }

        /// <summary>
        /// Called when [ending d3 d].
        /// </summary>
        protected override void OnEndingD3D()
        {
            Logger.Log(LogLevel.Information, "", nameof(DefaultRenderHost));
            asyncTask?.Wait();
            getTriangleCountTask?.Wait();
            base.OnEndingD3D();
        }
    }
}
