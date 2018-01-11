//#define OLD

using global::SharpDX;
using global::SharpDX.DXGI;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using System;
using System.Collections.Generic;
using System.Diagnostics;
#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Core2D;
    using Model;
    using Utilities;
    using System.Linq;
    using System.Threading.Tasks;
    using Core;
    public class CommonRenderer : IRenderer
    {
        private List<IRenderCore> pendingRenders = new List<IRenderCore>(100);
        private readonly Stack<IEnumerator<IRenderable>> stackCache1 = new Stack<IEnumerator<IRenderable>>(20);
        private readonly Stack<IEnumerator<IRenderable>> stackCache2 = new Stack<IEnumerator<IRenderable>>(20);
        /// <summary>
        /// Renders the scene.
        /// </summary>
        public void Render(IRenderContext context, IEnumerable<IRenderable> renderables, RenderParameter parameter)
        {
            if (parameter == null)
            { return; }
#if OLD
            UpdateGlobalVariables(context, renderables).Wait();
            SetRenderTargets(context.DeviceContext, parameter);
            foreach(var item in renderables)
            {
                item.Render(context);
            }
#else
            UpdateGlobalVariables(context, renderables).Wait();

            SetRenderTargets(context.DeviceContext, parameter);
          
            pendingRenders.Clear();
            pendingRenders.AddRange(renderables.PreorderDFTGetCores((x) =>
            {
                x.Update(context);
                return x.IsRenderable && !(x is ILight3D);
            }, stackCache1));

            //task.Wait();

            foreach (var renderable in pendingRenders)
            {
                renderable.Render(context);
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="renderables"></param>
        /// <returns></returns>
        private async Task UpdateGlobalVariables(IRenderContext context, IEnumerable<IRenderable> renderables)
        {
            context.LightScene.ResetLightCount();
            foreach (IRenderable e in renderables.Take(Constants.MaxLights)
                .PreorderDFT((x)=> x is ILight3D && x.IsRenderable, stackCache2).Take(Constants.MaxLights))
            {
                e.Render(context);
            }
            context.UpdatePerFrameData();
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

    /*
    public abstract class DX11RenderHostCommon : DisposeObject, IRenderHost
    {
        protected IDX11RenderBufferProxy renderBuffer;

        public Device Device
        {
            get
            {
                return EffectsManager.Device;
            }
        }

        private Color4 clearColor = Color.White;
        public Color4 ClearColor
        {
            get
            {
                return clearColor;
            }
            set
            {
                clearColor = value;
                if (renderBuffer != null)
                {
                    renderBuffer.ClearColor = value;
                    InvalidateRender();
                }
            }
        }

        public bool IsShadowMapEnabled
        {
            set; get;
        } = false;

        private MSAALevel msaa = MSAALevel.Disable;
        public MSAALevel MSAA
        {
            set
            {
                if (Set(ref msaa, value))
                {
                    Restart(true);
                }
            }
            get { return msaa; }
        }

        private IViewport3DX renderable;
        /// <summary>
        /// <see cref="IRenderHost.Renderable"/>
        /// </summary>
        public IViewport3DX Renderable
        {
            set
            {
                if (renderable == value)
                {
                    return;
                }
                DetachRenderable();
                renderable = value;
                AttachRenderable(deviceContext);
            }
            get { return renderable; }
        }

        private IRenderContext renderContext;
        /// <summary>
        /// 
        /// </summary>
        public IRenderContext RenderContext
        {
            get { return renderContext; }
        }

        private IEffectsManager effectsManager;
        public IEffectsManager EffectsManager
        {
            set
            {
                if (Set(ref effectsManager, value))
                {
                    Restart(false);
                }
            }
            get
            {
                return effectsManager;
            }
        }

        public IRenderTechnique RenderTechnique
        {
            protected set; get;
        }

        public bool IsDeferredLighting
        {
            get
            {
                return false;
            }
        }

        public double ActualHeight
        {
            private set; get;
        }

        public double ActualWidth
        {
            private set; get;
        }

        public abstract bool IsBusy
        {
            get;
        }

        public Light3DSceneShared Light3DSceneShared
        {
            get { return RenderContext.LightScene; }
        }

        public bool EnableRenderFrustum
        {
            set; get;
        }

        public uint MaxFPS
        {
            set; get;
        }

        public bool EnableSharingModelMode
        {
            set; get;
        }

        public IModelContainer SharedModelContainer
        {
            set; get;
        }

        public bool IsRendering
        {
            set; get;
        }

        public bool IsInitialized { private set; get; } = false;

        public RenderTargetView ColorBufferView
        {
            get
            {
                return renderBuffer.ColorBufferView;
            }
        }

        public DepthStencilView DepthStencilBufferView
        {
            get
            {
                return renderBuffer.DepthStencilBufferView;
            }
        }

        protected DeviceContext deviceContext { private set; get; }

        private ID2DTarget d2dTarget;
        public ID2DTarget D2DControls
        {
            get { return d2dTarget; }
        }

        protected abstract IDX11RenderBufferProxy CreateRenderBuffer();
        protected abstract ID2DTarget CreateD2DTarget();

        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;

        public abstract void InvalidateRender();
        /// <summary>
        /// Set default render target to specify context.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="clear"></param>
        /// <returns>Set successful?</returns>
        public bool SetDefaultRenderTargets(DeviceContext context, bool clear = true)
        {
            if (!IsInitialized) { return false; }
            renderBuffer.SetDefaultRenderTargets(context, clear);
            return true;
        }

        protected void Restart(bool light)
        {
            if (!IsInitialized)
            { return; }
            if (light)
            {
                StopRendering();
                DisposeBuffers();
                CreateAndBindBuffers();
                StartRendering();
            }
            else
            {
                EndD3D();
                StartD3D(deviceContext);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void StartD3D(DeviceContext context)
        {
            deviceContext = context;
            CreateAndBindBuffers();
            IsInitialized = true;
            AttachRenderable(deviceContext);         
            StartRendering();
        }

        protected abstract void StartRendering();
        protected void CreateAndBindBuffers()
        {
            renderBuffer = Collect(CreateRenderBuffer());
            d2dTarget = Collect(CreateD2DTarget());
            OnBindBuffers(renderBuffer);
        }

        protected virtual void OnBindBuffers(IDX11RenderBufferProxy buffer) { }

        protected virtual void AttachRenderable(DeviceContext context)
        {
            if (!IsInitialized) { return; }
            Renderable?.Attach(this);
            renderContext = Collect(CreateRenderContext(context));
        }

        protected virtual IRenderContext CreateRenderContext(DeviceContext context)
        {
            return new RenderContext(this, context);
        }
        /// <summary>
        /// 
        /// </summary>
        public void EndD3D()
        {
            StopRendering();
            IsInitialized = false;
            DetachRenderable();
            DisposeBuffers();
        }

        protected abstract void StopRendering();

        protected virtual void DisposeBuffers()
        {
            RemoveAndDispose(ref d2dTarget);
            RemoveAndDispose(ref renderBuffer);          
        }

        protected virtual void DetachRenderable()
        {
            RemoveAndDispose(ref renderContext);
            Renderable?.Detach();
        }

        public void Resize(double width, double height)
        {
            StopRendering();
            ActualWidth = width;
            ActualHeight = height;
            OnNewRenderTargetTexture?.Invoke(this, renderBuffer.Resize((int)width, (int)height));
            StartRendering();
        }

        public virtual void SetDefaultRenderTargets(bool clear)
        {
            renderBuffer.SetDefaultRenderTargets(Device.ImmediateContext, clear);
        }

        protected abstract void Render(TimeSpan time);

        public event EventHandler<Texture2D> OnNewRenderTargetTexture;

        protected override void Dispose(bool disposeManagedResources)
        {
            IsInitialized = false;
            base.Dispose(disposeManagedResources);
        }
    }
    */
}
