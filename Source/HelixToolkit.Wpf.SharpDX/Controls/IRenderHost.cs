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
    using global::SharpDX;

    using global::SharpDX.Direct3D11;

    public interface IRenderHost
    {
        Device Device { get; }
        Color4 ClearColor { get; }
        bool IsShadowMapEnabled { get; }
        //bool IsDeferredEnabled { get;  }
        bool IsMSAAEnabled { get; }
        IRenderer Renderable { get; }
        void SetDefaultRenderTargets();
        void SetDefaultColorTargets(DepthStencilView dsv);

        IEffectsManager EffectsManager { get; }

        IRenderTechniquesManager RenderTechniquesManager { get; }

        /// <summary>
        /// This technique is used for the entire render pass 
        /// by all Element3D if not specified otherwise in
        /// the elements itself
        /// </summary>
        RenderTechnique RenderTechnique { get; }

        double ActualHeight { get; }
        double ActualWidth { get; }

        bool SupportDeferredRender { get; }
    }
}