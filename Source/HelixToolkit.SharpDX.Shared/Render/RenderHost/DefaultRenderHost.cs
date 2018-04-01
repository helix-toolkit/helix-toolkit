/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using global::SharpDX.Direct3D11;

#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#else
using Device = SharpDX.Direct3D11.Device;
#endif

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Core;
    using HelixToolkit.Logger;
    using Model.Scene;
    using Model.Scene2D;
    /// <summary>
    /// 
    /// </summary>
    public class DefaultRenderHost : DX11RenderHostBase
    {
        #region Per frame render list
        protected readonly List<SceneNode> viewportRenderables = new List<SceneNode>();
        /// <summary>
        /// The pending renderables
        /// </summary>
        protected readonly List<SceneNode> perFrameRenderables = new List<SceneNode>();
        /// <summary>
        /// The light renderables
        /// </summary>
        protected readonly List<RenderCore> lightRenderables = new List<RenderCore>();
        /// <summary>
        /// The pending render cores
        /// </summary>
        protected readonly List<RenderCore> generalRenderCores = new List<RenderCore>();
        /// <summary>
        /// The pending render cores
        /// </summary>
        protected readonly List<RenderCore> preProcRenderCores = new List<RenderCore>();
        /// <summary>
        /// The pending render cores
        /// </summary>
        protected readonly List<RenderCore> postProcRenderCores = new List<RenderCore>();
        /// <summary>
        /// The render cores for post render
        /// </summary>
        protected readonly List<RenderCore> renderCoresForPostRender = new List<RenderCore>();
        /// <summary>
        /// The pending render cores
        /// </summary>
        protected readonly List<RenderCore> screenSpacedRenderCores = new List<RenderCore>();

        /// <summary>
        /// The viewport renderable2D
        /// </summary>
        protected readonly List<SceneNode2D> viewportRenderable2D = new List<SceneNode2D>();

        /// <summary>
        /// Gets the current frame renderables.
        /// </summary>
        /// <value>
        /// The per frame renderables.
        /// </value>
        public override List<SceneNode> PerFrameRenderables { get { return perFrameRenderables; } }
        /// <summary>
        /// Gets the per frame lights.
        /// </summary>
        /// <value>
        /// The per frame lights.
        /// </value>
        public override IEnumerable<LightCoreBase> PerFrameLights
        {
            get { return lightRenderables.Select(x=>x as LightCoreBase); }
        }
        /// <summary>
        /// Gets the per frame render cores for normal rendering routine. <see cref="RenderType.Opaque"/>, <see cref="RenderType.Transparent"/>, <see cref="RenderType.Particle"/>
        /// <para>This does not include <see cref="RenderType.PreProc"/>, <see cref="RenderType.PostProc"/>, <see cref="RenderType.Light"/>, <see cref="RenderType.ScreenSpaced"/></para>
        /// </summary>
        public override List<RenderCore> PerFrameGeneralRenderCores { get { return generalRenderCores; } }
        /// <summary>
        /// Gets the per frame post effects cores. It is the subset of <see cref="PerFrameGeneralRenderCores"/>
        /// </summary>
        /// <value>
        /// The per frame post effects cores.
        /// </value>
        public override List<RenderCore> PerFrameGeneralCoresWithPostEffect
        {
            get { return renderCoresForPostRender; }
        }
        #endregion

        private Task asyncTask;
        private Task getTriangleCountTask;
        private Task getPostEffectCoreTask;
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SeparateRenderables()
        {
            Clear();
            viewportRenderables.AddRange(Viewport.Renderables);
            renderer.UpdateSceneGraph(RenderContext, viewportRenderables, perFrameRenderables);

            for(int i = 0; i < perFrameRenderables.Count; ++i)
            {
                var renderable = perFrameRenderables[i];
                switch (renderable.RenderCore.RenderType)
                {
                    case RenderType.Light:
                        lightRenderables.Add(renderable.RenderCore);
                        break;
                    case RenderType.Opaque:
                    case RenderType.Transparent:
                    case RenderType.Particle:
                        generalRenderCores.Add(renderable.RenderCore);
                        if(renderable.RenderCore.NeedUpdate) // Run update function at the beginning of actual rendering.
                        {
                            renderable.RenderCore.Update(RenderContext, renderer.ImmediateContext);
                        }
                        break;
                    case RenderType.PreProc:
                        preProcRenderCores.Add(renderable.RenderCore);
                        break;
                    case RenderType.PostProc:
                        postProcRenderCores.Add(renderable.RenderCore);
                        break;
                    case RenderType.ScreenSpaced:
                        screenSpacedRenderCores.Add(renderable.RenderCore);
                        break;
                }
            }
            //Get RenderCores with post effect specified.
            if(postProcRenderCores.Count > 0)
            {
                if(generalRenderCores.Count > 50)
                {
                    getPostEffectCoreTask = Task.Run(() =>
                    {
                        for(int i = 0; i < generalRenderCores.Count; ++i)
                        {
                            if (generalRenderCores[i].HasAnyPostEffect)
                            {
                                renderCoresForPostRender.Add(generalRenderCores[i]);
                            }
                        }
                    });
                }
                else
                {
                    for (int i = 0; i < generalRenderCores.Count; ++i)
                    {
                        if (generalRenderCores[i].HasAnyPostEffect)
                        {
                            renderCoresForPostRender.Add(generalRenderCores[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// <see cref="DX11RenderHostBase.PreRender"/>
        /// </summary>
        protected override void PreRender()
        {
            base.PreRender();

            SeparateRenderables();

            asyncTask = Task.Factory.StartNew(() =>
            {
                renderer?.UpdateNotRenderParallel(RenderContext, perFrameRenderables);
            });
            if ((ShowRenderDetail & RenderDetail.TriangleInfo) == RenderDetail.TriangleInfo)
            {
                getTriangleCountTask = Task.Factory.StartNew(() =>
                {
                    int count = 0;
                    foreach(var core in generalRenderCores)
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
            renderer.SetRenderTargets(ref renderParameter);
            renderer.UpdateGlobalVariables(RenderContext, lightRenderables, ref renderParameter);
            renderer.RenderPreProc(RenderContext, preProcRenderCores, ref renderParameter);
            renderer.RenderScene(RenderContext, generalRenderCores, ref renderParameter);
            getPostEffectCoreTask?.Wait();
            getPostEffectCoreTask = null;
            renderer.RenderPostProc(RenderContext, postProcRenderCores, ref renderParameter);
            renderer.RenderPostProc(RenderContext, screenSpacedRenderCores, ref renderParameter);
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
            viewportRenderable2D.Clear();
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
            viewportRenderable2D.AddRange(Viewport.D2DRenderables);
            renderer.UpdateSceneGraph2D(RenderContext2D, viewportRenderable2D);      
            if (ShowRenderDetail != RenderDetail.None)
            {
                getTriangleCountTask?.Wait();
                RenderStatistics.NumModel3D = perFrameRenderables.Count;
                RenderStatistics.NumCore3D = preProcRenderCores.Count + generalRenderCores.Count + postProcRenderCores.Count + screenSpacedRenderCores.Count;
            }
            for (int i = 0; i < viewportRenderable2D.Count; ++i)
            {
                viewportRenderable2D[i].Render(RenderContext2D);
            }
            //Draw bitmap cache to render target
            RenderContext2D.PushRenderTarget(D2DTarget.D2DTarget, false);
            if (renderD2D || ShowRenderDetail != RenderDetail.None)
            {
                for (int i = 0; i < viewportRenderable2D.Count; ++i)
                {
                    viewportRenderable2D[i].RenderBitmapCache(RenderContext2D);
                }
            }
            RenderContext2D.PopRenderTarget();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Clear()
        {
            viewportRenderables.Clear();
            perFrameRenderables.Clear();
            generalRenderCores.Clear();
            lightRenderables.Clear();
            postProcRenderCores.Clear();
            preProcRenderCores.Clear();
            screenSpacedRenderCores.Clear();
            renderCoresForPostRender.Clear();
        }

        /// <summary>
        /// Called when [ending d3 d].
        /// </summary>
        protected override void OnEndingD3D()
        {
            Logger.Log(LogLevel.Information, "", nameof(DefaultRenderHost));            
            asyncTask?.Wait();
            getTriangleCountTask?.Wait();
            getPostEffectCoreTask?.Wait();
            Clear();
            base.OnEndingD3D();
        }
    }
}
