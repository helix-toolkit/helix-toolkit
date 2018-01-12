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

        protected bool needMatrixUpdate { private set; get; } = true;

        private Matrix modelMatrix = Matrix.Identity;
        /// <summary>
        /// 
        /// </summary>
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
        public bool Visible
        {
            set
            {
                if(Set(ref visible, value))
                { InvalidateRender(); }
            }
            get { return visible; }
        }

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

        public virtual IEnumerable<IRenderable> Items
        {
            get
            {
                return System.Linq.Enumerable.Empty<IRenderable>();
            }
        }

        public bool IsHitTestVisible { set; get; } = true;
        #region Handling Transforms
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

        public event EventHandler<Matrix> OnTransformChanged;
        #endregion
        #region RenderCore
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
                if (renderCore == null)
                {
                    RenderCore = OnCreateRenderCore();
                    AssignDefaultValuesToCore(RenderCore);
                }
                return renderCore;
            }
        }
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
            return renderTechnique == null ? host.RenderTechnique : this.renderTechnique;
        }

        protected virtual IRenderCore OnCreateRenderCore() { return new EmptyRenderCore(); }

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
        /// <param name="timeSpan">The time since last update.</param>
        public virtual void Update(IRenderContext context)
        {
            if (needMatrixUpdate)
            {
                TotalModelMatrix = modelMatrix * parentMatrix;
                needMatrixUpdate = false;
            }
            IsRenderable = CanRender(context);
        }

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
        public static readonly BoundingBox MaxBound = new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MaxValue));
        public static readonly global::SharpDX.BoundingSphere MaxBoundSphere = new global::SharpDX.BoundingSphere(Vector3.Zero, float.MaxValue);

        public virtual BoundingBox Bounds
        {
            get { return MaxBound; }
        }

        public virtual BoundingBox BoundsWithTransform
        {
            get { return MaxBound; }
        }

        public virtual global::SharpDX.BoundingSphere BoundsSphere
        {
            get { return MaxBoundSphere; }
        }

        public virtual global::SharpDX.BoundingSphere BoundsSphereWithTransform
        {
            get { return MaxBoundSphere; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<BoundChangeArgs<BoundingBox>> OnBoundChanged;
        public event EventHandler<BoundChangeArgs<BoundingBox>> OnTransformBoundChanged;
        public event EventHandler<BoundChangeArgs<global::SharpDX.BoundingSphere>> OnBoundSphereChanged;
        public event EventHandler<BoundChangeArgs<global::SharpDX.BoundingSphere>> OnTransformBoundSphereChanged;

        protected void RaiseOnTransformBoundChanged(BoundChangeArgs<BoundingBox> args)
        {
            OnTransformBoundChanged?.Invoke(this, args);
        }

        protected void RaiseOnBoundChanged(BoundChangeArgs<BoundingBox> args)
        {
            OnBoundChanged?.Invoke(this, args);
        }


        protected void RaiseOnTransformBoundSphereChanged(BoundChangeArgs<global::SharpDX.BoundingSphere> args)
        {
            OnTransformBoundSphereChanged?.Invoke(this, args);
        }

        protected void RaiseOnBoundSphereChanged(BoundChangeArgs<global::SharpDX.BoundingSphere> args)
        {
            OnBoundSphereChanged?.Invoke(this, args);
        }
        #endregion


        #region INotifyPropertyChanged
        private bool disablePropertyChangedEvent = false;
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

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!DisablePropertyChangedEvent)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
