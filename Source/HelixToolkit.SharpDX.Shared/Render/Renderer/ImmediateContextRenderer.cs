/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

//#define OLD
using System;
using System.Collections.Generic;
using SharpDX.DXGI;
#if DX11_1
using Device = global::SharpDX.Direct3D11.Device1;
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
        using Model.Scene;
        using Model.Scene2D;

        /// <summary>
        /// 
        /// </summary>
        public class ImmediateContextRenderer : DisposeObject, IRenderer
        {
            private readonly Stack<KeyValuePair<int, IList<SceneNode>>> stackCache1 = new Stack<KeyValuePair<int, IList<SceneNode>>>(20);
            private readonly Stack<KeyValuePair<int, IList<SceneNode2D>>> stack2DCache1 = new Stack<KeyValuePair<int, IList<SceneNode2D>>>(20);
            private OrderIndependentTransparentRenderCore transparentRenderCore;
            private PostEffectFXAA postFXAACore;
            private SSAOCore preSSAOCore;
            private DeviceContextProxy immediateContext;
            /// <summary>
            /// Gets or sets the immediate context.
            /// </summary>
            /// <value>
            /// The immediate context.
            /// </value>
            public DeviceContextProxy ImmediateContext => immediateContext;

            /// <summary>
            /// Initializes a new instance of the <see cref="ImmediateContextRenderer"/> class.
            /// </summary>
            /// <param name="deviceResource">The deviceResource.</param>
            public ImmediateContextRenderer(IDevice3DResources deviceResource)
            {
#if DX11_1
                immediateContext = new DeviceContextProxy(deviceResource.Device.ImmediateContext1, deviceResource.Device);
#else
                immediateContext = new DeviceContextProxy(deviceResource.Device.ImmediateContext, deviceResource.Device);
#endif
                transparentRenderCore = new OrderIndependentTransparentRenderCore();
                postFXAACore = new PostEffectFXAA();
                preSSAOCore = new SSAOCore();
            }

            private static readonly Func<SceneNode, RenderContext, bool> updateFunc = (x, context) =>
            {
                return true;
            };
            /// <summary>
            /// Updates the scene graph.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="renderables">The renderables.</param>
            /// <param name="results">Returns list of flattened scene graph with depth index as KeyValuePair.Key</param>
            /// <returns></returns>
            public virtual void UpdateSceneGraph(RenderContext context, FastList<SceneNode> renderables, FastList<KeyValuePair<int, SceneNode>> results)
            {
                renderables.PreorderDFT(context, updateFunc, results, stackCache1);
            }

            /// <summary>
            /// Updates the scene graph.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="renderables">The renderables.</param>
            /// <returns></returns>
            public void UpdateSceneGraph2D(RenderContext2D context, FastList<SceneNode2D> renderables)
            {
                renderables.PreorderDFTRun((x) =>
                {
                    x.Update(context);
                    return x.IsRenderable;
                }, stack2DCache1);
            }
            /// <summary>
            /// Updates the global variables. Such as light buffer and global transformation buffer
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="lights">The lights.</param>
            /// <param name="parameter">The parameter.</param>
            public virtual void UpdateGlobalVariables(RenderContext context, FastList<SceneNode> lights, ref RenderParameter parameter)
            {
                ImmediateContext.Reset();
                if (parameter.RenderLight)
                {
                    context.LightScene.LightModels.ResetLightCount();
                    var count = lights.Count;
                    for (var i = 0; i < count && i < Constants.MaxLights; ++i)
                    {
                        lights[i].Render(context, ImmediateContext);
                    }
                }
                if (parameter.UpdatePerFrameData)
                {
                    context.UpdatePerFrameData(ImmediateContext);
                }
            }

            /// <summary>
            /// Renders the scene.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="renderables">The renderables.</param>
            /// <param name="parameter">The parameter.</param>
            /// <param name="testFrustum"></param>
            /// <returns>Number of node has been rendered</returns>
            public virtual int RenderOpaque(RenderContext context, FastList<SceneNode> renderables,
                ref RenderParameter parameter, bool testFrustum)
            {
                var renderedCount = 0;
                var count = renderables.Count;
                var frustum = context.BoundingFrustum;
                if (!testFrustum)
                {
                    for (var i = 0; i < count; ++i)
                    {
                        renderables[i].Render(context, ImmediateContext);
                        ++renderedCount;
                    }
                }
                else
                {
                    for (var i = 0; i < count; ++i)
                    {
                        if (!renderables[i].TestViewFrustum(ref frustum))
                        {
                            continue;
                        }
                        renderables[i].Render(context, ImmediateContext);
                        ++renderedCount;
                    }
                }
                return renderedCount;
            }

            /// <summary>
            /// Renders the transparent.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="renderables">The renderables.</param>
            /// <param name="parameter">The parameter.</param>
            /// <returns></returns>
            public virtual int RenderTransparent(RenderContext context, FastList<SceneNode> renderables, ref RenderParameter parameter)
            {
                if (context.RenderHost.RenderConfiguration.EnableOITRendering
                    && context.RenderHost.FeatureLevel >= global::SharpDX.Direct3D.FeatureLevel.Level_11_0)
                {
                    transparentRenderCore.ExternRenderParameter = parameter;
                    transparentRenderCore.Render(context, ImmediateContext);
                    return transparentRenderCore.RenderCount;
                }
                else
                {
                    var renderedCount = 0;
                    var frustum = context.BoundingFrustum;
                    var count = renderables.Count;
                    for (var i = 0; i < count; ++i)
                    {
                        renderables[i].Render(context, ImmediateContext);
                        ++renderedCount;
                    }
                    return renderedCount;
                }
            }
            /// <summary>
            /// Updates the no render parallel. <see cref="IRenderer.UpdateNotRenderParallel(RenderContext, FastList{KeyValuePair{int, SceneNode}})"/>
            /// </summary>
            /// <param name="renderables">The renderables.</param>
            /// <param name="context"></param>
            /// <returns></returns>
            public virtual void UpdateNotRenderParallel(RenderContext context, FastList<KeyValuePair<int, SceneNode>> renderables)
            {
                var count = renderables.Count;
                for (var i = 0; i < count; ++i)
                {
                    renderables[i].Value.UpdateNotRender(context);
                }
            }

            /// <summary>
            /// Sets the render targets.
            /// </summary>
            /// <param name="parameter">The parameter.</param>
            public void SetRenderTargets(ref RenderParameter parameter)
            {
                ImmediateContext.SetRenderTargets(parameter.DepthStencilView, parameter.RenderTargetView);
                ImmediateContext.SetViewport(ref parameter.ViewportRegion);
                ImmediateContext.SetScissorRectangle(parameter.ScissorRegion.Left, parameter.ScissorRegion.Top,
                    parameter.ScissorRegion.Right, parameter.ScissorRegion.Bottom);
            }

            /// <summary>
            /// Render2s the d.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="renderables">The renderables.</param>
            /// <param name="parameter">The parameter.</param>
            public virtual void RenderScene2D(RenderContext2D context, FastList<SceneNode2D> renderables, ref RenderParameter2D parameter)
            {
                var count = renderables.Count;
                for (var i = 0; i < count; ++i)
                {
                    renderables[i].Render(context);
                }
            }

            /// <summary>
            /// Renders the pre proc.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="renderables">The renderables.</param>
            /// <param name="parameter">The parameter.</param>
            public virtual void RenderPreProc(RenderContext context, FastList<SceneNode> renderables, ref RenderParameter parameter)
            {
                var count = renderables.Count;
                for (var i = 0; i < count; ++i)
                {
                    renderables[i].Render(context, ImmediateContext);
                }
                if (context.SSAOEnabled)
                {
                    preSSAOCore.Radius = context.RenderHost.RenderConfiguration.SSAORadius;
                    preSSAOCore.Quality = context.RenderHost.RenderConfiguration.SSAOQuality;
                    preSSAOCore.Render(context, ImmediateContext);
                }
            }

            /// <summary>
            /// Renders the post proc.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="renderables">The renderables.</param>
            /// <param name="parameter">The parameter.</param>
            public virtual void RenderPostProc(RenderContext context, FastList<SceneNode> renderables, ref RenderParameter parameter)
            {
                var count = renderables.Count;
                for (var i = 0; i < count; ++i)
                {
                    renderables[i].Render(context, ImmediateContext);
                }
            }

            /// <summary>
            /// Renders the screenspaced node's post proc.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="screenSpacedWithPostEffects"></param>
            /// <param name="nodesWithPostEffects"></param>
            /// <param name="postProcNodes"></param>
            /// <param name="parameter">The parameter.</param>
            public virtual void RenderScreenSpacedPostProc(RenderContext context,
                FastList<SceneNode> screenSpacedWithPostEffects,
                FastList<SceneNode> nodesWithPostEffects,
                FastList<SceneNode> postProcNodes, ref RenderParameter parameter)
            {
                var i = 0;
                while (i < screenSpacedWithPostEffects.Count)
                {
                    if (screenSpacedWithPostEffects[i].AffectsGlobalVariable)
                    {
                        context.RestoreGlobalTransform();
                        screenSpacedWithPostEffects[i].Render(context, ImmediateContext);
                        nodesWithPostEffects.Clear();
                        while (++i < screenSpacedWithPostEffects.Count
                            && !screenSpacedWithPostEffects[i].AffectsGlobalVariable)
                        {
                            nodesWithPostEffects.Add(screenSpacedWithPostEffects[i]);
                        }
                        RenderPostProc(context, postProcNodes, ref parameter);
                        continue;
                    }
                    ++i;
                }
            }

            /// <summary>
            /// Renders to ping pong buffer.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="parameter">The parameter.</param>
            public virtual void RenderToPingPongBuffer(RenderContext context, ref RenderParameter parameter)
            {
                var buffer = context.RenderHost.RenderBuffer;
                buffer.FullResPPBuffer.Initialize();
                if (parameter.IsMSAATexture)
                {
                    ImmediateContext.ResolveSubresource(parameter.CurrentTargetTexture, 0, buffer.FullResPPBuffer.CurrentTexture, 0, buffer.Format);
                }
                else
                {
                    ImmediateContext.CopyResource(parameter.CurrentTargetTexture, buffer.FullResPPBuffer.CurrentTexture);
                }
            }
            /// <summary>
            /// Renders the screen spaced.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="renderables">The renderables.</param>
            /// <param name="start">Start index in renderables</param>
            /// <param name="count">Number of renderables to render.</param>
            /// <param name="parameter">The parameter.</param>
            public virtual void RenderScreenSpaced(RenderContext context, FastList<SceneNode> renderables, int start, int count,
                ref RenderParameter parameter)
            {
                if (count > 0)
                {
                    var hasMSAA = context.RenderHost.RenderBuffer.ColorBufferSampleDesc.Count == 1;
                    var buffer = context.RenderHost.RenderBuffer;
                    var depthStencilBuffer = hasMSAA ? buffer.DepthStencilBuffer : context.GetOffScreenDS(OffScreenTextureSize.Full, Format.D32_Float_S8X24_UInt);
                    ImmediateContext.SetRenderTargets(depthStencilBuffer, parameter.RenderTargetView);

                    for (var i = start; i < start + count; ++i)
                    {
                        renderables[i].Render(context, ImmediateContext);
                    }
                    if (!hasMSAA)
                    {
                        depthStencilBuffer.Dispose();
                    }
                }
            }
            /// <summary>
            /// Renders to back buffer.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="parameter">The parameter.</param>
            public virtual void RenderToBackBuffer(RenderContext context, ref RenderParameter parameter)
            {
                if (context.RenderHost.FeatureLevel >= global::SharpDX.Direct3D.FeatureLevel.Level_11_0
                    && context.RenderHost.RenderConfiguration.FXAALevel != FXAALevel.None)
                {
                    postFXAACore.FXAALevel = context.RenderHost.RenderConfiguration.FXAALevel;
                    postFXAACore.Render(context, ImmediateContext);
                }
                ImmediateContext.Flush();
                var buffer = context.RenderHost.RenderBuffer;
                if (parameter.IsMSAATexture)
                {
                    ImmediateContext.ResolveSubresource(parameter.CurrentTargetTexture, 0, buffer.BackBuffer.Resource, 0, buffer.Format);
                }
                else
                {
                    ImmediateContext.CopyResource(parameter.CurrentTargetTexture, buffer.BackBuffer.Resource);
                }
            }

            protected override void OnDispose(bool disposeManagedResources)
            {
                Detach();
                RemoveAndDispose(ref immediateContext);
                RemoveAndDispose(ref transparentRenderCore);
                RemoveAndDispose(ref postFXAACore);
                RemoveAndDispose(ref preSSAOCore);
                base.OnDispose(disposeManagedResources);
            }

            public void Attach(IRenderHost host)
            {
                if (host.FeatureLevel >= global::SharpDX.Direct3D.FeatureLevel.Level_11_0)
                {
                    transparentRenderCore.Attach(host.EffectsManager.GetTechnique(DefaultRenderTechniqueNames.MeshOITQuad));
                    postFXAACore.Attach(host.EffectsManager.GetTechnique(DefaultRenderTechniqueNames.PostEffectFXAA));
                    preSSAOCore.Attach(host.EffectsManager.GetTechnique(DefaultRenderTechniqueNames.SSAO));
                }
            }

            public void Detach()
            {
                stackCache1.Clear();
                stack2DCache1.Clear();
                transparentRenderCore.Detach();
                postFXAACore.Detach();
                preSSAOCore.Detach();
            }
        }
    }
}
