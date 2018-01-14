// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRenderHost.cs" company="Helix Toolkit">
//   Copyright (c) 2018 Helix Toolkit contributors
// </copyright>
// <summary>
//   This technique is used for the entire render pass 
//   by all Element3D if not specified otherwise in
//   the elements itself
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using System;

    using global::SharpDX;

    using global::SharpDX.Direct3D11;

    using Utilities;
    using Model;
    using Core2D;

    public interface IRenderHost
    {
        /// <summary>
        /// Fired whenever an exception occurred on this object.
        /// </summary>
        event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;
        event EventHandler<Texture2D> OnNewRenderTargetTexture;
        event EventHandler<bool> StartRenderLoop;
        event EventHandler<bool> StopRenderLoop;

        Device Device { get; }
        Color4 ClearColor { set; get; }
        bool IsShadowMapEnabled { set; get; }
#if MSAA
        MSAALevel MSAA { get; set; }
#endif
        IViewport3DX Viewport { get; set; }

        IRenderContext RenderContext { get; }

        void SetDefaultRenderTargets(bool clear = true);

        IEffectsManager EffectsManager { get; set; }

        /// <summary>
        /// This technique is used for the entire render pass 
        /// by all Element3D if not specified otherwise in
        /// the elements itself
        /// </summary>
        IRenderTechnique RenderTechnique { get; }

        bool IsDeferredLighting { get; }

        double ActualHeight { get; }
        double ActualWidth { get; }

        /// <summary>
        /// Indicates if DPFCanvas busy on rendering.
        /// </summary>
        bool IsBusy { get; }

        bool EnableRenderFrustum { set; get; }

        uint MaxFPS { set; get; }

        bool EnableSharingModelMode { set; get; }

        IModelContainer SharedModelContainer { set; get; }

        bool IsRendering { set; get; }

        RenderTargetView ColorBufferView { get; }
        DepthStencilView DepthStencilBufferView { get; }

        ID2DTarget D2DControls { get; }

        void StartD3D(double width, double height);
        void EndD3D();
        void UpdateAndRender();
        void InvalidateRender();
        void Resize(double width, double height);
    }
}
