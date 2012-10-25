namespace HelixToolkit.SharpDX.Wpf
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using global::SharpDX;
    using global::SharpDX.DXGI;
    using global::SharpDX.Direct3D10;

    using Direct3D = global::SharpDX.Direct3D;
    using Direct3D10 = global::SharpDX.Direct3D10;
    using Device = global::SharpDX.Direct3D10.Device;

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
        private Device m_device;
        private Texture2D m_renderTarget;
        private Texture2D m_depthStencil;
        private RenderTargetView m_renderTargetView;
        private DepthStencilView m_depthStencilView;
        private DX10ImageSource m_surfaceD3D;
        private Stopwatch m_renderTimer;
        private IRenderable m_renderRenderable;
        private bool m_sceneAttached;        

        public Color4 ClearColor = global::SharpDX.Color.Black;

        static DPFCanvas()
        {
            StretchProperty.OverrideMetadata(typeof(DPFCanvas), new FrameworkPropertyMetadata(Stretch.Fill));
        }

        public DPFCanvas()
        {
            this.m_renderTimer = new Stopwatch();
            this.Loaded += this.WindowLoaded;
            this.Unloaded += this.WindowClosing;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
            {
                return;
            }

            this.StartD3D();
            this.StartRendering();
        }

        private void WindowClosing(object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
            {
                return;
            }

            this.StopRendering();
            this.EndD3D();
        }

        private void StartD3D()
        {
            this.m_device = new Direct3D10.Device1(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);
            this.m_surfaceD3D = new DX10ImageSource();
            this.m_surfaceD3D.IsFrontBufferAvailableChanged += this.OnIsFrontBufferAvailableChanged;

            this.CreateAndBindTargets();

            this.Source = this.m_surfaceD3D;
        }

        private void EndD3D()
        {
            if (this.m_renderRenderable != null)
            {
                this.m_renderRenderable.Detach();
                this.m_sceneAttached = false;
            }

            this.m_surfaceD3D.IsFrontBufferAvailableChanged -= this.OnIsFrontBufferAvailableChanged;
            this.Source = null;

            Disposer.RemoveAndDispose(ref this.m_surfaceD3D);
            Disposer.RemoveAndDispose(ref this.m_renderTargetView);
            Disposer.RemoveAndDispose(ref this.m_depthStencilView);
            Disposer.RemoveAndDispose(ref this.m_renderTarget);
            Disposer.RemoveAndDispose(ref this.m_depthStencil);
            Disposer.RemoveAndDispose(ref this.m_device);
        }

        private void CreateAndBindTargets()
        {
            this.m_surfaceD3D.SetRenderTargetDX10(null);

            Disposer.RemoveAndDispose(ref this.m_renderTargetView);
            Disposer.RemoveAndDispose(ref this.m_depthStencilView);
            Disposer.RemoveAndDispose(ref this.m_renderTarget);
            Disposer.RemoveAndDispose(ref this.m_depthStencil);

            int width = Math.Max((int)this.ActualWidth, 100);
            int height = Math.Max((int)this.ActualHeight, 100);

            var colordesc = new Texture2DDescription
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

            var depthdesc = new Texture2DDescription
            {
                BindFlags = BindFlags.DepthStencil,
                Format = Format.D32_Float_S8X24_UInt,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1,
            };

            this.m_renderTarget = new Texture2D(this.m_device, colordesc);
            this.m_depthStencil = new Texture2D(this.m_device, depthdesc);
            this.m_renderTargetView = new RenderTargetView(this.m_device, this.m_renderTarget);
            this.m_depthStencilView = new DepthStencilView(this.m_device, this.m_depthStencil);

            this.m_surfaceD3D.SetRenderTargetDX10(this.m_renderTarget);
        }

        private void StartRendering()
        {
            if (this.m_renderTimer.IsRunning)
                return;

            CompositionTarget.Rendering += OnRendering;
            this.m_renderTimer.Start();
        }

        private void StopRendering()
        {
            if (!this.m_renderTimer.IsRunning)
                return;

            CompositionTarget.Rendering -= OnRendering;
            this.m_renderTimer.Stop();
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (!this.m_renderTimer.IsRunning)
                return;

            this.Render(this.m_renderTimer.Elapsed);
            this.m_surfaceD3D.InvalidateD3DImage();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            this.CreateAndBindTargets();
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void Render(TimeSpan sceneTime)
        {
            var device = this.m_device;
            if (device == null)
                return;

            var renderTarget = this.m_renderTarget;
            if (renderTarget == null)
                return;

            int targetWidth = renderTarget.Description.Width;
            int targetHeight = renderTarget.Description.Height;

            device.OutputMerger.SetTargets(this.m_depthStencilView, this.m_renderTargetView);
            device.Rasterizer.SetViewports(new Viewport(0, 0, targetWidth, targetHeight, 0.0f, 1.0f));

            device.ClearRenderTargetView(this.m_renderTargetView, this.ClearColor);
            device.ClearDepthStencilView(this.m_depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

            if (this.Renderable != null)
            {
                if (!this.m_sceneAttached)
                {
                    this.m_sceneAttached = true;
                    this.m_renderRenderable.Attach(this);
                }

                this.Renderable.Update(this.m_renderTimer.Elapsed);
                this.Renderable.Render();
            }

            device.Flush();
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // this fires when the screensaver kicks in, the machine goes into sleep or hibernate
            // and any other catastrophic losses of the d3d device from WPF's point of view
            if (this.m_surfaceD3D.IsFrontBufferAvailable)
            {
                this.StartRendering();
            }
            else
            {
                this.StopRendering();
            }
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

        public IRenderable Renderable
        {
            get
            {
                return this.m_renderRenderable;
            }

            set
            {
                if (ReferenceEquals(this.m_renderRenderable, value))
                {
                    return;
                }

                if (this.m_renderRenderable != null)
                {
                    this.m_renderRenderable.Detach();
                }

                this.m_sceneAttached = false;
                this.m_renderRenderable = value;
            }
        }

        Device IRenderHost.Device
        {
            get { return this.m_device; }
        }

    }

    public static class Disposer
    {
        /// <summary>
        /// Dispose an object instance and set the reference to null
        /// </summary>
        /// <typeparam name="T">The type of object to dispose</typeparam>
        /// <param name="resource">A reference to the instance for disposal</param>
        /// <remarks>This method hides any thrown exceptions that might occur during disposal of the object (by design)</remarks>
        public static void RemoveAndDispose<T>(ref T resource) where T : class, IDisposable
        {
            if (resource == null)
                return;

            try
            {
                resource.Dispose();
            }
            catch
            {
            }

            resource = null;
        }
    }
}
