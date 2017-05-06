// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DPFCanvas.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;

    using global::SharpDX;

    using global::SharpDX.Direct3D11;

    using global::SharpDX.DXGI;

    using HelixToolkit.Wpf.SharpDX.Utilities;

    using HelixToolkit.Wpf.SharpDX.Extensions;
    using Device = global::SharpDX.Direct3D11.Device;
    using Model.Lights3D;
    using Helpers;
    using System.Threading;
    using System.Runtime.CompilerServices;

    // ---- BASED ON ORIGNAL CODE FROM DPFCanvas.cs-----
    // Seperate the rendering thread from Main Composite rendering thread. Use deferred rendering.
    // Refer to https://msdn.microsoft.com/en-us/library/windows/desktop/ff476892(v=vs.85).aspx
    // https://blogs.msdn.microsoft.com/dwayneneed/2007/04/26/multithreaded-ui-hostvisual/
    public class DPFCanvasThreading : VisualWrapper, IRenderHost
    {
        private sealed class RenderThreadWrapper
        {
            public bool IsInitalized { get { return surfaceD3D != null && renderContext != null; } }
            public volatile bool IsBusy = false;
            private Thread RenderThread;
            private AutoResetEvent renderThreadEvent = new AutoResetEvent(false);
            private Image image;
            private VisualTargetPresentationSource visualTarget;
            private DX11ImageSource surfaceD3D;
            private DeviceContext renderContext;
            private Texture2D colorBuffer;
#if MSAA
            private Texture2D renderTargetNMS;
#endif

            public HostVisual InitializeRenderThread()
            {
                HostVisual hostVisual = new HostVisual();
                RenderThread = new Thread(new ParameterizedThreadStart(RenderGraphics));
                RenderThread.SetApartmentState(ApartmentState.STA);
                RenderThread.IsBackground = true;
                RenderThread.Start(hostVisual);
                renderThreadEvent.WaitOne();
                return hostVisual;
            }

            public void DestoryRenderThread()
            {
                SynchronizeToCurrentThread(() => {
                    image.Source = null;
                    Disposer.RemoveAndDispose(ref surfaceD3D);
                });
                image.Dispatcher.InvokeShutdown();
            }

            private void RenderGraphics(object arg)
            {
                var hostVisual = (HostVisual)arg;
                visualTarget = new VisualTargetPresentationSource(hostVisual);                
                renderThreadEvent.Set();
                image = new Image() { Stretch = Stretch.UniformToFill };
                visualTarget.RootVisual = image;
                Dispatcher.Run();
            }

            public void CreateImageSource(int adapterIdx)
            {
                SynchronizeToCurrentThread(() => {
                    image.Source = null;
                    Disposer.RemoveAndDispose(ref surfaceD3D);
                    surfaceD3D = new DX11ImageSource(adapterIdx);
                    image.Source = surfaceD3D;
                });
            }

            public bool InvalidateD3D(CommandList command)
            {
                if (IsBusy)
                { return false; }
                IsBusy = true;
                AsyncInvoke(() => {
                    Invalidate(command);
                }, DispatcherPriority.Render);
                return true;
            }

            private void Invalidate(CommandList command)
            {
                if (!IsInitalized)
                {
                    return;
                }
                renderContext.ExecuteCommandList(command, true);
#if MSAA
                renderContext.ResolveSubresource(colorBuffer, 0, renderTargetNMS, 0, Format.B8G8R8A8_UNorm);
#endif
                renderContext.Flush();
                surfaceD3D.InvalidateD3DImage();
                Disposer.RemoveAndDispose(ref command);
                IsBusy = false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void SynchronizeToCurrentThread(Action action)
            {
                visualTarget?.Dispatcher?.Invoke(action);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void AsyncInvoke(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
            {
                visualTarget?.Dispatcher?.BeginInvoke(action, priority);
            }

            public void SetRenderTarget(Texture2D target)
            {
                SynchronizeToCurrentThread(() => {
                    surfaceD3D?.SetRenderTargetDX11(target);
                });
            }

            public void SetRenderDevice(Device device, Texture2D colorBuffer, Texture2D renderTarget)
            {
                this.renderContext = device.ImmediateContext;
                this.colorBuffer = colorBuffer;
                this.renderTargetNMS = renderTarget;
            }
        }
        private readonly RenderThreadWrapper mRenderThread = new RenderThreadWrapper();
        private readonly Stopwatch renderTimer;
        private Device device;
        private Texture2D colorBuffer;
        private Texture2D depthStencilBuffer;
        private RenderTargetView colorBufferView;
        private DepthStencilView depthStencilBufferView;

        private IRenderer renderRenderable;
        private RenderContext renderContext;
        private DeferredRenderer deferredRenderer;
        private bool sceneAttached;
        private int targetWidth, targetHeight;
        private int pendingValidationCycles;
        private TimeSpan lastRenderingDuration;
        private RenderTechnique deferred;
        private RenderTechnique gbuffer;



        private int renderCycles = 1;
        /// <summary>
        /// Set render cycles to 2 if experiences lagging during rotation. Benefits on some old laptop graphics cards.
        /// Default value = <value>1</value>
        /// </summary>
        public int RenderCycles
        {
            set { renderCycles = value; }
            get { return renderCycles; }
        }

        public RenderContext RenderContext { get { return renderContext; } }

        private readonly Light3DSceneShared light3DPerScene = new Light3DSceneShared();
        /// <summary>
        /// Light3D shared data per each secne
        /// </summary>
        public Light3DSceneShared Light3DSceneShared { get { return light3DPerScene; } }
#if MSAA
        private Texture2D renderTargetNMS;
#endif

        /// <summary>
        /// Fired whenever an exception occurred on this object.
        /// </summary>
        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred = delegate { };

        /// <summary>
        /// 
        /// </summary>
        public Color4 ClearColor { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsShadowMapEnabled { get; private set; }

#if MSAA
        /// <summary>
        /// Set MSAA level. If set to Two/Four/Eight, the actual level is set to minimum between Maximum and Two/Four/Eight
        /// </summary>
        public MSAALevel MSAA { get; set; }
#endif
        /// <summary>
        /// Gets or sets the maximum time that rendering is allowed to take. When exceeded,
        /// the next cycle will be enqueued at <see cref="DispatcherPriority.Input"/> to reduce input lag.
        /// </summary>
        public TimeSpan MaxRenderingDuration { get; set; }

        private uint mMaxFPS = 60;
        public uint MaxFPS
        {
            set
            {
                mMaxFPS = value;
                skipper.Threshold = (long)Math.Floor(1000.0 / mMaxFPS);
            }
            get
            {
                return mMaxFPS;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public RenderTechnique RenderTechnique
        {
            get { return renderTechnique; }
            private set
            {
                renderTechnique = value;
                IsDeferredLighting = RenderTechniquesManager != null && (renderTechnique == RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.Deferred)
                    || renderTechnique == RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.GBuffer));
            }
        }
        private RenderTechnique renderTechnique;

        public bool IsDeferredLighting { private set; get; } = false;

        private bool enableRenderFrustum = false;
        public bool EnableRenderFrustum
        {
            set
            {
                enableRenderFrustum = value;
                if (renderContext != null)
                {
                    renderContext.EnableBoundingFrustum = value;
                }
            }
            get
            {
                return enableRenderFrustum;
            }
        }
        /// <summary>
        /// The instance of currently attached IRenderable - this is in general the Viewport3DX
        /// </summary>
        IRenderer IRenderHost.Renderable
        {
            get { return renderRenderable; }
            set
            {
                if (ReferenceEquals(renderRenderable, value))
                {
                    return;
                }

                if (renderRenderable != null)
                {
                    renderRenderable.Detach();
                    renderRenderable = null;
                }

                sceneAttached = false;
                renderRenderable = value;
                InvalidateRender();
            }
        }

        /// <summary>
        /// The currently used Direct3D Device
        /// </summary>
        Device IRenderHost.Device
        {
            get { return device; }
        }

        public static readonly DependencyProperty EffectsManagerProperty =
            DependencyProperty.Register("EffectsManager", typeof(IEffectsManager), typeof(DPFCanvasThreading),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
                (s, e) => ((DPFCanvasThreading)s).EffectsManagerPropertyChanged()));

        public IEffectsManager EffectsManager
        {
            get { return (IEffectsManager)GetValue(EffectsManagerProperty); }
            set { SetValue(EffectsManagerProperty, value); }
        }

        public static readonly DependencyProperty RenderTechniquesManagerProperty =
            DependencyProperty.Register("RenderTechniquesManager", typeof(IRenderTechniquesManager), typeof(DPFCanvasThreading),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
                (s, e) => ((DPFCanvasThreading)s).RenderTechniquesManagerPropertyChanged()));

        public IRenderTechniquesManager RenderTechniquesManager
        {
            get { return (IRenderTechniquesManager)GetValue(RenderTechniquesManagerProperty); }
            set { SetValue(RenderTechniquesManagerProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                var prop = DesignerProperties.IsInDesignModeProperty;
                return (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
            }
        }

        /// <summary>
        /// Indicates if DPFCanvas busy on rendering.
        /// </summary>
        public bool IsBusy { get { return pendingValidationCycles > 0; } }

        public bool EnableSharingModelMode
        {
            set; get;
        } = false;

        public IModelContainer SharedModelContainer
        {
            set; get;
        } = null;

        /// <summary>
        /// 
        /// </summary>
        private DispatcherOperation pendingInvalidateOperation = null;
        /// <summary>
        /// 
        /// </summary>
        private readonly Action invalidAction;

        /// <summary>
        /// 
        /// </summary>
        public DPFCanvasThreading()
        {
            renderTimer = new Stopwatch();
            MaxRenderingDuration = TimeSpan.FromMilliseconds(20.0);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            ClearColor = global::SharpDX.Color.Gray;
            IsShadowMapEnabled = false;
            MSAA = MSAALevel.Maximum;
            invalidAction = new Action(InvalidateRender);
        }

        /// <summary>
        /// Invalidates the current render and requests an update.
        /// </summary>
        private void InvalidateRender()
        {
            if (RenderCycles == 1)
            {
                System.Threading.Interlocked.CompareExchange(ref pendingValidationCycles, RenderCycles, 0);
            }
            else
            {
                //Use pendingInvalidateOperation to check if there is a pending operation.
                if (pendingInvalidateOperation != null)
                {
                    switch (pendingInvalidateOperation.Status)
                    {
                        case DispatcherOperationStatus.Pending:
                            //If there is a pending invalidation operation, try to set cycle to 2.
                            //Does not matter if it is failed or not, since the pending one will eventually invalidate.
                            //But this is required for mouse rotation, because it requires invalidate asap (Input priority is higher than background).
                            System.Threading.Interlocked.CompareExchange(ref pendingValidationCycles, RenderCycles, 0);
                            return;
                    }
                    pendingInvalidateOperation = null;
                }
                // For some reason, we need two render cycles to recover from 
                // UAC popup or sleep when MSAA is enabled.
                if (System.Threading.Interlocked.CompareExchange(ref pendingValidationCycles, RenderCycles, 0) != 0)
                {
                    //If invalidate failed, schedule an async operation to invalidate in future
                    pendingInvalidateOperation
                        = Dispatcher.BeginInvoke(invalidAction, DispatcherPriority.Background);
                }
            }
        }


        /// <summary>
        /// Invalidates the current render and requests an update.
        /// </summary>
        void IRenderHost.InvalidateRender()
        {
            InvalidateRender();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
            {
                return;
            }

            try
            {
                Child = mRenderThread.InitializeRenderThread();
                StartD3D();
                StartRendering();
            }
            catch (Exception ex)
            {
                // Exceptions in the Loaded event handler are silently swallowed by WPF.
                // https://social.msdn.microsoft.com/Forums/vstudio/en-US/9ed3d13d-0b9f-48ac-ae8d-daf0845c9e8f/bug-in-wpf-windowloaded-exception-handling?forum=wpf
                // http://stackoverflow.com/questions/19140593/wpf-exception-thrown-in-eventhandler-is-swallowed
                // tl;dr: M$ says it's "by design" and "working as indended" but may change in the future :).

                if (!HandleExceptionOccured(ex))
                {
                    MessageBox.Show(string.Format("DPFCanvas: Error starting rendering: {0}", ex.Message), "Error");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
            {
                return;
            }

            StopRendering();
            EndD3D();
            mRenderThread.DestoryRenderThread();
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartD3D()
        {
            if (EffectsManager == null || RenderTechniquesManager == null)
            {
                // Make sure both are null
                RenderTechniquesManager = null;
                EffectsManager = null;
                RenderTechniquesManager = new DefaultRenderTechniquesManager();
                EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
                return; // StardD3D() is called from DP changed handler
            }
            mRenderThread.CreateImageSource(EffectsManager.AdapterIndex);
            //   surfaceD3D = new DX11ImageSource(EffectsManager.AdapterIndex);
            //   surfaceD3D.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;
            device = EffectsManager.Device;
            deferredRenderer = new DeferredRenderer();
            renderRenderable.DeferredRenderer = deferredRenderer;

            CreateAndBindTargets();
            SetDefaultRenderTargets();
            //  Source = surfaceD3D;
            InvalidateRender();
        }


        /// <summary>
        /// 
        /// </summary>
        private void EndD3D()
        {
            if (renderRenderable != null)
            {
                renderRenderable.Detach();
                sceneAttached = false;
            }

            //if (surfaceD3D == null)
            //{
            //    return;
            //}

            // surfaceD3D.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
            // Source = null;

            Disposer.RemoveAndDispose(ref deferredRenderer);
            // Disposer.RemoveAndDispose(ref surfaceD3D);
            Disposer.RemoveAndDispose(ref colorBufferView);
            Disposer.RemoveAndDispose(ref depthStencilBufferView);
            Disposer.RemoveAndDispose(ref colorBuffer);
            Disposer.RemoveAndDispose(ref depthStencilBuffer);
#if MSAA
            Disposer.RemoveAndDispose(ref renderTargetNMS);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateAndBindTargets()
        {
            // surfaceD3D.SetRenderTargetDX11(null);
            mRenderThread.SetRenderTarget(null);
            int width = System.Math.Max((int)ActualWidth, 100);
            int height = System.Math.Max((int)ActualHeight, 100);

            Disposer.RemoveAndDispose(ref colorBufferView);
            Disposer.RemoveAndDispose(ref depthStencilBufferView);
            Disposer.RemoveAndDispose(ref colorBuffer);
            Disposer.RemoveAndDispose(ref depthStencilBuffer);
#if MSAA
            Disposer.RemoveAndDispose(ref renderTargetNMS);

            int sampleCount = 1;
            int sampleQuality = 0;
            if (MSAA != MSAALevel.Disable)
            {
                do
                {
                    var newSampleCount = sampleCount * 2;
                    var newSampleQuality = device.CheckMultisampleQualityLevels(Format.B8G8R8A8_UNorm, newSampleCount) - 1;

                    if (newSampleQuality < 0)
                        break;

                    sampleCount = newSampleCount;
                    sampleQuality = newSampleQuality;
                    if (sampleCount == (int)MSAA)
                    {
                        break;
                    }
                } while (sampleCount < 32);
            }

            var sampleDesc = new SampleDescription(sampleCount, sampleQuality);
            var optionFlags = ResourceOptionFlags.None;
#else
            var sampleDesc = new SampleDescription(1, 0);
            var optionFlags = ResourceOptionFlags.Shared;
#endif

            var colordesc = new Texture2DDescription
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = sampleDesc,
                Usage = ResourceUsage.Default,
                OptionFlags = optionFlags,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            };

            var depthdesc = new Texture2DDescription
            {
                BindFlags = BindFlags.DepthStencil,
                //Format = Format.D24_UNorm_S8_UInt,
                Format = Format.D32_Float_S8X24_UInt,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = sampleDesc,
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1,
            };

            colorBuffer = new Texture2D(device, colordesc);
            depthStencilBuffer = new Texture2D(device, depthdesc);

            colorBufferView = new RenderTargetView(device, colorBuffer);
            depthStencilBufferView = new DepthStencilView(device, depthStencilBuffer);

#if MSAA
            var colordescNMS = new Texture2DDescription
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.Shared,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            };

            renderTargetNMS = new Texture2D(device, colordescNMS);
            device.ImmediateContext.ResolveSubresource(colorBuffer, 0, renderTargetNMS, 0, Format.B8G8R8A8_UNorm);
            mRenderThread.SetRenderTarget(renderTargetNMS);
            //surfaceD3D.SetRenderTargetDX11(renderTargetNMS);
#else
            this.surfaceD3D.SetRenderTargetDX11(this.colorBuffer);
#endif            
            mRenderThread.SetRenderDevice(this.device, colorBuffer, renderTargetNMS);
        }

        /// <summary>
        /// Sets the default render-targets
        /// </summary>
        public void SetDefaultRenderTargets(bool clear = true)
        {
            SetDefaultRenderTargets(colorBuffer.Description.Width, colorBuffer.Description.Height, clear);
        }

        /// <summary>
        /// Sets the default render-targets
        /// </summary>
        public void SetDefaultRenderTargets(int width, int height, bool clear = true)
        {
            targetWidth = width;
            targetHeight = height;

            device.ImmediateContext.OutputMerger.SetTargets(depthStencilBufferView, colorBufferView);
            device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height, 0.0f, 1.0f);
            this.device.ImmediateContext.Rasterizer.SetScissorRectangle(0, 0, width, height);

            renderContext?.DeviceContext.OutputMerger.SetTargets(depthStencilBufferView, colorBufferView);
            renderContext?.DeviceContext.Rasterizer.SetViewport(0, 0, width, height, 0f, 1f);
            renderContext?.DeviceContext.Rasterizer.SetScissorRectangle(0, 0, width, height);
            if (clear)
            {
                ClearRenderTarget();
            }
        }

        /// <summary>
        /// Sets the default render-targets
        /// </summary>
        public void SetDefaultColorTargets(DepthStencilView dsv)
        {
            targetWidth = colorBuffer.Description.Width;
            targetHeight = colorBuffer.Description.Height;

            device.ImmediateContext.OutputMerger.SetTargets(dsv, colorBufferView);
            device.ImmediateContext.Rasterizer.SetViewport(0, 0, colorBuffer.Description.Width, colorBuffer.Description.Height, 0.0f, 1.0f);
        }

        /// <summary>
        /// Clears the buffers with the clear-color
        /// </summary>
        /// <param name="clearBackBuffer"></param>
        /// <param name="clearDepthStencilBuffer"></param>
        internal void ClearRenderTarget(bool clearBackBuffer = true, bool clearDepthStencilBuffer = true)
        {
            if (clearBackBuffer)
            {
               // device.ImmediateContext.ClearRenderTargetView(colorBufferView, ClearColor);
                renderContext?.DeviceContext.ClearRenderTargetView(colorBufferView, ClearColor);
            }

            if (clearDepthStencilBuffer)
            {
              //  device.ImmediateContext.ClearDepthStencilView(depthStencilBufferView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
                renderContext?.DeviceContext.ClearDepthStencilView(depthStencilBufferView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
            }
        }

        private void Render(TimeSpan timeStamp)
        {
            var device = this.device;
            if (device == null)
                return;

            var renderTarget = colorBuffer;
            if (renderTarget == null)
                return;
            if (renderRenderable != null)
            {
                // ---------------------------------------------------------------------------
                // this part is done only if the scene is not attached
                // it is an attach and init pass for all elements in the scene-graph                
                if (!sceneAttached)
                {
                    try
                    {
                        Light3DSceneShared.Reset();
                        sceneAttached = true;
                        ClearColor = renderRenderable.BackgroundColor;
                        IsShadowMapEnabled = renderRenderable.IsShadowMappingEnabled;

                        RenderTechnique = renderRenderable.RenderTechnique == null ? RenderTechniquesManager?.RenderTechniques[DefaultRenderTechniqueNames.Blinn] : renderRenderable.RenderTechnique;

                        if (renderContext != null)
                        {
                            renderContext.Dispose();
                        }
                        renderContext = new RenderContext(this, EffectsManager.GetEffect(RenderTechnique), new DeviceContext(device));
                        renderContext.EnableBoundingFrustum = EnableRenderFrustum;
                        if (EnableSharingModelMode && SharedModelContainer != null)
                        {
                            SharedModelContainer.CurrentRenderHost = this;
                            renderRenderable.Attach(SharedModelContainer);
                        }
                        else
                        {
                            renderRenderable.Attach(this);
                        }

                        RenderTechniquesManager.RenderTechniques.TryGetValue(DeferredRenderTechniqueNames.GBuffer, out gbuffer);
                        RenderTechniquesManager.RenderTechniques.TryGetValue(DeferredRenderTechniqueNames.Deferred, out deferred);

                        if (RenderTechnique == deferred)
                        {
                            deferredRenderer.InitBuffers(this, Format.R32G32B32A32_Float);
                        }
                        else if (RenderTechnique == gbuffer)
                        {
                            deferredRenderer.InitBuffers(this, Format.B8G8R8A8_UNorm);
                        }
                        SetDefaultRenderTargets();
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("DPFCanvas: Error attaching element: " + string.Format(ex.Message), "Error");
                        Debug.WriteLine("DPFCanvas: Error attaching element: " + string.Format(ex.Message), "Error");
                        throw;
                    }
                }

                // ---------------------------------------------------------------------------
                // this part is per frame
                // ---------------------------------------------------------------------------
                renderContext.TimeStamp = timeStamp;
                if (EnableSharingModelMode && SharedModelContainer != null)
                {
                    SharedModelContainer.CurrentRenderHost = this;
                }
                else
                {
                    ClearRenderTarget();
                }

                if (RenderTechnique == deferred)
                {
                    // set G-Buffer                    
                    deferredRenderer.SetGBufferTargets(renderContext);
                    // render G-Buffer pass                
                    renderRenderable.Render(renderContext);

                    // call deferred render 
                    deferredRenderer.RenderDeferred(renderContext, renderRenderable);

                }
                else if (RenderTechnique == gbuffer)
                {
                    // set G-Buffer
                    deferredRenderer.SetGBufferTargets(targetWidth / 2, targetHeight / 2, renderContext);
                    // render G-Buffer pass                    
                    renderRenderable.Render(renderContext);

                    // reset render targets and run lighting pass                                         
#if MSAA
                    deferredRenderer.RenderGBufferOutput(renderContext, ref renderTargetNMS);
#else
                    this.deferredRenderer.RenderGBufferOutput(ref this.colorBuffer);
#endif
                }
                else
                {
                    renderRenderable.Render(renderContext);
                }
                var command = renderContext.DeviceContext.FinishCommandList(true);
                mRenderThread.InvalidateD3D(command);
            }
            //#if MSAA
            //            device.ImmediateContext.ResolveSubresource(colorBuffer, 0, renderTargetNMS, 0, Format.B8G8R8A8_UNorm);
            //#endif
            //            this.device.ImmediateContext.Flush();
        }

        private void StartRendering()
        {
            if (renderTimer.IsRunning)
                return;

            lastRenderingDuration = TimeSpan.Zero;
            CompositionTarget.Rendering += OnRendering;
            renderTimer.Start();
        }

        private void StopRendering()
        {
            if (!renderTimer.IsRunning)
                return;

            CompositionTarget.Rendering -= OnRendering;
            renderTimer.Stop();
        }

        /// <summary>
        /// Handles the <see cref="CompositionTarget.Rendering"/> event.
        /// </summary>
        /// <param name="sender">The sender is in fact a the UI <see cref="Dispatcher"/>.</param>
        /// <param name="e">Is in fact <see cref="RenderingEventArgs"/>.</param>
        private void OnRendering(object sender, EventArgs e)
        {

            if (!renderTimer.IsRunning)
                return;
            UpdateAndRender();
        }

        private readonly EventSkipper skipper = new EventSkipper();
        /// <summary>
        /// Updates and renders the scene.
        /// </summary>
        private void UpdateAndRender()
        {
            try
            {
                if (pendingValidationCycles > 0 && !mRenderThread.IsBusy && !skipper.IsSkip())
                {
                    var t0 = renderTimer.Elapsed;
                    if (mRenderThread.IsInitalized && renderRenderable != null)
                    {
                        // Update all renderables before rendering 
                        // giving them the chance to invalidate the current render.                                                            
                        //renderRenderable.Update(t0);
                        var cycle = System.Threading.Interlocked.Decrement(ref pendingValidationCycles);
                        if (cycle == RenderCycles - 1)
                        {
                            Render(t0);
                        }
                        //if (cycle == 0)
                        //{
                        //    surfaceD3D.InvalidateD3DImage();
                        //}
                    }

                    lastRenderingDuration = renderTimer.Elapsed - t0;
                }
            }
            catch (Exception ex)
            {
                if (!HandleExceptionOccured(ex))
                {
                    MessageBox.Show(string.Format("DPFCanvas: Error while rendering: {0}", ex.Message), "Error");
                }
            }
        }

        private DispatcherOperation resizeOperation = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (resizeOperation != null && resizeOperation.Status == DispatcherOperationStatus.Pending)
            {
                resizeOperation.Abort();
            }
            resizeOperation = Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
            {
                if (mRenderThread.IsInitalized)
                {
                    if (RenderTechnique != null)
                    {
                        if (RenderTechnique == deferred)
                        {
                            deferredRenderer.InitBuffers(this, Format.R32G32B32A32_Float);
                        }
                        else if (RenderTechnique == gbuffer)
                        {
                            deferredRenderer.InitBuffers(this, Format.B8G8R8A8_UNorm);
                        }
                    }
                    StopRendering();
                    CreateAndBindTargets();
                    SetDefaultRenderTargets();
                    StartRendering();
                    InvalidateRender();
                }
            }));
        }

        /// <summary>
        /// Handles the change of the effects manager.
        /// </summary>
        private void EffectsManagerPropertyChanged()
        {
            if (EffectsManager != null && RenderTechniquesManager != null)
            {
                StartD3D();
            }
        }

        /// <summary>
        /// Invoked whenever an exception occurs. Stops rendering, frees resources and throws 
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        /// <returns><c>true</c> if the exception has been handled, <c>false</c> otherwise.</returns>
        private bool HandleExceptionOccured(Exception exception)
        {
            pendingValidationCycles = 0;
            StopRendering();
            EndD3D();

            var args = new RelayExceptionEventArgs(exception);
            ExceptionOccurred(this, args);
            return args.Handled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // We don't need to handle this on NET45+ because of software fallback (Remote Desktop, Sleep).
            // See: http://msdn.microsoft.com/en-us/library/hh140978%28v=vs.110%29.aspx
#if NET40
            // this fires when the screensaver kicks in, the machine goes into sleep or hibernate
            // and any other catastrophic losses of the d3d device from WPF's point of view
            if (this.surfaceD3D.IsFrontBufferAvailable)
            {
                // We need to re-create the render targets because the get lost on NET40.
                this.CreateAndBindTargets();
                this.SetDefaultRenderTargets();
                this.InvalidateRender();
                this.StartRendering();
            }
            else
            {
                this.StopRendering();
            }
#endif
        }

        /// <summary>
        /// Handles the change of the render techniques manager.       
        /// </summary>
        private void RenderTechniquesManagerPropertyChanged()
        {
            if (EffectsManager != null && RenderTechniquesManager != null)
            {
                StartD3D();
            }
        }
    }
}
