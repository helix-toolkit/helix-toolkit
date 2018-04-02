/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using SharpDX.Direct3D11;
using SharpDX;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#endif
#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Core2D;
    using Utilities;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using HelixToolkit.Logger;
    using Core;
    using Model.Scene;
    /// <summary>
    /// 
    /// </summary>
    public abstract class DX11RenderHostBase : DisposeObject, IRenderHost
    {
        private const int MinWidth = 10;
        private const int MinHeight = 10;
        private static readonly LogWrapper NullLogger = new LogWrapper(new NullLogger());
        #region Properties        
        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid GUID { get; } = Guid.NewGuid();

        private IDX11RenderBufferProxy renderBuffer;
        /// <summary>
        /// Gets the render buffer.
        /// </summary>
        /// <value>
        /// The render buffer.
        /// </value>
        public IDX11RenderBufferProxy RenderBuffer { get { return renderBuffer; } }
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
        /// <summary>
        /// Gets the device2d.
        /// </summary>
        /// <value>
        /// The device2d.
        /// </value>
        public global::SharpDX.Direct2D1.Device Device2D { get { return EffectsManager.Device2D; } }

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
                Log(LogLevel.Information, $"Set Viewport, Initialized = {IsInitialized}");
                DetachRenderable();
                viewport = value;
                if (IsInitialized)
                {
                    AttachRenderable(EffectsManager);
                }
            }
            get { return viewport; }
        }

        protected RenderContext renderContext;
        /// <summary>
        /// 
        /// </summary>
        public IRenderContext RenderContext
        {
            get { return renderContext; }
        }

        private RenderContext2D renderContext2D;
        /// <summary>
        /// Gets the render context2d.
        /// </summary>
        /// <value>
        /// The render context2d.
        /// </value>
        public IRenderContext2D RenderContext2D
        {
            get { return renderContext2D; }
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
                var currentManager = effectsManager;
                if (Set(ref effectsManager, value))
                {
                    Log(LogLevel.Information, $"Set new EffectsManager;");
                    if (currentManager != null)
                    {
                        currentManager.OnDisposeResources -= OnManagerDisposed;
                    }
                    if (effectsManager != null)
                    {
                        effectsManager.OnDisposeResources += OnManagerDisposed;
                        RenderTechnique = viewport == null || viewport.RenderTechnique == null ? EffectsManager?[DefaultRenderTechniqueNames.Blinn] : viewport.RenderTechnique;
                        if (IsInitialized)
                        {
                            Restart(false);
                        }
                        else if(isLoaded)
                        {
                            StartD3D(ActualWidth, ActualHeight);
                        }
                    }
                    else
                    {
                        RenderTechnique = null;
                    }
                }
            }
            get
            {
                return effectsManager;
            }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public LogWrapper Logger { get { return EffectsManager != null ? EffectsManager.Logger : NullLogger; } }

        private IRenderTechnique renderTechnique;
        /// <summary>
        /// Gets or sets the render technique.
        /// </summary>
        /// <value>
        /// The render technique.
        /// </value>
        public IRenderTechnique RenderTechnique
        {
            set
            {
                if(Set(ref renderTechnique, value) && IsInitialized)
                {
                    Restart(false);
                }
            }
            get
            {
                return renderTechnique;
            }
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

        private double height = MinHeight;
        /// <summary>
        /// Gets or sets the actual height.
        /// </summary>
        /// <value>
        /// The actual height.
        /// </value>
        public double ActualHeight
        {
            private set
            {
                height = Math.Max(MinHeight, value);
            }
            get
            {
                return height;
            }
        }

        private double width = MinWidth;
        /// <summary>
        /// Gets or sets the actual width.
        /// </summary>
        /// <value>
        /// The actual width.
        /// </value>
        public double ActualWidth
        {
            private set
            {
                width = Math.Max(MinWidth, value);
            }
            get { return width; }
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
        } = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { private set; get; } = false;

        private bool isLoaded = false;

        /// <summary>
        /// Gets the color buffer view.
        /// </summary>
        /// <value>
        /// The color buffer view.
        /// </value>
        public RenderTargetView RenderTargetBufferView
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
        public ID2DTargetProxy D2DTarget
        {
            get { return RenderBuffer.D2DTarget; }
        }

        /// <summary>
        /// Gets the render statistics.
        /// </summary>
        /// <value>
        /// The render statistics.
        /// </value>
        public IRenderStatistics RenderStatistics { get; } = new RenderStatistics();

        /// <summary>
        /// Gets the current frame renderables for rendering.
        /// </summary>
        /// <value>
        /// The per frame renderable.
        /// </value>
        public abstract List<SceneNode> PerFrameRenderables { get; }
        /// <summary>
        /// Gets the per frame lights.
        /// </summary>
        /// <value>
        /// The per frame lights.
        /// </value>
        public abstract IEnumerable<LightCoreBase> PerFrameLights { get; }
        /// <summary>
        /// Gets the post effects render cores for this frame
        /// </summary>
        /// <value>
        /// The post effects render cores.
        /// </value>
        public abstract List<RenderCore> PerFrameGeneralCoresWithPostEffect { get; }
        /// <summary>
        /// Gets the per frame render cores.
        /// </summary>
        /// <value>
        /// The per frame render cores.
        /// </value>
        public abstract List<RenderCore> PerFrameGeneralRenderCores { get; }

        #region Configuration
        /// <summary>
        /// Gets or sets a value indicating whether [show render statistics].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show render statistics]; otherwise, <c>false</c>.
        /// </value>
        public RenderDetail ShowRenderDetail
        {
            set { RenderStatistics.FrameDetail = value; }
            get { return RenderStatistics.FrameDetail; }
        }

        public DX11RenderHostConfiguration RenderConfiguration { set; get; } 
            = new DX11RenderHostConfiguration() { UpdatePerFrameData = true, RenderD2D = true, RenderLights = true, ClearEachFrame = true };
        #endregion
        #endregion

        #region Events
        /// <summary>
        /// Occurs when [exception occurred].
        /// </summary>
        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;
        /// <summary>
        /// Occurs when [start render loop].
        /// </summary>
        public event EventHandler<EventArgs> StartRenderLoop;
        /// <summary>
        /// Occurs when [stop render loop].
        /// </summary>
        public event EventHandler<EventArgs> StopRenderLoop;
        /// <summary>
        /// Occurs when [on new render target texture].
        /// </summary>
        public event EventHandler<Texture2DArgs> OnNewRenderTargetTexture;

        private readonly Func<Device, IRenderer> createRendererFunction;
        #endregion

        #region Private variables
        /// <summary>
        /// The renderer
        /// </summary>
        protected IRenderer renderer;
        /// <summary>
        /// The update requested
        /// </summary>
        protected volatile bool UpdateRequested = true;

        private TimeSpan lastRenderingDuration = TimeSpan.Zero;

        private TimeSpan lastRenderTime = TimeSpan.Zero;

        private int updateCounter = 0; // Used to render at least twice. D3DImage sometimes not getting refresh if only render once.
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DX11RenderHostBase"/> class.
        /// </summary>
        public DX11RenderHostBase() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DX11RenderHostBase"/> class.
        /// </summary>
        /// <param name="createRenderer">The create renderer.</param>
        public DX11RenderHostBase(Func<Device, IRenderer> createRenderer)
        {
            createRendererFunction = createRenderer;           
        }

        /// <summary>
        /// Creates the render buffer.
        /// </summary>
        /// <returns></returns>
        protected abstract IDX11RenderBufferProxy CreateRenderBuffer();

        /// <summary>
        /// Invalidates the render.
        /// </summary>
        public void InvalidateRender()
        {
            UpdateRequested = true;
            updateCounter = 0;
        }
        /// <summary>
        /// Determines whether this instance can render.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can render; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanRender()
        {
            return IsInitialized && IsRendering && (UpdateRequested || updateCounter < 2) && viewport != null && ActualWidth > 10 && ActualHeight > 10;
        }
        /// <summary>
        /// Updates the and render.
        /// </summary>
        public void UpdateAndRender()
        {
            if (CanRender())
            {
                IsBusy = true;
                var t0 = TimeSpan.FromSeconds((double)Stopwatch.GetTimestamp()/Stopwatch.Frequency);
                RenderStatistics.FPSStatistics.Push((t0 - lastRenderTime).TotalMilliseconds);
                lastRenderTime = t0;
                UpdateRequested = false;
                ++updateCounter;
                renderContext.AutoUpdateOctree = RenderConfiguration.AutoUpdateOctree;
                renderContext.EnableBoundingFrustum = EnableRenderFrustum;               
                if (RenderConfiguration.UpdatePerFrameData)
                {
                    viewport.Update(t0);                    
                    renderContext.TimeStamp = t0;
                    renderContext.Camera = viewport.CameraCore;
                    renderContext.WorldMatrix = viewport.WorldMatrix;
                    renderContext.Camera = viewport.CameraCore;
                }
                PreRender();
                try
                {                    
                    if (renderBuffer.BeginDraw())
                    {
                        OnRender(t0);
                        renderBuffer.EndDraw();
                    }
                    if (RenderConfiguration.RenderD2D)
                    { OnRender2D(t0); }
                    renderBuffer.Present();
                }
                catch (SharpDXException ex)
                {
                    var desc = ResultDescriptor.Find(ex.ResultCode);
                    if (desc == global::SharpDX.DXGI.ResultCode.DeviceRemoved || desc == global::SharpDX.DXGI.ResultCode.DeviceReset 
                        || desc == global::SharpDX.DXGI.ResultCode.DeviceHung || desc == global::SharpDX.Direct2D1.ResultCode.RecreateTarget
                        || desc == global::SharpDX.DXGI.ResultCode.AccessLost)
                    {
                        Log(LogLevel.Warning, $"Device Lost, code = {desc.Code}");
                        RenderBuffer_OnDeviceLost(RenderBuffer, EventArgs.Empty);
                    }
                    else
                    {
                        Log(LogLevel.Error, ex);
                        EndD3D();
                        ExceptionOccurred?.Invoke(this, new RelayExceptionEventArgs(ex));
                    }
                }
                catch(Exception ex)
                {
                    Log(LogLevel.Error, ex);
                    EndD3D();
                    ExceptionOccurred?.Invoke(this, new RelayExceptionEventArgs(ex));  
                }
                finally
                {
                    PostRender();
                    IsBusy = false;
                }
                lastRenderingDuration = TimeSpan.FromSeconds((double)Stopwatch.GetTimestamp() / Stopwatch.Frequency) - t0;
                RenderStatistics.LatencyStatistics.Push(lastRenderingDuration.TotalMilliseconds);                
            }
        }

        /// <summary>
        /// Clears the render target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="clearBackBuffer">if set to <c>true</c> [clear back buffer].</param>
        /// <param name="clearDepthStencilBuffer">if set to <c>true</c> [clear depth stencil buffer].</param>
        public void ClearRenderTarget(DeviceContext context, bool clearBackBuffer,bool clearDepthStencilBuffer)
        {
            renderBuffer?.ClearRenderTarget(context, ClearColor, clearBackBuffer, clearDepthStencilBuffer);
        }
        /// <summary>
        /// Called before OnRender.
        /// </summary>
        protected virtual void PreRender()
        {
#if DX11_1
            SetDefaultRenderTargets(Device.ImmediateContext1, RenderConfiguration.ClearEachFrame);
#else
            SetDefaultRenderTargets(Device.ImmediateContext, RenderConfiguration.ClearEachFrame);
#endif
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
            Log(LogLevel.Information, $"Init = {IsInitialized}; HotRestart = {hotRestart};");
            if (!IsInitialized)
            {
                return;
            }
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
            Log(LogLevel.Information, $"Width = {width}; Height = {height};");
            ActualWidth = width;
            ActualHeight = height;
            isLoaded = true;
            if (EffectsManager == null || EffectsManager.Device == null || EffectsManager.Device.IsDisposed)
            {
                Log(LogLevel.Information, $"EffectsManager is not valid");
                return;
            }
            CreateAndBindBuffers();
            IsInitialized = true;
            AttachRenderable(EffectsManager);
            StartRendering();
        }
        /// <summary>
        /// Starts the rendering.
        /// </summary>
        protected virtual void StartRendering()
        {
            Log(LogLevel.Information, "");
            RenderStatistics.Reset();
            lastRenderingDuration = TimeSpan.Zero;
            lastRenderTime = TimeSpan.Zero;
            InvalidateRender();
            StartRenderLoop?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Creates the and bind buffers.
        /// </summary>
        protected void CreateAndBindBuffers()
        {
            Log(LogLevel.Information, "");
            RemoveAndDispose(ref renderBuffer);
            renderBuffer = Collect(CreateRenderBuffer());
            renderBuffer.OnNewBufferCreated += RenderBuffer_OnNewBufferCreated;
            renderBuffer.OnDeviceLost += RenderBuffer_OnDeviceLost;

            RemoveAndDispose(ref renderer);
            renderer = Collect(CreateRenderer());
            OnInitializeBuffers(renderBuffer, renderer);
        }

        private void RenderBuffer_OnDeviceLost(object sender, EventArgs e)
        {
            EndD3D();
            EffectsManager?.OnDeviceError();
            StartD3D(ActualWidth, ActualHeight);
        }

        /// <summary>
        /// Creates the renderer.
        /// </summary>
        /// <returns></returns>
        private IRenderer CreateRenderer()
        {
            if (createRendererFunction != null)
            {
                return createRendererFunction.Invoke(Device);
            }
            else
            {
                return new ImmediateContextRenderer(Device);
            }
        }

        private void RenderBuffer_OnNewBufferCreated(object sender, Texture2DArgs e)
        {
            OnNewRenderTargetTexture?.Invoke(this, e);
        }
        /// <summary>
        /// Called when [initialize buffers].
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="renderer">The renderer.</param>
        protected virtual void OnInitializeBuffers(IDX11RenderBufferProxy buffer, IRenderer renderer)
        {
            buffer.Initialize((int)ActualWidth, (int)ActualHeight, MSAA);
        }

        /// <summary>
        /// Attaches the renderable.
        /// </summary>
        /// <param name="deviceResources">The device resources.</param>
        protected virtual void AttachRenderable(IDeviceResources deviceResources)
        {
            if (!IsInitialized || Viewport == null)
            { return; }
            Log(LogLevel.Information, "");
            if (EnableSharingModelMode && SharedModelContainer != null)
            {
                SharedModelContainer.CurrentRenderHost = this;
                viewport.Attach(SharedModelContainer);
            }
            else
            {
                viewport.Attach(this);
            }
#if DX11_1
            renderContext = Collect(CreateRenderContext(deviceResources.Device.ImmediateContext1));
#else
            renderContext = Collect(CreateRenderContext(deviceResources.Device.ImmediateContext));
#endif

            renderContext2D = Collect(CreateRenderContext2D(deviceResources.DeviceContext2D));
        }
        /// <summary>
        /// Creates the render context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual RenderContext CreateRenderContext(DeviceContext context)
        {
            return new RenderContext(this, context);
        }
        /// <summary>
        /// Creates the render context2 d.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected virtual RenderContext2D CreateRenderContext2D(global::SharpDX.Direct2D1.DeviceContext context)
        {
            return new RenderContext2D(context, this);
        }
        /// <summary>
        /// 
        /// </summary>
        public void EndD3D()
        {
            Log(LogLevel.Information, "");
            isLoaded = false;
            StopRendering();
            IsInitialized = false;
            OnEndingD3D();
            DetachRenderable();
            DisposeBuffers();
        }
        /// <summary>
        /// Called when [ending d3 d].
        /// </summary>
        protected virtual void OnEndingD3D() { }

        private void OnManagerDisposed(object sender, EventArgs args)
        {
            Log(LogLevel.Information, "");
            EndD3D();
        }
        /// <summary>
        /// Stops the rendering.
        /// </summary>
        protected virtual void StopRendering()
        {
            Log(LogLevel.Information, "");
            StopRenderLoop?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Disposes the buffers.
        /// </summary>
        protected virtual void DisposeBuffers()
        {
            Log(LogLevel.Information, "");
            if (renderBuffer != null)
            {
                renderBuffer.OnNewBufferCreated -= RenderBuffer_OnNewBufferCreated;
                renderBuffer.OnDeviceLost -= RenderBuffer_OnDeviceLost;
            }
            RemoveAndDispose(ref renderer);
            RemoveAndDispose(ref renderBuffer);
        }
        /// <summary>
        /// Detaches the renderable.
        /// </summary>
        protected virtual void DetachRenderable()
        {
            Log(LogLevel.Information, "");
            RemoveAndDispose(ref renderContext);
            RemoveAndDispose(ref renderContext2D);
            Viewport?.Detach();
        }
        /// <summary>
        /// Resizes
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void Resize(double width, double height)
        {
            if(ActualWidth == width && ActualHeight == height)
            {
                return;
            }
            ActualWidth = width;
            ActualHeight = height;
            Log(LogLevel.Information, $"Width = {width}; Height = {height};");
            if (IsInitialized)
            {
                StopRendering();
                var texture = renderBuffer.Resize((int)ActualWidth, (int)ActualHeight);
                OnNewRenderTargetTexture?.Invoke(this, new Texture2DArgs(texture));
                if (Viewport != null)
                {
                    var overlay = Viewport.D2DRenderables.FirstOrDefault();
                    if (overlay != null)
                    {
                        overlay.InvalidateAll();
                    }
                }
                StartRendering();
            }
        }
        /// <summary>
        /// Sets the default render targets.
        /// </summary>
        /// <param name="clear">if set to <c>true</c> [clear].</param>
        public virtual void SetDefaultRenderTargets(bool clear)
        {
#if DX11_1
            SetDefaultRenderTargets(Device.ImmediateContext1, clear);
#else
            SetDefaultRenderTargets(Device.ImmediateContext, clear);
#endif
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            IsInitialized = false;
            OnNewRenderTargetTexture = null;
            ExceptionOccurred = null;
            StartRenderLoop = null;
            StopRenderLoop = null;
            base.OnDispose(disposeManagedResources);
        }

        private void Log<Type>(LogLevel level, Type msg, [CallerMemberName]string caller = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(level, msg, nameof(DX11RenderHostBase), caller, sourceLineNumber);
        }
    }
}
