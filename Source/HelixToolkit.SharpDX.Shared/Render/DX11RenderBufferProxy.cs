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
    public class DX11RenderBufferProxy : DisposeObject, IDX11RenderBufferProxy
    {
        protected Texture2D colorBuffer;
        protected Texture2D depthStencilBuffer;
        protected RenderTargetView colorBufferView;
        protected DepthStencilView depthStencilBufferView;

        public int TargetWidth { private set; get; }
        public int TargetHeight { private set; get; }

        public RenderTargetView ColorBufferView { get { return colorBufferView; } }
        public DepthStencilView DepthStencilBufferView { get { return depthStencilBufferView; } }
        public Texture2D ColorBuffer { get { return colorBuffer; } }
        public Texture2D DepthStencilBuffer { get { return depthStencilBuffer; } }

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

        public DX11RenderBufferProxy(Device device)
        {
            Device = device;
        }

        private Texture2D CreateRenderTarget(int width, int height, MSAALevel msaa)
        {
            MSAA = msaa;
            TargetWidth = width;
            TargetHeight = height;
            RemoveAndDispose(ref colorBufferView);
            RemoveAndDispose(ref depthStencilBufferView);
            RemoveAndDispose(ref colorBuffer);
            RemoveAndDispose(ref depthStencilBuffer);
#if MSAA
            RemoveAndDispose(ref renderTargetNMS);
#endif
            var texture = OnCreateRenderTargetAndDepthBuffers(width, height);
            Initialized = true;
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
            return renderTargetNMS;
#else
            return colorBufferView;
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

        public void ClearRenderTargetBinding(DeviceContext context)
        {
            context.OutputMerger.SetTargets(null, new RenderTargetView[0]);
        }

        public void ClearRenderTarget(DeviceContext context, Color4 color)
        {
            ClearRenderTarget(context, color, true, true);
        }
        /// <summary>
        /// Clears the buffers with the clear-color
        /// </summary>
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

        public Texture2D StartD3D(int width, int height, MSAALevel msaa)
        {
            return CreateRenderTarget(width, height, msaa);
        }
        /// <summary>
        /// Resize render target and depthbuffer resolution
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public virtual Texture2D Resize(int width, int height)
        {
            return CreateRenderTarget(width, height, MSAA);
        }

        /// <summary>
        /// Dispose all resources, same as calling <see cref="DisposeObject.DisposeAndClear"/>
        /// </summary>
        public void EndD3D()
        {
            Initialized = false;
            DisposeAndClear();
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            Initialized = false;
            base.Dispose(disposeManagedResources);
        }
    }

    public class DX11SwapChainRenderBufferProxy : DX11RenderBufferProxy
    {
        private D2DControlWrapper d2dControls;
        public D2DControlWrapper D2DControls
        {
            get { return d2dControls; }
        }
        private SwapChain1 swapChain;
        public SwapChain1 SwapChain { get { return swapChain; } }

        private System.IntPtr surfacePtr;
        public DX11SwapChainRenderBufferProxy(System.IntPtr surfacePointer, Device device) : base(device)
        {
            surfacePtr = surfacePointer;
            swapChain = Collect(CreateSwapChain(surfacePtr));
            d2dControls = Collect(new D2DControlWrapper());
            d2dControls.Initialize(swapChain);
        }

        protected override Texture2D OnCreateRenderTargetAndDepthBuffers(int width, int height)
        {
            if (swapChain == null || swapChain.IsDisposed)
            {
                swapChain = Collect(CreateSwapChain(surfacePtr));
                RemoveAndDispose(ref d2dControls);
                d2dControls = Collect(new D2DControlWrapper());
                d2dControls.Initialize(swapChain);
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
            return colorBuffer;
        }

        private SwapChain1 CreateSwapChain(System.IntPtr surfacePointer)
        {
            var desc = CreateSwapChainDescription();
            using (var dxgiDevice2 = Device.QueryInterface<global::SharpDX.DXGI.Device2>())
            using (var dxgiAdapter = dxgiDevice2.Adapter)
            using (var dxgiFactory2 = dxgiAdapter.GetParent<Factory2>())
            using (var output = dxgiAdapter.Outputs.First())
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
    }
}
