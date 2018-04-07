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

namespace HelixToolkit.UWP.CommonDX
{
    using Logger;
    using Render;
    /// <summary>
    /// 
    /// </summary>
    public class SwapChainCompositionRenderHost : DefaultRenderHost
    {
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