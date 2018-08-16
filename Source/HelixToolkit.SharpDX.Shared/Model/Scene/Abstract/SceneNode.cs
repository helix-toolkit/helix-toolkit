/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using HelixToolkit.Mathematics;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
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
        public Guid GUID { get { return RenderCore.GUID; } }

        /// <summary>
        /// Do not assgin this field. This is updated by <see cref="ComputeTransformMatrix"/>.
        /// Used as field only for performance consideration.
        /// </summary>
        public Matrix TotalModelMatrix = Matrix.Identity;

        /// <summary>
        /// Gets or sets the order key.
        /// </summary>
        /// <value>
        /// The render order key.
        /// </value>
        public OrderKey RenderOrderKey
        {
            private set;
            get;
        }

        private ushort renderOrder = 0;
        /// <summary>
        /// Gets or sets the render order. Manually specify the render order
        /// </summary>
        /// <value>
        /// The render order.
        /// </value>
        public ushort RenderOrder
        {
            set
            {
                if(Set(ref renderOrder, value))
                {
                    InvalidatePerFrameRenderables();
                }
            }
            get { return renderOrder; }
        }
                

        /// <summary>
        /// Gets or sets a value indicating whether [need matrix update].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [need matrix update]; otherwise, <c>false</c>.
        /// </value>
        protected bool NeedMatrixUpdate = true;

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
                    NeedMatrixUpdate = true;
                    InvalidateRender();
                }
            }
            get { return modelMatrix; }
        }

        private SceneNode parent = NullSceneNode.NullNode;
        public SceneNode Parent
        {
            internal set
            {
                if(Set(ref parent, value))
                {
                    NeedMatrixUpdate = true;
                    if (value == null)
                    {
                        value = NullSceneNode.NullNode;
                    }
                }
            }
            get { return parent; }
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

        private bool isRenderable = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is renderable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is renderable; otherwise, <c>false</c>.
        /// </value>
        public bool IsRenderable
        {
            private set
            {
                if(Set(ref isRenderable, value))
                {
                    InvalidatePerFrameRenderables();
                }
            }
            get { return isRenderable; }
        }

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
        /// Gets or sets the type of the render.
        /// </summary>
        /// <value>
        /// The type of the render.
        /// </value>
        public RenderType RenderType
        {
            get
            {
                return RenderCore.RenderType;
            }
            set
            {
                if (RenderCore.RenderType != value)
                {
                    RenderCore.RenderType = value;
                    InvalidatePerFrameRenderables();
                }
            }
        }

        private IRenderTechnique renderTechnique;
        /// <summary>
        /// Gets the effects technique.
        /// </summary>
        /// <value>
        /// The effects technique.
        /// </value>
        public IRenderTechnique EffectTechnique { get { return renderTechnique; } }
        #region Handling Transforms

        /// <summary>
        /// Transforms the changed.
        /// </summary>
        /// <param name="totalTransform">The total transform.</param>
        protected virtual void TransformChanged(ref Matrix totalTransform)
        {
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

        private RenderCore core;
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneNode"/> class.
        /// </summary>
        public SceneNode()
        {
            WrapperSource = this;
            renderCore = new Lazy<RenderCore>(() => 
            {
                core = OnCreateRenderCore();
                core.OnInvalidateRenderer += RenderCore_OnInvalidateRenderer;
                return core;
            }, true);
        }

        /// <summary>
        /// <para>Attaches the element to the specified host. To overide Attach, please override <see cref="OnAttach(IRenderHost)"/> function.</para>
        /// <para>To set different render technique instead of using technique from host, override <see cref="OnCreateRenderTechnique"/></para>
        /// <para>Attach Flow: <see cref="OnCreateRenderTechnique(IRenderHost)"/> -> Set RenderHost -> Get Effect -> <see cref="OnAttach(IRenderHost)"/> -> <see cref="InvalidateSceneGraph"/></para>
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
                NeedMatrixUpdate = true;
                Attached();
                OnAttached?.Invoke(this, EventArgs.Empty);
            }
            InvalidateSceneGraph();
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
                InvalidateSceneGraph();
                RenderCore.Detach();
                OnDetach();
                DisposeAndClear();
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
        /// Tries to invalidate the current render, causes re-render
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvalidateRender()
        {
            renderHost?.InvalidateRender();
        }

        /// <summary>
        /// Invalidates the scene graph. Use this if scene graph has been changed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void InvalidateSceneGraph()
        {
            renderHost?.InvalidateSceneGraph();
        }

        /// <summary>
        /// Invalidates the per frame renderables.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void InvalidatePerFrameRenderables()
        {
            renderHost?.InvalidatePerFrameRenderables();
        }
        /// <summary>
        /// Updates the element total transforms, determine renderability, etc. by the specified time span.
        /// </summary>
        /// <param name="context">The time since last update.</param>
        public virtual void Update(RenderContext context)
        {
            IsRenderable = CanRender(context);
            if (!IsRenderable)
            {
                return;
            }
            ComputeTransformMatrix();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ComputeTransformMatrix()
        {
            if (NeedMatrixUpdate)
            {
                core.ModelMatrix = TotalModelMatrix = modelMatrix * parent.TotalModelMatrix;
                for (int i = 0; i < Items.Count; ++i)
                {
                    Items[i].NeedMatrixUpdate = true;
                }
                NeedMatrixUpdate = false;
                TransformChanged(ref TotalModelMatrix);
                OnTransformChanged?.Invoke(this, new TransformArgs(ref TotalModelMatrix));               
            }
        }
        /// <summary>
        /// Updates the render order key.
        /// </summary>
        public void UpdateRenderOrderKey()
        {
            RenderOrderKey = OnUpdateRenderOrderKey();
        }

        protected virtual OrderKey OnUpdateRenderOrderKey()
        {
            return OrderKey.Create(RenderOrder, 0);
        }

        /// <summary>
        ///
        /// </summary>
        public virtual void UpdateNotRender(RenderContext context) { }

        #region Rendering

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool CanRender(RenderContext context)
        {
            return visible && IsAttached;
        }

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Render(RenderContext context, DeviceContextProxy deviceContext)
        {
            core.Render(context, deviceContext);
        }

        /// <summary>
        /// Renders the shadow.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
            core.RenderShadow(context, deviceContext);
        }
        /// <summary>
        /// Renders the custom.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
            core.RenderCustom(context, deviceContext);
        }
        /// <summary>
        /// View frustum test.
        /// </summary>
        /// <param name="viewFrustum">The frustum.</param>
        /// <returns></returns>
        public virtual bool TestViewFrustum(ref BoundingFrustum viewFrustum)
        {
            return true;
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
        public virtual bool HitTest(RenderContext context, Ray ray, ref List<HitTestResult> hits)
        {
            if (CanHitTest(context))
            {
                return OnHitTest(context, TotalModelMatrix, ref ray, ref hits);
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
        protected virtual bool CanHitTest(RenderContext context)
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
        protected abstract bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits);

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
        protected void RaiseOnTransformBoundSphereChanged(BoundChangeArgs<BoundingSphere> args)
        {
            OnTransformBoundSphereChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the on bound sphere changed.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected void RaiseOnBoundSphereChanged(BoundChangeArgs<BoundingSphere> args)
        {
            OnBoundSphereChanged?.Invoke(this, args);
        }

        #endregion IBoundable

        #region POST EFFECT        
        /// <summary>
        /// Gets or sets the post effects.
        /// </summary>
        /// <value>
        /// The post effects.
        /// </value>
        private readonly Dictionary<string, IEffectAttributes> postEffectNames = new Dictionary<string, IEffectAttributes>();

        /// <summary>
        /// Gets the post effect names.
        /// </summary>
        /// <value>
        /// The post effect names.
        /// </value>
        public IEnumerable<string> PostEffectNames
        {
            get { return postEffectNames.Keys; }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has any post effect.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has any post effect; otherwise, <c>false</c>.
        /// </value>
        public bool HasAnyPostEffect { get { return postEffectNames.Count > 0; } }
        /// <summary>
        /// Adds the post effect.
        /// </summary>
        /// <param name="effect">The effect.</param>
        public void AddPostEffect(IEffectAttributes effect)
        {
            if (postEffectNames.ContainsKey(effect.EffectName))
            {
                return;
            }
            postEffectNames.Add(effect.EffectName, effect);
            InvalidateRender();
        }
        /// <summary>
        /// Removes the post effect.
        /// </summary>
        /// <param name="effectName">Name of the effect.</param>
        public void RemovePostEffect(string effectName)
        {
            if (postEffectNames.Remove(effectName))
            {
                InvalidateRender();
            }
        }
        /// <summary>
        /// Determines whether [has post effect] [the specified effect name].
        /// </summary>
        /// <param name="effectName">Name of the effect.</param>
        /// <returns>
        ///   <c>true</c> if [has post effect] [the specified effect name]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasPostEffect(string effectName)
        {
            return postEffectNames.ContainsKey(effectName);
        }
        /// <summary>
        /// Tries the get post effect.
        /// </summary>
        /// <param name="effectName">Name of the effect.</param>
        /// <param name="effect">The effect.</param>
        /// <returns></returns>
        public bool TryGetPostEffect(string effectName, out IEffectAttributes effect)
        {
            return postEffectNames.TryGetValue(effectName, out effect);
        }
        /// <summary>
        /// Clears the post effect.
        /// </summary>
        public void ClearPostEffect()
        {
            postEffectNames.Clear();
            InvalidateRender();
        }
        #endregion

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

        /// <summary>
        /// Sets the affects scene graph.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField">The backing field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool SetAffectsSceneGraph<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.RaisePropertyChanged(propertyName);
            InvalidateSceneGraph();
            return true;
        }
    }

    public sealed class NullSceneNode : SceneNode
    {
        public static readonly NullSceneNode NullNode = new NullSceneNode();

        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            return false;
        }
    }
}