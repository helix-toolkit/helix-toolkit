/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
    using HelixToolkit.Logger;
    using Model.Scene;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Utilities;
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

        private DX11RenderBufferProxyBase renderBuffer;
        /// <summary>
        /// Gets the render buffer.
        /// </summary>
        /// <value>
        /// The render buffer.
        /// </value>
        public DX11RenderBufferProxyBase RenderBuffer { get { return renderBuffer; } }
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

        private DeviceContextProxy immediateDeviceContext;
        /// <summary>
        /// Gets the immediate device context.
        /// </summary>
        /// <value>
        /// The immediate device context.
        /// </value>
        public DeviceContextProxy ImmediateDeviceContext
        {
            get { return immediateDeviceContext; }
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
        public RenderContext RenderContext
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
        public RenderContext2D RenderContext2D
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
                        currentManager.OnInvalidateRenderer -= EffectsManager_OnInvalidateRenderer;
                    }
                    RemoveAndDispose(ref immediateDeviceContext);
                    if (effectsManager != null)
                    {
                        effectsManager.OnDisposeResources += OnManagerDisposed;
                        effectsManager.OnInvalidateRenderer += EffectsManager_OnInvalidateRenderer;
                        RenderTechnique = viewport == null || viewport.RenderTechnique == null ? EffectsManager?[DefaultRenderTechniqueNames.Blinn] : viewport.RenderTechnique;
                        FeatureLevel = effectsManager.Device.FeatureLevel;
#if DX11_1
                        immediateDeviceContext = Collect(new DeviceContextProxy(effectsManager.Device.ImmediateContext1, effectsManager.Device));
#else
                        immediateDeviceContext = Collect(new DeviceContextProxy(effectsManager.Device.ImmediateContext, effectsManager.Device));
#endif
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

        private void EffectsManager_OnInvalidateRenderer(object sender, EventArgs e)
        {
            InvalidateRender();
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
        } = true;

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
                return renderBuffer.ColorBuffer;
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
                return renderBuffer.DepthStencilBuffer;
            }
        }

        /// <summary>
        /// Gets the d2d controls.
        /// </summary>
        /// <value>
        /// The d2 d controls.
        /// </value>
        public D2DTargetProxy D2DTarget
        {
            get { return RenderBuffer.D2DTarget; }
        }

        /// <summary>
        /// Gets the render statistics.
        /// </summary>
        /// <value>
        /// The render statistics.
        /// </value>
        public IRenderStatistics RenderStatistics { get { return renderStatistics; } }
        protected readonly RenderStatistics renderStatistics = new RenderStatistics();
#region Perframe renderables
        /// <summary>
        /// Gets the current frame renderables for rendering.
        /// </summary>
        /// <value>
        /// The per frame renderable.
        /// </value>
        public abstract List<KeyValuePair<int, SceneNode>> PerFrameFlattenedScene { get; }
        /// <summary>
        /// Gets the per frame lights.
        /// </summary>
        /// <value>
        /// The per frame lights.
        /// </value>
        public abstract IEnumerable<LightNode> PerFrameLights { get; }
        /// <summary>
        /// Gets the post effects render cores for this frame
        /// </summary>
        /// <value>
        /// The post effects render cores.
        /// </value>
        public abstract List<SceneNode> PerFrameNodesWithPostEffect { get; }
        /// <summary>
        /// Gets the per frame render cores.
        /// </summary>
        /// <value>
        /// The per frame render cores.
        /// </value>
        public abstract List<SceneNode> PerFrameOpaqueNodes { get; }
        /// <summary>
        /// Gets the per frame transparent nodes.
        /// </summary>
        /// <value>
        /// The per frame transparent nodes.
        /// </value>
        public abstract List<SceneNode> PerFrameParticleNodes { get; }
        /// <summary>
        /// Gets the per frame transparent nodes.
        /// </summary>
        /// <value>
        /// The per frame transparent nodes.
        /// </value>
        public abstract List<SceneNode> PerFrameTransparentNodes { get; }
