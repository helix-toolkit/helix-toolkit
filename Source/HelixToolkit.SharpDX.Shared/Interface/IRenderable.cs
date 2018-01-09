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
    /// <summary>
    /// 
    /// </summary>
    public interface IRenderable 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        void Attach(IRenderHost host);
        /// <summary>
        /// 
        /// </summary>
        void Detach();
        //void Update(TimeSpan timeSpan);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Render(IRenderContext context);
        /// <summary>
        /// 
        /// </summary>
        bool IsAttached { get; }
    }

    public interface IRenderer
    {
        void Attach(IRenderHost host);
        void Detach();
        //void Update(TimeSpan timeSpan);
        void Render(IRenderContext context);

        void RenderD2D(IRenderContext context);
       
        bool IsShadowMappingEnabled { get; }
        IEffectsManager EffectsManager { set; get; }
        IRenderTechnique RenderTechnique { set; get; }
        CameraCore CameraCore { get; }
        Color4 BackgroundColor { get; }

        //DeferredRenderer DeferredRenderer { get; set; }

        IEnumerable<IRenderable> Renderables { get; }

        IEnumerable<IRenderable> D2DRenderables { get; }

        void InvalidateRender();
    }
}