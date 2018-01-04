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

        private IRenderer renderable;
        /// <summary>
        /// <see cref="IRenderHost.Renderable"/>
        /// </summary>
        public IRenderer Renderable
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

        public ID2DTarget D2DControls
        {
            private set; get;
        }

        public DX11RenderHostCommon()
        {
        }

        protected abstract IDX11RenderBufferProxy CreateRenderBuffer();

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
}
