// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRenderable.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    using global::SharpDX;
    using Cameras;
    using System.Collections.Generic;
    using Core;
    using System;
    using Render;
    /// <summary>
    /// 
    /// </summary>
    public interface IRenderable : IAttachable, IBoundable, IGUID, ITransform
    {
        /// <summary>
        /// Gets a value indicating whether this instance is renderable. Test includes Visible and view frustum test.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is renderable; otherwise, <c>false</c>.
        /// </value>
        bool IsRenderable { get; }

        /// <summary>
        /// Whether is visible, controlled by Visibility and IsRendering
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Optional for scene graph traverse
        /// </summary>
        IList<IRenderable> Items { get; }

        RenderCore RenderCore { get; }
        /// <summary>
        /// Update render related parameters such as model matrix by scene graph and bounding boxes
        /// </summary>
        /// <param name="context"></param>
        void Update(IRenderContext context);
        /// <summary>
        /// Update things not related to rendering, such as OctreeManager etc. Called parallel with rendering process.
        /// </summary>
        /// <param name="context"></param>
        void UpdateNotRender(IRenderContext context);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext"></param>
        void Render(IRenderContext context, DeviceContextProxy deviceContext);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IViewport3DX
    {
        /// <summary>
        /// Attaches the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        void Attach(IRenderHost host);
        /// <summary>
        /// Detaches this instance.
        /// </summary>
        void Detach();
        /// <summary>
        /// Gets the render host.
        /// </summary>
        /// <value>
        /// The render host.
        /// </value>
        IRenderHost RenderHost { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is shadow mapping enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is shadow mapping enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsShadowMappingEnabled { get; }
        /// <summary>
        /// Gets or sets the effects manager.
        /// </summary>
        /// <value>
        /// The effects manager.
        /// </value>
        IEffectsManager EffectsManager { set; get; }
        /// <summary>
        /// Gets or sets the render technique.
        /// </summary>
        /// <value>
        /// The render technique.
        /// </value>
        IRenderTechnique RenderTechnique { set; get; }
        /// <summary>
        /// Gets the camera core.
        /// </summary>
        /// <value>
        /// The camera core.
        /// </value>
        CameraCore CameraCore { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeStamp"></param>
        //DeferredRenderer DeferredRenderer { get; set; }
        void Update(TimeSpan timeStamp);
        /// <summary>
        /// Gets the world matrix.
        /// </summary>
        /// <value>
        /// The world matrix.
        /// </value>
        Matrix WorldMatrix { get; }
        /// <summary>
        /// Gets the renderables.
        /// </summary>
        /// <value>
        /// The renderables.
        /// </value>
        IEnumerable<IRenderable> Renderables { get; }
        /// <summary>
        /// Gets the d2 d renderables.
        /// </summary>
        /// <value>
        /// The d2 d renderables.
        /// </value>
        IEnumerable<IRenderable2D> D2DRenderables { get; }
        /// <summary>
        /// Invalidates the render.
        /// </summary>
        void InvalidateRender();
    }
}