//#define OLD

using SharpDX.Direct3D11;
#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using global::SharpDX;
    using HelixToolkit.Wpf.SharpDX.Core2D;
    using HelixToolkit.Wpf.SharpDX.Utilities;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public abstract class DX11RenderHostBase : DisposeObject, IRenderHost
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
                InvalidateRender();
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

        private IViewport3DX viewport;
        /// <summary>
        /// <see cref="IRenderHost.Renderable"/>
        /// </summary>
        public IViewport3DX Viewport
        {
            set
            {
                if (viewport == value)
                {
                    return;
                }
                DetachRenderable();
                viewport = value;
                if (IsInitialized)
                {
                    AttachRenderable(Device.ImmediateContext);
                }
            }
            get { return viewport; }
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
                if (Set(ref effectsManager, value) && IsInitialized)
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

        public bool IsBusy
        {
            private set; get;
        } = false;

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

        private ID2DTarget d2dTarget;
        public ID2DTarget D2DControls
        {
            get { return d2dTarget; }
        }

        protected IRenderer renderer;

        protected volatile bool UpdateRequested = true;

        private readonly Stopwatch renderTimer = new Stopwatch();

        private TimeSpan lastRenderingDuration;
        private readonly FrameRateRegulator skipper = new FrameRateRegulator();

        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;
        public event EventHandler<bool> StartRenderLoop;
        public event EventHandler<bool> StopRenderLoop;

        protected abstract IDX11RenderBufferProxy CreateRenderBuffer();
        protected abstract ID2DTarget CreateD2DTarget();
        protected abstract IRenderer CreateRenderer();

        public void InvalidateRender()
        {
            UpdateRequested = true;
        }

        public void UpdateAndRender()
        {
            if (((IsInitialized && UpdateRequested && !skipper.IsSkip()) || skipper.DelayTrigger()) && viewport != null)
            {
                var t0 = renderTimer.Elapsed;
                UpdateRequested = false;
                RenderContext.EnableBoundingFrustum = EnableRenderFrustum;
                RenderContext.TimeStamp = t0;
                RenderContext.Camera = viewport.CameraCore;
                RenderContext.WorldMatrix = viewport.WorldMatrix;
                PreRender();              
                try
                {                   
                    viewport.UpdateFPS(t0);
                    renderBuffer.BeginDraw();
                    OnRender(t0);
                    OnRender2D(t0);
                    renderBuffer.EndDraw();
                }
                catch (Exception ex)
                {
                    EndD3D();
                    ExceptionOccurred?.Invoke(this, new RelayExceptionEventArgs(ex));
                }
                finally
                {
                    PostRender();
                }
                lastRenderingDuration = renderTimer.Elapsed - t0;
                skipper.Push(lastRenderingDuration.TotalMilliseconds);
            }
        }

        protected virtual void PreRender()
        {
            SetDefaultRenderTargets(Device.ImmediateContext, true);
        }
        protected abstract void PostRender();
        protected abstract void OnRender(TimeSpan time);
        protected abstract void OnRender2D(TimeSpan time);

        /// <summary>
        /// Set default render target to specify context.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="clear"></param>
        /// <returns>Set successful?</returns>
        public bool SetDefaultRenderTargets(DeviceContext context, bool clear = true)
        {
            if (!IsInitialized) { return false; }
            renderBuffer.SetDefaultRenderTargets(context);
            if (clear)
            {
                renderBuffer.ClearRenderTarget(context, ClearColor);
            }
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
                StartD3D(this.ActualWidth, this.ActualHeight);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void StartD3D(double width, double height)
        {
            if(EffectsManager == null)
            {
                return;
            }
            ActualWidth = Math.Max(1,width);
            ActualHeight = Math.Max(1,height);
            CreateAndBindBuffers();      
            IsInitialized = true;            
            AttachRenderable(Device.ImmediateContext);
            StartRendering();
        }

        protected virtual void StartRendering()
        {
            renderTimer.Restart();
            StartRenderLoop?.Invoke(this, true);
        }

        protected void CreateAndBindBuffers()
        {
            renderBuffer = Collect(CreateRenderBuffer());
            renderBuffer.OnNewBufferCreated += RenderBuffer_OnNewBufferCreated;
            d2dTarget = Collect(CreateD2DTarget());
            renderer = Collect(CreateRenderer());
            OnInitializeBuffers(renderBuffer, d2dTarget, renderer);
        }

        private void RenderBuffer_OnNewBufferCreated(object sender, Texture2D e)
        {
            OnNewRenderTargetTexture?.Invoke(this, e);
        }

        protected abstract void OnInitializeBuffers(IDX11RenderBufferProxy buffer, ID2DTarget d2dTarget, IRenderer renderer);

        protected virtual void AttachRenderable(DeviceContext context)
        {
            if (!IsInitialized || Viewport == null) { return; }
            RenderTechnique = viewport.RenderTechnique == null ? EffectsManager?[DefaultRenderTechniqueNames.Blinn] : viewport.RenderTechnique;
            if (EnableSharingModelMode && SharedModelContainer != null)
            {
                SharedModelContainer.CurrentRenderHost = this;
                viewport.Attach(SharedModelContainer);
            }
            else
            {
                viewport.Attach(this);
            }         
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

        protected virtual void StopRendering()
        {            
            StopRenderLoop?.Invoke(this, true);
            renderTimer.Stop();
        }

        protected virtual void DisposeBuffers()
        {
            renderBuffer.OnNewBufferCreated -= RenderBuffer_OnNewBufferCreated;
            RemoveAndDispose(ref renderer);
            RemoveAndDispose(ref d2dTarget);
            RemoveAndDispose(ref renderBuffer);
        }

        protected virtual void DetachRenderable()
        {
            RemoveAndDispose(ref renderContext);            
            Viewport?.Detach();
        }

        public void Resize(double width, double height)
        {
            StopRendering();
            ActualWidth = Math.Max(1, width);
            ActualHeight = Math.Max(1, height);
            OnNewRenderTargetTexture?.Invoke(this, renderBuffer.Resize((int)ActualWidth, (int)ActualHeight));
            StartRendering();
        }

        public virtual void SetDefaultRenderTargets(bool clear)
        {
            SetDefaultRenderTargets(Device.ImmediateContext, clear);
        }

        public event EventHandler<Texture2D> OnNewRenderTargetTexture;

        protected override void Dispose(bool disposeManagedResources)
        {
            IsInitialized = false;
            base.Dispose(disposeManagedResources);
        }
    }
}
