// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DPFCanvas.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------
//#define DoubleBuffer
namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    using global::SharpDX;

    using global::SharpDX.Direct3D11;

    using global::SharpDX.DXGI;

    using Utilities;
    using Extensions;

    using Device = global::SharpDX.Direct3D11.Device;
    using Model;
    using Helpers;
    using System.Linq;
    using System.Windows.Interop;

    using Controls;
    using Core2D;
    using Utility;

    // ---- BASED ON ORIGNAL CODE FROM -----
    // Copyright (c) 2010-2012 SharpDX - Alexandre Mutel
    // 
    // Permission is hereby granted, free of charge, to any person obtaining a copy
    // of this software and associated documentation files (the "Software"), to deal
    // in the Software without restriction, including without limitation the rights
    // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    // copies of the Software, and to permit persons to whom the Software is
    // furnished to do so, subject to the following conditions:
    // 
    // The above copyright notice and this permission notice shall be included in
    // all copies or substantial portions of the Software.
    // 
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    // THE SOFTWARE.

    /// <summary>
    /// <para>Use HwndHost as rendering surface, swapchain for rendering. Much faster than using D3DImage.</para>
    /// <para>Render on seperate rendering thread. EnableSwapChainRendering=True to use this rendering method.</para>
    /// <para>Drawbacks: The rendering surface will cover all WPF controls in the same Viewport region. Move controls out of viewport region to solve this problem.</para>
    /// <para>For displaying ViewCube and CoordinateSystem, separate Model needs to create to render along with the other models. WPF viewport will not be visibled.</para>
    /// </summary>
    public class DPFSurfaceSwapChain : WinformHostExtend, IRenderHost
    {
        private readonly Stopwatch renderTimer;
        private Device device;
        private Texture2D depthStencilBuffer;
        private Texture2D colorBuffer;
        private RenderTargetView colorBufferView;
        private DepthStencilView depthStencilBufferView;
        private RenderControl surfaceD3D;
        private IRenderer renderRenderable;
        private RenderContext renderContext;
    //    private DeferredRenderer deferredRenderer;
        private bool sceneAttached;
        private int targetWidth, targetHeight;
        private bool pendingValidationCycles;
        private TimeSpan lastRenderingDuration;
        //private IRenderTechnique deferred;
        //private IRenderTechnique gbuffer;
        private Texture2D backBuffer;
        private bool loaded = false;
        private IEffectsManager defaultEffectsManager = null;
        private global::SharpDX.DXGI.SwapChain1 swapChain;
        public RenderTargetView ColorBufferView { get { return colorBufferView; } }
        public DepthStencilView DepthStencilBufferView { get { return depthStencilBufferView; } }
        public bool IsRendering { set; get; } = true;
        /// <summary>
        /// Get RenderContext
        /// </summary>
        public IRenderContext RenderContext { get { return renderContext; } }

        /// <summary>
        /// Light3D shared data per each secne
        /// </summary>
        public Light3DSceneShared Light3DSceneShared { get { return renderContext.LightScene; } }

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
        
        private MSAALevel msaa = MSAALevel.Disable;
        /// <summary>
        /// Set MSAA level. If set to Two/Four/Eight, the actual level is set to minimum between Maximum and Two/Four/Eight
        /// </summary>
        public MSAALevel MSAA
        {
            get { return msaa; }
            set
            {
                if (msaa != value)
                {
                    msaa = value;
                    if (renderTimer.IsRunning)
                    {
                        StopRendering();
                        EndD3D(false);
                        StartD3D();
                        StartRendering();
                    }
                }
            }
        }
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
        public IRenderTechnique RenderTechnique
        {
            get { return renderTechnique; }
            private set
            {
                renderTechnique = value;
                //IsDeferredLighting = RenderTechniquesManager != null && (renderTechnique == RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.Deferred)
                //    || renderTechnique == RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.GBuffer));
            }
        }
        private IRenderTechnique renderTechnique;

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

                DetachRenderables();
                renderRenderable = value;
                ParentControl = renderRenderable as UIElement;
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
        /// <summary>
        /// 
        /// </summary>
        public bool EnableSharingModelMode { set; get; } = false;

        /// <summary>
        /// 
        /// </summary>
        public IModelContainer SharedModelContainer { set; get; } = null;

        private IEffectsManager effectsManager;
        public IEffectsManager EffectsManager
        {
            set
            {
                if (effectsManager != value)
                {
                    effectsManager = value;
                    RestartRendering();
                }
            }
            get
            {
                return effectsManager;
            }
        }

     //   public IRenderTechniquesManager RenderTechniquesManager { get { return EffectsManager != null ? EffectsManager.RenderTechniquesManager : null; } }

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
        public bool IsBusy { get { return pendingValidationCycles; } }

        private ID2DTarget d2dTarget;
        public ID2DTarget D2DControls { get { return d2dTarget; } }
        /// <summary>
        /// 
        /// </summary>
        public DPFSurfaceSwapChain()
        {
            renderTimer = new Stopwatch();
            MaxRenderingDuration = TimeSpan.FromMilliseconds(20.0);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            ClearColor = global::SharpDX.Color.Gray;
            IsShadowMapEnabled = false;
        }

        /// <summary>
        /// Invalidates the current render and requests an update.
        /// </summary>
        private void InvalidateRender()
        {
            pendingValidationCycles = true;
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
            loaded = true;
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(OnWindowMessage);
            try
            {
                if (StartD3D())
                { StartRendering(); }
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
            loaded = false;
            ((HwndSource)PresentationSource.FromVisual(this)).RemoveHook(OnWindowMessage);
            StopRendering();
            EndD3D(true);
        }

        /// <summary>
        /// 
        /// </summary>
        private bool StartD3D()
        {
            if (!loaded || EffectsManager == null)
            {
                //if (EffectsManager == null)
                //{
                //    EffectsManager = defaultEffectsManager = new DefaultEffectsManager(new DefaultRenderTechniquesManager());
                //}
                //RenderTechniquesManager = DefaultRenderTechniquesManagerObj.Value;
                //EffectsManager = DefaultEffectsManagerObj.Value;
                return false; // StardD3D() is called from DP changed handler
            }
            this.IsHitTestVisible = false;
            surfaceD3D = new RenderControl();
            Child = surfaceD3D;
            device = EffectsManager.Device;
            //deferredRenderer = new DeferredRenderer();
            //renderRenderable.DeferredRenderer = deferredRenderer;

            CreateAndBindTargets();
            SetDefaultRenderTargets();

            //Source = surfaceD3D;
            InvalidateRender();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndD3D(bool dispose)
        {
            DetachRenderables();
            this.Child = null;
            Disposer.RemoveAndDispose(ref d2dTarget);
            Disposer.RemoveAndDispose(ref renderContext);
      //      Disposer.RemoveAndDispose(ref deferredRenderer);
            Disposer.RemoveAndDispose(ref surfaceD3D);
            Disposer.RemoveAndDispose(ref colorBufferView);
            Disposer.RemoveAndDispose(ref colorBuffer);
            Disposer.RemoveAndDispose(ref backBuffer);
            Disposer.RemoveAndDispose(ref depthStencilBufferView);
            Disposer.RemoveAndDispose(ref depthStencilBuffer);
            Disposer.RemoveAndDispose(ref swapChain);
            if (dispose && defaultEffectsManager != null)
            {
                (defaultEffectsManager as IDisposable)?.Dispose();
                if (effectsManager == defaultEffectsManager)
                { effectsManager = null; }
                defaultEffectsManager = null;
            }
        }
        private void DetachRenderables()
        {
            if (renderRenderable != null && sceneAttached)
            {
                renderRenderable.Detach();
            }
            sceneAttached = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void CreateAndBindTargets()
        {
            //surfaceD3D.SetRenderTargetDX11(null);

            int width = System.Math.Max((int)ActualWidth, 100);
            int height = System.Math.Max((int)ActualHeight, 100);
            device.ImmediateContext.OutputMerger.ResetTargets();
            Disposer.RemoveAndDispose(ref colorBufferView);
            Disposer.RemoveAndDispose(ref colorBuffer);
            Disposer.RemoveAndDispose(ref backBuffer);
            Disposer.RemoveAndDispose(ref depthStencilBufferView);
            Disposer.RemoveAndDispose(ref depthStencilBuffer);
            Disposer.RemoveAndDispose(ref d2dTarget);
            CreateSwapChain();
            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);

#if DoubleBuffer
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
            var colordesc = new Texture2DDescription
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = sampleDesc,
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            };
            colorBuffer = new Texture2D(device, colordesc);
            colorBufferView = new RenderTargetView(device, colorBuffer);
#else
            var sampleDesc = swapChain.Description1.SampleDescription;
            colorBufferView = new RenderTargetView(device, backBuffer);
#endif
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
            depthStencilBuffer = new Texture2D(device, depthdesc);        
            depthStencilBufferView = new DepthStencilView(device, depthStencilBuffer);                     
            this.device.ImmediateContext.Rasterizer.SetScissorRectangle(0, 0, width, height);
            d2dTarget = new D2DControlWrapper();
            D2DControls.Initialize(swapChain);
        }

        private void CreateSwapChain()
        {
            if (swapChain != null)
            {
                swapChain.ResizeBuffers(swapChain.Description1.BufferCount, (int)ActualWidth, (int)ActualHeight, swapChain.Description.ModeDescription.Format, swapChain.Description.Flags);
            }
            else
            {
                Disposer.RemoveAndDispose(ref swapChain);
                var desc = CreateSwapChainDescription();
                using (var dxgiDevice2 = device.QueryInterface<global::SharpDX.DXGI.Device2>())
                using (var dxgiAdapter = dxgiDevice2.Adapter)
                using (var dxgiFactory2 = dxgiAdapter.GetParent<global::SharpDX.DXGI.Factory2>())
                using (var output = dxgiAdapter.Outputs.First())
                {
                    // The CreateSwapChain method is used so we can descend
                    // from this class and implement a swapchain for a desktop
                    // or a Windows 8 AppStore app
                    swapChain = new SwapChain1(dxgiFactory2, device, surfaceD3D.Handle, ref desc);
                }
            }
        }

        /// <summary>
        /// Creates the swap chain description.
        /// </summary>
        /// <returns>A swap chain description</returns>
        /// <remarks>
        /// This method can be overloaded in order to modify default parameters.
        /// </remarks>
        protected virtual global::SharpDX.DXGI.SwapChainDescription1 CreateSwapChainDescription()
        {
            int sampleCount = 1;
            int sampleQuality = 0;
            // SwapChain description
#if DoubleBuffer
#else
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
#endif
            var desc = new global::SharpDX.DXGI.SwapChainDescription1()
            {
                Width = (int)ActualWidth,
                Height = (int)ActualHeight,
                // B8G8R8A8_UNorm gives us better performance 
                Format = global::SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                Stereo = false,
                SampleDescription = new global::SharpDX.DXGI.SampleDescription(sampleCount, sampleQuality),
#if DoubleBuffer
                Usage = Usage.BackBuffer,
                BufferCount = 2,
                SwapEffect = global::SharpDX.DXGI.SwapEffect.FlipSequential,
#else
                Usage = Usage.RenderTargetOutput,
                BufferCount = 1,
                SwapEffect = global::SharpDX.DXGI.SwapEffect.Discard,
#endif
                Scaling = global::SharpDX.DXGI.Scaling.Stretch,
                Flags = SwapChainFlags.AllowModeSwitch
            };
            return desc;
        }
        /// <summary>
        /// Sets the default render-targets
        /// </summary>
        public void SetDefaultRenderTargets(bool clear = true)
        {
            SetDefaultRenderTargets(swapChain.Description1.Width, swapChain.Description1.Height, clear);
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
            targetWidth = swapChain.Description1.Width;
            targetHeight = swapChain.Description1.Height;

            device.ImmediateContext.OutputMerger.SetTargets(dsv, colorBufferView);
            device.ImmediateContext.Rasterizer.SetViewport(0, 0, targetWidth, targetHeight, 0.0f, 1.0f);

            device.ImmediateContext.ClearRenderTargetView(colorBufferView, ClearColor);
            device.ImmediateContext.ClearDepthStencilView(depthStencilBufferView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
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
                device.ImmediateContext.ClearRenderTargetView(colorBufferView, ClearColor);
            }

            if (clearDepthStencilBuffer)
            {
                device.ImmediateContext.ClearDepthStencilView(depthStencilBufferView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
            }
        }

        private void Render(TimeSpan timeStamp)
        {
            var device = this.device;
            if (device == null)
                return;
            if (swapChain == null)
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
                        sceneAttached = true;
                        ClearColor = renderRenderable.BackgroundColor;
                        IsShadowMapEnabled = renderRenderable.IsShadowMappingEnabled;

                        RenderTechnique = renderRenderable.RenderTechnique == null ? EffectsManager?[DefaultRenderTechniqueNames.Blinn] : renderRenderable.RenderTechnique;

                        renderContext?.Dispose();
                        renderContext = new RenderContext(this, device.ImmediateContext);
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

                        //RenderTechniquesManager.RenderTechniques.TryGetValue(DeferredRenderTechniqueNames.GBuffer, out gbuffer);
                        //RenderTechniquesManager.RenderTechniques.TryGetValue(DeferredRenderTechniqueNames.Deferred, out deferred);

                        //if (RenderTechnique == deferred)
                        //{
                        //    deferredRenderer.InitBuffers(this, Format.R32G32B32A32_Float);
                        //}
                        //else if (RenderTechnique == gbuffer)
                        //{
                        //    deferredRenderer.InitBuffers(this, Format.B8G8R8A8_UNorm);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("DPFCanvas: Error attaching element: " + string.Format(ex.Message), "Error");
                        Debug.WriteLine("DPFCanvas: Error attaching element: " + string.Format(ex.Message), "Error");
                        throw;
                    }
                }
                renderContext.TimeStamp = timeStamp;
                // ---------------------------------------------------------------------------
                // this part is per frame
                // ---------------------------------------------------------------------------
                if (EnableSharingModelMode && SharedModelContainer != null)
                {
                    SharedModelContainer.CurrentRenderHost = this;
                    ClearRenderTarget();
                }
                else
                {
                    SetDefaultRenderTargets(true);
                }
//                if (RenderTechnique == deferred)
//                {
//                    // set G-Buffer                    
//                    deferredRenderer.SetGBufferTargets(renderContext);

//                    // render G-Buffer pass                
//                    renderRenderable.Render(renderContext);

//                    // call deferred render 
//                    deferredRenderer.RenderDeferred(renderContext, renderRenderable);

//                }
//                else if (RenderTechnique == gbuffer)
//                {
//                    // set G-Buffer
//                    deferredRenderer.SetGBufferTargets(targetWidth / 2, targetHeight / 2, renderContext);

//                    // render G-Buffer pass                    
//                    renderRenderable.Render(renderContext);

//                    // reset render targets and run lighting pass                                         
//#if DoubleBuffer
//                    deferredRenderer.RenderGBufferOutput(renderContext, ref backBuffer);
//#else
//                    this.deferredRenderer.RenderGBufferOutput(renderContext, ref this.backBuffer);
//#endif
//                }
//                else
                {
                    renderRenderable.Render(renderContext);
                }
#if DoubleBuffer
                device.ImmediateContext.ResolveSubresource(colorBuffer, 0, backBuffer, 0, Format.B8G8R8A8_UNorm);
#endif
                if (D2DControls.D2DTarget != null)
                {
                    D2DControls.D2DTarget.BeginDraw();
                    renderRenderable.RenderD2D(renderContext);
                    D2DControls.D2DTarget.Transform = Matrix3x2.Identity;
                    D2DControls.D2DTarget.EndDraw();
                }
            }
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

            if (!renderTimer.IsRunning || !IsRendering)
                return;
            UpdateAndRender();
        }

        private readonly EventSkipper skipper = new EventSkipper();
        private readonly PresentParameters presentParams = new PresentParameters();
        /// <summary>
        /// Updates and renders the scene.
        /// </summary>
        private void UpdateAndRender()
        {
            if (((pendingValidationCycles && !skipper.IsSkip()) || skipper.DelayTrigger()) && renderRenderable != null)
            {
                var t0 = renderTimer.Elapsed;

                // Update all renderables before rendering 
                // giving them the chance to invalidate the current render.                                                            
                //renderRenderable.Update(t0);
                try
                {
                    Render(t0);
                    var res = swapChain.Present(0, PresentFlags.None, presentParams);
                    if (res.Success)
                    {
                        pendingValidationCycles = false;
                    }
                    else
                    {
                        swapChain.Present(0, PresentFlags.Restart, presentParams);
                    }
                }
                catch (Exception ex)
                {
                    if (!HandleExceptionOccured(ex))
                    {
                        MessageBox.Show(string.Format("DPFCanvas: Error while rendering: {0}", ex.Message), "Error");
                    }
                }
                lastRenderingDuration = renderTimer.Elapsed - t0;
                skipper.Push(lastRenderingDuration.TotalMilliseconds);
            }
        }

        private DispatcherOperation resizeOperation = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (!loaded)
            {
                return;
            }
            if (resizeOperation != null && resizeOperation.Status == DispatcherOperationStatus.Pending)
            {
                resizeOperation.Abort();
            }
            resizeOperation = Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
            {
                //if (surfaceD3D != null)
                {
                    try
                    {
                        //if (RenderTechnique != null)
                        //{
                        //    if (RenderTechnique == deferred)
                        //    {
                        //        deferredRenderer.InitBuffers(this, Format.R32G32B32A32_Float);
                        //    }
                        //    else if (RenderTechnique == gbuffer)
                        //    {
                        //        deferredRenderer.InitBuffers(this, Format.B8G8R8A8_UNorm);
                        //    }
                        //}

                        StopRendering();
                        CreateAndBindTargets();
                        SetDefaultRenderTargets();
                        StartRendering();
                        InvalidateRender();
                    }
                    catch (Exception ex)
                    {
                        if (!HandleExceptionOccured(ex))
                        {
                            MessageBox.Show(string.Format("DPFCanvas: Error while rendering: {0}", ex.Message), "Error");
                        }
                    }
                }
            }));
        }

        /// <summary>
        /// Handles the change of the effects manager.
        /// </summary>
        private void RestartRendering()
        {
            StopRendering();
            EndD3D(false);
            if (loaded)
            {
                //if (EffectsManager != null && RenderTechniquesManager != null)
                //{
                //    IsDeferredLighting = (renderTechnique == RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.Deferred)
                //        || renderTechnique == RenderTechniquesManager.RenderTechniques.Get(DeferredRenderTechniqueNames.GBuffer));
                //}
                if (StartD3D())
                { StartRendering(); }
            }
        }

        /// <summary>
        /// Invoked whenever an exception occurs. Stops rendering, frees resources and throws 
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        /// <returns><c>true</c> if the exception has been handled, <c>false</c> otherwise.</returns>
        private bool HandleExceptionOccured(Exception exception)
        {
            pendingValidationCycles = false;
            StopRendering();
            EndD3D(true);

            var sdxException = exception as SharpDXException;
            if (sdxException != null &&
                (sdxException.Descriptor == global::SharpDX.DXGI.ResultCode.DeviceRemoved ||
                 sdxException.Descriptor == global::SharpDX.DXGI.ResultCode.DeviceReset))
            {
                // Try to recover from DeviceRemoved/DeviceReset
                RestartRendering();
                return true;
            }
            else
            {
                var args = new RelayExceptionEventArgs(exception);
                ExceptionOccurred(this, args);
                return args.Handled;
            }
        }

        private IntPtr OnWindowMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_DISPLAYCHANGE = 0x7E;
            if (msg == WM_DISPLAYCHANGE)
            {
                // Realize invalidation caused by DeviceRemoved/DeviceReset
                // if there is no rendering pending (SimpleDemo)
                InvalidateRender();
            }

            return IntPtr.Zero;
        }
    }
}
