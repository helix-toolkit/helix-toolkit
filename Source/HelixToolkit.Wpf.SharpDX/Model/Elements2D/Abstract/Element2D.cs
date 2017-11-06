using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    public abstract class Element2D : FrameworkContentElement, IDisposable, IRenderable, IGUID, IHitable2D
    {
        #region Dependency Properties
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

        public new static readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(Element2D), new AffectsRenderPropertyMetadata(false, (d, e) =>
            {
                var model = d as Element2D;
                if(model.renderCore == null) { return; }
                model.renderCore.IsMouseOver = (bool)e.NewValue;
            }));

        public new bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            private set { SetValue(IsMouseOverProperty, value); }
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
        #endregion
        private readonly Guid guid = Guid.NewGuid();

        public Guid GUID { get { return guid; } }

        protected bool isRenderingInternal { private set; get; } = true;

        protected bool isHitTestVisibleInternal { private set; get; } = true;

        public bool IsAttached { private set; get; }

        public RectangleF Bound
        {
            get { return new RectangleF(layoutTranslate.X, layoutTranslate.Y, (float)Width, (float)Height); }
        }

        private readonly Stack<Vector2> layoutTranslateStack = new Stack<Vector2>();
        private Vector2 layoutTranslate = Vector2.Zero;
        /// <summary>
        /// Layout Translation matrix. Layout only allows translation
        /// </summary>
        public Vector2 LayoutTranslate
        {
            set
            {
                if (layoutTranslate != value)
                {
                    layoutTranslate = value;
                    layoutTranslationChanged = true;
                }
            }
            get
            {
                return this.layoutTranslate;
            }
        }

        private bool layoutTranslationChanged { set; get; } = true;

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

        #region Events
        public delegate void Mouse2DRoutedEventHandler(object sender, Mouse2DEventArgs e);
        public static readonly RoutedEvent MouseDown2DEvent =
            EventManager.RegisterRoutedEvent("MouseDown2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Element2D));

        public static readonly RoutedEvent MouseUp2DEvent =
            EventManager.RegisterRoutedEvent("MouseUp2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Element2D));

        public static readonly RoutedEvent MouseMove2DEvent =
            EventManager.RegisterRoutedEvent("MouseMove2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Element2D));

        public static readonly RoutedEvent MouseEnter2DEvent =
            EventManager.RegisterRoutedEvent("MouseEnter2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Element2D));

        public static readonly RoutedEvent MouseLeave2DEvent =
            EventManager.RegisterRoutedEvent("MouseLeave2D", RoutingStrategy.Bubble, typeof(Mouse2DRoutedEventHandler), typeof(Element2D));

        public event Mouse2DRoutedEventHandler MouseDown2D
        {
            add { AddHandler(MouseDown2DEvent, value); }
            remove { RemoveHandler(MouseDown2DEvent, value); }
        }

        public event Mouse2DRoutedEventHandler MouseUp2D
        {
            add { AddHandler(MouseUp2DEvent, value); }
            remove { RemoveHandler(MouseUp2DEvent, value); }
        }

        public event Mouse2DRoutedEventHandler MouseMove2D
        {
            add { AddHandler(MouseMove2DEvent, value); }
            remove { RemoveHandler(MouseMove2DEvent, value); }
        }

        public event Mouse2DRoutedEventHandler MouseEnter2D
        {
            add { AddHandler(MouseEnter2DEvent, value); }
            remove { RemoveHandler(MouseEnter2DEvent, value); }
        }

        public event Mouse2DRoutedEventHandler MouseLeave2D
        {
            add { AddHandler(MouseLeave2DEvent, value); }
            remove { RemoveHandler(MouseLeave2DEvent, value); }
        }
        #endregion

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
                this.MouseEnter2D += Element2D_MouseEnter2D;
                this.MouseLeave2D += Element2D_MouseLeave2D;
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

        protected virtual void Element2D_MouseLeave2D(object sender, RoutedEventArgs e)
        {
            if (renderCore == null) { return; }
            IsMouseOver = false;
#if DEBUG
            //Debug.WriteLine("Element2D_MouseLeave2D");
#endif
        }

        protected virtual void Element2D_MouseEnter2D(object sender, RoutedEventArgs e)
        {
            if (renderCore == null) { return; }
            IsMouseOver = true;
#if DEBUG
            //Debug.WriteLine("Element2D_MouseEnter2D");
#endif
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
            this.MouseEnter2D -= Element2D_MouseEnter2D;
            this.MouseLeave2D -= Element2D_MouseLeave2D;
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

        public void PushLayoutTranslate(Vector2 v)
        {
            layoutTranslateStack.Push(layoutTranslate);
            layoutTranslate = layoutTranslate + v;
        }

        public void PopLayoutTranslate()
        {
            layoutTranslate = layoutTranslateStack.Pop();
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
                if (layoutTranslationChanged)
                {
                    OnLayoutTranslationChanged(layoutTranslate);
                    layoutTranslationChanged = false;
                }
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
            if (renderCore != null)
            {
                renderCore.Rect = this.Bound;
            }
        }

        public bool HitTest(Vector2 mousePoint, out HitTest2DResult hitResult)
        {      
            if (!IsHitTestVisible || !IsAttached)
            {
                hitResult = null;
                return false;
            }
            return OnHitTest(ref mousePoint, out hitResult);
        }

        protected abstract bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult);

        
        protected virtual void OnLayoutTranslationChanged(Vector2 translation)
        {

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

    public class Mouse2DEventArgs : RoutedEventArgs
    {
        public HitTest2DResult HitTest2DResult { get; private set; }
        public Viewport3DX Viewport { get; private set; }
        public System.Windows.Point Position { get; private set; }

        public InputEventArgs InputArgs { get; private set; }
        public Mouse2DEventArgs(RoutedEvent routedEvent, object source, HitTest2DResult hitTestResult, System.Windows.Point position, Viewport3DX viewport = null, InputEventArgs inputArgs = null)
            : base(routedEvent, source)
        {
            this.HitTest2DResult = hitTestResult;
            this.Position = position;
            this.Viewport = viewport;
            InputArgs = inputArgs;
        }
        public Mouse2DEventArgs(RoutedEvent routedEvent, object source, Viewport3DX viewport = null)
            : base(routedEvent, source)
        {
            this.Viewport = viewport;
        }
    }
}
