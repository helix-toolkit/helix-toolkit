namespace HelixToolkit.SharpDX.Wpf
{
    using System;
    using System.Windows;

    /// <summary>
    /// Base class for renderable elements.
    /// </summary>
    public abstract class Element3D : FrameworkElement
    {
        public virtual void Attach(IRenderHost host)
        {
        }

        public virtual void Update(TimeSpan timeSpan)
        {
        }

        public virtual void Detach()
        {
        }

        public virtual void Render(RenderContext context)
        {
        }
    }
}