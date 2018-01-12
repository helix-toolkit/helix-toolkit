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
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Update(IRenderContext context);
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
       
        bool IsShadowMappingEnabled { get; }
        IEffectsManager EffectsManager { set; get; }
        IRenderTechnique RenderTechnique { set; get; }
        CameraCore CameraCore { get; }
        Color4 BackgroundColor { get; }

        //DeferredRenderer DeferredRenderer { get; set; }
        void UpdateFPS(TimeSpan timeStamp);

        Matrix WorldMatrix { get; }

        IEnumerable<IRenderable> Renderables { get; }

        IEnumerable<IRenderable> D2DRenderables { get; }

        void InvalidateRender();
    }
}