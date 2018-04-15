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
using System.Diagnostics;

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
        protected readonly List<KeyValuePair<int, SceneNode>> perFrameFlattenedScene = new List<KeyValuePair<int, SceneNode>>();
        /// <summary>
        /// The light renderables
        /// </summary>
        protected readonly List<SceneNode> lightNodes = new List<SceneNode>();
        /// <summary>
        /// The pending render nodes
        /// </summary>
        protected readonly List<SceneNode> generalNodes = new List<SceneNode>();
        /// <summary>
        /// The pending render nodes
        /// </summary>
        protected readonly List<SceneNode> preProcNodes = new List<SceneNode>();
        /// <summary>
        /// The pending render nodes
        /// </summary>
        protected readonly List<SceneNode> postProcNodes = new List<SceneNode>();
        /// <summary>
        /// The render nodes for post render
        /// </summary>
        protected readonly List<SceneNode> nodesForPostRender = new List<SceneNode>();
        /// <summary>
        /// The pending render nodes
        /// </summary>
        protected readonly List<SceneNode> screenSpacedNodes = new List<SceneNode>();

        /// <summary>
        /// The viewport renderable2D
        /// </summary>
        protected readonly List<SceneNode2D> viewportRenderable2D = new List<SceneNode2D>();

        /// <summary>
        /// Gets the current frame flattened scene graph. KeyValuePair.Key is the depth of the node.
        /// </summary>
        /// <value>
        /// Gets the current frame flattened scene graph
        /// </value>
        public override List<KeyValuePair<int, SceneNode>> PerFrameFlattenedScene { get { return perFrameFlattenedScene; } }
        /// <summary>
        /// Gets the per frame lights.
        /// </summary>
        /// <value>
        /// The per frame lights.
        /// </value>
        public override IEnumerable<LightNode> PerFrameLights
        {
            get { return lightNodes.Select(x=>x as LightNode); }
        }
        /// <summary>
        /// Gets the per frame render cores for normal rendering routine. <see cref="RenderType.Opaque"/>, <see cref="RenderType.Transparent"/>, <see cref="RenderType.Particle"/>
        /// <para>This does not include <see cref="RenderType.PreProc"/>, <see cref="RenderType.PostProc"/>, <see cref="RenderType.Light"/>, <see cref="RenderType.ScreenSpaced"/></para>
        /// </summary>
        public override List<SceneNode> PerFrameGeneralNodes { get { return generalNodes; } }
        /// <summary>
        /// Gets the per frame post effects cores. It is the subset of <see cref="PerFrameGeneralNodes"/>
        /// </summary>
        /// <value>
        /// The per frame post effects cores.
        /// </value>
        public override List<SceneNode> PerFrameNodesWithPostEffect
        {
            get { return nodesForPostRender; }
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
        private void SeparateRenderables(IRenderContext context)
        {
            Clear(UpdateSceneGraphRequested);
            if (UpdateSceneGraphRequested)
            {
                viewportRenderables.AddRange(Viewport.Renderables);
                renderer.UpdateSceneGraph(RenderContext, viewportRenderables, perFrameFlattenedScene);
#if DEBUG
                Debug.WriteLine("Flatten Scene Graph");
#endif
            }
            var frustum = context.BoundingFrustum;
            for(int i = 0; i < perFrameFlattenedScene.Count;)
            {
                var renderable = perFrameFlattenedScene[i];
                renderable.Value.Update(context);

                if (!renderable.Value.IsRenderable)//Skip scene graph depth larger than current node
                {
                    int depth = renderable.Key;
                    ++i;
                    for(; i <perFrameFlattenedScene.Count; ++i)
                    {
                        if(perFrameFlattenedScene[i].Key <= depth)
                        {
                            break;
                        }
                        i += perFrameFlattenedScene[i].Value.Items.Count;
                    }
                    continue;
                }

                switch (renderable.Value.RenderType)
                {
                    case RenderType.Light:
                        lightNodes.Add(renderable.Value);
                        break;
                    case RenderType.Opaque:
                    case RenderType.Transparent:
                    case RenderType.Particle:
                        if (context.EnableBoundingFrustum && !renderable.Value.TestViewFrustum(ref frustum))
                        {
                            break;
                        }
                        generalNodes.Add(renderable.Value);
                        if(renderable.Value.RenderCore.NeedUpdate) // Run update function at the beginning of actual rendering.
                        {
                            renderable.Value.RenderCore.Update(RenderContext, renderer.ImmediateContext);
                        }
                        break;
                    case RenderType.PreProc:
                        preProcNodes.Add(renderable.Value);
                        break;
                    case RenderType.PostProc:
                        postProcNodes.Add(renderable.Value);
                        break;
                    case RenderType.ScreenSpaced:
                        screenSpacedNodes.Add(renderable.Value);
                        break;
                }
                ++i;
            }
            //Get RenderCores with post effect specified.
            if(postProcNodes.Count > 0)
            {
                if(generalNodes.Count > 50)
                {
                    getPostEffectCoreTask = Task.Run(() =>
                    {
                        for(int i = 0; i < generalNodes.Count; ++i)
                        {
                            if (generalNodes[i].HasAnyPostEffect)
                            {
                                nodesForPostRender.Add(generalNodes[i]);
                            }
                        }
                    });
                }
                else
                {
                    for (int i = 0; i < generalNodes.Count; ++i)
                    {
                        if (generalNodes[i].HasAnyPostEffect)
                        {
                            nodesForPostRender.Add(generalNodes[i]);
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

            SeparateRenderables(RenderContext);

            asyncTask = Task.Factory.StartNew(() =>
            {
                renderer?.UpdateNotRenderParallel(RenderContext, perFrameFlattenedScene);
            });
            if ((ShowRenderDetail & RenderDetail.TriangleInfo) == RenderDetail.TriangleInfo)
            {
                getTriangleCountTask = Task.Factory.StartNew(() =>
                {
                    int count = 0;
                    foreach(var core in generalNodes)
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
            renderer.UpdateGlobalVariables(RenderContext, lightNodes, ref renderParameter);
            renderer.RenderPreProc(RenderContext, preProcNodes, ref renderParameter);
            renderer.RenderScene(RenderContext, generalNodes, ref renderParameter);
            getPostEffectCoreTask?.Wait();
            getPostEffectCoreTask = null;
            renderer.RenderPostProc(RenderContext, postProcNodes, ref renderParameter);
            renderer.RenderPostProc(RenderContext, screenSpacedNodes, ref renderParameter);
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
                RenderStatistics.NumModel3D = perFrameFlattenedScene.Count;
                RenderStatistics.NumCore3D = preProcNodes.Count + generalNodes.Count + postProcNodes.Count + screenSpacedNodes.Count;
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
        private void Clear(bool clearFrameRenderables)
        {
            viewportRenderables.Clear();
            if (clearFrameRenderables)
            {
                perFrameFlattenedScene.Clear();
            }
            generalNodes.Clear();
            lightNodes.Clear();
            postProcNodes.Clear();
            preProcNodes.Clear();
            screenSpacedNodes.Clear();
            nodesForPostRender.Clear();
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
            Clear(true);
            base.OnEndingD3D();
        }
    }
}
