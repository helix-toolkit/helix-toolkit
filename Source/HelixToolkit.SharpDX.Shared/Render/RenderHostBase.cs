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
        private IDX11RenderBufferProxy renderBuffer;
        /// <summary>
        /// Gets the render buffer.
        /// </summary>
        /// <value>
        /// The render buffer.
        /// </value>
        protected IDX11RenderBufferProxy RenderBuffer { get { return renderBuffer; } }
        /// <summary>
        /// Gets the device.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        public Device Device
        {
            get
            {
                return EffectsManager.Device;
            }
        }

        private Color4 clearColor = Color.White;
        /// <summary>
        /// Gets or sets the color of the clear.
        /// </summary>
        /// <value>
        /// The color of the clear.
        /// </value>
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
        /// <summary>
        /// Gets or sets a value indicating whether shadow map enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is shadow map enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsShadowMapEnabled
        {
            set; get;
        } = false;

        private MSAALevel msaa = MSAALevel.Disable;
        /// <summary>
        /// Gets or sets the Multi-Sampling-Anti-Alias.
        /// </summary>
        /// <value>
        /// The msaa.
        /// </value>
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
        /// <see cref="IRenderHost.Viewport"/>
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
        /// <summary>
        /// Gets or sets the effects manager.
        /// </summary>
        /// <value>
        /// The effects manager.
        /// </value>
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
        /// <summary>
        /// Gets or sets the render technique.
        /// </summary>
        /// <value>
        /// The render technique.
        /// </value>
        public IRenderTechnique RenderTechnique
        {
            protected set; get;
        }
        /// <summary>
        /// Gets a value indicating whether this instance is deferred lighting.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deferred lighting; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeferredLighting
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Gets or sets the actual height.
        /// </summary>
        /// <value>
        /// The actual height.
        /// </value>
        public double ActualHeight
        {
            private set; get;
        }
        /// <summary>
        /// Gets or sets the actual width.
        /// </summary>
        /// <value>
        /// The actual width.
        /// </value>
        public double ActualWidth
        {
            private set; get;
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            private set; get;
        } = false;
        /// <summary>
        /// Gets or sets a value indicating whether [enable render frustum].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable render frustum]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRenderFrustum
        {
            set; get;
        }

        private uint maxFPS = 60;
        /// <summary>
        /// Gets or sets the maximum FPS.
        /// </summary>
        /// <value>
        /// The maximum FPS.
        /// </value>
        public uint MaxFPS
        {
            set
            {
                maxFPS = value;
                frameRegulator.Threshold = 1000.0 / maxFPS;
            }
            get
            {
                return maxFPS;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable sharing model mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable sharing model mode]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSharingModelMode
        {
            set; get;
        }
        /// <summary>
        /// Gets or sets the shared model container.
        /// </summary>
        /// <value>
        /// The shared model container.
        /// </value>
        public IModelContainer SharedModelContainer
        {
            set; get;
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is rendering.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is rendering; otherwise, <c>false</c>.
        /// </value>
        public bool IsRendering
        {
            set; get;
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { private set; get; } = false;
        /// <summary>
        /// Gets the color buffer view.
        /// </summary>
        /// <value>
        /// The color buffer view.
        /// </value>
        public RenderTargetView ColorBufferView
        {
            get
            {
                return renderBuffer.ColorBufferView;
            }
        }
        /// <summary>
        /// Gets the depth stencil buffer view.
        /// </summary>
        /// <value>
        /// The depth stencil buffer view.
        /// </value>
        public DepthStencilView DepthStencilBufferView
        {
            get
            {
                return renderBuffer.DepthStencilBufferView;
            }
        }

        /// <summary>
        /// Gets the d2d controls.
        /// </summary>
        /// <value>
        /// The d2 d controls.
        /// </value>
        public ID2DTarget D2DControls
        {
            get { return RenderBuffer.D2DControls; }
        }
        /// <summary>
        /// The renderer
        /// </summary>
        protected IRenderer renderer;
        /// <summary>
        /// The update requested
        /// </summary>
        protected volatile bool UpdateRequested = true;

        private readonly Stopwatch renderTimer = new Stopwatch();

        private TimeSpan lastRenderingDuration;
        private readonly FrameRateRegulator frameRegulator = new FrameRateRegulator();
        /// <summary>
        /// Occurs when [exception occurred].
        /// </summary>
        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;
        /// <summary>
        /// Occurs when [start render loop].
        /// </summary>
        public event EventHandler<bool> StartRenderLoop;
        /// <summary>
        /// Occurs when [stop render loop].
        /// </summary>
        public event EventHandler<bool> StopRenderLoop;
        /// <summary>
        /// Creates the render buffer.
        /// </summary>
        /// <returns></returns>
        protected abstract IDX11RenderBufferProxy CreateRenderBuffer();
        /// <summary>
        /// Creates the renderer.
        /// </summary>
        /// <returns></returns>
        protected abstract IRenderer CreateRenderer();
        /// <summary>
        /// Invalidates the render.
        /// </summary>
        public void InvalidateRender()
        {
            UpdateRequested = true;
        }
        /// <summary>
        /// Determines whether this instance can render.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can render; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanRender()
        {
            return (IsInitialized && IsRendering && UpdateRequested && viewport != null && !frameRegulator.IsSkip())
                || frameRegulator.DelayTrigger();
        }
        /// <summary>
        /// Updates the and render.
        /// </summary>
        public void UpdateAndRender()
        {
            if (CanRender())
            {
                IsBusy = true;
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
                    if (renderBuffer.BeginDraw())
                    {
                        OnRender(t0);
                        renderBuffer.EndDraw();
                    }
                    if (renderBuffer.BeginDraw2D())
                    {
                        OnRender2D(t0);
                        renderBuffer.EndDraw2D();
                    }
                    renderBuffer.Present();
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
                frameRegulator.Push(lastRenderingDuration.TotalMilliseconds);
                IsBusy = false;
            }
        }
        /// <summary>
        /// Called before OnRender.
        /// </summary>
        protected virtual void PreRender()
        {
            SetDefaultRenderTargets(Device.ImmediateContext, true);
        }
        /// <summary>
        /// Called after OnRender.
        /// </summary>
        protected abstract void PostRender();
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="time">The time.</param>
        protected abstract void OnRender(TimeSpan time);
        /// <summary>
        /// Called when [render2d].
        /// </summary>
        /// <param name="time">The time.</param>
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

        /// <summary>
        /// Restarts the render host. 
        /// <para>If HotRestart = true, only recreate buffers, otherwise dispose all resources and call StartD3D.</para>
        /// </summary>
        /// <param name="hotRestart">if set to <c>true</c> [hotRestart].</param>
        protected void Restart(bool hotRestart)
        {
            if (!IsInitialized)
            { return; }
            if (hotRestart)
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
            if (EffectsManager == null)
            {
                return;
            }
            ActualWidth = Math.Max(1, width);
            ActualHeight = Math.Max(1, height);
            CreateAndBindBuffers();
            IsInitialized = true;
            AttachRenderable(Device.ImmediateContext);
            StartRendering();
        }
        /// <summary>
        /// Starts the rendering.
        /// </summary>
        protected virtual void StartRendering()
        {
            renderTimer.Restart();
            InvalidateRender();
            StartRenderLoop?.Invoke(this, true);
        }
        /// <summary>
        /// Creates the and bind buffers.
        /// </summary>
        protected void CreateAndBindBuffers()
        {
            renderBuffer = Collect(CreateRenderBuffer());
            renderBuffer.OnNewBufferCreated += RenderBuffer_OnNewBufferCreated;
            renderer = Collect(CreateRenderer());
            OnInitializeBuffers(renderBuffer, renderer);
        }

        private void RenderBuffer_OnNewBufferCreated(object sender, Texture2D e)
        {
            OnNewRenderTargetTexture?.Invoke(this, e);
        }
        /// <summary>
        /// Called when [initialize buffers].
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="d2dTarget">The D2D target.</param>
        /// <param name="renderer">The renderer.</param>
        protected virtual void OnInitializeBuffers(IDX11RenderBufferProxy buffer, IRenderer renderer)
        {
            buffer.Initialize((int)ActualWidth, (int)ActualHeight, MSAA);
        }
        /// <summary>
        /// Attaches the renderable.
        /// </summary>
        /// <param name="context">The context.</param>
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
        /// <summary>
        /// Creates the render context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Stops the rendering.
        /// </summary>
        protected virtual void StopRendering()
        {            
            StopRenderLoop?.Invoke(this, true);
            renderTimer.Stop();
        }
        /// <summary>
        /// Disposes the buffers.
        /// </summary>
        protected virtual void DisposeBuffers()
        {
            if (renderBuffer != null)
            {
                renderBuffer.OnNewBufferCreated -= RenderBuffer_OnNewBufferCreated;
            }
            RemoveAndDispose(ref renderer);
            RemoveAndDispose(ref renderBuffer);
        }
        /// <summary>
        /// Detaches the renderable.
        /// </summary>
        protected virtual void DetachRenderable()
        {
            RemoveAndDispose(ref renderContext);            
            Viewport?.Detach();
        }
        /// <summary>
        /// Resizes
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void Resize(double width, double height)
        {
            ActualWidth = Math.Max(1, width);
            ActualHeight = Math.Max(1, height);
            if (IsInitialized)
            {
                StopRendering();
                OnNewRenderTargetTexture?.Invoke(this, renderBuffer.Resize((int)ActualWidth, (int)ActualHeight));
                StartRendering();
            }
        }
        /// <summary>
        /// Sets the default render targets.
        /// </summary>
        /// <param name="clear">if set to <c>true</c> [clear].</param>
        public virtual void SetDefaultRenderTargets(bool clear)
        {
            SetDefaultRenderTargets(Device.ImmediateContext, clear);
        }
        /// <summary>
        /// Occurs when [on new render target texture].
        /// </summary>
        public event EventHandler<Texture2D> OnNewRenderTargetTexture;
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposeManagedResources)
        {
            IsInitialized = false;
            base.Dispose(disposeManagedResources);
        }
    }
}