#endregion
#region Configuration
        /// <summary>
        /// Gets or sets a value indicating whether [show render statistics].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show render statistics]; otherwise, <c>false</c>.
        /// </value>
        public RenderDetail ShowRenderDetail
        {
            set
            {
                if (RenderStatistics.FrameDetail != value)
                {
                    RenderStatistics.FrameDetail = value;
                    InvalidateRender();
                }
            }
            get { return RenderStatistics.FrameDetail; }
        }

        public DX11RenderHostConfiguration RenderConfiguration { set; get; } 
            = new DX11RenderHostConfiguration() { UpdatePerFrameData = true, RenderD2D = true, RenderLights = true, ClearEachFrame = true, EnableOITRendering = true };
        /// <summary>
        /// Gets the feature level.
        /// </summary>
        /// <value>
        /// The feature level.
        /// </value>
        public global::SharpDX.Direct3D.FeatureLevel FeatureLevel { get; private set; } = global::SharpDX.Direct3D.FeatureLevel.Level_11_0;
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
        /// <summary>
        /// Occurs when each render frame finished rendering.
        /// </summary>
        public event EventHandler OnRendered;

        private readonly Func<IDevice3DResources, IRenderer> createRendererFunction;
#endregion

#region Private variables

        protected IRenderer renderer;
        /// <summary>
        /// The renderer
        /// </summary>
        public IRenderer Renderer { get { return renderer; } }
        /// <summary>
        /// The update requested
        /// </summary>
        protected volatile bool UpdateRequested = true;

        private TimeSpan lastRenderingDuration = TimeSpan.Zero;

        private TimeSpan lastRenderTime = TimeSpan.Zero;

        private int updateCounter = 0; // Used to render at least twice. D3DImage sometimes not getting refresh if only render once.

        private volatile bool UpdateSceneGraphRequested = true;

        private volatile bool UpdatePerFrameRenderableRequested = true;
