/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Logger;
using System;
using System.Runtime.CompilerServices;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Render

{
    public class ScreenCloneRenderHost : SwapChainRenderHost
    {
        public ScreenCloneRenderHost(IntPtr surface) : base(surface)
        {
            RenderConfiguration = new DX11RenderHostConfiguration()
            {
                ClearEachFrame=false, RenderD2D=false, RenderLights=false, UpdatePerFrameData=false
            };
        }

        protected override IDX11RenderBufferProxy CreateRenderBuffer()
        {
            Logger.Log(LogLevel.Information, "DX11SwapChainRenderBufferProxy", nameof(ScreenCloneRenderHost));
            return new DX11SwapChainRenderBufferProxy(surface, EffectsManager, false);
        }
    }
}
#endif