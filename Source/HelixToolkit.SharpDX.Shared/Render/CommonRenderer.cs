//#define OLD

using SharpDX.Direct3D11;
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Core;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public class CommonRenderer : DisposeObject, IRenderer
    {
        private List<IRenderCore> pendingRenders = new List<IRenderCore>(100);
        private List<IRenderable> pendingUpdates = new List<IRenderable>(100);
        private readonly Stack<IEnumerator<IRenderable>> stackCache1 = new Stack<IEnumerator<IRenderable>>(20);
        private readonly Stack<IEnumerator<IRenderable>> stackCache2 = new Stack<IEnumerator<IRenderable>>(20);

        private Device device;
        protected DeviceContextProxy ImmediateContext;

        public CommonRenderer(Device device)
        {
            this.device = device;
            ImmediateContext = new DeviceContextProxy(device.ImmediateContext);
        }

        public virtual void Initialize()
        {

        }
        /// <summary>
        /// Renders the scene.
        /// </summary>
        public Task Render(IRenderContext context, IEnumerable<IRenderable> renderables, RenderParameter parameter)
        {
            if (parameter == null)
            { return null; }
#if OLD
            UpdateGlobalVariables(context, renderables).Wait();
            SetRenderTargets(context.DeviceContext, parameter);
            foreach(var item in renderables)
            {
                item.Render(context);
            }
#else
            UpdateGlobalVariables(context, renderables);

            SetRenderTargets(ImmediateContext, parameter);

            pendingUpdates.Clear();
            pendingUpdates.AddRange(renderables.PreorderDFT((x) =>
            {
                x.Update(context);
                return x.IsRenderable && !(x is ILight3D);
            }, stackCache1));

            pendingRenders.Clear();
            pendingRenders.AddRange(pendingUpdates.Select(x => x.RenderCore));

            var task = Task.Factory.StartNew(() => { UpdateNotRender(renderables); });
            
            foreach (var renderable in pendingRenders)
            {
                renderable.Render(context, ImmediateContext);
            }
            return task;
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderables"></param>
        /// <returns></returns>
        private void UpdateGlobalVariables(IRenderContext context, IEnumerable<IRenderable> renderables)
        {
            context.LightScene.LightModels.ResetLightCount();
            foreach (IRenderable e in renderables.Take(Constants.MaxLights)
                .PreorderDFT((x)=> x is ILight3D && x.IsRenderable, stackCache2).Take(Constants.MaxLights))
            {
                e.Render(context, ImmediateContext);
            }
            context.UpdatePerFrameData();
        }

        private void UpdateNotRender(IEnumerable<IRenderable> renderables)
        {
            foreach(var model in pendingUpdates)
            {
                model.UpdateNotRender();
            }
        }

        private void SetRenderTargets(DeviceContext context, RenderParameter parameter)
        {
            context.OutputMerger.SetTargets(parameter.depthStencil, parameter.target);
            context.Rasterizer.SetViewport(parameter.ViewportRegion);
            context.Rasterizer.SetScissorRectangle(parameter.ScissorRegion.Left, parameter.ScissorRegion.Top, 
                parameter.ScissorRegion.Right, parameter.ScissorRegion.Bottom);
        }



        public void Render2D(IRenderContext2D context, IEnumerable<IRenderable2D> renderables, RenderParameter2D parameter)
        {
            foreach (var e in renderables)
            {
                e.Render(context);
            }
        }
    }

}
