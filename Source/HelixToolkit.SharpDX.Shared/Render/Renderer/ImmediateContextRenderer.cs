/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

//#define OLD
using System.Collections.Generic;
using SharpDX.DXGI;
#if DX11_1
using Device = global::SharpDX.Direct3D11.Device1;
#endif
#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Core;
    using System;
    using Model.Scene;
    using Model.Scene2D;

    /// <summary>
    /// 
    /// </summary>
    public class ImmediateContextRenderer : DisposeObject, IRenderer
    {
        private readonly Stack<KeyValuePair<int, IList<SceneNode>>> stackCache1 = new Stack<KeyValuePair<int, IList<SceneNode>>>(20);
        private readonly Stack<KeyValuePair<int, IList<SceneNode2D>>> stack2DCache1 = new Stack<KeyValuePair<int, IList<SceneNode2D>>>(20);
        private readonly OrderIndependentTransparentRenderCore transparentRenderCore;
        private readonly PostEffectFXAA postFXAACore;
        /// <summary>
        /// Gets or sets the immediate context.
        /// </summary>
        /// <value>
        /// The immediate context.
        /// </value>
        public DeviceContextProxy ImmediateContext { private set; get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmediateContextRenderer"/> class.
        /// </summary>
        /// <param name="deviceResource">The deviceResource.</param>
        public ImmediateContextRenderer(IDevice3DResources deviceResource)
        {
#if DX11_1
            ImmediateContext = Collect(new DeviceContextProxy(deviceResource.Device.ImmediateContext1, deviceResource.Device));
#else
            ImmediateContext = Collect(new DeviceContextProxy(deviceResource.Device.ImmediateContext, deviceResource.Device));
#endif
            transparentRenderCore = Collect(new OrderIndependentTransparentRenderCore());
            postFXAACore = Collect(new PostEffectFXAA());
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
        public virtual void UpdateSceneGraph(RenderContext context, List<SceneNode> renderables, List<KeyValuePair<int, SceneNode>> results)
        {
            renderables.PreorderDFT(context, updateFunc, results, stackCache1);
        }

        /// <summary>
        /// Updates the scene graph.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="renderables">The renderables.</param>
        /// <returns></returns>
        public void UpdateSceneGraph2D(RenderContext2D context, List<SceneNode2D> renderables)
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
        public virtual void UpdateGlobalVariables(RenderContext context, List<SceneNode> lights, ref RenderParameter parameter)
        {
            ImmediateContext.Reset();
            if (parameter.RenderLight)
            {
                context.LightScene.LightModels.ResetLightCount();
                int count = lights.Count;
                for (int i = 0; i < count && i < Constants.MaxLights; ++i)
                {
                    lights[i].RenderCore.Render(context, ImmediateContext);
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
        /// <returns>Number of node has been rendered</returns>
        public virtual int RenderOpaque(RenderContext context, List<SceneNode> renderables, ref RenderParameter parameter)
        {
            int renderedCount = 0;
            int count = renderables.Count;
            var frustum = context.BoundingFrustum;
            for (int i = 0; i < count; ++i)
            {
                if (context.EnableBoundingFrustum && !renderables[i].TestViewFrustum(ref frustum))
                {
                    continue;
                }
                renderables[i].RenderCore.Render(context, ImmediateContext);
                ++renderedCount;
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
        public virtual int RenderTransparent(RenderContext context, List<SceneNode> renderables, ref RenderParameter parameter)
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
                int renderedCount = 0;
                var frustum = context.BoundingFrustum;
                int count = renderables.Count;
                for (int i = 0; i < count; ++i)
                {
                    if (context.EnableBoundingFrustum && !renderables[i].TestViewFrustum(ref frustum))
                    {
                        continue;
                    }
                    renderables[i].RenderCore.Render(context, ImmediateContext);
                    ++renderedCount;
                }
                return renderedCount;
            }
        }
        /// <summary>
        /// Updates the no render parallel. <see cref="IRenderer.UpdateNotRenderParallel(RenderContext, List{KeyValuePair{int, SceneNode}})"/>
        /// </summary>
        /// <param name="renderables">The renderables.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual void UpdateNotRenderParallel(RenderContext context, List<KeyValuePair<int, SceneNode>> renderables)
        {
            int count = renderables.Count;
            for(int i = 0; i < count; ++i)
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
        public virtual void RenderScene2D(RenderContext2D context, List<SceneNode2D> renderables, ref RenderParameter2D parameter)
        {
            int count = renderables.Count;
            for (int i = 0; i < count; ++ i)
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
        public virtual void RenderPreProc(RenderContext context, List<SceneNode> renderables, ref RenderParameter parameter)
        {
            int count = renderables.Count;
            for (int i = 0; i < count; ++i)
            {
                renderables[i].RenderCore.Render(context, ImmediateContext);
            }
        }

        /// <summary>
        /// Renders the post proc.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="renderables">The renderables.</param>
        /// <param name="parameter">The parameter.</param>
        public virtual void RenderPostProc(RenderContext context, List<SceneNode> renderables, ref RenderParameter parameter)
        {            
            int count = renderables.Count;
            for (int i = 0; i < count; ++i)
            {
                renderables[i].RenderCore.Render(context, ImmediateContext);
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
            if (context.RenderHost.FeatureLevel < global::SharpDX.Direct3D.FeatureLevel.Level_11_0 
                || context.RenderHost.RenderConfiguration.FXAALevel == FXAALevel.None || parameter.IsMSAATexture)
            {
                if (parameter.IsMSAATexture)
                {
                    ImmediateContext.ResolveSubresource(parameter.CurrentTargetTexture, 0, buffer.FullResPPBuffer.CurrentTexture, 0, buffer.Format);
                }
                else
                {
                    ImmediateContext.CopyResource(parameter.CurrentTargetTexture, buffer.FullResPPBuffer.CurrentTexture);                    
                }
            }
            else
            {
                ImmediateContext.CopyResource(parameter.CurrentTargetTexture, buffer.FullResPPBuffer.CurrentTexture);
                postFXAACore.FXAALevel = context.RenderHost.RenderConfiguration.FXAALevel;
                postFXAACore.Render(context, ImmediateContext);
            }
        }
        /// <summary>
        /// Renders the screen spaced.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="renderables">The renderables.</param>
        /// <param name="parameter">The parameter.</param>
        public virtual void RenderScreenSpaced(RenderContext context, List<SceneNode> renderables, ref RenderParameter parameter)
        {
            int count = renderables.Count;
            if(count > 0)
            {
                var buffer = context.RenderHost.RenderBuffer;
                bool useDefault = parameter.RenderTargetView[0] == buffer.ColorBuffer.RenderTargetView;

                var depthStencilBuffer = useDefault ? buffer.DepthStencilBuffer : buffer.FullResDepthStencilPool.Get(Format.D32_Float_S8X24_UInt);
                ImmediateContext.SetRenderTargets(depthStencilBuffer, parameter.RenderTargetView);

                for (int i = 0; i < count; ++i)
                {
                    renderables[i].RenderCore.Render(context, ImmediateContext);
                }
                if (!useDefault)
                {
                    buffer.FullResDepthStencilPool.Put(Format.D32_Float_S8X24_UInt, depthStencilBuffer);
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
            base.OnDispose(disposeManagedResources);
        }

        public void Attach(IRenderHost host)
        {
            if (host.FeatureLevel >= global::SharpDX.Direct3D.FeatureLevel.Level_11_0)
            {
                transparentRenderCore.Attach(host.EffectsManager.GetTechnique(DefaultRenderTechniqueNames.MeshOITQuad));
                postFXAACore.Attach(host.EffectsManager.GetTechnique(DefaultRenderTechniqueNames.PostEffectFXAA));
            }
        }

        public void Detach()
        {
            stackCache1.Clear();
            stack2DCache1.Clear();
            transparentRenderCore.Detach();
            postFXAACore.Detach();
        }
    }

}
