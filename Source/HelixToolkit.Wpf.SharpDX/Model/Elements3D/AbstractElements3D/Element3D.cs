// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Element3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Base class for renderable elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using SharpDX;
namespace HelixToolkit.Wpf.SharpDX
{

    using System;
    using System.Diagnostics;
    using System.Windows;
    using Media = System.Windows.Media;
    using Core;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using System.Collections.Generic;


    /// <summary>
    /// Base class for renderable elements.
    /// </summary>    
    public abstract class Element3D : FrameworkContentElement, IDisposable, IRenderable, IGUID, ITransformable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public delegate IRenderTechnique SetRenderTechniqueFunc(IRenderHost host);
        /// <summary>
        /// A delegate function to change render technique. 
        /// <para>There are two ways to set render technique, one is use this <see cref="OnSetRenderTechnique"/> delegate.
        /// The other one is to override the <see cref="OnCreateRenderTechnique"/> function.</para>
        /// <para>If <see cref="OnSetRenderTechnique"/> is set, then <see cref="OnSetRenderTechnique"/> instead of <see cref="OnCreateRenderTechnique"/> function will be called.</para>
        /// </summary>
        public SetRenderTechniqueFunc OnSetRenderTechnique;

        #region Dependency Properties
        /// <summary>
        /// Indicates, if this element should be rendered,
        /// default is true
        /// </summary>
        public static readonly DependencyProperty IsRenderingProperty =
            DependencyProperty.Register("IsRendering", typeof(bool), typeof(Element3D), new AffectsRenderPropertyMetadata(true,
                (d, e) =>
                {
                    (d as Element3D).isRenderingInternal = (bool)e.NewValue;
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
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsHitTestVisibleProperty =
            DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(Element3D), new PropertyMetadata(true, (d, e) =>
            {
                (d as Element3D).isHitTestVisibleInternal = (bool)e.NewValue;
            }));
        /// <summary>
        /// 
        /// </summary>
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


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(Element3D), new AffectsRenderPropertyMetadata(Visibility.Visible, (d, e) =>
            {
                (d as Element3D).visibleInternal = (Visibility)e.NewValue == Visibility.Visible;
            }));

        /// <summary>
        /// 
        /// </summary>
        public Visibility Visibility
        {
            set
            {
                SetValue(VisibilityProperty, value);
            }
            get
            {
                return (Visibility)GetValue(VisibilityProperty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Transform3D), typeof(Element3D), new AffectsRenderPropertyMetadata(Transform3D.Identity, (d,e)=>
            {
                ((Element3D)d).modelMatrix = e.NewValue != null ? ((Transform3D)e.NewValue).Value.ToMatrix() : Matrix.Identity;
                ((Element3D)d).OnTransformChanged();
            }));
        /// <summary>
        /// 
        /// </summary>
        public Transform3D Transform
        {
            get { return (Transform3D)this.GetValue(TransformProperty); }
            set { this.SetValue(TransformProperty, value); }
        }
        #endregion

        protected Matrix totalModelMatrix = Matrix.Identity;

        protected Matrix modelMatrix = Matrix.Identity;

        private readonly Stack<Matrix> matrixStack = new Stack<Matrix>();

        private IRenderCore renderCore = null;
        public IRenderCore RenderCore
        {
            private set
            {
                if (renderCore != value)
                {
                    if (renderCore != null)
                    {
                        renderCore.OnInvalidateRenderer -= RenderCore_OnInvalidateRenderer;
                    }
                    renderCore = value;
                    if (renderCore != null)
                    {
                        renderCore.OnInvalidateRenderer += RenderCore_OnInvalidateRenderer;
                    }
                }
            }
            get
            {
                if(renderCore == null)
                {
                    RenderCore = OnCreateRenderCore();
                    AssignDefaultValuesToCore(RenderCore);
                }
                return renderCore;
            }
        }

        protected IRenderTechnique renderTechnique;

        protected IRenderHost renderHost;

        private bool isAttached = false;

        private readonly Guid guid = Guid.NewGuid();

        public Guid GUID { get { return guid; } }

        protected bool isRenderingInternal { private set; get; } = true;

        protected bool visibleInternal { private set; get; } = true;

        protected bool isHitTestVisibleInternal
        {
            private set; get;
        } = true;
        /// <summary>
        /// If this has been attached onto renderhost. 
        /// </summary>
        public bool IsAttached
        {
            private set;get;
        }

        public IRenderHost RenderHost
        {
            get { return renderHost; }
        }

        protected global::SharpDX.Direct3D11.Device Device
        {
            get { return renderHost.Device; }
        }

        public virtual IEnumerable<IRenderable> Items
        {
            get
            {
                return System.Linq.Enumerable.Empty<IRenderable>();
            }
        }

        private void RenderCore_OnInvalidateRenderer(object sender, bool e)
        {
            InvalidateRender();
        }

        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="OnSetRenderTechnique"/> is set, then <see cref="OnSetRenderTechnique"/> instead of <see cref="OnCreateRenderTechnique"/> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>Return RenderTechnique</returns>
        protected virtual IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return this.renderTechnique == null ? host.RenderTechnique : this.renderTechnique;           
        }

        protected virtual IRenderCore OnCreateRenderCore() { return new EmptyRenderCore(); }

        protected virtual void AssignDefaultValuesToCore(IRenderCore core) { }
        #region Handling Transforms

        public void PushMatrix(Matrix matrix)
        {
            matrixStack.Push(this.modelMatrix);
            this.modelMatrix = this.modelMatrix * matrix;
            this.totalModelMatrix = this.modelMatrix;
        }

        public void PopMatrix()
        {
            this.modelMatrix = matrixStack.Pop();
        }

        public Matrix ModelMatrix
        {
            get { return this.modelMatrix; }
        }

        public Matrix TotalModelMatrix
        {
            get { return this.totalModelMatrix; }
        }

        protected virtual void OnTransformChanged() { }
        #endregion
        /// <summary>
        /// <para>Attaches the element to the specified host. To overide Attach, please override <see cref="OnAttach(IRenderHost)"/> function.</para>
        /// <para>To set different render technique instead of using technique from host, override <see cref="OnCreateRenderTechnique"/></para>
        /// <para>Attach Flow: <see cref="OnCreateRenderTechnique(IRenderHost)"/> -> Set RenderHost -> Get Effect -> <see cref="OnAttach(IRenderHost)"/> -> <see cref="OnAttached"/> -> <see cref="InvalidateRender"/></para>
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
            this.renderTechnique = OnSetRenderTechnique != null ? OnSetRenderTechnique(host) : OnCreateRenderTechnique(host);
            if (renderTechnique != null)
            {
                renderTechnique = RenderHost.EffectsManager[renderTechnique.Name];
                //effect = renderHost.EffectsManager.GetEffect(renderTechnique);
                IsAttached = OnAttach(host);
            }
            InvalidateRender();
        }       

