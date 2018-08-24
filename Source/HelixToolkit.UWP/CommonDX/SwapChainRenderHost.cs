/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HelixToolkit.UWP
{
    using CommonDX;
    using Render;
    using Utilities;
    using Windows.Foundation;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;

    public class SwapChainRenderHost : SwapChainPanel
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

        public SwapChainRenderHost(bool enableDeferredRendering)
        {   
            if (enableDeferredRendering)
            {
                renderHost = new SwapChainCompositionRenderHost((device) => { return new DeferredContextRenderer(device, new AutoRenderTaskScheduler()); });
            }
            else
            {
                renderHost = new SwapChainCompositionRenderHost();
            }
            renderHost.OnNewRenderTargetTexture += SwapChainRenderHost_OnNewRenderTargetTexture;
            renderHost.StartRenderLoop += SwapChainRenderHost_StartRenderLoop;
            renderHost.StopRenderLoop += SwapChainRenderHost_StopRenderLoop;
            SizeChanged += Target_SizeChanged;            
            Loaded += SwapChainRenderHost_Loaded;
            Unloaded += SwapChainRenderHost_Unloaded;
        }

        private void SwapChainRenderHost_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            EndD3D();
        }

        private void SwapChainRenderHost_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            StartD3D();
        }

        private void SwapChainRenderHost_OnNewRenderTargetTexture(object sender, Texture2DArgs e)
        {
            // Obtain a reference to the native COM object of the SwapChainPanel.
            using (global::SharpDX.DXGI.ISwapChainPanelNative nativeObject = global::SharpDX.ComObject.As<global::SharpDX.DXGI.ISwapChainPanelNative>(this))
            {
                // Set its swap chain.
                nativeObject.SwapChain = (renderHost.RenderBuffer as DX11SwapChainCompositionRenderBufferProxy).SwapChain;
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

        IAsyncAction resizeAction;
        private void Target_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            if (resizeAction != null)
            {
                resizeAction.Cancel();
            }
            resizeAction = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, async() =>
            {
                try
                {
                    renderHost.Resize(ActualWidth, ActualHeight);
                }
                catch (Exception ex)
                {
                    if (!HandleExceptionOccured(ex))
                    {
                        var dialog = new MessageDialog($"DPFCanvas: Error during rendering: {ex.Message} \n StackTrace: {ex.StackTrace.ToString()}", "Error");
                        await dialog.ShowAsync();
                    }
                }
                finally
                {
                    resizeAction = null;
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private bool StartD3D()
        {
            RenderHost.StartD3D(ActualWidth, ActualHeight);
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
    }
}
