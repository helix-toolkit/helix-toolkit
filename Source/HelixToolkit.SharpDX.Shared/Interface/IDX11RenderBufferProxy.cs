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
    /// <summary>
    /// 
    /// </summary>
    public interface IDX11RenderBufferProxy : IDisposable
    {
        /// <summary>
        /// Occurs when [on new buffer created].
        /// </summary>
        event EventHandler<Texture2D> OnNewBufferCreated;
        /// <summary>
        /// Occurs when [on device lost].
        /// </summary>
        event EventHandler<bool> OnDeviceLost;
        /// <summary>
        /// Gets a value indicating whether this <see cref="IDX11RenderBufferProxy"/> is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if initialized; otherwise, <c>false</c>.
        /// </value>
        bool Initialized { get; }
        /// <summary>
        /// Gets the color buffer view.
        /// </summary>
        /// <value>
        /// The color buffer view.
        /// </value>
        RenderTargetView ColorBufferView { get; }
        /// <summary>
        /// Gets the depth stencil buffer view.
        /// </summary>
        /// <value>
        /// The depth stencil buffer view.
        /// </value>
        DepthStencilView DepthStencilBufferView { get; }
        /// <summary>
        /// Gets the color buffer.
        /// </summary>
        /// <value>
        /// The color buffer.
        /// </value>
        Texture2D ColorBuffer { get; }
        /// <summary>
        /// Gets the depth stencil buffer.
        /// </summary>
        /// <value>
        /// The depth stencil buffer.
        /// </value>
        Texture2D DepthStencilBuffer { get; }
#if MSAA
        /// <summary>
        /// 
        /// </summary>
        MSAALevel MSAA { get; }
#endif        
        /// <summary>
        /// Gets the height of the target.
        /// </summary>
        /// <value>
        /// The height of the target.
        /// </value>
        int TargetHeight { get; }
        /// <summary>
        /// Gets the width of the target.
        /// </summary>
        /// <value>
        /// The width of the target.
        /// </value>
        int TargetWidth { get; }
        /// <summary>
        /// Clears the render target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="color">The color.</param>
        void ClearRenderTarget(DeviceContext context, Color4 color);
        /// <summary>
        /// Clears the render target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="color">The color.</param>
        /// <param name="clearBackBuffer">if set to <c>true</c> [clear back buffer].</param>
        /// <param name="clearDepthStencilBuffer">if set to <c>true</c> [clear depth stencil buffer].</param>
        void ClearRenderTarget(DeviceContext context, Color4 color, bool clearBackBuffer, bool clearDepthStencilBuffer);
        /// <summary>
        /// Resizes the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        Texture2D Resize(int width, int height);
        /// <summary>
        /// Sets the default render targets.
        /// </summary>
        /// <param name="context">The context.</param>
        void SetDefaultRenderTargets(DeviceContext context);
        /// <summary>
        /// Initializes the specified width.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="msaa">The msaa.</param>
        /// <returns></returns>
        Texture2D Initialize(int width, int height, MSAALevel msaa);
        /// <summary>
        /// Gets the d2 d controls.
        /// </summary>
        /// <value>
        /// The d2 d controls.
        /// </value>
        ID2DTarget D2DControls { get; }
        /// <summary>
        /// Begins the draw.
        /// </summary>
        /// <returns></returns>
        bool BeginDraw();
        /// <summary>
        /// Ends the draw.
        /// </summary>
        /// <returns></returns>
        bool EndDraw();
        /// <summary>
        /// Begins the draw2 d.
        /// </summary>
        /// <returns></returns>
        bool BeginDraw2D();
        /// <summary>
        /// Ends the draw2 d.
        /// </summary>
        /// <returns></returns>
        bool EndDraw2D();
        /// <summary>
        /// Presents this instance.
        /// </summary>
        /// <returns></returns>
        bool Present();
    }
}