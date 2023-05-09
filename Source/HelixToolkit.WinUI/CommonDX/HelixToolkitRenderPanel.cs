/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace HelixToolkit.WinUI
{
    using CommonDX;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Input;
    using HelixToolkit.SharpDX.Core.Render;
    using HelixToolkit.SharpDX.Core.Utilities;
    using Windows.Foundation;
    using Windows.UI.Popups;
    using WinRT;
    using HelixToolkit.SharpDX.Core;

    // https://github.com/RolandKoenig/SeeingSharp2/blob/dae33fd85f38a781a348155aa1ee07ce1f170152/SeeingSharp.WinUI/Multimedia/Views/SeeingSharpPanelPainter.cs
    public class HelixToolkitRenderPanel : SwapChainPanel, IDisposable
    {
        /// <summary>
        /// Fired whenever an exception occurred on this object.
        /// </summary>
        public event EventHandler<RelayExceptionEventArgs> ExceptionOccurred = delegate { };

        private IRenderHost renderHost;
        /// <summary>
        /// Gets or sets the render host.
        /// </summary>
        /// <value>
        /// The render host.
        /// </value>
        public IRenderHost RenderHost { get { return renderHost; } }

        private readonly CompositionTargetEx compositionTarget = new CompositionTargetEx();

        private ISwapChainPanelNative panelNativeDesktop = null;


        private float dpiScale = 1;
        public float DpiScale
        {
            set
            {
                dpiScale = value;
                RenderHost.DpiScale = enableDpiScale ? value : 1;
            }
            get => dpiScale;
        }

        private bool enableDpiScale = true;

        public bool EnableDpiScale
        {
            set
            {
                enableDpiScale = value;
                RenderHost.DpiScale = value ? DpiScale : 1;
            }
            get => enableDpiScale;
        }

        public HelixToolkitRenderPanel(bool enableDeferredRendering)
        {
            if (enableDeferredRendering)
            {
                renderHost = new SwapChainCompositionRenderHost((device) => { return new DeferredContextRenderer(device, new AutoRenderTaskScheduler()); });
                // renderHost = new SwapChainSurfaceRenderHost(swapChainPanelNativePtr, (device) => { return new DeferredContextRenderer(device, new AutoRenderTaskScheduler()); });
            }
            else
            {
                renderHost = new SwapChainCompositionRenderHost();
                // renderHost = new SwapChainSurfaceRenderHost(swapChainPanelNativePtr);
            }
            renderHost.OnNewRenderTargetTexture += SwapChainRenderHost_OnNewRenderTargetTexture;
            renderHost.StartRenderLoop += SwapChainRenderHost_StartRenderLoop;
            renderHost.StopRenderLoop += SwapChainRenderHost_StopRenderLoop;
            Loaded += SwapChainRenderHost_Loaded;
            Unloaded += SwapChainRenderHost_Unloaded;
            SizeChanged += Target_SizeChanged;
        }


        private void SwapChainRenderHost_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            GetPanelNative();
            StartD3D();
        }

        private void SwapChainRenderHost_Unloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (DataContext == null && renderHost.EffectsManager == null)
            {
                EndD3D();
            }
            else
            {
                renderHost.StopRendering();
            }
            DestroyPanelNative();
        }

        private void GetPanelNative()
        {
            if (panelNativeDesktop == null)
            {
                try
                {
                    panelNativeDesktop = this.As<ISwapChainPanelNative>();
                }
                catch (global::SharpDX.SharpDXException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        private void DestroyPanelNative()
        {
            if (panelNativeDesktop != null)
            {
                panelNativeDesktop.SetSwapChain(IntPtr.Zero);
                panelNativeDesktop = null;
            }
        }


        private void SwapChainRenderHost_OnNewRenderTargetTexture(object sender, Texture2DArgs e)
        {
            var swapChain = (renderHost.RenderBuffer as DX11SwapChainCompositionRenderBufferProxy).SwapChain;
            swapChain.MatrixTransform = new global::SharpDX.Mathematics.Interop.RawMatrix3x2(1 / DpiScale, 0, 0, 1 / DpiScale, 0, 0);
            if (panelNativeDesktop != null)
            {
                panelNativeDesktop.SetSwapChain(swapChain.NativePointer);
            }
        }

        private void SwapChainRenderHost_StopRenderLoop(object sender, EventArgs e)
        {
            compositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void SwapChainRenderHost_StartRenderLoop(object sender, EventArgs e)
        {
            compositionTarget.Rendering -= CompositionTarget_Rendering;
            compositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, RenderingEventArgs e)
        {
            RenderHost.UpdateAndRender();
        }

        private void Target_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                try
                {
                    renderHost.Resize((int)ActualWidth, (int)ActualHeight);
                    /*
                    this.PointerExited += this.OnSwapChainPanel_PointerExited;
                    this.PointerEntered += this.OnSwapChainPanel_PointerEntered;
                    this.PointerWheelChanged += this.OnSwapChainPanel_PointerWheelChanged;
                    this.PointerPressed += this.OnSwapChainPanel_PointerPressed;
                    this.PointerReleased += this.OnSwapChainPanel_PointerReleased;
                    this.PointerMoved += this.OnSwapChainPanel_PointerMoved;

                    // Create the dummy button for focus management
                    //  see posts on: https://social.msdn.microsoft.com/Forums/en-US/54e4820d-d782-45d9-a2b1-4e3a13340788/set-focus-on-swapchainpanel-control?forum=winappswithcsharp
                    _dummyButtonForFocus = new Button
                    {
                        Content = "Button",
                        Width = 0,
                        Height = 0,
                        HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Left,
                        VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Top,
                        BorderThickness = new Microsoft.UI.Xaml.Thickness(0.0)
                    };

                    _dummyButtonForFocus.KeyDown += this.OnDummyButtonForFocus_KeyDown;
                    _dummyButtonForFocus.KeyUp += this.OnDummyButtonForFocus_KeyUp;
                    _dummyButtonForFocus.LostFocus += this.OnDummyButtonForFocus_LostFocus;
                    _dummyButtonForFocus.GotFocus += this.OnDummyButtonForFocus_GotFocus;

                    this.Children.Insert(0, _dummyButtonForFocus);
                    _dummyButtonForFocus.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
                    */

                }
                catch (Exception ex)
                {
                    if (!HandleExceptionOccured(ex))
                    {
                        var dialog = new MessageDialog($"DPFCanvas: Error during rendering: {ex.Message} \n StackTrace: {ex.StackTrace.ToString()}", "Error");
                        await dialog.ShowAsync();
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private bool StartD3D()
        {
            RenderHost.StartD3D((int)ActualWidth, (int)ActualHeight);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void EndD3D()
        {
            renderHost?.EndD3D();
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


        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    EndD3D();
                    renderHost?.Dispose();
                    compositionTarget?.Dispose();
                    DestroyPanelNative();
                }
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
