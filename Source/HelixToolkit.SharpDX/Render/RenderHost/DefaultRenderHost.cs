using HelixToolkit.Logger;
using HelixToolkit.SharpDX.Core;
using Microsoft.Extensions.Logging;
using SharpDX;
using SharpDX.Direct3D11;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;

namespace HelixToolkit.SharpDX.Render;

/// <summary>
/// 
/// </summary>
public partial class DefaultRenderHost : DX11RenderHostBase
{
    private static readonly ILogger logger = LogManager.Create<DefaultRenderHost>();
    private AsyncActionWaitable? asyncTask;
    private AsyncActionWaitable? getTriangleCountTask;
    private AsyncActionWaitable? getPostEffectCoreTask;
    private Action FrustumTestAction;
    private int numRendered = 0;
    private readonly AsyncActionThread parallelThread = new();
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
    protected override DX11RenderBufferProxyBase? CreateRenderBuffer()
    {
        logger.LogInformation("Creating DX11Texture2DRenderBufferProxy");
        return EffectsManager is null ? null : new DX11Texture2DRenderBufferProxy(EffectsManager);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SeparateRenderables(RenderContext context, bool invalidateSceneGraph, bool invalidatePerFrameRenderables)
    {
        Clear(invalidateSceneGraph, invalidatePerFrameRenderables);
        if (invalidateSceneGraph && Viewport is not null && RenderContext is not null)
        {
            viewportRenderables.AddRange(Viewport.Renderables);
            renderer?.UpdateSceneGraph(RenderContext, viewportRenderables, perFrameFlattenedScene);
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Flatten Scene Graph");
            }
        }
        var sceneCount = perFrameFlattenedScene.Count;
        if (invalidatePerFrameRenderables)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Get PerFrameRenderables");
            }
            var isInScreenSpacedGroup = false;
            var screenSpacedGroupDepth = int.MaxValue;
            for (var i = 0; i < sceneCount;)
            {
                var renderable = perFrameFlattenedScene[i];
                renderable.Value.Update(context);
                var type = renderable.Value.RenderType;
                var depth = renderable.Key;
                if (!renderable.Value.IsRenderable)
                {
                    //Skip scene graph depth larger than current node                         
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
                // Add node into screen spaced array if the node belongs to a screen spaced group.
                if (isInScreenSpacedGroup && depth > screenSpacedGroupDepth)
                {
                    screenSpacedNodes.Add(renderable.Value);
                    continue;
                }
                isInScreenSpacedGroup = false;
                screenSpacedGroupDepth = int.MaxValue;
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
                    case RenderType.PostEffect:
                        postEffectNodes.Add(renderable.Value);
                        break;
                    case RenderType.GlobalEffect:
                        globalEffectNodes.Add(renderable.Value);
                        break;
                    case RenderType.ScreenSpaced:
                        screenSpacedNodes.Add(renderable.Value);
                        isInScreenSpacedGroup = true;
                        screenSpacedGroupDepth = renderable.Key;
                        break;
                }
            }
            if (RenderConfiguration.EnableRenderOrder)
            {
                for (var i = 0; i < preProcNodes.Count; ++i)
                {
                    preProcNodes[i].UpdateRenderOrderKey();
                }
                preProcNodes.Sort();
                for (var i = 0; i < opaqueNodes.Count; ++i)
                {
                    opaqueNodes[i].UpdateRenderOrderKey();
                }
                opaqueNodes.Sort();
                for (var i = 0; i < postEffectNodes.Count; ++i)
                {
                    postEffectNodes[i].UpdateRenderOrderKey();
                }
                postEffectNodes.Sort();
                for (var i = 0; i < particleNodes.Count; ++i)
                {
                    particleNodes[i].UpdateRenderOrderKey();
                }
                particleNodes.Sort();
            }
            SetupFrustumTestFunctions();
        }
        else
        {
            for (var i = 0; i < sceneCount;)
            {
                var renderable = perFrameFlattenedScene[i];
                renderable.Value.Update(context);
                if (!renderable.Value.IsRenderable)
                {
                    //Skip scene graph depth larger than current node
                    var depth = renderable.Key;
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
        parallelThread.Enabled = EnableParallelProcessing;

        if (RenderContext is null)
        {
            return;
        }

        SeparateRenderables(RenderContext, invalidateSceneGraph, invalidatePerFrameRenderables);
        if (invalidateSceneGraph)
        {
            TriggerSceneGraphUpdated();
        }
        asyncTask = parallelThread.EnqueueAction(() =>
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
            getTriangleCountTask = parallelThread.EnqueueAction(() =>
            {
                var count = 0;
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
        if (postEffectNodes.Count > 0)
        {
            if (opaqueNodesInFrustum.Count + transparentNodesInFrustum.Count > 50)
            {
                getPostEffectCoreTask = parallelThread.EnqueueAction(() =>
                {
                    for (var i = 0; i < opaqueNodesInFrustum.Count; ++i)
                    {
                        if (opaqueNodesInFrustum[i].HasAnyPostEffect)
                        {
                            nodesWithPostEffect.Add(opaqueNodesInFrustum[i]);
                        }
                    }
                    for (var i = 0; i < transparentNodesInFrustum.Count; ++i)
                    {
                        if (transparentNodesInFrustum[i].HasAnyPostEffect)
                        {
                            nodesWithPostEffect.Add(transparentNodesInFrustum[i]);
                        }
                    }
                });
            }
            else
            {
                for (var i = 0; i < opaqueNodesInFrustum.Count; ++i)
                {
                    if (opaqueNodesInFrustum[i].HasAnyPostEffect)
                    {
                        nodesWithPostEffect.Add(opaqueNodesInFrustum[i]);
                    }
                }
                for (var i = 0; i < transparentNodesInFrustum.Count; ++i)
                {
                    if (transparentNodesInFrustum[i].HasAnyPostEffect)
                    {
                        nodesWithPostEffect.Add(transparentNodesInFrustum[i]);
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
        if (renderer is null || RenderContext is null || RenderBuffer is null || renderer?.ImmediateContext is null)
        {
            return;
        }

        var renderParameter = new RenderParameter()
        {
            RenderTargetView = new global::SharpDX.Direct3D11.RenderTargetView?[] { RenderTargetBufferView },
            DepthStencilView = DepthStencilBufferView,
            CurrentTargetTexture = RenderBuffer.ColorBuffer?.Resource,
            IsMSAATexture = RenderBuffer.ColorBufferSampleDesc.Count > 1,
            ScissorRegion = new Rectangle(0, 0, RenderBuffer.TargetWidth, RenderBuffer.TargetHeight),
            ViewportRegion = new ViewportF(0, 0, RenderBuffer.TargetWidth, RenderBuffer.TargetHeight),
            RenderLight = RenderConfiguration.RenderLights,
            UpdatePerFrameData = RenderConfiguration.UpdatePerFrameData
        };
        renderer.SetRenderTargets(ref renderParameter);
        renderer.UpdateGlobalVariables(RenderContext, lightNodes, ref renderParameter);
        for (var i = 0; i < needUpdateCores.Count; ++i)
        {
            needUpdateCores[i].Update(RenderContext, renderer.ImmediateContext);
        }
        numRendered += needUpdateCores.Count;
        if (RenderBuffer.HasMSAA)
        {
            numRendered += DoDepthPrepass();
            renderer.SetRenderTargets(ref renderParameter);
        }
        renderer.RenderPreProc(RenderContext, preProcNodes, ref renderParameter);
        numRendered += renderer.RenderOpaque(RenderContext, opaqueNodesInFrustum, ref renderParameter, false);
        numRendered += renderer.RenderOpaque(RenderContext, particleNodes, ref renderParameter, true);
        numRendered += renderer.RenderTransparent(RenderContext, transparentNodesInFrustum, ref renderParameter);

        getPostEffectCoreTask?.Wait();
        RemoveAndDispose(ref getPostEffectCoreTask);
        if (RenderConfiguration.FXAALevel != FXAALevel.None
            || postEffectNodes.Count > 0 || globalEffectNodes.Count > 0)
        {
            renderer.RenderToPingPongBuffer(RenderContext, ref renderParameter);
            renderParameter.IsMSAATexture = false;
            renderParameter.CurrentTargetTexture = RenderBuffer?.FullResPPBuffer?.CurrentTexture;
            renderParameter.RenderTargetView[0] = RenderBuffer?.FullResPPBuffer?.CurrentRTV;
        }
        if (postEffectNodes.Count > 0)
        {
            renderer.RenderPostProc(RenderContext, postEffectNodes, ref renderParameter);
            renderParameter.CurrentTargetTexture = RenderBuffer?.FullResPPBuffer?.CurrentTexture;
            renderParameter.RenderTargetView[0] = RenderBuffer?.FullResPPBuffer?.CurrentRTV;
        }
        if (globalEffectNodes.Count > 0)
        {
            renderer.RenderPostProc(RenderContext, globalEffectNodes, ref renderParameter);
            renderParameter.CurrentTargetTexture = RenderBuffer?.FullResPPBuffer?.CurrentTexture;
            renderParameter.RenderTargetView[0] = RenderBuffer?.FullResPPBuffer?.CurrentRTV;
        }
        if (screenSpacedNodes.Count > 0)
        {
            var start = 0;
            while (start < screenSpacedNodes.Count)
            {
                if (screenSpacedNodes[start].AffectsGlobalVariable)
                {
                    nodesWithPostEffect.Clear();
                    var i = start + 1;
                    for (; i < screenSpacedNodes.Count; ++i)
                    {
                        if (screenSpacedNodes[i].AffectsGlobalVariable)
                        {
                            break;
                        }
                        if (screenSpacedNodes[i].HasAnyPostEffect)
                        {
                            nodesWithPostEffect.Add(screenSpacedNodes[i]);
                        }
                    }
                    renderer.RenderScreenSpaced(RenderContext, screenSpacedNodes, start, i - start, ref renderParameter);
                    renderer.RenderPostProc(RenderContext, postEffectNodes, ref renderParameter);
                    RenderContext.RestoreGlobalTransform();
                    start = i;
                }
                else
                {
                    ++start;
                }
            }
        }
        renderer.RenderToBackBuffer(RenderContext, ref renderParameter);
        numRendered += preProcNodes.Count + postEffectNodes.Count + screenSpacedNodes.Count;
        if (ShowRenderDetail != RenderDetail.None)
        {
            getTriangleCountTask?.Wait();
            renderStatistics.NumModel3D = perFrameFlattenedScene.Count;
            renderStatistics.NumCore3D = numRendered;
        }
    }

    private int DoDepthPrepass()
    {
        if (renderer is null || RenderContext is null || renderer.ImmediateContext is null)
        {
            return 0;
        }

        renderer.ImmediateContext.ClearDepthStencilView(RenderBuffer?.DepthStencilBufferNoMSAA,
            DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil);
        renderer.ImmediateContext.SetRenderTarget(RenderBuffer?.DepthStencilBufferNoMSAA, null);
        RenderContext.CustomPassName = DefaultPassNames.DepthPrepass;
        for (var i = 0; i < PerFrameOpaqueNodesInFrustum.Count; ++i)
        {
            PerFrameOpaqueNodesInFrustum[i].RenderDepth(RenderContext, renderer.ImmediateContext, null);
        }
        return PerFrameOpaqueNodesInFrustum.Count;
    }

    /// <summary>
    /// <see cref="DX11RenderHostBase.PostRender"/>
    /// </summary>
    protected override void PostRender()
    {
        asyncTask?.Wait();
        RemoveAndDispose(ref asyncTask);
        getTriangleCountTask?.Wait();
        RemoveAndDispose(ref getTriangleCountTask);
    }

    /// <summary>
    /// Called when [render2 d].
    /// </summary>
    /// <param name="time">The time.</param>
    protected override void OnRender2D(TimeSpan time)
    {
        if (Viewport is null || renderer is null)
        {
            return;
        }

        viewportRenderable2D.Clear();
        var d2dRoot = Viewport.D2DRenderables.FirstOrDefault();
        var renderD2D = false;
        if (d2dRoot != null && d2dRoot.ItemsInternal.Count > 0 && RenderConfiguration.RenderD2D)
        {
            renderD2D = true;
            d2dRoot.Measure(new Size2F((float)ActualWidth, (float)ActualHeight));
            d2dRoot.Arrange(new RectangleF(0, 0, (float)ActualWidth, (float)ActualHeight));
        }
        if (!renderD2D || RenderContext2D is null || D2DTarget?.D2DTarget is null)
        {
            return;
        }
        viewportRenderable2D.AddRange(Viewport.D2DRenderables);
        renderer.UpdateSceneGraph2D(RenderContext2D, viewportRenderable2D);

        for (var i = 0; i < viewportRenderable2D.Count; ++i)
        {
            viewportRenderable2D[i].Render(RenderContext2D);
        }
        //Draw bitmap cache to render target
        RenderContext2D.PushRenderTarget(D2DTarget.D2DTarget, false);
        if (renderD2D || ShowRenderDetail != RenderDetail.None)
        {
            for (var i = 0; i < viewportRenderable2D.Count; ++i)
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
        var fastClear = !clearFrameRenderables;
        viewportRenderables.Clear(fastClear);
        needUpdateCores.Clear(fastClear);
        nodesWithPostEffect.Clear(fastClear);
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
            postEffectNodes.Clear(fastClear);
            globalEffectNodes.Clear(fastClear);
            preProcNodes.Clear(fastClear);
            screenSpacedNodes.Clear(fastClear);
        }
    }

    protected override void OnStartD3D()
    {
        base.OnStartD3D();

        // if !WINUI
        parallelThread.Start();
    }

    /// <summary>
    /// Called when [ending d3 d].
    /// </summary>
    protected override void OnEndingD3D()
    {
        logger.LogInformation("On Ending D3D");
        asyncTask?.Wait();
        getTriangleCountTask?.Wait();
        getPostEffectCoreTask?.Wait();
        RemoveAndDispose(ref asyncTask);
        RemoveAndDispose(ref getTriangleCountTask);
        RemoveAndDispose(ref getPostEffectCoreTask);

        // if !WINUI
        parallelThread.Stop();

        Clear(true, true);
        base.OnEndingD3D();
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        parallelThread.Dispose();
        base.OnDispose(disposeManagedResources);
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
            FrustumTestAction = FrustumTestDefault;
        }
    }

    private void NoFrustumTest()
    {
        opaqueNodesInFrustum.AddAll(opaqueNodes);
        transparentNodesInFrustum.AddAll(transparentNodes);
    }

    private void FrustumTestDefault()
    {
        if (renderContext is null)
        {

            return;
        }

        var frustum = renderContext.BoundingFrustum;
        for (var i = 0; i < opaqueNodes.Count; ++i)
        {
            opaqueNodes.Items[i].IsInFrustum = opaqueNodes.Items[i].TestViewFrustum(ref frustum);
            if (opaqueNodes.Items[i].IsInFrustum)
            {
                opaqueNodesInFrustum.Add(opaqueNodes.Items[i]);
            }
        }
        for (var i = 0; i < transparentNodes.Count; ++i)
        {
            transparentNodes.Items[i].IsInFrustum = transparentNodes.Items[i].TestViewFrustum(ref frustum);
            if (transparentNodes.Items[i].IsInFrustum)
            {
                transparentNodesInFrustum.Add(transparentNodes.Items[i]);
            }
        }
    }
    #endregion
}
