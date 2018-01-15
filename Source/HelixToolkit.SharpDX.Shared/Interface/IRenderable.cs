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
        bool IsRenderable { get; }

        /// <summary>
        /// Optional for scene graph traverse
        /// </summary>
        IEnumerable<IRenderable> Items { get; }

        IRenderCore RenderCore { get; }
        /// <summary>
        /// Update render related parameters such as model matrix by scene graph and bounding boxes
        /// </summary>
        /// <param name="context"></param>
        void Update(IRenderContext context);
        /// <summary>
        /// Update things not related to rendering, such as OctreeManager etc. Called parallel with rendering process.
        /// </summary>
        void UpdateNotRender();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Render(IRenderContext context, DeviceContextProxy deviceContext);
    }

    public interface IViewport3DX
    {
        void Attach(IRenderHost host);
        void Detach();
        IRenderHost RenderHost { get; }
        bool IsShadowMappingEnabled { get; }
        IEffectsManager EffectsManager { set; get; }
        IRenderTechnique RenderTechnique { set; get; }
        CameraCore CameraCore { get; }

        //DeferredRenderer DeferredRenderer { get; set; }
        void UpdateFPS(TimeSpan timeStamp);

        Matrix WorldMatrix { get; }

        IEnumerable<IRenderable> Renderables { get; }

        IEnumerable<IRenderable> D2DRenderables { get; }

        void InvalidateRender();
    }
}