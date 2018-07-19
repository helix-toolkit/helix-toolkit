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
using global::SharpDX.Direct3D11;
using HelixToolkit.Mathematics;
using System;
using System.Collections.Generic;
#if DX11_1
using Device = SharpDX.Direct3D11.Device1;
using DeviceContext = SharpDX.Direct3D11.DeviceContext1;
#endif

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{

    using Core2D;
    using HelixToolkit.Logger;
    using Model.Scene;
    using Render;
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public interface IRenderHost : IGUID, IDisposable
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        LogWrapper Logger { get; }
        /// <summary>
        /// Fired whenever an exception occurred on this object.
        /// </summary>
        event EventHandler<RelayExceptionEventArgs> ExceptionOccurred;
        /// <summary>
        /// Occurs when [on new render target texture].
        /// </summary>
        event EventHandler<Texture2DArgs> OnNewRenderTargetTexture;
        /// <summary>
        /// Occurs when [start render loop].
        /// </summary>
        event EventHandler<EventArgs> StartRenderLoop;
        /// <summary>
        /// Occurs when [stop render loop].
        /// </summary>
        event EventHandler<EventArgs> StopRenderLoop;
        /// <summary>
        /// Occurs when each render frame finished rendering.
        /// </summary>
        event EventHandler OnRendered;
        /// <summary>
        /// Gets the device.
        /// </summary>
        /// <value>
        /// The device.
        /// </value>
        Device Device { get; }
        /// <summary>
        /// Gets the device2d.
        /// </summary>
        /// <value>
        /// The device2d.
        /// </value>
        global::SharpDX.Direct2D1.Device Device2D { get; }
        /// <summary>
        /// Gets or sets the color of the clear.
        /// </summary>
        /// <value>
        /// The color of the clear.
        /// </value>
        Color4 ClearColor { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is shadow map enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is shadow map enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsShadowMapEnabled { set; get; }
#if MSAA
        MSAALevel MSAA { get; set; }
#endif        
        /// <summary>
        /// Gets or sets the viewport.
        /// </summary>
        /// <value>
        /// The viewport.
        /// </value>
        IViewport3DX Viewport { get; set; }
        /// <summary>
        /// Gets the render context.
        /// </summary>
        /// <value>
        /// The render context.
        /// </value>
        RenderContext RenderContext { get; }
        /// <summary>
        /// Renderer
        /// </summary>
        IRenderer Renderer { get; }
        /// <summary>
        /// Sets the default render targets.
        /// </summary>
        /// <param name="clear">if set to <c>true</c> [clear].</param>
        void SetDefaultRenderTargets(bool clear = true);
        /// <summary>
        /// Gets or sets the effects manager.
        /// </summary>
        /// <value>
        /// The effects manager.
        /// </value>
        IEffectsManager EffectsManager { get; set; }

        /// <summary>
        /// This technique is used for the entire render pass 
        /// by all Element3D if not specified otherwise in
        /// the elements itself
        /// </summary>
        IRenderTechnique RenderTechnique { set; get; }
        /// <summary>
        /// Gets the feature level.
        /// </summary>
        /// <value>
        /// The feature level.
        /// </value>
        global::SharpDX.Direct3D.FeatureLevel FeatureLevel { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is deferred lighting.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deferred lighting; otherwise, <c>false</c>.
        /// </value>
        bool IsDeferredLighting { get; }
        /// <summary>
        /// Gets the actual height.
        /// </summary>
        /// <value>
        /// The actual height.
        /// </value>
        double ActualHeight { get; }
        /// <summary>
        /// Gets the actual width.
        /// </summary>
        /// <value>
        /// The actual width.
        /// </value>
        double ActualWidth { get; }

        /// <summary>
        /// Indicates if DPFCanvas busy on rendering.
        /// </summary>
        bool IsBusy { get; }
        /// <summary>
        /// Gets or sets a value indicating whether [enable render frustum].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable render frustum]; otherwise, <c>false</c>.
        /// </value>
        bool EnableRenderFrustum { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether [enable sharing model mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable sharing model mode]; otherwise, <c>false</c>.
        /// </value>
        bool EnableSharingModelMode { set; get; }
        /// <summary>
        /// Gets or sets the shared model container.
        /// </summary>
        /// <value>
        /// The shared model container.
        /// </value>
        IModelContainer SharedModelContainer { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is rendering.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is rendering; otherwise, <c>false</c>.
        /// </value>
        bool IsRendering { set; get; }
        /// <summary>
        /// Gets the color buffer view.
        /// </summary>
        /// <value>
        /// The color buffer view.
        /// </value>
        RenderTargetView RenderTargetBufferView { get; }
        /// <summary>
        /// Gets the depth stencil buffer view.
        /// </summary>
        /// <value>
        /// The depth stencil buffer view.
        /// </value>
        DepthStencilView DepthStencilBufferView { get; }
        /// <summary>
        /// Gets the d2d target.
        /// </summary>
        /// <value>
        /// The d2d target.
        /// </value>
        D2DTargetProxy D2DTarget { get; }
        /// <summary>
        /// Gets the current frame flattened scene graph
        /// </summary>
        /// <value>
        /// The per frame renderable.
        /// </value>
        List<KeyValuePair<int, SceneNode>> PerFrameFlattenedScene { get; }
        /// <summary>
        /// Gets the current frame lights
        /// </summary>
        /// <value>
        /// The per frame lights.
        /// </value>
        IEnumerable<LightNode> PerFrameLights { get; }
        /// <summary>
        /// Gets the per frame nodes with post effects. It is the subset of <see cref="PerFrameOpaqueNodes"/>
        /// </summary>
        /// <value>
        /// Gets the per frame nodes with post effects.
        /// </value>
        List<SceneNode> PerFrameNodesWithPostEffect { get; }
        /// <summary>
        /// Gets the per frame nodes for opaque rendering. <see cref="RenderType.Opaque"/>
        /// <para>This does not include <see cref="RenderType.Transparent"/>, <see cref="RenderType.Particle"/>, <see cref="RenderType.PreProc"/>, <see cref="RenderType.PostProc"/>, <see cref="RenderType.Light"/>, <see cref="RenderType.ScreenSpaced"/></para>
        /// </summary>
        List<SceneNode> PerFrameOpaqueNodes { get; }
        /// <summary>
        /// Gets the per frame particle nodes. <see cref="RenderType.Particle"/>
        /// </summary>
        /// <value>
        /// The per frame particle nodes.
        /// </value>
        List<SceneNode> PerFrameParticleNodes { get; }
        /// <summary>
        /// Gets the per frame transparent nodes. , <see cref="RenderType.Transparent"/>
        /// </summary>
        /// <value>
        /// The per frame transparent nodes.
        /// </value>
        List<SceneNode> PerFrameTransparentNodes { get; }
        /// <summary>
        /// Starts the d3 d.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        void StartD3D(double width, double height);
        /// <summary>
        /// Ends the d3 d.
        /// </summary>
        void EndD3D();
        /// <summary>
        /// Updates the and render.
        /// </summary>
        void UpdateAndRender();
        /// <summary>
        /// Invalidates the render.
        /// </summary>
        void InvalidateRender();
        /// <summary>
        /// Invalidates the scene graph. This also calls <see cref="InvalidatePerFrameRenderables"/>
        /// </summary>
        void InvalidateSceneGraph();
        /// <summary>
        /// Invalidates the per frame renderables. Called when <see cref="SceneNode.IsRenderable"/> changed or <see cref="SceneNode.RenderType"/> changed.
        /// </summary>
        void InvalidatePerFrameRenderables();
        /// <summary>
        /// Resizes
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        void Resize(double width, double height);
        /// <summary>
        /// Gets or sets a value indicating whether [show statistics].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show statistics]; otherwise, <c>false</c>.
        /// </value>
        RenderDetail ShowRenderDetail { set; get; }
        /// <summary>
        /// Gets the render statistics.
        /// </summary>
        /// <value>
        /// The render statistics.
        /// </value>
        IRenderStatistics RenderStatistics { get; }
        /// <summary>
        /// Gets or sets the render configuration.
        /// </summary>
        /// <value>
        /// The render configuration.
        /// </value>
        DX11RenderHostConfiguration RenderConfiguration { set; get; }
        /// <summary>
        /// Gets the render buffer.
        /// </summary>
        /// <value>
        /// The render buffer.
        /// </value>
        DX11RenderBufferProxyBase RenderBuffer { get; }

        /// <summary>
        /// Clears the render target.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="clearBackBuffer">if set to <c>true</c> [clear back buffer].</param>
        /// <param name="clearDepthStencilBuffer">if set to <c>true</c> [clear depth stencil buffer].</param>
        void ClearRenderTarget(DeviceContextProxy context, bool clearBackBuffer, bool clearDepthStencilBuffer);
    }

    public sealed class DX11RenderHostConfiguration
    {
        /// <summary>
        /// The render d2d
        /// </summary>
        public bool RenderD2D { set; get; } = true;
        /// <summary>
        /// The update global variable
        /// </summary>
        public bool UpdatePerFrameData { set; get; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether [render lights].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render lights]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderLights { set; get; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether [clear render target before each frame].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [clear render target before each frame]; otherwise, <c>false</c>.
        /// </value>
        public bool ClearEachFrame { set; get; } = true;

        /// <summary>
        /// Auto update octree in geometry during rendering. 
        /// </summary>
        public bool AutoUpdateOctree { set; get; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether [enable oit rendering].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable oit rendering]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableOITRendering { set; get; } = true;
        /// <summary>
        /// Gets or sets the OIT weight power used for color weight calculation. Default = 3.
        /// </summary>
        /// <value>
        /// The OIT weight power.
        /// </value>
        public float OITWeightPower { set; get; } = 3;

        /// <summary>
        /// Gets or sets the oit weight depth slope. Used to increase resolution for particular range of depth values. 
        /// <para>If value = 2, the depth range from 0-0.5 expands to 0-1 to increase resolution. However, values from 0.5 - 1 will be pushed to 1</para>
        /// </summary>
        /// <value>
        /// The oit weight depth slope.
        /// </value>
        public float OITWeightDepthSlope { set; get; } = 1;
        /// <summary>
        /// Gets or sets the oit weight mode.
        /// <para>Please refer to http://jcgt.org/published/0002/02/09/ </para>
        /// <para>Linear0: eq7; Linear1: eq8; Linear2: eq9; NonLinear: eq10</para>
        /// </summary>
        /// <value>
        /// The oit weight mode.
        /// </value>
        public OITWeightMode OITWeightMode
        {
            set; get;
        } = OITWeightMode.Linear1;
        /// <summary>
        /// Enable FXAA. If MSAA used, FXAA will be disabled automatically
        /// </summary>
        public FXAALevel FXAALevel { set; get; } = FXAALevel.None;

        /// <summary>
        /// Gets or sets a value indicating whether [enable render order] specified by user.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable render order]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRenderOrder { set; get; } = false;
    }
}
