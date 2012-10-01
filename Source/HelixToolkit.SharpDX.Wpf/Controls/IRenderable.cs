namespace HelixToolkit.SharpDX.Wpf
{
    using System;

    public interface IRenderable
    {
        void Attach(IRenderHost host);
        void Detach();
        void Update(TimeSpan timeSpan);
        void Render();
    }
}