using HelixToolkit.Logger;
using HelixToolkit.UWP.Render;
using HelixToolkit.UWP.Utilities;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HelixToolkit.UWP.CommonDX
{
    /// <summary>
    /// 
    /// </summary>
    public class SwapChainCompositionRenderHost : DefaultRenderHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwapChainRenderHost"/> class.
        /// </summary>
        /// <param name="surface">The window PTR.</param>
        public SwapChainCompositionRenderHost()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwapChainRenderHost"/> class.
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <param name="createRenderer">The create renderer.</param>
        public SwapChainCompositionRenderHost(Func<Device, IRenderer> createRenderer) : base(createRenderer)
        {
        }
        /// <summary>
        /// Creates the render buffer.
        /// </summary>
        /// <returns></returns>
        protected override IDX11RenderBufferProxy CreateRenderBuffer()
        {
            Logger.Log(LogLevel.Information, "DX11SwapChainCompositionRenderBufferProxy", nameof(SwapChainRenderHost));
            return new DX11SwapChainCompositionRenderBufferProxy(EffectsManager);
        }
    }
}

namespace HelixToolkit.UWP
{
    using CommonDX;

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

        public SwapChainRenderHost()
        {
            renderHost = new SwapChainCompositionRenderHost();
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

        private void Target_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            try
            {
                renderHost.Resize(ActualWidth, ActualHeight);
            }
            catch (Exception ex)
            {
                if (!HandleExceptionOccured(ex))
                {
                    //MessageBox.Show($"DPFCanvas: Error during rendering: {ex.Message} \n StackTrace: {ex.StackTrace.ToString()}", "Error");
                }
            }
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

            var sdxException = exception as SharpDXException;
            if (sdxException != null &&
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
