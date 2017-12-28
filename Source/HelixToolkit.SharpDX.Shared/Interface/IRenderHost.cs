// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRenderHost.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
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

#if MSAA
    public enum MSAALevel
    {
        Disable = 0, Maximum = 1, Two = 2, Four = 4, Eight = 8
    }
#endif
    public interface IRenderHost
    {
        /// <summary>
        /// Fired whenever an exception occurred on this object.
        /// </summary>
        event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;

        Device Device { get; }
        Color4 ClearColor { get; }
        bool IsShadowMapEnabled { get; }
        //bool IsDeferredEnabled { get;  }
#if MSAA
        MSAALevel MSAA { get; set; }
#endif
        IRenderer Renderable { get; set; }

        IRenderContext RenderContext { get; }
        /// <summary>
        /// Invalidates the current render and requests an update.
        /// </summary>
        void InvalidateRender();
        void SetDefaultRenderTargets(bool clear = true);
        void SetDefaultColorTargets(DepthStencilView dsv);

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

        /// <summary>
        /// Shared light data per scene
        /// </summary>
        Light3DSceneShared Light3DSceneShared { get; }

        bool EnableRenderFrustum { set; get; }

        uint MaxFPS { set; get; }

        bool EnableSharingModelMode { set; get; }

        IModelContainer SharedModelContainer { set; get; }

        bool IsRendering { set; get; }

        RenderTargetView ColorBufferView { get; }
        DepthStencilView DepthStencilBufferView { get; }

        D2DControlWrapper D2DControls { get; }
    }
}
