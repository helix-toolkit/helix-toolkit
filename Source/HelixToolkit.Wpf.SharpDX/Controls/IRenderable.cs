// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRenderable.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
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
        void Render(RenderContext context);
        bool IsAttached { get; }
    }

    public interface IRenderer
    {
        void Attach(IRenderHost host);
        void Detach();
        //void Update(TimeSpan timeSpan);
        void Render(RenderContext context);
       
        bool IsShadowMappingEnabled { get; }
        RenderTechnique RenderTechnique { get; }
        Camera Camera { get; }
        Color4 BackgroundColor { get; }

        DeferredRenderer DeferredRenderer { get; set; }

        IEnumerable<IRenderable> Renderables { get; }
    }
}