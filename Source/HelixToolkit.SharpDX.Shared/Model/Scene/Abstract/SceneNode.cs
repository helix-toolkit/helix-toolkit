/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    using Render;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///
    /// </summary>
    public abstract partial class SceneNode : DisposeObject
    {
        #region Properties

        /// <summary>
        ///
        /// </summary>
        public Guid GUID { get; } = Guid.NewGuid();

        private Matrix totalModelMatrix = Matrix.Identity;
        protected bool forceUpdateTransform = false;

        /// <summary>
        ///
        /// </summary>
        public Matrix TotalModelMatrix
        {
            private set
            {
                if (Set(ref totalModelMatrix, value) || forceUpdateTransform)
                {
                    TransformChanged(ref value);
                    OnTransformChanged?.Invoke(this, new TransformArgs(ref value));
                    RenderCore.ModelMatrix = value;
                    forceUpdateTransform = false;
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
                if (Set(ref modelMatrix, value))
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
                if (Set(ref parentMatrix, value))
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
        /// Gets or sets a value indicating whether this <see cref="SceneNode"/> is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible
        {
            internal set
            {
                if (Set(ref visible, value))
                {
                    OnVisibleChanged?.Invoke(this, value ? BoolArgs.TrueArgs : BoolArgs.FalseArgs);
                    InvalidateRender();
                }
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
            private set; get;
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
        /// Gets the effects manager.
        /// </summary>
        /// <value>
        /// The effects manager.
        /// </value>
        protected IEffectsManager EffectsManager { get { return renderHost.EffectsManager; } }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public virtual IList<SceneNode> Items
        {
            get;
        } = Constants.EmptyRenderableArray;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hit test visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is hit test visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsHitTestVisible { set; get; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is throwing shadow.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is throwing shadow; otherwise, <c>false</c>.
        /// </value>
        public bool IsThrowingShadow
        {
            set
            {
                RenderCore.IsThrowingShadow = value;
            }
            get
            {
                return RenderCore.IsThrowingShadow;
            }
        }

        #region Handling Transforms

        /// <summary>
        /// Transforms the changed.
        /// </summary>
        /// <param name="totalTransform">The total transform.</param>
        protected virtual void TransformChanged(ref Matrix totalTransform)
        {
            for (int i = 0; i < Items.Count; ++i)
            {
                Items[i].ParentMatrix = totalTransform;
            }
        }

        /// <summary>
        /// Occurs when [on transform changed].
        /// </summary>
        public event EventHandler<TransformArgs> OnTransformChanged;

        #endregion Handling Transforms

        #region RenderCore

        private Lazy<RenderCore> renderCore;
        public RenderCore RenderCore
        {
            get { return renderCore.Value; }
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
        protected virtual RenderCore OnCreateRenderCore() { return new EmptyRenderCore(); }

        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected virtual void AssignDefaultValuesToCore(RenderCore core) { }

        private void RenderCore_OnInvalidateRenderer(object sender, EventArgs e)
        {
            InvalidateRender();
        }

        #endregion RenderCore

        /// <summary>
        /// Gets or sets the wrapper source used for such as hit test model, etc. The wrapper must set this so the <see cref="HitTestResult.ModelHit"/> is the wrapper.
        /// </summary>
        /// <value>
        /// The hit test source.
        /// </value>
        public object WrapperSource { internal set; get; }

        #endregion Properties

        #region Events

        public event EventHandler<BoolArgs> OnVisibleChanged;

        public event EventHandler OnAttached;

        public event EventHandler OnDetached;

        #endregion Events

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneNode"/> class.
        /// </summary>
        public SceneNode()
        {
            WrapperSource = this;
            renderCore = new Lazy<RenderCore>(() => 
            {
                var c = OnCreateRenderCore();
                c.OnInvalidateRenderer += RenderCore_OnInvalidateRenderer;
                return c;
            }, true);
        }

        /// <summary>
        /// <para>Attaches the element to the specified host. To overide Attach, please override <see cref="OnAttach(IRenderHost)"/> function.</para>
        /// <para>To set different render technique instead of using technique from host, override <see cref="OnCreateRenderTechnique"/></para>
        /// <para>Attach Flow: <see cref="OnCreateRenderTechnique(IRenderHost)"/> -> Set RenderHost -> Get Effect -> <see cref="OnAttach(IRenderHost)"/> -> <see cref="InvalidateRender"/></para>
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
            if (renderTechnique == null)
            {
                var techniqueName = RenderHost.EffectsManager.RenderTechniques.FirstOrDefault();
                if (string.IsNullOrEmpty(techniqueName))
                {
                    return;
                }
                renderTechnique = RenderHost.EffectsManager[techniqueName];
            }
            IsAttached = OnAttach(host);
            if (IsAttached)
            {
                Attached();
                OnAttached?.Invoke(this, EventArgs.Empty);
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
            AssignDefaultValuesToCore(RenderCore);
            return RenderCore == null ? false : RenderCore.IsAttached;
        }

        /// <summary>
        /// Called when [attached] and <see cref="IsAttached"/> = true.
        /// </summary>
        protected virtual void Attached() { }

        /// <summary>
        /// Detaches the element from the host. Override <see cref="OnDetach"/>
        /// </summary>
        public void Detach()
        {
            if (IsAttached)
            {
                IsAttached = false;
                RenderCore.Detach();
                OnDetach();
                OnDetached?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Used to override Detach
        /// </summary>
        protected virtual void OnDetach()
        {
            renderHost = null;
        }

        protected void InvalidateRenderEvent(object sender, EventArgs arg)
        {
            renderHost?.InvalidateRender();
        }

        /// <summary>
        /// Tries to invalidate the current render.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            if (needMatrixUpdate || forceUpdateTransform)
            {
                TotalModelMatrix = modelMatrix * parentMatrix;
                needMatrixUpdate = false;
            }
            IsRenderable = CanRender(context);
        }

        /// <summary>
        ///
        /// </summary>
        public virtual void UpdateNotRender(IRenderContext context) { }

        #region Rendering

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool CanRender(IRenderContext context)
        {
            return Visible && IsAttached;
        }

        #endregion Rendering

        #region Hit Test

        /// <summary>
        /// Hits the test.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Determines whether this instance [can hit test] the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can hit test] the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanHitTest(IRenderContext context)
        {
            return IsHitTestVisible && IsRenderable;
        }

        /// <summary>
        /// Called when [hit test].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="totalModelMatrix">The total model matrix.</param>
        /// <param name="ray">The ray.</param>
        /// <param name="hits">The hits.</param>
        /// <returns></returns>
        protected abstract bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits);

        #endregion Hit Test

        #region IBoundable

        /// <summary>
        /// The maximum bound
        /// </summary>
        public static readonly BoundingBox MaxBound = new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MaxValue));

        /// <summary>
        /// The maximum bound sphere
        /// </summary>
        public static readonly BoundingSphere MaxBoundSphere = new BoundingSphere(Vector3.Zero, float.MaxValue);

        /// <summary>
        /// <see cref="IBoundable.OriginalBounds"/>
        /// </summary>
        /// <value>
        /// The original bounds.
        /// </value>
        public virtual BoundingBox OriginalBounds { get { return MaxBound; } }

        /// <summary>
        /// <see cref="IBoundable.OriginalBoundsSphere"/>
        /// </summary>
        /// <value>
        /// The original bounds sphere.
        /// </value>
        public virtual BoundingSphere OriginalBoundsSphere { get { return MaxBoundSphere; } }

        /// <summary>
        /// <see cref="IBoundable.Bounds"/>
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public virtual BoundingBox Bounds
        {
            get { return MaxBound; }
        }

        /// <summary>
        /// <see cref="IBoundable.BoundsWithTransform"/>
        /// </summary>
        /// <value>
        /// The bounds with transform.
        /// </value>
        public virtual BoundingBox BoundsWithTransform
        {
            get { return MaxBound; }
        }

        /// <summary>
        /// <see cref="IBoundable.BoundsSphere"/>
        /// </summary>
        /// <value>
        /// The bounds sphere.
        /// </value>
        public virtual BoundingSphere BoundsSphere
        {
            get { return MaxBoundSphere; }
        }

        /// <summary>
        /// <see cref="IBoundable.BoundsSphereWithTransform"/>
        /// </summary>
        /// <value>
        /// The bounds sphere with transform.
        /// </value>
        public virtual BoundingSphere BoundsSphereWithTransform
        {
            get { return MaxBoundSphere; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has bound.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has bound; otherwise, <c>false</c>.
        /// </value>
        public bool HasBound { protected set; get; } = false;

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
        public event EventHandler<BoundChangeArgs<BoundingSphere>> OnBoundSphereChanged;

        /// <summary>
        /// Occurs when [on transform bound sphere changed].
        /// </summary>
        public event EventHandler<BoundChangeArgs<BoundingSphere>> OnTransformBoundSphereChanged;

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

        #endregion IBoundable

        protected override void OnDispose(bool disposeManagedResources)
        {
            if (!Items.IsReadOnly)
            {
                Items.Clear();
            }
            RenderCore.Dispose();
            OnVisibleChanged = null;
            OnTransformChanged = null;
            OnSetRenderTechnique = null;
            OnBoundChanged = null;
            OnTransformBoundChanged = null;
            OnBoundSphereChanged = null;
            OnTransformBoundSphereChanged = null;
            OnAttached = null;
            OnDetached = null;
            WrapperSource = null;
            base.OnDispose(disposeManagedResources);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetAffectsRender<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.RaisePropertyChanged(propertyName);
            InvalidateRender();
            return true;
        }
    }
}