#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DX11RenderHostBase"/> class.
        /// </summary>
        public DX11RenderHostBase() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DX11RenderHostBase"/> class.
        /// </summary>
        /// <param name="createRenderer">The create renderer.</param>
        public DX11RenderHostBase(Func<IDevice3DResources, IRenderer> createRenderer)
        {
            createRendererFunction = createRenderer;           
        }

        /// <summary>
        /// Creates the render buffer.
        /// </summary>
        /// <returns></returns>
        protected abstract DX11RenderBufferProxyBase CreateRenderBuffer();

        /// <summary>
        /// Invalidates the render.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvalidateRender()
        {
            UpdateRequested = true;
            updateCounter = 0;
        }
        /// <summary>
        /// Invalidates the scene graph, request a complete scene graph traverse during next frame.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvalidateSceneGraph()
        {
            UpdateSceneGraphRequested = true;
            InvalidatePerFrameRenderables();
        }
        /// <summary>
        /// Invalidates the per frame renderables.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvalidatePerFrameRenderables()
        {
            UpdatePerFrameRenderableRequested = true;
            InvalidateRender();
        }
        /// <summary>
        /// Determines whether this instance can render.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can render; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanRender()
        {
            return IsInitialized && IsRendering && (UpdateRequested || updateCounter < 2) && viewport != null && viewport.CameraCore != null && ActualWidth > 10 && ActualHeight > 10;
        }
        /// <summary>
        /// Updates the and render.
        /// </summary>
        public void UpdateAndRender()
        {
            if (CanRender())
            {
                if (EnableSharingModelMode && SharedModelContainer != null)
                {
                    SharedModelContainer.CurrentRenderHost = this;
                }
                IsBusy = true;
                var t0 = TimeSpan.FromSeconds((double)Stopwatch.GetTimestamp()/Stopwatch.Frequency);
                renderStatistics.FPSStatistics.Push((t0 - lastRenderTime).TotalMilliseconds);
                renderStatistics.Camera = viewport.CameraCore;
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
                    renderContext.OITWeightPower = RenderConfiguration.OITWeightPower;
                    renderContext.OITWeightDepthSlope = RenderConfiguration.OITWeightDepthSlope;
                    renderContext.OITWeightMode = RenderConfiguration.OITWeightMode;
                }
                bool updateSceneGraph = UpdateSceneGraphRequested;
                bool updatePerFrameRenderable = UpdatePerFrameRenderableRequested;
                UpdateSceneGraphRequested = false;
                UpdatePerFrameRenderableRequested = false;
                PreRender(updateSceneGraph, updatePerFrameRenderable);
                try
                {                    
                    if (renderBuffer.BeginDraw())
                    {
                        OnRender(t0);
                        renderBuffer.EndDraw();
                        renderStatistics.NumDrawCalls = renderer.ImmediateContext.ResetDrawCalls() + EffectsManager.DeviceContextPool.ResetDrawCalls();
                    }
                    if (RenderConfiguration.RenderD2D && D2DTarget.D2DTarget != null)
                    { OnRender2D(t0); }
                    renderBuffer.Present();
                }
                catch (SharpDXException ex)
                {
                    var desc = ResultDescriptor.Find(ex.ResultCode);
                    if (desc == global::SharpDX.DXGI.ResultCode.DeviceRemoved || desc == global::SharpDX.DXGI.ResultCode.DeviceReset 
                        || desc == global::SharpDX.DXGI.ResultCode.DeviceHung
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
                OnRendered?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Clears the render target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="clearBackBuffer">if set to <c>true</c> [clear back buffer].</param>
        /// <param name="clearDepthStencilBuffer">if set to <c>true</c> [clear depth stencil buffer].</param>
        public void ClearRenderTarget(DeviceContextProxy context, bool clearBackBuffer,bool clearDepthStencilBuffer)
        {
            renderBuffer?.ClearRenderTarget(context, ClearColor, clearBackBuffer, clearDepthStencilBuffer);
        }
        /// <summary>
        /// Called before OnRender.
        /// </summary>
        protected virtual void PreRender(bool invalidateSceneGraph, bool invalidatePerFrameRenderables)
        {
            SetDefaultRenderTargets(immediateDeviceContext, RenderConfiguration.ClearEachFrame);
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
        public bool SetDefaultRenderTargets(DeviceContextProxy context, bool clear = true)
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
            renderStatistics.Reset();
            lastRenderingDuration = TimeSpan.Zero;
            lastRenderTime = TimeSpan.Zero;
            InvalidateSceneGraph();
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
            renderer?.Detach();
            RemoveAndDispose(ref renderer);
            renderer = Collect(CreateRenderer());
            renderer.Attach(this);
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
                return createRendererFunction.Invoke(EffectsManager);
            }
            else
            {
                return new ImmediateContextRenderer(EffectsManager);
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
        protected virtual void OnInitializeBuffers(DX11RenderBufferProxyBase buffer, IRenderer renderer)
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
            renderContext = Collect(CreateRenderContext());
#else
            renderContext = Collect(CreateRenderContext());
#endif

            renderContext2D = Collect(CreateRenderContext2D(deviceResources.DeviceContext2D));
        }
        /// <summary>
        /// Creates the render context.
        /// </summary>
        /// <returns></returns>
        protected virtual RenderContext CreateRenderContext()
        {
            return new RenderContext(this);
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
            renderer?.Detach();
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
            SetDefaultRenderTargets(immediateDeviceContext, clear);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                EffectsManager = null;
                IsInitialized = false;
                OnNewRenderTargetTexture = null;
                ExceptionOccurred = null;
                StartRenderLoop = null;
                StopRenderLoop = null;
                OnRendered = null;                
            }
            base.OnDispose(disposeManagedResources);
        }

        private void Log<Type>(LogLevel level, Type msg, [CallerMemberName]string caller = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(level, msg, nameof(DX11RenderHostBase), caller, sourceLineNumber);
        }
    }
}
