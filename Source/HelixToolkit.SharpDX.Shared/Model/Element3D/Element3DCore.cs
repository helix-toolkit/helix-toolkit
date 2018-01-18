/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


#if NETFX_CORE
namespace HelixToolkit.UWP.Core
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX.Core
#endif
{
    using Render;
    /// <summary>
    /// 
    /// </summary>
#if NETFX_CORE
    public abstract class Element3DCore : IDisposable, IRenderable, IGUID, ITransform, INotifyPropertyChanged
#else
    public abstract class Element3DCore : FrameworkContentElement, IDisposable, IRenderable, INotifyPropertyChanged
#endif
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid GUID { get; } = Guid.NewGuid();

        private Matrix totalModelMatrix = Matrix.Identity;
        /// <summary>
        /// 
        /// </summary>
        public Matrix TotalModelMatrix
        {
            private set
            {
                if(Set(ref totalModelMatrix, value))
                {
                    TransformChanged(ref value);
                    OnTransformChanged?.Invoke(this, value);
                    RenderCore.ModelMatrix = value;
                }
            }
            get
            {
                return totalModelMatrix;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [need matrix update].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [need matrix update]; otherwise, <c>false</c>.
        /// </value>
        protected bool needMatrixUpdate { private set; get; } = true;

        private Matrix modelMatrix = Matrix.Identity;
        /// <summary>
        /// Gets or sets the model matrix.
        /// </summary>
        /// <value>
        /// The model matrix.
        /// </value>
        public Matrix ModelMatrix
        {
            set
            {
                if(Set(ref modelMatrix, value))
                {
                    needMatrixUpdate = true;
                    InvalidateRender();
                }
            }
            get { return modelMatrix; }
        } 


        private Matrix parentMatrix = Matrix.Identity;
        /// <summary>
        /// Gets or sets the parent matrix.
        /// </summary>
        /// <value>
        /// The parent matrix.
        /// </value>
        public Matrix ParentMatrix
        {
            set
            {
                if(Set(ref parentMatrix, value))
                {
                    needMatrixUpdate = true;
                }
            }
            get
            {
                return parentMatrix;
            }
        }

        private bool visible = true;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Element3DCore"/> is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible
        {
            set
            {
                if(Set(ref visible, value))
                { InvalidateRender(); }
            }
            get { return visible; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is renderable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is renderable; otherwise, <c>false</c>.
        /// </value>
        public bool IsRenderable { private set; get; } = true;
        /// <summary>
        /// If this has been attached onto renderhost. 
        /// </summary>
        public bool IsAttached
        {
            private set;get;
        }

        private IRenderHost renderHost;
        /// <summary>
        /// 
        /// </summary>
        public IRenderHost RenderHost
        {
            get { return renderHost; }
        }
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public virtual IEnumerable<IRenderable> Items
        {
            get
            {
                return System.Linq.Enumerable.Empty<IRenderable>();
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is hit test visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is hit test visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsHitTestVisible { set; get; } = true;
        #region Handling Transforms        
        /// <summary>
        /// Transforms the changed.
        /// </summary>
        /// <param name="totalTransform">The total transform.</param>
        protected virtual void TransformChanged(ref Matrix totalTransform)
        {
            foreach (var item in Items)
            {
                if (item is ITransform)
                {
                    item.ParentMatrix = totalTransform;
                }
            }
        }
        /// <summary>
        /// Occurs when [on transform changed].
        /// </summary>
        public event EventHandler<Matrix> OnTransformChanged;
        #endregion
        #region RenderCore
        private IRenderCore renderCore = null;
        /// <summary>
        /// Gets or sets the render core.
        /// </summary>
        /// <value>
        /// The render core.
        /// </value>
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
                if (renderCore == null)
                {
                    RenderCore = OnCreateRenderCore();
                    AssignDefaultValuesToCore(RenderCore);
                }
                return renderCore;
            }
        }
        /// <summary>
        /// Gets or sets the render technique.
        /// </summary>
        /// <value>
        /// The render technique.
        /// </value>
        protected IRenderTechnique renderTechnique { private set; get; }
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
        /// <summary>
        /// Override this function to set render technique during Attach Host.
        /// <para>If <see cref="OnSetRenderTechnique"/> is set, then <see cref="OnSetRenderTechnique"/> instead of <see cref="OnCreateRenderTechnique"/> function will be called.</para>
        /// </summary>
        /// <param name="host"></param>
        /// <returns>Return RenderTechnique</returns>
        protected virtual IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.RenderTechnique;
        }
        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected virtual IRenderCore OnCreateRenderCore() { return new EmptyRenderCore(); }
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected virtual void AssignDefaultValuesToCore(IRenderCore core) { }

        private void RenderCore_OnInvalidateRenderer(object sender, bool e)
        {
            InvalidateRender();
        }
        #endregion
        /// <summary>
        /// <para>Attaches the element to the specified host. To overide Attach, please override <see cref="OnAttach(IRenderHost)"/> function.</para>
        /// <para>To set different render technique instead of using technique from host, override <see cref="OnCreateRenderTechnique"/></para>
        /// <para>Attach Flow: <see cref="OnCreateRenderTechnique(IRenderHost)"/> -> Set RenderHost -> Get Effect -> <see cref="OnAttach(IRenderHost)"/> -> <see cref="OnAttached"/> -> <see cref="InvalidateRender"/></para>
        /// </summary>
        /// <param name="host">The host.</param>
        public void Attach(IRenderHost host)
        {
            if (IsAttached || host == null || host.EffectsManager == null)
            {
                return;
            }
            renderHost = host;
            this.renderTechnique = OnSetRenderTechnique != null ? OnSetRenderTechnique(host) : OnCreateRenderTechnique(host);
            if (renderTechnique != null)
            {
                renderTechnique = RenderHost.EffectsManager[renderTechnique.Name];
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
            RenderCore.Attach(renderTechnique);
            return RenderCore == null ? false : RenderCore.IsAttached;
        }
        /// <summary>
        /// Detaches the element from the host. Override <see cref="OnDetach"/>
        /// </summary>
        public void Detach()
        {
            IsAttached = false;
            RenderCore.Detach();
            OnDetach();
        }

        /// <summary>
        /// Used to override Detach
        /// </summary>
        protected virtual void OnDetach()
        {
            renderHost = null;
        }

        /// <summary>
        /// Tries to invalidate the current render.
        /// </summary>
        public void InvalidateRender()
        {
            renderHost?.InvalidateRender();
        }

        /// <summary>
        /// Updates the element total transforms, determine renderability, etc. by the specified time span.
        /// </summary>
        /// <param name="context">The time since last update.</param>
        public virtual void Update(IRenderContext context)
        {
            if (needMatrixUpdate)
            {
                TotalModelMatrix = modelMatrix * parentMatrix;
                needMatrixUpdate = false;
            }
            IsRenderable = CanRender(context);
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void UpdateNotRender() { }

        #region Rendering

        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// <para>Default returns <see cref="IsAttached"/> &amp;&amp; <see cref="IsRendering"/> &amp;&amp; <see cref="Visibility"/> == <see cref="Visibility.Visible"/></para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool CanRender(IRenderContext context)
        {
            return Visible && IsAttached;
        }
        /// <summary>
        /// <para>Renders the element in the specified context. To override Render, please override <see cref="OnRender"/></para>
        /// <para>Uses <see cref="CanRender"/>  to call OnRender or not. </para>
        /// </summary>
        /// <param name="context">The context.</param>
        public void Render(IRenderContext context, DeviceContextProxy deviceContext)
        {
            Update(context);
            if (IsRenderable)
            {
                OnRender(context, deviceContext);
            }
        }

        protected virtual void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            RenderCore.Render(context, deviceContext);
        }
        #endregion

        #region Hit Test
        public virtual bool HitTest(IRenderContext context, Ray ray, ref List<HitTestResult> hits)
        {
            if (CanHitTest(context))
            {
                return OnHitTest(context, totalModelMatrix, ref ray, ref hits);
            }
            else
            {
                return false;
            }
        }

        protected virtual bool CanHitTest(IRenderContext context)
        {
            return IsHitTestVisible && IsRenderable;
        }

        protected abstract bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits);
        #endregion

        #region IBoundable        
        /// <summary>
        /// The maximum bound
        /// </summary>
        public static readonly BoundingBox MaxBound = new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MaxValue));
        /// <summary>
        /// The maximum bound sphere
        /// </summary>
        public static readonly global::SharpDX.BoundingSphere MaxBoundSphere = new global::SharpDX.BoundingSphere(Vector3.Zero, float.MaxValue);
        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public virtual BoundingBox Bounds
        {
            get { return MaxBound; }
        }
        /// <summary>
        /// Gets the bounds with transform.
        /// </summary>
        /// <value>
        /// The bounds with transform.
        /// </value>
        public virtual BoundingBox BoundsWithTransform
        {
            get { return MaxBound; }
        }
        /// <summary>
        /// Gets the bounds sphere.
        /// </summary>
        /// <value>
        /// The bounds sphere.
        /// </value>
        public virtual global::SharpDX.BoundingSphere BoundsSphere
        {
            get { return MaxBoundSphere; }
        }
        /// <summary>
        /// Gets the bounds sphere with transform.
        /// </summary>
        /// <value>
        /// The bounds sphere with transform.
        /// </value>
        public virtual global::SharpDX.BoundingSphere BoundsSphereWithTransform
        {
            get { return MaxBoundSphere; }
        }
        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Occurs when [on bound changed].
        /// </summary>
        public event EventHandler<BoundChangeArgs<BoundingBox>> OnBoundChanged;
        /// <summary>
        /// Occurs when [on transform bound changed].
        /// </summary>
        public event EventHandler<BoundChangeArgs<BoundingBox>> OnTransformBoundChanged;
        /// <summary>
        /// Occurs when [on bound sphere changed].
        /// </summary>
        public event EventHandler<BoundChangeArgs<global::SharpDX.BoundingSphere>> OnBoundSphereChanged;
        /// <summary>
        /// Occurs when [on transform bound sphere changed].
        /// </summary>
        public event EventHandler<BoundChangeArgs<global::SharpDX.BoundingSphere>> OnTransformBoundSphereChanged;
        /// <summary>
        /// Raises the on transform bound changed.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected void RaiseOnTransformBoundChanged(BoundChangeArgs<BoundingBox> args)
        {
            OnTransformBoundChanged?.Invoke(this, args);
        }
        /// <summary>
        /// Raises the on bound changed.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected void RaiseOnBoundChanged(BoundChangeArgs<BoundingBox> args)
        {
            OnBoundChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the on transform bound sphere changed.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected void RaiseOnTransformBoundSphereChanged(BoundChangeArgs<global::SharpDX.BoundingSphere> args)
        {
            OnTransformBoundSphereChanged?.Invoke(this, args);
        }
        /// <summary>
        /// Raises the on bound sphere changed.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected void RaiseOnBoundSphereChanged(BoundChangeArgs<global::SharpDX.BoundingSphere> args)
        {
            OnBoundSphereChanged?.Invoke(this, args);
        }
        #endregion


        #region INotifyPropertyChanged
        private bool disablePropertyChangedEvent = false;
        /// <summary>
        /// Gets or sets a value indicating whether [disable property changed event].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [disable property changed event]; otherwise, <c>false</c>.
        /// </value>
        public bool DisablePropertyChangedEvent
        {
            set
            {
                if (disablePropertyChangedEvent == value)
                {
                    return;
                }
                disablePropertyChangedEvent = value;
                RaisePropertyChanged();
            }
            get
            {
                return disablePropertyChangedEvent;
            }
        }
        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!DisablePropertyChangedEvent)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Sets the specified backing field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField">The backing field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// Sets the specified backing field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField">The backing field.</param>
        /// <param name="value">The value.</param>
        /// <param name="raisePropertyChanged">if set to <c>true</c> [raise property changed].</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T value, bool raisePropertyChanged, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            if (raisePropertyChanged)
            { this.RaisePropertyChanged(propertyName); }
            return true;
        }
#endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Element3DCore() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
