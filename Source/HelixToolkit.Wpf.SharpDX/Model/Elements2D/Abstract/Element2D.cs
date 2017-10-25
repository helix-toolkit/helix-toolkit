using HelixToolkit.SharpDX.Core2D;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX
{
    public abstract class Element2D : FrameworkContentElement, IDisposable, IRenderable, IGUID
    {
        /// <summary>
        /// Indicates, if this element should be rendered,
        /// default is true
        /// </summary>
        public static readonly DependencyProperty IsRenderingProperty =
            DependencyProperty.Register("IsRendering", typeof(bool), typeof(Element2D), new AffectsRenderPropertyMetadata(true,
                (d, e) =>
                {
                    (d as Element2D).isRenderingInternal = (bool)e.NewValue;
                }));

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

        public static readonly DependencyProperty IsHitTestVisibleProperty =
            DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(Element2D), new PropertyMetadata(true, (d, e) =>
            {
                (d as Element2D).isHitTestVisibleInternal = (bool)e.NewValue;
            }));

        public bool IsHitTestVisible
        {
            set
            {
                SetValue(IsHitTestVisibleProperty, value);
            }
            get
            {
                return (bool)GetValue(IsHitTestVisibleProperty);
            }
        }

        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof(double), typeof(Element2D),
            new AffectsRenderPropertyMetadata(0.0));

        public double Left
        {
            set
            {
                SetValue(LeftProperty, value);
            }
            get
            {
                return (double)GetValue(LeftProperty);
            }
        }
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(Element2D),
            new AffectsRenderPropertyMetadata(0.0));

        public double Top
        {
            set
            {
                SetValue(TopProperty, value);
            }
            get
            {
                return (double)GetValue(TopProperty);
            }
        }

        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(Element2D),
            new AffectsRenderPropertyMetadata(100.0));

        public double Width
        {
            set
            {
                SetValue(WidthProperty, value);
            }
            get
            {
                return (double)GetValue(WidthProperty);
            }
        }

        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(Element2D),
            new AffectsRenderPropertyMetadata(100.0));

        public double Height
        {
            set
            {
                SetValue(HeightProperty, value);
            }
            get
            {
                return (double)GetValue(HeightProperty);
            }
        }

        private readonly Guid guid = Guid.NewGuid();

        public Guid GUID { get { return guid; } }

        protected bool isRenderingInternal { private set; get; } = true;

        protected bool isHitTestVisibleInternal { private set; get; } = true;

        public bool IsAttached { private set; get; }

        public RectangleF Bound
        {
            get { return new RectangleF((float)Left, (float)Top, (float)Width, (float)Height); }
        }

        protected IRenderHost renderHost;
        public IRenderHost RenderHost
        {
            get { return renderHost; }
        }

        private global::SharpDX.Direct2D1.RenderTarget renderTarget = null;
        protected global::SharpDX.Direct2D1.RenderTarget RenderTarget
        {
            set
            {
                if (renderTarget != value)
                {
                    renderTarget = value;
                    OnRenderTargetChanged(value);
                }
            }
            get
            {
                return renderTarget;
            }
        }

        protected IRenderable2D renderCore { private set; get; }

        protected abstract IRenderable2D CreateRenderCore(IRenderHost host);

        /// <summary>
        /// <para>Attaches the element to the specified host. To overide Attach, please override <see cref="OnAttach(IRenderHost)"/> function.</para>
        /// <para>To set different render technique instead of using technique from host, override <see cref="SetRenderTechnique"/></para>
        /// <para>Attach Flow: <see cref="SetRenderTechnique(IRenderHost)"/> -> Set RenderHost -> Get Effect -> <see cref="OnAttach(IRenderHost)"/> -> <see cref="OnAttached"/> -> <see cref="InvalidateRender"/></para>
        /// </summary>
        /// <param name="host">The host.</param>
        public void Attach(IRenderHost host)
        {
            if (IsAttached || host == null)
            {
                return;
            }
            renderHost = host;
            if (host.EffectsManager == null)
            {
                throw new ArgumentException("EffectManger does not exist. Please make sure the proper EffectManager has been bind from view model.");
            }
            IsAttached = OnAttach(host);
            if (IsAttached)
            {               
                OnAttached();
            }
            InvalidateRender();
        }

        /// <summary>
        /// Called after <see cref="OnAttach(IRenderHost)"/>
        /// </summary>
        protected virtual void OnAttached()
        {

        }
        /// <summary>
        /// To override Attach routine, please override this.
        /// </summary>
        /// <param name="host"></param>       
        /// <returns>Return true if attached</returns>
        protected virtual bool OnAttach(IRenderHost host)
        {
            renderCore = CreateRenderCore(host);
            return true;
        }

        protected abstract void OnRenderTargetChanged(global::SharpDX.Direct2D1.RenderTarget newTarget);

        public virtual void Dispose()
        {
            Detach();
        }

        public void Detach()
        {
            OnDetach();
        }

        /// <summary>
        /// Used to override Detach
        /// </summary>
        protected virtual void OnDetach()
        {
            renderCore.Dispose();
            renderCore = null;
            renderHost = null;
        }

        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// <para>Default returns <see cref="IsAttached"/> &amp;&amp; <see cref="IsRendering"/> &amp;&amp; <see cref="Visibility"/> == <see cref="Visibility.Visible"/></para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool CanRender(RenderContext context)
        {
            return IsAttached && isRenderingInternal && RenderHost.D2DControls.D2DTarget != null && renderCore != null;
        }
        /// <summary>
        /// <para>Renders the element in the specified context. To override Render, please override <see cref="OnRender"/></para>
        /// <para>Uses <see cref="CanRender"/>  to call OnRender or not. </para>
        /// </summary>
        /// <param name="context">The context.</param>
        public void Render(RenderContext context)
        {
            if (CanRender(context))
            {
                PreRender(context);
                OnRender(context);
            }
        }

        /// <summary>
        /// Used to overriding <see cref="Render"/> routine.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnRender(RenderContext context)
        {
            renderCore.Render(context, RenderTarget);
        }

        protected virtual void PreRender(RenderContext context)
        {
            RenderTarget = RenderHost.D2DControls.D2DTarget;
            renderCore.Rect = this.Bound;
        }

        /// <summary>
        /// Tries to invalidate the current render.
        /// </summary>
        public void InvalidateRender()
        {
            renderHost?.InvalidateRender();
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="Element2D"/> has been updated.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CheckAffectsRender(e))
            {
                this.InvalidateRender();
            }
            base.OnPropertyChanged(e);
        }
        /// <summary>
        /// Check if dependency property changed event affects render
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual bool CheckAffectsRender(DependencyPropertyChangedEventArgs e)
        {
            // Possible improvement: Only invalidate if the property metadata has the flag "AffectsRender".
            // => Need to change all relevant DP's metadata to FrameworkPropertyMetadata or to a new "AffectsRenderPropertyMetadata".
            PropertyMetadata fmetadata = null;
            return ((fmetadata = e.Property.GetMetadata(this)) != null
                && (fmetadata is IAffectsRender
                || (fmetadata is FrameworkPropertyMetadata && (fmetadata as FrameworkPropertyMetadata).AffectsRender)
                ));
        }              
    }
}
