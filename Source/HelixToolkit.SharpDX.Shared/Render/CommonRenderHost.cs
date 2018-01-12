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
    using global::SharpDX;
    using HelixToolkit.Wpf.SharpDX.Core2D;
    using HelixToolkit.Wpf.SharpDX.Utilities;
    using System;
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
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
                if (renderBuffer.Initialized)
                {
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
                AttachRenderable(deviceContext);
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

        protected DeviceContext deviceContext { private set; get; }

        private ID2DTarget d2dTarget;
        public ID2DTarget D2DControls
        {
            get { return d2dTarget; }
        }

        protected IRenderer RenderEngine;

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
            if (((UpdateRequested && !skipper.IsSkip()) || skipper.DelayTrigger()) && viewport != null)
            {
                var t0 = renderTimer.Elapsed;
                UpdateRequested = false;
                PreRender();              
                try
                {                   
                    viewport.UpdateFPS(t0);
                    Render(t0);
                    Render2D(t0);
                }
                catch (Exception ex)
                {
                    EndD3D();
                    ExceptionOccurred?.Invoke(this, new RelayExceptionEventArgs(ex));
                }
                lastRenderingDuration = renderTimer.Elapsed - t0;
                skipper.Push(lastRenderingDuration.TotalMilliseconds);
            }
        }

        protected abstract void PreRender();
        protected abstract void PostRender();

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
            RenderEngine = CreateRenderer();
            AttachRenderable(deviceContext);
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
            d2dTarget = Collect(CreateD2DTarget());
            OnBindBuffers(renderBuffer);
        }

        protected virtual void OnBindBuffers(IDX11RenderBufferProxy buffer) { }

        protected virtual void AttachRenderable(DeviceContext context)
        {
            if (!IsInitialized) { return; }
            Viewport?.Attach(this);
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
            ActualWidth = width;
            ActualHeight = height;
            OnNewRenderTargetTexture?.Invoke(this, renderBuffer.Resize((int)width, (int)height));
            StartRendering();
        }

        public virtual void SetDefaultRenderTargets(bool clear)
        {
            SetDefaultRenderTargets(Device.ImmediateContext, clear);
        }

        protected abstract void Render(TimeSpan time);
        protected abstract void Render2D(TimeSpan time);

        public event EventHandler<Texture2D> OnNewRenderTargetTexture;

        protected override void Dispose(bool disposeManagedResources)
        {
            IsInitialized = false;
            base.Dispose(disposeManagedResources);
        }
    }
}
