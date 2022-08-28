/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
#else
using Device = SharpDX.Direct3D11.Device;
#endif
using System;
using Microsoft.Extensions.Logging;

namespace HelixToolkit.UWP.CommonDX
{
    using Logger;
    using Render;
    /// <summary>
    /// 
    /// </summary>
    public class SwapChainCompositionRenderHost : DefaultRenderHost
    {
        static readonly ILogger logger = LogManager.Create<SwapChainCompositionRenderHost>();
        /// <summary>
        /// Initializes a new instance of the <see cref="SwapChainRenderHost"/> class.
        /// </summary>
        public SwapChainCompositionRenderHost()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwapChainRenderHost"/> class.
        /// </summary>
        /// <param name="createRenderer">The create renderer.</param>
        public SwapChainCompositionRenderHost(Func<IDevice3DResources, IRenderer> createRenderer) : base(createRenderer)
        {
        }
        /// <summary>
        /// Creates the render buffer.
        /// </summary>
        /// <returns></returns>
        protected override DX11RenderBufferProxyBase CreateRenderBuffer()
        {
            logger.LogInformation("DX11SwapChainCompositionRenderBufferProxy");
            return new DX11SwapChainCompositionRenderBufferProxy(EffectsManager);
        }
    }
}