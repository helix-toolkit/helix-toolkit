// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Element3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Base class for renderable elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
            get { return renderHost != null; }
        }

        protected global::SharpDX.Direct3D11.Device Device
        {
            get { return renderHost.Device; }
        }

        /// <summary>
        /// Attaches the element to the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        public virtual void Attach(IRenderHost host)
        {
            renderTechnique = this.renderTechnique == null ? host.RenderTechnique : this.renderTechnique;
            effect = renderHost.EffectsManager.GetEffect(renderTechnique);
            renderHost = host;
            InvalidateRender();
        }

        /// <summary>
        /// Detaches the element from the host.
        /// </summary>
        public virtual void Detach()
        {
            renderTechnique = null;            
            effect = null;
            renderHost = null;           
        }

        /// <summary>
        /// Tries to invalidate the current render.
        /// </summary>
        public void InvalidateRender()
        {
            // ToDo: Add InvalidateRender() to IRenderHost?
            var rh = renderHost as DPFCanvas;
            if (rh != null)
            {
                rh.InvalidateRender();
            }
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
            get { return (bool)GetValue(IsRenderingProperty); }
            set { SetValue(IsRenderingProperty, value); }
        }

        /// <summary>
        /// Looks for the first visual ancestor of type <see cref="T"/>>.
        /// </summary>
        /// <typeparam name="T">The type of visual ancestor.</typeparam>
        /// <param name="obj">The respective <see cref="DependencyObject"/>.</param>
        /// <returns>
        /// The first visual ancestor of type <see cref="T"/> if exists, else <c>null</c>.
        /// </returns>
        public static T FindVisualAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
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
            }

            return null;
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="Element3D"/> has been updated.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            // Possible improvement: Only invalidate if the property metadata has the flag "AffectsRender".
            // => Need to change all relevant DP's metadata to FrameworkPropertyMetadata or to a new "Element3DPropertyMetadata".
            //var fmetadata = e.Property.GetMetadata(this) as FrameworkPropertyMetadata;
            //if (fmetadata != null && fmetadata.AffectsRender)
            {
                this.InvalidateRender();
            }
        }
    }
}
