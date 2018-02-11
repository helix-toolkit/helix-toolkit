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
            Log(LogLevel.Information, "DX11SwapChainRenderBufferProxy");
            return new DX11SwapChainRenderBufferProxy(surface, EffectsManager, false);
        }

        private void Log<Type>(LogLevel level, Type msg, [CallerMemberName]string caller = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Logger.Log(level, msg, nameof(ScreenCloneRenderHost), caller, sourceLineNumber);
        }
    }
}
#endif