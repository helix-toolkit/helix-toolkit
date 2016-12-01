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

namespace HelixToolkit.Wpf.SharpDX
{
    using System;

    using global::SharpDX;

    using global::SharpDX.Direct3D11;

    using HelixToolkit.Wpf.SharpDX.Utilities;

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

        /// <summary>
        /// Invalidates the current render and requests an update.
        /// </summary>
        void InvalidateRender();
        void SetDefaultRenderTargets();
        void SetDefaultColorTargets(DepthStencilView dsv);

        IEffectsManager EffectsManager { get; set; }

        IRenderTechniquesManager RenderTechniquesManager { get; set; }

        /// <summary>
        /// This technique is used for the entire render pass 
        /// by all Element3D if not specified otherwise in
        /// the elements itself
        /// </summary>
        RenderTechnique RenderTechnique { get; }

        double ActualHeight { get; }
        double ActualWidth { get; }

        /// <summary>
        /// Indicates if DPFCanvas busy on rendering.
        /// </summary>
        bool IsBusy { get; }

        int RenderCycles { set; get; }
    }
}
