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
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Render
    {
        using Core2D;
        using Utilities;
        /// <summary>
        /// 
        /// </summary>
        public class DX11Texture2DRenderBufferProxy : DX11RenderBufferProxyBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DX11Texture2DRenderBufferProxy"/> class.
            /// </summary>
            /// <param name="deviceResources"></param>
            public DX11Texture2DRenderBufferProxy(IDeviceResources deviceResources) : base(deviceResources)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            protected override ShaderResourceViewProxy OnCreateBackBuffer(int width, int height)
            {
                var colordescNMS = new Texture2DDescription
                {
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    Format = Format,
                    Width = width,
                    Height = height,
                    MipLevels = 1,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    OptionFlags = ResourceOptionFlags.Shared,
                    CpuAccessFlags = CpuAccessFlags.None,
                    ArraySize = 1
                };

                var backBuffer = Collect(new ShaderResourceViewProxy(Device, colordescNMS));
                d2dTarget = Collect(new D2DTargetProxy());
                d2dTarget.Initialize(backBuffer.Resource as Texture2D, DeviceContext2D);
                return backBuffer;
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

}
