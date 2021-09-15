/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Logger;
using SharpDX.Direct3D11;
using System;
using System.Runtime.CompilerServices;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#else
using Device = SharpDX.Direct3D11.Device;
#endif

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
        /// <summary>
        /// 
        /// </summary>
        public class SwapChainRenderHost : DefaultRenderHost
        {
            protected readonly IntPtr surface;
            /// <summary>
            /// Initializes a new instance of the <see cref="SwapChainRenderHost"/> class.
            /// </summary>
            /// <param name="surface">The window PTR.</param>
            public SwapChainRenderHost(IntPtr surface)
            {
                this.surface = surface;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SwapChainRenderHost"/> class.
            /// </summary>
            /// <param name="surface">The surface.</param>
            /// <param name="createRenderer">The create renderer.</param>
            public SwapChainRenderHost(IntPtr surface, Func<IDevice3DResources, IRenderer> createRenderer) : base(createRenderer)
            {
                this.surface = surface;
            }
            /// <summary>
            /// Creates the render buffer.
            /// </summary>
            /// <returns></returns>
            protected override DX11RenderBufferProxyBase CreateRenderBuffer()
            {
                Logger.Log(LogLevel.Information, "DX11SwapChainRenderBufferProxy", nameof(SwapChainRenderHost));
                return new DX11SwapChainRenderBufferProxy(surface, EffectsManager);
            }
        }
    }

}
