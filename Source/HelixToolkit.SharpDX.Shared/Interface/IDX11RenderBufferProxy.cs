/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
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
        bool Initialized { get; }
        RenderTargetView ColorBufferView { get; }
        DepthStencilView DepthStencilBufferView { get; }
        Texture2D ColorBuffer { get; }
        Texture2D DepthStencilBuffer { get; }
        MSAALevel MSAA { get; }
        int TargetHeight { get; }
        int TargetWidth { get; }
        void ClearRenderTarget(DeviceContext context, Color4 color);
        void ClearRenderTarget(DeviceContext context, Color4 color, bool clearBackBuffer, bool clearDepthStencilBuffer);
        void EndD3D();
        Texture2D Resize(int width, int height);
        void SetDefaultRenderTargets(DeviceContext context);
        Texture2D StartD3D(int width, int height, MSAALevel msaa);
    }
}