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
    public class DX11SwapChainRenderBufferProxy : DX11RenderBufferProxyBase
    {
        private SwapChain1 swapChain;
        /// <summary>
        /// Gets the swap chain.
        /// </summary>
        /// <value>
        /// The swap chain.
        /// </value>
        public SwapChain1 SwapChain { get { return swapChain; } }
        /// <summary>
        /// The surface pointer
        /// </summary>
        protected readonly System.IntPtr surfacePtr;
        /// <summary>
        /// Initializes a new instance of the <see cref="DX11SwapChainRenderBufferProxy"/> class.
        /// </summary>
        /// <param name="surfacePointer">The surface pointer.</param>
        /// <param name="deviceResource"></param>
        public DX11SwapChainRenderBufferProxy(System.IntPtr surfacePointer, IDeviceResources deviceResource) : base(deviceResource)
        {
            surfacePtr = surfacePointer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DX11SwapChainRenderBufferProxy"/> class.
        /// </summary>
        /// <param name="surfacePointer">The surface pointer.</param>
        /// <param name="deviceResource"></param>
        /// <param name="useDepthStencilBuffer"></param>
        public DX11SwapChainRenderBufferProxy(System.IntPtr surfacePointer, IDeviceResources deviceResource, bool useDepthStencilBuffer)
            : base(deviceResource, useDepthStencilBuffer)
        {
            surfacePtr = surfacePointer;
        }
        /// <summary>
        /// Called when [create render target and depth buffers].
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="createDepthStencilBuffer"></param>
        /// <returns></returns>
        protected override Texture2D OnCreateRenderTargetAndDepthBuffers(int width, int height, bool createDepthStencilBuffer)
        {
            if (swapChain == null || swapChain.IsDisposed)
            {
                swapChain = CreateSwapChain(surfacePtr);
            }
            else
            {
                swapChain.ResizeBuffers(swapChain.Description1.BufferCount, TargetWidth, TargetHeight, swapChain.Description.ModeDescription.Format, swapChain.Description.Flags);
            }
            colorBuffer = Collect(Texture2D.FromSwapChain<Texture2D>(swapChain, 0));
            var sampleDesc = swapChain.Description1.SampleDescription;
            colorBufferView = Collect(new RenderTargetView(Device, colorBuffer));

            if (createDepthStencilBuffer)
            {
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
                depthStencilBuffer = Collect(new Texture2D(Device, depthdesc));
                depthStencilBufferView = Collect(new DepthStencilView(Device, depthStencilBuffer));
            }
            d2dTarget = Collect(new D2DTargetProxy());
            d2dTarget.Initialize(swapChain, DeviceContext2D);
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
                if (desc == global::SharpDX.DXGI.ResultCode.DeviceRemoved || desc == global::SharpDX.DXGI.ResultCode.DeviceReset || desc == global::SharpDX.DXGI.ResultCode.DeviceHung)
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
        /// <summary>
        /// Must release swapchain at last after all its created resources have been released.
        /// </summary>
        public override void DisposeAndClear()
        {
            base.DisposeAndClear();
            Disposer.RemoveAndDispose(ref swapChain);
        }
    }
}
