namespace HelixToolkit.SharpDX.Wpf
{
    using System;
    using System.Windows;

    /// <summary>
    /// Base class for renderable elements.
    /// </summary>
    public abstract class Element3D : FrameworkElement, IRenderable
    {
        protected virtual void Attach(IRenderHost host)
        {
        }

        protected virtual void Update(TimeSpan timeSpan)
        {
        }

        protected virtual void Detach()
        {
        }

        protected virtual void Render()
        {
        }

        void IRenderable.Update(TimeSpan timeSpan)
        {
            this.Update(timeSpan);
        }

        void IRenderable.Detach()
        {
            this.Detach();
        }

        void IRenderable.Render()
        {
            this.Render();
        }

        void IRenderable.Attach(IRenderHost host)
        {
            this.Attach(host);
        }
    }
}