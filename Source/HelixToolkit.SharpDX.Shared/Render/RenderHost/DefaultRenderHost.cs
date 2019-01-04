/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
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
    namespace Render
    {
        using Core;
        using HelixToolkit.Logger;
        using System.Collections.Concurrent;


        /// <summary>
        /// 
        /// </summary>
        public partial class DefaultRenderHost : DX11RenderHostBase
        {
            private Task asyncTask;
            private Task getTriangleCountTask;
            private Task getPostEffectCoreTask;
            private OrderablePartitioner<Tuple<int, int>> opaquePartitioner;
            private OrderablePartitioner<Tuple<int, int>> transparentPartitioner;
            private Action FrustumTestAction;

            private int numRendered = 0;
            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultRenderHost"/> class.
            /// </summary>
            public DefaultRenderHost()
            {
                FrustumTestAction = NoFrustumTest;
                FrustumEnabledChanged += (s, e) => { SetupFrustumTestFunctions(); };
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultRenderHost"/> class.
            /// </summary>
            /// <param name="createRenderer">The create renderer.</param>
            public DefaultRenderHost(Func<IDevice3DResources, IRenderer> createRenderer) : base(createRenderer)
            {
                FrustumTestAction = NoFrustumTest;
                FrustumEnabledChanged += (s, e) => { SetupFrustumTestFunctions(); };
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
                                i += perFrameFlattenedScene[i].Value.ItemsInternal.Count;
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
                        preProcNodes.Sort();
                        for (int i = 0; i < opaqueNodes.Count; ++i)
                        {
                            opaqueNodes[i].UpdateRenderOrderKey();
                        }
                        opaqueNodes.Sort();
                        for (int i = 0; i < postProcNodes.Count; ++i)
                        {
                            postProcNodes[i].UpdateRenderOrderKey();
                        }
                        postProcNodes.Sort();
                        for (int i = 0; i < particleNodes.Count; ++i)
                        {
                            particleNodes[i].UpdateRenderOrderKey();
                        }
                        particleNodes.Sort();
                    }
                    opaquePartitioner = opaqueNodes.Count > 0 ? Partitioner.Create(0, opaqueNodes.Count, FrustumPartitionSize) : null;
                    transparentPartitioner = transparentNodes.Count > 0 ? Partitioner.Create(0, transparentNodes.Count, FrustumPartitionSize) : null;
                    SetupFrustumTestFunctions();
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
                                i += perFrameFlattenedScene[i].Value.ItemsInternal.Count;
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
                var ft = Stopwatch.GetTimestamp();
                FrustumTestAction();
                ft = Stopwatch.GetTimestamp() - ft;
                renderStatistics.FrustumTestTime = (float)ft / Stopwatch.Frequency;
                CollectPostEffectNodes();
                if ((ShowRenderDetail & RenderDetail.TriangleInfo) == RenderDetail.TriangleInfo)
                {
                    getTriangleCountTask = Task.Factory.StartNew(() =>
                    {
                        int count = 0;
                        foreach (var core in opaqueNodesInFrustum.Select(x => x.RenderCore))
                        {
                            if (core is IGeometryRenderCore c)
                            {
                                if (c.GeometryBuffer is IGeometryBufferModel geo && geo.Geometry != null && geo.Geometry.Indices != null)
                                    count += geo.Geometry.Indices.Count / 3;
                            }
                        }
                        foreach (var core in transparentNodesInFrustum.Select(x => x.RenderCore))
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

            private void CollectPostEffectNodes()
            {
                //Get RenderCores with post effect specified.
                if (postProcNodes.Count > 0)
                {
                    if (opaqueNodesInFrustum.Count + transparentNodesInFrustum.Count > 50)
                    {
                        getPostEffectCoreTask = Task.Run(() =>
                        {
                            for (int i = 0; i < opaqueNodesInFrustum.Count; ++i)
                            {
                                if (opaqueNodesInFrustum[i].HasAnyPostEffect)
                                {
                                    nodesForPostRender.Add(opaqueNodesInFrustum[i]);
                                }
                            }
                            for (int i = 0; i < transparentNodesInFrustum.Count; ++i)
                            {
                                if (transparentNodesInFrustum[i].HasAnyPostEffect)
                                {
                                    nodesForPostRender.Add(transparentNodesInFrustum[i]);
                                }
                            }
                        });
                    }
                    else
                    {
                        for (int i = 0; i < opaqueNodesInFrustum.Count; ++i)
                        {
                            if (opaqueNodesInFrustum[i].HasAnyPostEffect)
                            {
                                nodesForPostRender.Add(opaqueNodesInFrustum[i]);
                            }
                        }
                        for (int i = 0; i < transparentNodesInFrustum.Count; ++i)
                        {
                            if (transparentNodesInFrustum[i].HasAnyPostEffect)
                            {
                                nodesForPostRender.Add(transparentNodesInFrustum[i]);
                            }
                        }
                    }
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
                numRendered += renderer.RenderOpaque(RenderContext, opaqueNodesInFrustum, ref renderParameter, false);
                numRendered += renderer.RenderOpaque(RenderContext, particleNodes, ref renderParameter, true);
                numRendered += renderer.RenderTransparent(RenderContext, transparentNodesInFrustum, ref renderParameter);

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
                if (d2dRoot != null && d2dRoot.ItemsInternal.Count > 0 && RenderConfiguration.RenderD2D)
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
                bool fastClear = !clearFrameRenderables;
                viewportRenderables.Clear(fastClear);
                needUpdateCores.Clear(fastClear);
                nodesForPostRender.Clear(fastClear);
                opaqueNodesInFrustum.Clear(fastClear);
                transparentNodesInFrustum.Clear(fastClear);
                if (clearFrameRenderables)
                {
                    perFrameFlattenedScene.Clear();
                }
                if (clearPerFrameRenderables)
                {
                    opaqueNodes.Clear(fastClear);
                    transparentNodes.Clear(fastClear);
                    particleNodes.Clear(fastClear);
                    lightNodes.Clear(fastClear);
                    postProcNodes.Clear(fastClear);
                    preProcNodes.Clear(fastClear);
                    screenSpacedNodes.Clear(fastClear);
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

            #region FrustumTest
            protected void SetupFrustumTestFunctions()
            {
                if (!EnableRenderFrustum)
                {
                    FrustumTestAction = NoFrustumTest;
                }
                else
                {
                    if (opaqueNodes.Count < FrustumPartitionSize && transparentNodes.Count < FrustumPartitionSize)
                    {
                        FrustumTestAction = FrustumTestDefault;
                    }
                    else
                    {
                        FrustumTestAction = FrustumTestParallel;
                    }
                }
            }

            private void NoFrustumTest()
            {
                opaqueNodesInFrustum.AddAll(opaqueNodes);
                transparentNodesInFrustum.AddAll(transparentNodes);
            }

            private void FrustumTestDefault()
            {
                var frustum = renderContext.BoundingFrustum;
                for (int i = 0; i < opaqueNodes.Count; ++i)
                {
                    opaqueNodes.Items[i].IsInFrustum = opaqueNodes.Items[i].TestViewFrustum(ref frustum);
                    if (opaqueNodes.Items[i].IsInFrustum)
                    {
                        opaqueNodesInFrustum.Add(opaqueNodes.Items[i]);
                    }
                }
                for (int i = 0; i < transparentNodes.Count; ++i)
                {
                    transparentNodes.Items[i].IsInFrustum = transparentNodes.Items[i].TestViewFrustum(ref frustum);
                    if (transparentNodes.Items[i].IsInFrustum)
                    {
                        transparentNodesInFrustum.Add(transparentNodes.Items[i]);
                    }
                }
            }

            private void FrustumTestParallel()
            {
                var frustum = renderContext.BoundingFrustum;
                if (opaquePartitioner != null)
                {
                    Parallel.ForEach(opaquePartitioner, (range) =>
                    {
                        for (int i = range.Item1; i < range.Item2; ++i)
                        {
                            opaqueNodes.Items[i].IsInFrustum = opaqueNodes.Items[i].TestViewFrustum(ref frustum);
                        }
                    });
                    for (int i = 0; i < opaqueNodes.Count; ++i)
                    {
                        if (opaqueNodes.Items[i].IsInFrustum)
                        {
                            opaqueNodesInFrustum.Add(opaqueNodes.Items[i]);
                        }
                    }
                }
                if (transparentPartitioner != null)
                {
                    Parallel.ForEach(transparentPartitioner, (range) =>
                    {
                        for (int i = range.Item1; i < range.Item2; ++i)
                        {
                            transparentNodes.Items[i].IsInFrustum = transparentNodes.Items[i].TestViewFrustum(ref frustum);
                        }
                    });
                    for (int i = 0; i < transparentNodes.Count; ++i)
                    {
                        if (transparentNodes.Items[i].IsInFrustum)
                        {
                            transparentNodesInFrustum.Add(transparentNodes.Items[i]);
                        }
                    }
                }        
            }
            #endregion
        }
    }

}
