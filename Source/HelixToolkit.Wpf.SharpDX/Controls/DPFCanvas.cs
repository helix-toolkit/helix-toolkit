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

    using Device = global::SharpDX.Direct3D11.Device;

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

    public class DPFCanvas : Image, IRenderHost
    {
        private readonly Action updateAndRenderAction;
        private readonly Stopwatch renderTimer;
        private Device device;
        private Texture2D colorBuffer;
        private Texture2D depthStencilBuffer;
        private RenderTargetView colorBufferView;
        private DepthStencilView depthStencilBufferView;
        private DX11ImageSource surfaceD3D;
        private IRenderer renderRenderable;
        private RenderContext renderContext;
        private DeferredRenderer deferredRenderer;
        private bool sceneAttached;        
        private int targetWidth, targetHeight;
        private int pendingValidationCycles;
        private TimeSpan lastRenderingDuration;
        private DispatcherOperation updateAndRenderOperation;
        private RenderTechnique deferred;
        private RenderTechnique gbuffer;
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

        /// <summary>
        /// 
        /// </summary>
        public bool IsMSAAEnabled { get; private set; }

        /// <summary>
        /// Gets or sets the maximum time that rendering is allowed to take. When exceeded,
        /// the next cycle will be enqueued at <see cref="DispatcherPriority.Input"/> to reduce input lag.
        /// </summary>
        public TimeSpan MaxRenderingDuration { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RenderTechnique RenderTechnique { get; private set; }

        /// <summary>
        /// The instance of currently attached IRenderable - this is in general the Viewport3DX
        /// </summary>
        public IRenderer Renderable
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
            DependencyProperty.Register("EffectsManager", typeof(EffectsManager), typeof(DPFCanvas), null);


        public IEffectsManager EffectsManager
        {
            get { return (EffectsManager)GetValue(EffectsManagerProperty); }
            set { SetValue(EffectsManagerProperty, value); }
        }

        public static readonly DependencyProperty RenderTechniquesManagerProperty =
            DependencyProperty.Register("RenderTechniquesManager", typeof(IRenderTechniquesManager), typeof(DPFCanvas), null);

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
        /// 
        /// </summary>
        static DPFCanvas()
        {
            StretchProperty.OverrideMetadata(typeof(DPFCanvas), new FrameworkPropertyMetadata(Stretch.Fill));
        }

        /// <summary>
        /// 
        /// </summary>
        public DPFCanvas()
        {
            updateAndRenderAction = UpdateAndRender;
            updateAndRenderOperation = null;
            renderTimer = new Stopwatch();
            MaxRenderingDuration = TimeSpan.FromMilliseconds(20.0);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            ClearColor = global::SharpDX.Color.Gray;
            IsShadowMapEnabled = false;
            IsMSAAEnabled = true;
        }

        /// <summary>
        /// Invalidates the current render and requests an update.
        /// </summary>
        public void InvalidateRender()
        {
            // For some reason, we need two render cycles to recover from 
            // UAC popup or sleep when MSAA is enabled.
            pendingValidationCycles = 2;
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

            gbuffer = RenderTechniquesManager.RenderTechniques[DeferredRenderTechniqueNames.GBuffer];
            deferred = RenderTechniquesManager.RenderTechniques[DeferredRenderTechniqueNames.Deferred];

            try
            {
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
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartD3D()
        {
            surfaceD3D = new DX11ImageSource();
            surfaceD3D.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;
            device = EffectsManager.Device;
            deferredRenderer = new DeferredRenderer();
            renderRenderable.DeferredRenderer = deferredRenderer;

            CreateAndBindTargets();
            SetDefaultRenderTargets();
            Source = surfaceD3D;
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

            if (surfaceD3D == null)
            {
                return;
            }

            surfaceD3D.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
            Source = null;

            Disposer.RemoveAndDispose(ref deferredRenderer);
            Disposer.RemoveAndDispose(ref surfaceD3D);
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
            surfaceD3D.SetRenderTargetDX11(null);

            int width = System.Math.Max((int)ActualWidth, 100);
            int height = System.Math.Max((int)ActualHeight, 100);

            Disposer.RemoveAndDispose(ref colorBufferView);
            Disposer.RemoveAndDispose(ref depthStencilBufferView);
            Disposer.RemoveAndDispose(ref colorBuffer);
            Disposer.RemoveAndDispose(ref depthStencilBuffer);
#if MSAA
            Disposer.RemoveAndDispose(ref renderTargetNMS);

            // check 8,4,2,1
            int sampleCount = 8;
            int sampleQuality = 0;

            if (IsMSAAEnabled)
            {
                do
                {
                    sampleQuality = device.CheckMultisampleQualityLevels(Format.B8G8R8A8_UNorm, sampleCount) - 1;
                    if (sampleQuality > 0)
                    {
                        break;
                    }
                    else
                    {
                        sampleCount /= 2;
                    }

                    if (sampleCount == 1)
                    {
                        sampleQuality = 0;
                        break;
                    }
                }
                while (true);
            }
            else
            {
                sampleCount = 1;
                sampleQuality = 0;
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
            surfaceD3D.SetRenderTargetDX11(renderTargetNMS);
#else
            this.surfaceD3D.SetRenderTargetDX11(this.colorBuffer);
#endif                       
        }

        /// <summary>
        /// Sets the default render-targets
        /// </summary>
        public void SetDefaultRenderTargets()
        {
            SetDefaultRenderTargets(colorBuffer.Description.Width, colorBuffer.Description.Height);
        }

        /// <summary>
        /// Sets the default render-targets
        /// </summary>
        public void SetDefaultRenderTargets(int width, int height)
        {
            targetWidth = width;
            targetHeight = height;

            device.ImmediateContext.OutputMerger.SetTargets(depthStencilBufferView, colorBufferView);
            device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height, 0.0f, 1.0f);

            device.ImmediateContext.ClearRenderTargetView(colorBufferView, ClearColor);
            device.ImmediateContext.ClearDepthStencilView(depthStencilBufferView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
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

        private void Render()
        {
            var device = this.device;
            if (device == null)
                return;

            var renderTarget = colorBuffer;
            if (renderTarget == null)
                return;

            if (Renderable != null)
            {
                /// ---------------------------------------------------------------------------
                /// this part is done only if the scene is not attached
                /// it is an attach and init pass for all elements in the scene-graph                
                if (!sceneAttached)
                {
                    try
                    {
                        Light3D.LightCount = 0;
                        sceneAttached = true;
                        ClearColor = renderRenderable.BackgroundColor;
                        IsShadowMapEnabled = renderRenderable.IsShadowMappingEnabled;

                        var blinn = RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Blinn];
                        RenderTechnique = renderRenderable.RenderTechnique == null ? blinn : renderRenderable.RenderTechnique;
                            
                        if (renderContext != null)
                        {
                            renderContext.Dispose();
                        }
                        renderContext = new RenderContext(this, EffectsManager.GetEffect(RenderTechnique));
                        renderRenderable.Attach(this);

#if DEFERRED
                        if(RenderTechnique == deferred)
                        {
                            deferredRenderer.InitBuffers(this, Format.R32G32B32A32_Float);
                        }

                        if (RenderTechnique == gbuffer)
                        {
                            deferredRenderer.InitBuffers(this, Format.B8G8R8A8_UNorm);
                        }
#endif
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("DPFCanvas: Error attaching element: " + string.Format(ex.Message), "Error");
                        Debug.WriteLine("DPFCanvas: Error attaching element: " + string.Format(ex.Message), "Error");
                        throw;
                    }
                }

                SetDefaultRenderTargets();

                /// ---------------------------------------------------------------------------
                /// this part is per frame                
#if DEFERRED
                
                if (RenderTechnique == deferred)
                {
                    /// set G-Buffer                    
                    deferredRenderer.SetGBufferTargets();

                    /// render G-Buffer pass                
                    renderRenderable.Render(renderContext);

                    /// call deferred render 
                    deferredRenderer.RenderDeferred(renderContext, renderRenderable);

                }
                else if (RenderTechnique == gbuffer)
                {
                    /// set G-Buffer
                    deferredRenderer.SetGBufferTargets(targetWidth / 2, targetHeight / 2);

                    /// render G-Buffer pass                    
                    renderRenderable.Render(renderContext);

                    /// reset render targets and run lighting pass                                         
#if MSAA
                    deferredRenderer.RenderGBufferOutput(ref renderTargetNMS);
#else
                    this.deferredRenderer.RenderGBufferOutput(ref this.colorBuffer);
#endif
                }
                else 
#endif
                {
                    this.device.ImmediateContext.ClearRenderTargetView(colorBufferView, ClearColor);
                    this.device.ImmediateContext.ClearDepthStencilView(depthStencilBufferView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

                    renderRenderable.Render(renderContext);
                }
            }

            this.device.ImmediateContext.Flush();
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

            // Check if there is a deferred updateAndRenderOperation in progress.
            if (updateAndRenderOperation != null)
            {
                // If the deferred updateAndRenderOperation has not yet ended...
                var status = updateAndRenderOperation.Status;
                if (status == DispatcherOperationStatus.Pending ||
                    status == DispatcherOperationStatus.Executing)
                {
                    // ... return immediately.
                    return;
                }

                updateAndRenderOperation = null;

                // Ensure that at least every other cycle is done at DispatcherPriority.Render.
                // Uncomment if animation stutters, but no need as far as I can see.
                // this.lastRenderingDuration = TimeSpan.Zero;
            }

            // If rendering took too long last time...
            if (lastRenderingDuration > MaxRenderingDuration)
            {
                // ... enqueue an updateAndRenderAction at DispatcherPriority.Input.
                updateAndRenderOperation = Dispatcher.BeginInvoke(
                    updateAndRenderAction, DispatcherPriority.Input);
            }
            else
            {
                UpdateAndRender();
            }
        }

        /// <summary>
        /// Updates and renders the scene.
        /// </summary>
        private void UpdateAndRender()
        {
            try
            {
                var t0 = renderTimer.Elapsed;

                // Update all renderables before rendering 
                // giving them the chance to invalidate the current render.
                renderRenderable.Update(t0);

                if (pendingValidationCycles > 0)
                {
                    pendingValidationCycles--;

                    // Safety check because of dispatcher deferred render call
                    if (surfaceD3D != null)
                    {
                        Render();
#if MSAA
                        device.ImmediateContext.ResolveSubresource(colorBuffer, 0, renderTargetNMS, 0, Format.B8G8R8A8_UNorm);
#endif
                        surfaceD3D.InvalidateD3DImage();
                    }
                }

                lastRenderingDuration = renderTimer.Elapsed - t0;
            }
            catch (Exception ex)
            {
                if (!HandleExceptionOccured(ex))
                {
                    MessageBox.Show(string.Format("DPFCanvas: Error while rendering: {0}", ex.Message), "Error");
                }
            }
        }

        private bool queued = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sizeInfo"></param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (queued) return;

            queued = true;

            Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
            {

                    if (surfaceD3D != null)
                    {
        #if DEFERRED
                        if (RenderTechnique == deferred)
                        {
                        deferredRenderer.InitBuffers(this, Format.R32G32B32A32_Float);
                        }
                        if (RenderTechnique == gbuffer)
                        {
                        deferredRenderer.InitBuffers(this, Format.B8G8R8A8_UNorm);
                        }
#endif
                    CreateAndBindTargets();
                    SetDefaultRenderTargets();
                    InvalidateRender();
                    }

                queued = false;
            }));
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
    }
}
