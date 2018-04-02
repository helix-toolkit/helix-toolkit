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
    public class DX11Texture2DRenderBufferProxy : DX11RenderBufferProxyBase
    {

#if MSAA
        private Texture2D renderTargetNMS;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="DX11Texture2DRenderBufferProxy"/> class.
        /// </summary>
        /// <param name="deviceResources"></param>
        public DX11Texture2DRenderBufferProxy(IDeviceResources deviceResources) : base(deviceResources)
        {
        }
        /// <summary>
        /// Disposes the buffers.
        /// </summary>
        protected override void DisposeBuffers()
        {
            base.DisposeBuffers();
#if MSAA
            RemoveAndDispose(ref renderTargetNMS);
#endif
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="createDepthStencilBuffer"></param>
        /// <returns></returns>
        protected override Texture2D OnCreateRenderTargetAndDepthBuffers(int width, int height, bool createDepthStencilBuffer)
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

            colorBuffer = Collect(new Texture2D(Device, colordesc));
            colorBufferView = Collect(new RenderTargetView(Device, colorBuffer));

            if (createDepthStencilBuffer)
            {
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
                depthStencilBuffer = Collect(new Texture2D(Device, depthdesc));
                depthStencilBufferView = Collect(new DepthStencilView(Device, depthStencilBuffer));
            }
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
            d2dTarget = Collect(new D2DTargetProxy());
            d2dTarget.Initialize(renderTargetNMS, DeviceContext2D);
            return renderTargetNMS;
#else
            return colorBuffer;
#endif            
        }

        /// <summary>
        /// Ends the draw.
        /// </summary>
        /// <returns></returns>
        public override bool EndDraw()
        {
            Device.ImmediateContext.Flush();
#if MSAA
            Device.ImmediateContext.ResolveSubresource(ColorBuffer, 0, renderTargetNMS, 0, Format.B8G8R8A8_UNorm);
#endif            
            return true;
        }
        /// <summary>
        /// Presents this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Present()
        {
            Device.ImmediateContext.Flush();
            return true;
        }
    }
}
