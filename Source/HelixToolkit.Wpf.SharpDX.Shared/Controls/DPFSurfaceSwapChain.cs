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
    public class DPFSurfaceSwapChain : WinformHostExtend, IRenderCanvas, IDisposable
    {
        private readonly IRenderHost renderHost;
        /// <summary>
        /// Gets or sets the render host.
        /// </summary>
        /// <value>
        /// The render host.
        /// </value>
        public IRenderHost RenderHost { get { return renderHost; } }
        private RenderControl surfaceD3D;
        private Window parentWindow;
        private readonly bool belongsToParentWindow;

        /// <summary>
        /// Fired whenever an exception occurred on this object.
        /// </summary>
        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred = delegate { };

        private readonly CompositionTargetEx compositionTarget = new CompositionTargetEx();

        /// <summary>
        /// 
        /// </summary>
        public DPFSurfaceSwapChain(bool deferredRendering = false, bool attachedToWindow = true)
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            surfaceD3D = new RenderControl(this);
            Child = surfaceD3D;
            if (deferredRendering)
            {
                renderHost = new SwapChainRenderHost(surfaceD3D.Handle,
                    (device) => { return new DeferredContextRenderer(device, new AutoRenderTaskScheduler()); });
            }
            else
            {
                renderHost = new SwapChainRenderHost(surfaceD3D.Handle);
            }
            RenderHost.StartRenderLoop += RenderHost_StartRenderLoop;
            RenderHost.StopRenderLoop += RenderHost_StopRenderLoop;
            RenderHost.ExceptionOccurred += (s, e) => { HandleExceptionOccured(e.Exception); };

            belongsToParentWindow = attachedToWindow;
        }

        public DPFSurfaceSwapChain(Func<IntPtr, IRenderHost> createRenderHost, bool attachedToWindow = true)
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            surfaceD3D = new RenderControl();
            Child = surfaceD3D;
            renderHost = createRenderHost(surfaceD3D.Handle);
            RenderHost.StartRenderLoop += RenderHost_StartRenderLoop;
            RenderHost.StopRenderLoop += RenderHost_StopRenderLoop;
            RenderHost.ExceptionOccurred += (s, e) => { HandleExceptionOccured(e.Exception); };

            belongsToParentWindow = attachedToWindow;
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
            RenderHost.UpdateAndRender();
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (!belongsToParentWindow)
                        EndD3D();

                    compositionTarget.Dispose();                    
                    // TODO: dispose managed state (managed objects).
                }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                disposedValue = true;
            }
        }
        #endregion
    }
}
