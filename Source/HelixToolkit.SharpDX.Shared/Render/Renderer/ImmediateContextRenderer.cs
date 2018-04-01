/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

//#define OLD

using SharpDX.Direct3D11;
using System.Collections.Generic;
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
        protected readonly List<RenderCore> filters = new List<RenderCore>();
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
        /// <param name="device">The device.</param>
        public ImmediateContextRenderer(Device device)
        {
#if DX11_1
            ImmediateContext = Collect(new DeviceContextProxy(device.ImmediateContext1));
#else
            ImmediateContext = Collect(new DeviceContextProxy(device.ImmediateContext));
#endif
        }

        private static readonly Func<SceneNode, IRenderContext, bool> updateFunc = (x, context) =>
        {
            x.Update(context);
            return x.IsRenderable;
        };
        /// <summary>
        /// Updates the scene graph.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="renderables">The renderables.</param>
        /// <param name="results"></param>
        /// <returns></returns>
        public virtual void UpdateSceneGraph(IRenderContext context, List<SceneNode> renderables, List<SceneNode> results)
        {
            renderables.PreorderDFT(context, updateFunc, results, stackCache1);
        }

        /// <summary>
        /// Updates the scene graph.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="renderables">The renderables.</param>
        /// <returns></returns>
        public void UpdateSceneGraph2D(IRenderContext2D context, List<SceneNode2D> renderables)
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
        public virtual void UpdateGlobalVariables(IRenderContext context, List<RenderCore> lights, ref RenderParameter parameter)
        {
            if (parameter.RenderLight)
            {
                context.LightScene.LightModels.ResetLightCount();
                int count = lights.Count;
                for (int i = 0; i < count && i < Constants.MaxLights; ++i)
                {
                    lights[i].Render(context, ImmediateContext);
                }
            }
            if (parameter.UpdatePerFrameData)
            {
                context.UpdatePerFrameData();
            }
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="renderables">The renderables.</param>
        /// <param name="parameter">The parameter.</param>
        public virtual void RenderScene(IRenderContext context, List<RenderCore> renderables, ref RenderParameter parameter)
        {
            int count = renderables.Count;
            filters.Clear();
            for (int i = 0; i < count; ++i)
            {
                if (renderables[i].RenderType == RenderType.Opaque)
                {
                    renderables[i].Render(context, ImmediateContext);
                }
                else
                {
                    filters.Add(renderables[i]);
                }
            }
            count = filters.Count;
            for (int i = 0; i < count; ++i)
            {
                if (filters[i].RenderType == RenderType.Particle)
                {
                    filters[i].Render(context, ImmediateContext);
                }
            }
            for (int i = 0; i < count; ++i)
            {
                if (filters[i].RenderType == RenderType.Transparent)
                {
                    filters[i].Render(context, ImmediateContext);
                }
            }
        }
        /// <summary>
        /// Updates the no render parallel. <see cref="IRenderer.UpdateNotRenderParallel(IRenderContext, List{SceneNode})"/>
        /// </summary>
        /// <param name="renderables">The renderables.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual void UpdateNotRenderParallel(IRenderContext context, List<SceneNode> renderables)
        {
            int count = renderables.Count;
            for(int i = 0; i < count; ++i)
            {
                renderables[i].UpdateNotRender(context);
            }
        }
        /// <summary>
        /// Sets the render targets.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public void SetRenderTargets(ref RenderParameter parameter)
        {
            ImmediateContext.DeviceContext.OutputMerger.SetTargets(parameter.DepthStencilView, parameter.RenderTargetView);
            ImmediateContext.DeviceContext.Rasterizer.SetViewport(parameter.ViewportRegion);
            ImmediateContext.DeviceContext.Rasterizer.SetScissorRectangle(parameter.ScissorRegion.Left, parameter.ScissorRegion.Top, 
                parameter.ScissorRegion.Right, parameter.ScissorRegion.Bottom);
        }

        /// <summary>
        /// Render2s the d.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="renderables">The renderables.</param>
        /// <param name="parameter">The parameter.</param>
        public virtual void RenderScene2D(IRenderContext2D context, List<SceneNode2D> renderables, ref RenderParameter2D parameter)
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
        public virtual void RenderPreProc(IRenderContext context, List<RenderCore> renderables, ref RenderParameter parameter)
        {
            int count = renderables.Count;
            for (int i = 0; i < count; ++i)
            {
                renderables[i].Render(context, ImmediateContext);
            }
        }

        /// <summary>
        /// Renders the post proc.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="renderables">The renderables.</param>
        /// <param name="parameter">The parameter.</param>
        public virtual void RenderPostProc(IRenderContext context, List<RenderCore> renderables, ref RenderParameter parameter)
        {
            int count = renderables.Count;
            for (int i = 0; i < count; ++i)
            {
                renderables[i].Render(context, ImmediateContext);
            }
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            filters.Clear();
            stackCache1.Clear();
            stack2DCache1.Clear();
            base.OnDispose(disposeManagedResources);
        }
    }

}
