namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Base class for renderable elements.
    /// </summary>    
    public abstract class Element3D : FrameworkElement, IDisposable, IRenderable
    {
        protected global::SharpDX.Direct3D11.Effect effect;

        protected RenderTechnique renderTechnique;

        protected IRenderHost renderHost;

        public bool IsAttached
        {
            get { return this.renderHost != null; }
        }

        protected global::SharpDX.Direct3D11.Device Device
        {
            get { return this.renderHost.Device; }
        }

        /// <summary>
        /// Attaches the element to the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        public virtual void Attach(IRenderHost host)
        {
            this.renderTechnique = this.renderTechnique == null ? host.RenderTechnique : this.renderTechnique;
            this.effect = EffectsManager.Instance.GetEffect(renderTechnique);
            this.renderHost = host;            
        }

        /// <summary>
        /// Detaches the element from the host.
        /// </summary>
        public virtual void Detach()
        {
            this.renderTechnique = null;            
            this.effect = null;
            this.renderHost = null;           
        }

        /// <summary>
        /// Updates the element by the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time since last update.</param>
        public virtual void Update(TimeSpan timeSpan) { }

        /// <summary>
        /// Renders the element in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Render(RenderContext context)
        {
        }

        /// <summary>
        /// Disposes the Element3D. Frees all DX resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.Detach();                        
        }

        /// <summary>
        /// Indicates, if this element should be rendered,
        /// default is true
        /// </summary>
        public static readonly DependencyProperty IsRenderingProperty =
            DependencyProperty.Register("IsRendering", typeof(bool), typeof(Element3D), new UIPropertyMetadata(true));

        /// <summary>
        /// Indicates, if this element should be rendered.
        /// Use this also to make the model visible/unvisible
        /// default is true
        /// </summary>
        public bool IsRendering
        {
            get { return (bool)this.GetValue(IsRenderingProperty); }
            set { this.SetValue(IsRenderingProperty, value); }
        }

        public static T FindVisualAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                var typed = parent as T;
                if (typed != null)
                {
                    return typed;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
}
