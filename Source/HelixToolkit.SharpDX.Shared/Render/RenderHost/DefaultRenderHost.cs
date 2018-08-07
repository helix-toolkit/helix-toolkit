/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#else
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
        protected readonly List<SceneNode> opaqueNodes = new List<SceneNode>();
        /// <summary>
        /// The transparent nodes
        /// </summary>
        protected readonly List<SceneNode> transparentNodes = new List<SceneNode>();
        /// <summary>
        /// The particle nodes
        /// </summary>
        protected readonly List<SceneNode> particleNodes = new List<SceneNode>();
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
        /// The need update cores
        /// </summary>
        private readonly List<RenderCore> needUpdateCores = new List<RenderCore>();

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
            get { return lightNodes.Select(x => x as LightNode); }
        }
        /// <summary>
        /// Gets the per frame nodes for opaque rendering. <see cref="RenderType.Opaque"/>
        /// <para>This does not include <see cref="RenderType.Transparent"/>, <see cref="RenderType.Particle"/>, <see cref="RenderType.PreProc"/>, <see cref="RenderType.PostProc"/>, <see cref="RenderType.Light"/>, <see cref="RenderType.ScreenSpaced"/></para>
        /// </summary>
        public override List<SceneNode> PerFrameOpaqueNodes { get { return opaqueNodes; } }
        /// <summary>
        /// Gets the per frame transparent nodes. , <see cref="RenderType.Transparent"/>, <see cref="RenderType.Particle"/>
        /// <para>This does not include <see cref="RenderType.Opaque"/>, <see cref="RenderType.PreProc"/>, <see cref="RenderType.PostProc"/>, <see cref="RenderType.Light"/>, <see cref="RenderType.ScreenSpaced"/></para>
        /// </summary>
        /// <value>
        /// The per frame transparent nodes.
        /// </value>
        public override List<SceneNode> PerFrameTransparentNodes { get { return transparentNodes; } }
        /// <summary>
        /// Gets the per frame transparent nodes.
        /// </summary>
        /// <value>
        /// The per frame transparent nodes.
        /// </value>
        public override List<SceneNode> PerFrameParticleNodes { get { return particleNodes; } }
        /// <summary>
        /// Gets the per frame post effects cores. It is the subset of <see cref="PerFrameOpaqueNodes"/>
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

        private int numRendered = 0;

        private static readonly Comparison<SceneNode> sortingDelegate = delegate (SceneNode a, SceneNode b) { return a.RenderOrderKey.Key > b.RenderOrderKey.Key ? 1 : a.RenderOrderKey.Key < b.RenderOrderKey.Key ? -1 : 0; };

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderHost"/> class.
        /// </summary>
        public DefaultRenderHost() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderHost"/> class.
        /// </summary>
        /// <param name="createRenderer">The create renderer.</param>
        public DefaultRenderHost(Func<IDevice3DResources, IRenderer> createRenderer) : base(createRenderer)
        {

        }

        /// <summary>
        /// Creates the render buffer.
        /// </summary>
        /// <returns></returns>
        protected override DX11RenderBufferProxyBase CreateRenderBuffer()
        {
            Logger.Log(LogLevel.Information, "DX11Texture2DRenderBufferProxy", nameof(DefaultRenderHost));
            return new DX11Texture2DRenderBufferProxy(EffectsManager);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SeparateRenderables(RenderContext context, bool invalidateSceneGraph, bool invalidatePerFrameRenderables)
        {
            Clear(invalidateSceneGraph, invalidatePerFrameRenderables);
            if (invalidateSceneGraph)
            {
                viewportRenderables.AddRange(Viewport.Renderables);
                renderer.UpdateSceneGraph(RenderContext, viewportRenderables, perFrameFlattenedScene);
#if DEBUG
                Debug.WriteLine("Flatten Scene Graph");
#endif
            }
            int sceneCount = perFrameFlattenedScene.Count;
            if (invalidatePerFrameRenderables)
            {
#if DEBUG
                Debug.WriteLine("Get PerFrameRenderables");
#endif               
                for (int i = 0; i < sceneCount;)
                {
                    var renderable = perFrameFlattenedScene[i];
                    renderable.Value.Update(context);
                    var type = renderable.Value.RenderType;
                    if (!renderable.Value.IsRenderable)
                    {
                        //Skip scene graph depth larger than current node
                        int depth = renderable.Key;
                        ++i;
                        for (; i < sceneCount; ++i)
                        {
                            if (perFrameFlattenedScene[i].Key <= depth)
                            {
                                break;
                            }
                            i += perFrameFlattenedScene[i].Value.Items.Count;
                        }
                        continue;
                    }
                    if (renderable.Value.RenderCore.NeedUpdate) // Run update function at the beginning of actual rendering.
                    {
                        needUpdateCores.Add(renderable.Value.RenderCore);
                    }
                    ++i;
                    switch (type)
                    {
                        case RenderType.Opaque:
                            opaqueNodes.Add(renderable.Value);
                            break;
                        case RenderType.Light:
                            lightNodes.Add(renderable.Value);
                            break;
                        case RenderType.Transparent:
                            transparentNodes.Add(renderable.Value);
                            break;
                        case RenderType.Particle:
                            particleNodes.Add(renderable.Value);
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
                }
                if (RenderConfiguration.EnableRenderOrder)
                {
                    for (int i = 0; i < preProcNodes.Count; ++i)
                    {
                        preProcNodes[i].UpdateRenderOrderKey();
                    }
                    preProcNodes.Sort(sortingDelegate);
                    for (int i = 0; i < opaqueNodes.Count; ++i)
                    {
                        opaqueNodes[i].UpdateRenderOrderKey();
                    }
                    opaqueNodes.Sort(sortingDelegate);
                    for (int i = 0; i < postProcNodes.Count; ++i)
                    {
                        postProcNodes[i].UpdateRenderOrderKey();
                    }
                    postProcNodes.Sort(sortingDelegate);
                    for (int i = 0; i < particleNodes.Count; ++i)
                    {
                        particleNodes[i].UpdateRenderOrderKey();
                    }
                    particleNodes.Sort(sortingDelegate);
                }
            }
            else
            {
                for (int i = 0; i < sceneCount;)
                {
                    var renderable = perFrameFlattenedScene[i];
                    renderable.Value.Update(context);
                    if (!renderable.Value.IsRenderable)
                    {
                        //Skip scene graph depth larger than current node
                        int depth = renderable.Key;
                        ++i;
                        for (; i < sceneCount; ++i)
                        {
                            if (perFrameFlattenedScene[i].Key <= depth)
                            {
                                break;
                            }
                            i += perFrameFlattenedScene[i].Value.Items.Count;
                        }
                        continue;
                    }
                    if (renderable.Value.RenderCore.NeedUpdate) // Run update function at the beginning of actual rendering.
                    {
                        needUpdateCores.Add(renderable.Value.RenderCore);
                    }
                    ++i;
                }
            }
            //Get RenderCores with post effect specified.
            if (postProcNodes.Count > 0)
            {
                if (opaqueNodes.Count + transparentNodes.Count > 50)
                {
                    getPostEffectCoreTask = Task.Run(() =>
                    {
                        for (int i = 0; i < opaqueNodes.Count; ++i)
                        {
                            if (opaqueNodes[i].HasAnyPostEffect)
                            {
                                nodesForPostRender.Add(opaqueNodes[i]);
                            }
                        }
                        for (int i = 0; i < transparentNodes.Count; ++i)
                        {
                            if (transparentNodes[i].HasAnyPostEffect)
                            {
                                nodesForPostRender.Add(transparentNodes[i]);
                            }
                        }
                    });
                }
                else
                {
                    for (int i = 0; i < opaqueNodes.Count; ++i)
                    {
                        if (opaqueNodes[i].HasAnyPostEffect)
                        {
                            nodesForPostRender.Add(opaqueNodes[i]);
                        }
                    }
                    for (int i = 0; i < transparentNodes.Count; ++i)
                    {
                        if (transparentNodes[i].HasAnyPostEffect)
                        {
                            nodesForPostRender.Add(transparentNodes[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// <see cref="DX11RenderHostBase.PreRender"/>
        /// </summary>
        protected override void PreRender(bool invalidateSceneGraph, bool invalidatePerFrameRenderables)
        {
            base.PreRender(invalidateSceneGraph, invalidatePerFrameRenderables);

            SeparateRenderables(RenderContext, invalidateSceneGraph, invalidatePerFrameRenderables);

            asyncTask = Task.Factory.StartNew(() =>
            {
                renderer?.UpdateNotRenderParallel(RenderContext, perFrameFlattenedScene);
            });
            if ((ShowRenderDetail & RenderDetail.TriangleInfo) == RenderDetail.TriangleInfo)
            {
                getTriangleCountTask = Task.Factory.StartNew(() =>
                {
                    int count = 0;
                    foreach (var core in opaqueNodes.Select(x => x.RenderCore))
                    {
                        if (core is IGeometryRenderCore c)
                        {
                            if (c.GeometryBuffer is IGeometryBufferModel geo && geo.Geometry != null && geo.Geometry.Indices != null)
                                count += geo.Geometry.Indices.Count / 3;
                        }
                    }
                    foreach (var core in transparentNodes.Select(x => x.RenderCore))
                    {
                        if (core is IGeometryRenderCore c)
                        {
                            if (c.GeometryBuffer is IGeometryBufferModel geo && geo.Geometry != null && geo.Geometry.Indices != null)
                                count += geo.Geometry.Indices.Count / 3;
                        }
                    }
                    renderStatistics.NumTriangles = count;
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
                RenderTargetView = new global::SharpDX.Direct3D11.RenderTargetView[] { RenderTargetBufferView },
                DepthStencilView = DepthStencilBufferView,
                CurrentTargetTexture = RenderBuffer.ColorBuffer.Resource,
                IsMSAATexture = RenderBuffer.ColorBufferSampleDesc.Count > 1,
                ScissorRegion = new Rectangle(0, 0, RenderBuffer.TargetWidth, RenderBuffer.TargetHeight),
                ViewportRegion = new ViewportF(0, 0, RenderBuffer.TargetWidth, RenderBuffer.TargetHeight),
                RenderLight = RenderConfiguration.RenderLights,
                UpdatePerFrameData = RenderConfiguration.UpdatePerFrameData
            };
            renderer.SetRenderTargets(ref renderParameter);
            renderer.UpdateGlobalVariables(RenderContext, lightNodes, ref renderParameter);
            for (int i = 0; i < needUpdateCores.Count; ++i)
            {
                needUpdateCores[i].Update(RenderContext, renderer.ImmediateContext);
            }
            renderer.RenderPreProc(RenderContext, preProcNodes, ref renderParameter);
            numRendered += renderer.RenderOpaque(RenderContext, opaqueNodes, ref renderParameter);
            numRendered += renderer.RenderOpaque(RenderContext, particleNodes, ref renderParameter);
            numRendered += renderer.RenderTransparent(RenderContext, transparentNodes, ref renderParameter);

            getPostEffectCoreTask?.Wait();
            getPostEffectCoreTask = null;
            if (RenderConfiguration.FXAALevel != FXAALevel.None || postProcNodes.Count > 0)
            {
                renderer.RenderToPingPongBuffer(RenderContext, ref renderParameter);
                renderParameter.IsMSAATexture = false;
                renderer.RenderPostProc(RenderContext, postProcNodes, ref renderParameter);
                renderParameter.CurrentTargetTexture = RenderBuffer.FullResPPBuffer.CurrentTexture;
                renderParameter.RenderTargetView = new global::SharpDX.Direct3D11.RenderTargetView[] { RenderBuffer.FullResPPBuffer.CurrentRTV };
            }

            renderer.RenderScreenSpaced(RenderContext, screenSpacedNodes, ref renderParameter);
            renderer.RenderToBackBuffer(RenderContext, ref renderParameter);
            numRendered += preProcNodes.Count + postProcNodes.Count + screenSpacedNodes.Count;
            if (ShowRenderDetail != RenderDetail.None)
            {
                getTriangleCountTask?.Wait();
                renderStatistics.NumModel3D = perFrameFlattenedScene.Count;
                renderStatistics.NumCore3D = numRendered;
            }
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
            if (!renderD2D)
            {
                return;
            }
            viewportRenderable2D.AddRange(Viewport.D2DRenderables);
            renderer.UpdateSceneGraph2D(RenderContext2D, viewportRenderable2D);

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
        private void Clear(bool clearFrameRenderables, bool clearPerFrameRenderables)
        {
            numRendered = 0;
            viewportRenderables.Clear();
            needUpdateCores.Clear();
            nodesForPostRender.Clear();
            if (clearFrameRenderables)
            {
                perFrameFlattenedScene.Clear();
            }
            if (clearPerFrameRenderables)
            {
                opaqueNodes.Clear();
                transparentNodes.Clear();
                particleNodes.Clear();
                lightNodes.Clear();
                postProcNodes.Clear();
                preProcNodes.Clear();
                screenSpacedNodes.Clear();
            }
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
            Clear(true, true);
            base.OnEndingD3D();
        }
    }
}
