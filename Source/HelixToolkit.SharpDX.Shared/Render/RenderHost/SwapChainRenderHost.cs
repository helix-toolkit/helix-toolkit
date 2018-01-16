/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Core;
    using Core2D;
    /// <summary>
    /// 
    /// </summary>
    public class SwapChainRenderHost : DefaultRenderHost
    {
        private IntPtr surface;
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
        public SwapChainRenderHost(IntPtr surface, Func<Device, IRenderer> createRenderer) : base(createRenderer)
        {
            this.surface = surface;
        }
        /// <summary>
        /// Creates the render buffer.
        /// </summary>
        /// <returns></returns>
        protected override IDX11RenderBufferProxy CreateRenderBuffer()
        {
            return new DX11SwapChainRenderBufferProxy(surface, Device);
        }
    }
}
