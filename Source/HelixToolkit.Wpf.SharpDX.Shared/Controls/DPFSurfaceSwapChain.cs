// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DPFCanvas.cs" company="Helix Toolkit">
//   Copyright (c) 2018 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using global::SharpDX;
using global::SharpDX.Direct3D9;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Diagnostics.CodeAnalysis;  
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#else
using Device = SharpDX.Direct3D11.Device;
#endif

#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Render;
using HelixToolkit.SharpDX.Core.Utilities;
#endif
namespace HelixToolkit.Wpf.SharpDX
{

    using Controls;
#if !COREWPF
    using Render;    
    using Utilities;
#endif

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Image" />
    public class DPFSurfaceSwapChain : Grid, IRenderCanvas, IDisposable
    {
        private IRenderHost renderHost;
        /// <summary>
        /// Gets or sets the render host.
        /// </summary>
        /// <value>
        /// The render host.
        /// </value>
        public IRenderHost RenderHost
        {
            get
            {
                return renderHost;
            }
        }
        private RenderControl surfaceD3D;
        private Window parentWindow;
        private readonly WinformHostExtend winformHost = new WinformHostExtend();
        private D3DImageExt image3D;
        private bool belongsToParentWindow;
        private readonly Image image = new Image
        {
            Width = 1,
            Height = 1
        };
        /// <summary>
        /// Fired whenever an exception occurred on this object.
        /// </summary>
        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred = delegate { };

        private readonly CompositionTargetEx compositionTarget = new CompositionTargetEx();

        private bool enableDpiScale = true;
        public bool EnableDpiScale
        {
            set
            {
                enableDpiScale = value;
                if (renderHost != null)
                {
                    renderHost.DpiScale = value ? (float)DpiScale : 1;
                }
            }
            get => enableDpiScale;
        }

        public double DpiScale { set => winformHost.DpiScale = value; get => winformHost.DpiScale; }

        public DPFSurfaceSwapChain(bool deferredRendering = false, bool attachedToWindow = true)
        {
            SetupVisual(attachedToWindow);
            SetupRenderHost(deferredRendering ? new SwapChainRenderHost(surfaceD3D.Handle,
                    (device) => { return new DeferredContextRenderer(device, new AutoRenderTaskScheduler()); }) :
                    new SwapChainRenderHost(surfaceD3D.Handle));
            SetupImage();
        }

        public DPFSurfaceSwapChain(Func<IntPtr, IRenderHost> createRenderHost, bool attachedToWindow = true)
        {
            SetupVisual(attachedToWindow);
            SetupRenderHost(createRenderHost(surfaceD3D.Handle)); 
            SetupImage();
        }

        private void DPFSurfaceSwapChain_DpiScaleChanged(object sender, double e)
        {
            if (RenderHost != null)
            {
                RenderHost.DpiScale = EnableDpiScale ? (float)e : 1;
            }
        }

        private void SetupVisual(bool attachedToWindow)
        {
            Children.Add(image);
            Children.Add(winformHost);
            winformHost.DpiScaleChanged += DPFSurfaceSwapChain_DpiScaleChanged;
            surfaceD3D = new RenderControl(this);
            winformHost.Child = surfaceD3D;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            belongsToParentWindow = attachedToWindow;
        }

        private void SetupRenderHost(IRenderHost host)
        {
            renderHost = host;
            RenderHost.DpiScale = EnableDpiScale ? (float)DpiScale : 1;
            RenderHost.StartRenderLoop += RenderHost_StartRenderLoop;
            RenderHost.StopRenderLoop += RenderHost_StopRenderLoop;
            RenderHost.ExceptionOccurred += (s, e) => { HandleExceptionOccured(e.Exception); };
            RenderHost.EffectsManagerChanged += (s, e) => { SetupImage(); };
        }

