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
    using Render;
    using Utilities;
    using Windows.Foundation;
    using Windows.UI.Popups;
    using WinRT;

    // https://github.com/RolandKoenig/SeeingSharp2/blob/dae33fd85f38a781a348155aa1ee07ce1f170152/SeeingSharp.WinUI/Multimedia/Views/SeeingSharpPanelPainter.cs
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

        public SwapChainRenderHost(bool enableDeferredRendering)
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
            SizeChanged += Target_SizeChanged;            
            Loaded += SwapChainRenderHost_Loaded;
            Unloaded += SwapChainRenderHost_Unloaded;
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
        }

        private void SwapChainRenderHost_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            StartD3D();
        }

        private void SwapChainRenderHost_OnNewRenderTargetTexture(object sender, Texture2DArgs e)
        {            
            try
            {
                panelNativeDesktop = this.As<ISwapChainPanelNative>();
            }
            catch (SharpDX.SharpDXException ex)
            {
                throw;
            }

            var swapChain = (renderHost.RenderBuffer as DX11SwapChainCompositionRenderBufferProxy).SwapChain;
            swapChain.MatrixTransform = new SharpDX.Mathematics.Interop.RawMatrix3x2(1 / DpiScale, 0, 0, 1 / DpiScale, 0, 0);

            if (panelNativeDesktop != null)
            {
                panelNativeDesktop.SetSwapChain(swapChain?.NativePointer ?? IntPtr.Zero);
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
            DispatcherQueue.TryEnqueue(async() =>
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

        private void OnDummyButtonForFocus_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void OnDummyButtonForFocus_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            e.Handled = true;
        }



        private void OnSwapChainPanel_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            /*
            if (_painter == null) { return; }
            if (_targetPanel == null) { return; }

            // Track mouse/pointer state
            var currentPoint = e.GetCurrentPoint(_targetPanel);
            var pointProperties = currentPoint.Properties;

            if (pointProperties.IsPrimary)
            {
                _stateMouseOrPointer.Internals.NotifyButtonStates(
                    pointProperties.IsLeftButtonPressed,
                    pointProperties.IsMiddleButtonPressed,
                    pointProperties.IsRightButtonPressed,
                    pointProperties.IsXButton1Pressed,
                    pointProperties.IsXButton2Pressed);
            }
            */
            // Needed here because we loose focus again by default on left mouse button
            e.Handled = true;
        }

        private void OnSwapChainPanel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.Focus(FocusState.Programmatic);
            /*
            if (_painter == null) { return; }
            if (_targetPanel == null) { return; }

            // Set focus on target
            _targetPanel.Focus(FocusState.Programmatic);

            // Track mouse/pointer state
            var currentPoint = e.GetCurrentPoint(_targetPanel);
            var pointProperties = currentPoint.Properties;

            if (pointProperties.IsPrimary)
            {
                _stateMouseOrPointer.Internals.NotifyButtonStates(
                    pointProperties.IsLeftButtonPressed,
                    pointProperties.IsMiddleButtonPressed,
                    pointProperties.IsRightButtonPressed,
                    pointProperties.IsXButton1Pressed,
                    pointProperties.IsXButton2Pressed);
            }

            _lastDragPoint = currentPoint;
            */
            // Needed here because we loose focus again by default on left mouse button
            e.Handled = true;
        }

        private void OnSwapChainPanel_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            /*
            if (_painter == null) { return; }
            if (_targetPanel == null) { return; }

            // Calculate move distance
            var currentPoint = e.GetCurrentPoint(_targetPanel);

            if (_lastDragPoint == null)
            {
                _lastDragPoint = currentPoint;
            }

            var moveDistance = new Vector2(
                (float)(currentPoint.Position.X - _lastDragPoint.Position.X),
                (float)(currentPoint.Position.Y - _lastDragPoint.Position.Y));
            var currentLocation = new Vector2(
                (float)currentPoint.Position.X,
                (float)currentPoint.Position.Y);

            // Track mouse/pointer state
            var pointProperties = currentPoint.Properties;

            if (pointProperties.IsPrimary)
            {
                _stateMouseOrPointer.Internals.NotifyButtonStates(
                    pointProperties.IsLeftButtonPressed,
                    pointProperties.IsMiddleButtonPressed,
                    pointProperties.IsRightButtonPressed,
                    pointProperties.IsXButton1Pressed,
                    pointProperties.IsXButton2Pressed);

                var actSize = _painter.ActualSize;
                _stateMouseOrPointer.Internals.NotifyMouseLocation(
                    currentLocation, moveDistance, actSize.ToVector2());
            }

            // Store last drag point
            _lastDragPoint = currentPoint;
            */
        }

        private void OnSwapChainPanel_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            /*
            if (_painter == null) { return; }
            if (!_hasFocus) { return; }
            if (_targetPanel == null) { return; }

            // Track mouse/pointer state
            var currentPoint = e.GetCurrentPoint(_targetPanel);
            var pointProperties = currentPoint.Properties;
            var wheelDelta = pointProperties.MouseWheelDelta;

            if (pointProperties.IsPrimary)
            {
                _stateMouseOrPointer.Internals.NotifyButtonStates(
                    pointProperties.IsLeftButtonPressed,
                    pointProperties.IsMiddleButtonPressed,
                    pointProperties.IsRightButtonPressed,
                    pointProperties.IsXButton1Pressed,
                    pointProperties.IsXButton2Pressed);
                _stateMouseOrPointer.Internals.NotifyMouseWheel(wheelDelta);
            }
            */
        }

        /// <summary>
        /// Called when mouse leaves the target panel.
        /// </summary>
        private void OnSwapChainPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            /*
            if (_painter == null) { return; }

            _stateMouseOrPointer.Internals.NotifyInside(false);
            */
        }

        private void OnSwapChainPanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            int j = 0;
            /*
            if (_painter == null) { return; }

            _stateMouseOrPointer.Internals.NotifyInside(true);
            */
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
    }
}
