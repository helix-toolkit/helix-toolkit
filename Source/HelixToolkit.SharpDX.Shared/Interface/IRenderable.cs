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
    using System;
    using System.Windows.Controls;

    using global::SharpDX;
    using System.Collections.Generic;

    public interface IRenderable
    {
        void Attach(IRenderHost host);
        void Detach();
        //void Update(TimeSpan timeSpan);
        void Render(IRenderContext context);
        bool IsAttached { get; }
       // IRenderCore RenderCore { get; }
    }

    public interface IRenderer
    {
        void Attach(IRenderHost host);
        void Detach();
        //void Update(TimeSpan timeSpan);
        void Render(IRenderContext context);

        void RenderD2D(IRenderContext context);
       
        bool IsShadowMappingEnabled { get; }
        IRenderTechnique RenderTechnique { get; }
        Camera Camera { get; }
        Color4 BackgroundColor { get; }

        //DeferredRenderer DeferredRenderer { get; set; }

        IEnumerable<IRenderable> Renderables { get; }

        IEnumerable<IRenderable> D2DRenderables { get; }
    }
}