        private void SetupImage()
        {
            if (image3D == null || (RenderHost.EffectsManager != null && RenderHost.EffectsManager.AdapterIndex != image3D.AdapterIndex))
            {
                image.Source = null;
                image3D?.Dispose();
                if (RenderHost.EffectsManager != null)
                {
                    image3D = new D3DImageExt(RenderHost.EffectsManager.AdapterIndex);
                    image.Source = image3D;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (belongsToParentWindow)
                {
                    parentWindow = FindVisualAncestor<Window>(this);
                    if (parentWindow != null)
                    {
                        parentWindow.Closed -= ParentWindow_Closed;
                        parentWindow.Closed += ParentWindow_Closed;
                    }
                }

                StartD3D();
            }
            catch (Exception ex)
            {
                // Exceptions in the Loaded event handler are silently swallowed by WPF.
                // https://social.msdn.microsoft.com/Forums/vstudio/en-US/9ed3d13d-0b9f-48ac-ae8d-daf0845c9e8f/bug-in-wpf-windowloaded-exception-handling?forum=wpf
                // http://stackoverflow.com/questions/19140593/wpf-exception-thrown-in-eventhandler-is-swallowed
                // tl;dr: M$ says it's "by design" and "working as indended" but may change in the future :).

                if (!HandleExceptionOccured(ex))
                {
                    MessageBox.Show($"DPFCanvas: Error while starting rendering: {ex.Message} \n StackTrace: {ex.StackTrace.ToString()}", "Error");
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
            if (belongsToParentWindow && parentWindow != null)
            {
                parentWindow.Closed -= ParentWindow_Closed;
            }
            if (DataContext == null && RenderHost.EffectsManager == null && belongsToParentWindow)
            {
                EndD3D();
            }
            else
            {
                RenderHost.StopRendering();
            }
        }

        private void ParentWindow_Closed(object sender, EventArgs e)
        {
            EndD3D();
        }

        /// <summary>
        /// 
        /// </summary>
        private bool StartD3D()
        {
            RenderHost.StartD3D((int)ActualWidth, (int)ActualHeight);
            return true;
        }

        private void RenderHost_StopRenderLoop(object sender, EventArgs e)
        {
            compositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void RenderHost_StartRenderLoop(object sender, EventArgs e)
        {
            compositionTarget.Rendering -= CompositionTarget_Rendering;
            compositionTarget.Rendering += CompositionTarget_Rendering;
        }


        private void CompositionTarget_Rendering(object sender, RenderingEventArgs e)
        {
            if (RenderHost.UpdateAndRender())
            {
                image3D?.InvalidateD3DImage();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndD3D()
        {
            renderHost?.EndD3D();
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
                if (IsLoaded)
                {
                    try
                    {
                        RenderHost?.Resize((int)ActualWidth, (int)ActualHeight);
                    }
                    catch (Exception ex)
                    {
                        if (!HandleExceptionOccured(ex))
                        {
                            MessageBox.Show($"DPFCanvas: Error during rendering: {ex.Message} \n StackTrace: {ex.StackTrace.ToString()}", "Error");
                        }
                    }
                }
            }));
        }

        /// <summary>
        /// Invoked whenever an exception occurs. Stops rendering, frees resources and throws 
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        /// <returns><c>true</c> if the exception has been handled, <c>false</c> otherwise.</returns>
        private bool HandleExceptionOccured(Exception exception)
        {
            EndD3D();

            if (exception is SharpDXException sdxException &&
                (sdxException.Descriptor == global::SharpDX.DXGI.ResultCode.DeviceRemoved ||
                 sdxException.Descriptor == global::SharpDX.DXGI.ResultCode.DeviceReset))
            {
                // Try to recover from DeviceRemoved/DeviceReset
                StartD3D();
                return true;
            }
            else
            {
                var args = new RelayExceptionEventArgs(exception);
                ExceptionOccurred(this, args);
                return args.Handled;
            }
        }

        public static T FindVisualAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                var parent = System.Windows.Media.VisualTreeHelper.GetParent(obj);
                while (parent != null)
                {
                    if (parent is T typed)
                    {
                        return typed;
                    }

                    parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
                }
            }

            return null;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected void Dispose(bool disposing)
        {            
            winformHost?.Dispose();
            image.Source = null;
            image3D?.Dispose();
            compositionTarget?.Dispose();
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!belongsToParentWindow)
                        EndD3D();              
                    // TODO: dispose managed state (managed objects).
                }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        /// <summary>
        /// Use a fake D3DImage to bump up frame rate.
        /// </summary>
        private sealed class D3DImageExt : D3DImage, IDisposable
        {
            private Direct3DEx context;
            private DeviceEx device;

            private readonly int adapterIndex;
            private Texture renderTarget;
            private Surface surface;

            public int AdapterIndex => adapterIndex;

            public D3DImageExt(int adapterIndex = 0)
            {
                this.adapterIndex = adapterIndex;
                this.StartD3D();

            }

            public void InvalidateD3DImage()
            {
                if (this.renderTarget != null)
                {
                    base.Lock();
                    base.AddDirtyRect(new Int32Rect(0, 0, 1, 1));
                    base.Unlock();
                }
            }

            private void StartD3D()
            {
                context = new Direct3DEx();
                // Ref: https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/wpf-and-direct3d9-interoperation
                var presentparams = new PresentParameters
                {
                    Windowed = true,
                    SwapEffect = SwapEffect.Discard,
                    PresentationInterval = PresentInterval.Immediate,
                    BackBufferHeight = 1,
                    BackBufferWidth = 1,
                    BackBufferFormat = Format.Unknown
                };

                device = new DeviceEx(context, this.adapterIndex, DeviceType.Hardware, IntPtr.Zero, CreateFlags.HardwareVertexProcessing, presentparams);
                try
                {
                    this.renderTarget = new Texture(device, 1, 1, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                    surface = this.renderTarget.GetSurfaceLevel(0);
                    base.Lock();
                    // "enableSoftwareFallback = true" makes Remote Desktop possible.
                    // See: http://msdn.microsoft.com/en-us/library/hh140978%28v=vs.110%29.aspx
                    base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.NativePointer, true);
                    base.AddDirtyRect(new Int32Rect(0, 0, 1, 1));
                    base.Unlock();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            private void EndD3D(bool disposeDevices)
            {
                base.Lock();
                base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                base.Unlock();
                Disposer.RemoveAndDispose(ref surface);
                Disposer.RemoveAndDispose(ref renderTarget);
                if (disposeDevices)
                {
                    Disposer.RemoveAndDispose(ref device);
                    Disposer.RemoveAndDispose(ref context);
                }
            }

            public bool IsDeviceStateOk()
            {
                if (device != null)
                {
                    var state = device.CheckDeviceState(IntPtr.Zero);
                    return state == DeviceState.Ok;
                }
                return false;
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            [SuppressMessage("Microsoft.Usage", "CA2213: Disposable fields should be disposed", Justification = "False positive.")]
            void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        EndD3D(true);
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~DX11ImageSource() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion

        }
    }
}
