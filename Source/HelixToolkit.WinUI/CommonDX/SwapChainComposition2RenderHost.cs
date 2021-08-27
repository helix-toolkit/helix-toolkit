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

namespace HelixToolkit.WinUI.CommonDX
{
    using Logger;
    using Microsoft.UI.Xaml.Controls;
    using Render;
    using SharpDX;
    using SharpDX.DXGI;
    using System.Runtime.InteropServices;
    using WinRT;

    /// <summary>
    /// 
    /// </summary>
    public class SwapChainComposition2RenderHost : DefaultRenderHost
    {
        protected readonly SwapChainPanel surface;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwapChainRenderHost"/> class.
        /// </summary>
        public SwapChainComposition2RenderHost(SwapChainPanel surface)
        {
            this.surface = surface;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwapChainRenderHost"/> class.
        /// </summary>
        /// <param name="createRenderer">The create renderer.</param>
        public SwapChainComposition2RenderHost(SwapChainPanel surface, Func<IDevice3DResources, IRenderer> createRenderer) : base(createRenderer)
        {
            this.surface = surface;
        }
        /// <summary>
        /// Creates the render buffer.
        /// </summary>
        /// <returns></returns>
        protected override DX11RenderBufferProxyBase CreateRenderBuffer()
        {
            Logger.Log(LogLevel.Information, "DX11SwapChainCompositionRenderBufferProxy", nameof(SwapChainRenderHost));
            var sccb = new DX11SwapChainCompositionRenderBufferProxy(EffectsManager);

            return sccb;
        }
    }
}