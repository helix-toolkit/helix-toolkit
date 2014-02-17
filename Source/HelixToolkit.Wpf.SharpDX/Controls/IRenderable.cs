namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Windows.Controls;

    using global::SharpDX;

    public interface IRenderable
    {
        void Attach(IRenderHost host);
        void Detach();
        void Update(TimeSpan timeSpan);
        void Render(RenderContext context);
        bool IsAttached { get; }
    }

    public interface IRenderer
    {
        void Attach(IRenderHost host);
        void Detach();
        void Update(TimeSpan timeSpan);
        void Render(RenderContext context);
       
        bool IsShadowMappingEnabled { get; }
        RenderTechnique RenderTechnique { get; }
        ItemCollection Items { get; }
        Camera Camera { get; }
        Color4 BackgroundColor { get; }

        DeferredRenderer DeferredRenderer { get; set; }
    }
}