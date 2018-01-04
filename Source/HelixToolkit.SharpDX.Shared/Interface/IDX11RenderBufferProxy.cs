/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Render
#else
namespace HelixToolkit.Wpf.SharpDX.Render
#endif
{
    using Core2D;
    public interface IDX11RenderBufferProxy : IDisposable
    {
        Color4 ClearColor { get; set; }
        RenderTargetView ColorBufferView { get; }
        DepthStencilView DepthStencilBufferView { get; }
        MSAALevel MSAA { get; }
        int TargetHeight { get; }
        int TargetWidth { get; }
        void ClearRenderTarget(DeviceContext context, bool clearBackBuffer = true, bool clearDepthStencilBuffer = true);
        void EndD3D();
        Texture2D Resize(int width, int height);
        void SetDefaultRenderTargets(DeviceContext context);
        void SetDefaultRenderTargets(DeviceContext context, bool clear);
        Texture2D StartD3D(int width, int height, MSAALevel msaa);
    }
}