/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using System.Linq;
using System;
#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Core2D;
    /// <summary>
    /// 
    /// </summary>
    public class DX11RenderBufferProxy : DisposeObject, IDX11RenderBufferProxy
    {
        /// <summary>
        /// Occurs when [on new buffer created].
        /// </summary>
        public event EventHandler<Texture2D> OnNewBufferCreated;
        /// <summary>
        /// Occurs when [on device lost].
        /// </summary>
        public event EventHandler<bool> OnDeviceLost;
        /// <summary>
        /// The color buffer
        /// </summary>
        protected Texture2D colorBuffer;
        /// <summary>
        /// The depth stencil buffer
        /// </summary>
        protected Texture2D depthStencilBuffer;
        /// <summary>
        /// The color buffer view
        /// </summary>
        protected RenderTargetView colorBufferView;
        /// <summary>
        /// The depth stencil buffer view
        /// </summary>
        protected DepthStencilView depthStencilBufferView;
        /// <summary>
        /// The D2D controls
        /// </summary>
        protected D2DControlWrapper d2dControls;
        /// <summary>
        /// Gets the d2 d controls.
        /// </summary>
        /// <value>
        /// The d2 d controls.
        /// </value>
        public ID2DTarget D2DControls
        {
            get { return d2dControls; }
        }
        /// <summary>
        /// Gets or sets the width of the target.
        /// </summary>
        /// <value>
        /// The width of the target.
        /// </value>
        public int TargetWidth { private set; get; }
        /// <summary>
        /// Gets or sets the height of the target.
        /// </summary>
        /// <value>
        /// The height of the target.
        /// </value>
        public int TargetHeight { private set; get; }
        /// <summary>
        /// Gets the color buffer view.
        /// </summary>
        /// <value>
        /// The color buffer view.
        /// </value>
        public RenderTargetView ColorBufferView { get { return colorBufferView; } }
        /// <summary>
        /// Gets the depth stencil buffer view.
        /// </summary>
        /// <value>
        /// The depth stencil buffer view.
        /// </value>
        public DepthStencilView DepthStencilBufferView { get { return depthStencilBufferView; } }
        /// <summary>
        /// Gets the color buffer.
        /// </summary>
        /// <value>
        /// The color buffer.
        /// </value>
        public Texture2D ColorBuffer { get { return colorBuffer; } }
        /// <summary>
        /// Gets the depth stencil buffer.
        /// </summary>
        /// <value>
        /// The depth stencil buffer.
        /// </value>
        public Texture2D DepthStencilBuffer { get { return depthStencilBuffer; } }

        private IDeviceContextPool deviceContextPool;
        /// <summary>
        /// Gets the device context pool.
        /// </summary>
        /// <value>
        /// The device context pool.
        /// </value>
        public IDeviceContextPool DeviceContextPool { get { return deviceContextPool; } }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DX11RenderBufferProxy"/> is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if initialized; otherwise, <c>false</c>.
        /// </value>
        public bool Initialized { private set; get; } = false;

#if MSAA
        private Texture2D renderTargetNMS;
#endif


#if MSAA
        /// <summary>
        /// Set MSAA level. If set to Two/Four/Eight, the actual level is set to minimum between Maximum and Two/Four/Eight
        /// </summary>
        public MSAALevel MSAA
        {
            private set; get;
        } = MSAALevel.Disable;
#endif

        /// <summary>
        /// The currently used Direct3D Device
        /// </summary>
        public Device Device
        {
            private set;get;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DX11RenderBufferProxy"/> class.
        /// </summary>
        /// <param name="device">The device.</param>
        public DX11RenderBufferProxy(Device device)
        {
            Device = device;
            deviceContextPool = Collect(new DeviceContextPool(device));
        }

        private Texture2D CreateRenderTarget(int width, int height, MSAALevel msaa)
        {
#if MSAA
            MSAA = msaa;
#endif
            TargetWidth = width;
            TargetHeight = height;
            RemoveAndDispose(ref d2dControls);
            RemoveAndDispose(ref colorBufferView);
            RemoveAndDispose(ref depthStencilBufferView);
            RemoveAndDispose(ref colorBuffer);
            RemoveAndDispose(ref depthStencilBuffer);
#if MSAA
            RemoveAndDispose(ref renderTargetNMS);
#endif
            var texture = OnCreateRenderTargetAndDepthBuffers(width, height);            
            Initialized = true;
            OnNewBufferCreated?.Invoke(this, texture);
            return texture;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected virtual Texture2D OnCreateRenderTargetAndDepthBuffers(int width, int height)
        {
#if MSAA
            int sampleCount = 1;
            int sampleQuality = 0;
            if (MSAA != MSAALevel.Disable)
            {
                do
                {
                    var newSampleCount = sampleCount * 2;
                    var newSampleQuality = Device.CheckMultisampleQualityLevels(Format.B8G8R8A8_UNorm, newSampleCount) - 1;

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

            colorBuffer = Collect(new Texture2D(Device, colordesc));
            depthStencilBuffer = Collect(new Texture2D(Device, depthdesc));

            colorBufferView = Collect(new RenderTargetView(Device, colorBuffer));
            depthStencilBufferView = Collect(new DepthStencilView(Device, depthStencilBuffer));
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

            renderTargetNMS = Collect(new Texture2D(Device, colordescNMS));
            Device.ImmediateContext.ResolveSubresource(colorBuffer, 0, renderTargetNMS, 0, Format.B8G8R8A8_UNorm);
            d2dControls = Collect(new D2DControlWrapper());
            d2dControls.Initialize(renderTargetNMS);
            return renderTargetNMS;
#else
            return colorBuffer;
#endif            
        }

        /// <summary>
        /// Sets the default render-targets
        /// </summary>
        public void SetDefaultRenderTargets(DeviceContext context)
        {
            context.OutputMerger.SetTargets(depthStencilBufferView, colorBufferView);
            context.Rasterizer.SetViewport(0, 0, TargetWidth, TargetWidth, 0.0f, 1.0f);
            context.Rasterizer.SetScissorRectangle(0, 0, TargetWidth, TargetHeight);
        }
        /// <summary>
        /// Clears the render target binding.
        /// </summary>
        /// <param name="context">The context.</param>
        public void ClearRenderTargetBinding(DeviceContext context)
        {
            context.OutputMerger.SetTargets(null, new RenderTargetView[0]);
        }
        /// <summary>
        /// Clears the render target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="color">The color.</param>
        public void ClearRenderTarget(DeviceContext context, Color4 color)
        {
            ClearRenderTarget(context, color, true, true);
        }
        /// <summary>
        /// Clears the buffers with the clear-color
        /// </summary>
        /// <param name="context"></param>
        /// <param name="color"></param>
        /// <param name="clearBackBuffer"></param>
        /// <param name="clearDepthStencilBuffer"></param>
        public void ClearRenderTarget(DeviceContext context, Color4 color, bool clearBackBuffer, bool clearDepthStencilBuffer)
        {
            if (clearBackBuffer)
            {
                context.ClearRenderTargetView(colorBufferView, color);
            }

            if (clearDepthStencilBuffer)
            {
                context.ClearDepthStencilView(depthStencilBufferView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
            }
        }
        /// <summary>
        /// Initializes.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="msaa">The msaa.</param>
        /// <returns></returns>
        public Texture2D Initialize(int width, int height, MSAALevel msaa)
        {
#if MSAA
            return CreateRenderTarget(width, height, msaa);
#else
            return CreateRenderTarget(width, height, MSAALevel.Disable);
#endif
        }
        /// <summary>
        /// Resize render target and depthbuffer resolution
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public virtual Texture2D Resize(int width, int height)
        {
#if MSAA
            return CreateRenderTarget(width, height, MSAA);
#else
            return CreateRenderTarget(width, height, MSAALevel.Disable);
#endif
        }
        /// <summary>
        /// Begins the draw.
        /// </summary>
        /// <returns></returns>
        public virtual bool BeginDraw()
        {
            return Initialized;
        }
        /// <summary>
        /// Ends the draw.
        /// </summary>
        /// <returns></returns>
        public virtual bool EndDraw()
        {
#if MSAA
            Device.ImmediateContext.ResolveSubresource(ColorBuffer, 0, renderTargetNMS, 0, Format.B8G8R8A8_UNorm);
#endif
            Device.ImmediateContext.Flush();
            return true;
        }
        /// <summary>
        /// Presents this drawing..
        /// </summary>
        /// <returns></returns>
        public virtual bool Present()
        {
            return true;
        }
        /// <summary>
        /// Begins the 2d drawing.
        /// </summary>
        /// <returns></returns>
        public virtual bool BeginDraw2D()
        {
            d2dControls.D2DTarget.BeginDraw();
            return true;
        }
        /// <summary>
        /// Ends the 2D drawing.
        /// </summary>
        /// <returns></returns>
        public virtual bool EndDraw2D()
        {
            d2dControls.D2DTarget.EndDraw();
            return true;
        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeManagedResources"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposeManagedResources)
        {
            OnNewBufferCreated = null;
            OnDeviceLost = null;
            Initialized = false;
            base.Dispose(disposeManagedResources);
        }

        #region ERROR HANDLING        
        /// <summary>
        /// Raises the on device lost.
        /// </summary>
        protected void RaiseOnDeviceLost()
        {
            OnDeviceLost?.Invoke(this, true);
        }
        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public class DX11SwapChainRenderBufferProxy : DX11RenderBufferProxy
    {
        private SwapChain1 swapChain;
        /// <summary>
        /// Gets the swap chain.
        /// </summary>
        /// <value>
        /// The swap chain.
        /// </value>
        public SwapChain1 SwapChain { get { return swapChain; } }

        private System.IntPtr surfacePtr;
        /// <summary>
        /// Initializes a new instance of the <see cref="DX11SwapChainRenderBufferProxy"/> class.
        /// </summary>
        /// <param name="surfacePointer">The surface pointer.</param>
        /// <param name="device">The device.</param>
        public DX11SwapChainRenderBufferProxy(System.IntPtr surfacePointer, Device device) : base(device)
        {
            surfacePtr = surfacePointer;
        }
        /// <summary>
        /// Called when [create render target and depth buffers].
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        protected override Texture2D OnCreateRenderTargetAndDepthBuffers(int width, int height)
        {
            if (swapChain == null || swapChain.IsDisposed)
            {
                swapChain = Collect(CreateSwapChain(surfacePtr));
            }
            else
            {
                swapChain.ResizeBuffers(swapChain.Description1.BufferCount, TargetWidth, TargetHeight, swapChain.Description.ModeDescription.Format, swapChain.Description.Flags);
            }
            colorBuffer = Collect(Texture2D.FromSwapChain<Texture2D>(swapChain, 0));
            var sampleDesc = swapChain.Description1.SampleDescription;
            colorBufferView = new RenderTargetView(Device, colorBuffer);
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
            depthStencilBuffer = new Texture2D(Device, depthdesc);
            depthStencilBufferView = new DepthStencilView(Device, depthStencilBuffer);

            d2dControls = Collect(new D2DControlWrapper());
            d2dControls.Initialize(swapChain);
            return colorBuffer;
        }

        private SwapChain1 CreateSwapChain(System.IntPtr surfacePointer)
        {
            var desc = CreateSwapChainDescription();
            using (var dxgiDevice2 = Device.QueryInterface<global::SharpDX.DXGI.Device2>())
            using (var dxgiAdapter = dxgiDevice2.Adapter)
            using (var dxgiFactory2 = dxgiAdapter.GetParent<Factory2>())
            {
                // The CreateSwapChain method is used so we can descend
                // from this class and implement a swapchain for a desktop
                // or a Windows 8 AppStore app
                return new SwapChain1(dxgiFactory2, Device, surfacePointer, ref desc);
            }
        }

        /// <summary>
        /// Creates the swap chain description.
        /// </summary>
        /// <returns>A swap chain description</returns>
        /// <remarks>
        /// This method can be overloaded in order to modify default parameters.
        /// </remarks>
        protected virtual SwapChainDescription1 CreateSwapChainDescription()
        {
            int sampleCount = 1;
            int sampleQuality = 0;
            // SwapChain description
#if MSAA
            if (MSAA != MSAALevel.Disable)
            {
                do
                {
                    var newSampleCount = sampleCount * 2;
                    var newSampleQuality = Device.CheckMultisampleQualityLevels(Format.B8G8R8A8_UNorm, newSampleCount) - 1;

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
            var desc = new SwapChainDescription1()
            {
                Width = Math.Max(1, TargetWidth),
                Height = Math.Max(1, TargetHeight),
                // B8G8R8A8_UNorm gives us better performance 
                Format = Format.B8G8R8A8_UNorm,
                Stereo = false,
                SampleDescription = new SampleDescription(sampleCount, sampleQuality),
                Usage = Usage.RenderTargetOutput,
                BufferCount = 1,
                SwapEffect = SwapEffect.Discard,
                Scaling = Scaling.Stretch,
                Flags = SwapChainFlags.AllowModeSwitch
            };
            return desc;
        }

        private readonly PresentParameters presentParams = new PresentParameters();

        /// <summary>
        /// Ends the draw.
        /// </summary>
        /// <returns></returns>
        public override bool EndDraw()
        {
            return true;
        }

        /// <summary>
        /// Presents this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Present()
        {
            var res = swapChain.Present(0, PresentFlags.None, presentParams);
            if (res.Success)
            {
                return true;
            }
            else
            {
                var desc = ResultDescriptor.Find(res);
                if(desc == global::SharpDX.DXGI.ResultCode.DeviceRemoved || desc == global::SharpDX.DXGI.ResultCode.DeviceReset || desc == global::SharpDX.DXGI.ResultCode.DeviceHung)
                {
                    RaiseOnDeviceLost();
                }
                else 
                {
                    swapChain.Present(0, PresentFlags.Restart, presentParams);
                }
                return false;
            }
        }
    }
}
