/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
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
            return new DX11SwapChainRenderBufferProxy(surface, EffectsManager, false);
        }
    }
}