        /// <summary>
        /// To override Attach routine, please override this.
        /// </summary>
        /// <param name="host"></param>       
        /// <returns>Return true if attached</returns>
        protected virtual bool OnAttach(IRenderHost host)
        {
            if (host == null)
            { return false; }
            RenderCore?.Attach(renderTechnique);
            return RenderCore == null ? false : RenderCore.IsAttached;
        }
        /// <summary>
        /// Detaches the element from the host. Override <see cref="OnDetach"/>
        /// </summary>
        public void Detach()
        {
            IsAttached = false;
            RenderCore?.Detach();
            OnDetach();
        }
        /// <summary>
        /// Used to override Detach
        /// </summary>
        protected virtual void OnDetach()
        {
            renderTechnique = null;            
            //effect = null;
            renderHost = null;           
        }

        /// <summary>
        /// Tries to invalidate the current render.
        /// </summary>
        public void InvalidateRender()
        {
            var rh = renderHost;
            if (renderHost != null)
            {
                rh.InvalidateRender();
            }
        }

        /// <summary>
        /// Updates the element by the specified time span.
        /// </summary>
        /// <param name="timeSpan">The time since last update.</param>
        //public virtual void Update(TimeSpan timeSpan) { }

        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// <para>Default returns <see cref="IsAttached"/> &amp;&amp; <see cref="IsRendering"/> &amp;&amp; <see cref="Visibility"/> == <see cref="Visibility.Visible"/></para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool CanRender(IRenderContext context)
        {
            return isRenderingInternal && visibleInternal;
        }
        /// <summary>
        /// <para>Renders the element in the specified context. To override Render, please override <see cref="OnRender"/></para>
        /// <para>Uses <see cref="CanRender"/>  to call OnRender or not. </para>
        /// </summary>
        /// <param name="context">The context.</param>
        public void Render(IRenderContext context)
        {
            if (CanRender(context))
            {
                RenderCore.ModelMatrix = this.ModelMatrix;
                OnRender(context);
            }
        }

        protected virtual void OnRender(IRenderContext context)
        {
            RenderCore?.Render(context);
        }

        /// <summary>
        /// Disposes the Element3D. Frees all DX resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.Detach();                        
        }

        /// <summary>
        /// Looks for the first visual ancestor of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of visual ancestor.</typeparam>
        /// <param name="obj">The respective <see cref="DependencyObject"/>.</param>
        /// <returns>
        /// The first visual ancestor of type <typeparamref name="T"/> if exists, else <c>null</c>.
        /// </returns>
        public static T FindVisualAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                var parent = Media.VisualTreeHelper.GetParent(obj);
                while (parent != null)
                {
                    var typed = parent as T;
                    if (typed != null)
                    {
                        return typed;
                    }

                    parent = Media.VisualTreeHelper.GetParent(parent);
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
