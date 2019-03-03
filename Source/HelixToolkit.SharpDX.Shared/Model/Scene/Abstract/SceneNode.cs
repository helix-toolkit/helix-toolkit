/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Model.Scene
    {
        using Core;
        using Render;     

        /// <summary>
        ///
        /// </summary>
        public abstract partial class SceneNode : DisposeObject, IComparable<SceneNode>, Animations.IAnimationNode
        {
            #region Properties

            /// <summary>
            ///
            /// </summary>
            public Guid GUID { get { return RenderCore.GUID; } }
            private string name = "Node";
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string Name
            {
                set
                {
                    if(Set(ref name, value))
                    {
                        NameChanged?.Invoke(this, new StringArgs(value));
                    }
                }
                get => name;
            }
            /// <summary>
            /// Do not assgin this field. This is updated by <see cref="ComputeTransformMatrix"/>.
            /// Used as field only for performance consideration.
            /// </summary>
            internal Matrix TotalModelMatrixInternal = Matrix.Identity;
            /// <summary>
            /// Gets the total model matrix.
            /// </summary>
            /// <value>
            /// The total model matrix.
            /// </value>
            public Matrix TotalModelMatrix { get => TotalModelMatrixInternal; }
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
                    if (SetAffectsRender(ref modelMatrix, value))
                    {
                        NeedMatrixUpdate = true;
                    }
                }
                get { return modelMatrix; }
            }

            private SceneNode parent = NullSceneNode.NullNode;
            /// <summary>
            /// Gets or sets the parent.
            /// </summary>
            /// <value>
            /// The parent.
            /// </value>
            public SceneNode Parent
            {
                internal set
                {
                    if(Set(ref parent, value))
                    {
                        NeedMatrixUpdate = true;
                        if (value == null)
                        {
                            parent = NullSceneNode.NullNode;
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
                set
                {
                    if (SetAffectsRender(ref visible, value))
                    {
                        VisibleChanged?.Invoke(this, value ? BoolArgs.TrueArgs : BoolArgs.FalseArgs);
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

            /// <summary>
            ///
            /// </summary>
            public IRenderHost RenderHost { get; private set; }

            /// <summary>
            /// Gets the effects manager.
            /// </summary>
            /// <value>
            /// The effects manager.
            /// </value>
            protected IEffectsManager EffectsManager { get { return RenderHost.EffectsManager; } }

            /// <summary>
            /// Gets the items.
            /// </summary>
            /// <value>
            /// The items.
            /// </value>
            internal ObservableCollection<SceneNode> ItemsInternal
            {
                set; get;
            } = Constants.EmptyRenderableArray;

            /// <summary>
            /// Gets the readonly child items from outside UI component access.
            /// </summary>
            /// <value>
            /// The children.
            /// </value>
            public ReadOnlyObservableCollection<SceneNode> Items { internal set; get; } = Constants.EmptyReadOnlyRenderableArray;
            /// <summary>
            /// Gets the items count.
            /// </summary>
            /// <value>
            /// The items count.
            /// </value>
            public int ItemsCount { get => Items.Count; }

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

            /// <summary>
            /// Gets or sets a value indicating whether this node is animation node.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is animation node; otherwise, <c>false</c>.
            /// </value>
            public bool IsAnimationNode { set; get; } = false;
            /// <summary>
            /// Gets a value indicating whether this node is animation node root.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this node is animation node root; otherwise, <c>false</c>.
            /// </value>
            public bool IsAnimationNodeRoot
            {
                get
                {
                    if (IsAnimationNode)
                    {
                        if (Parent is Animations.IAnimationNode n)
                        {
                            return !n.IsAnimationNode;
                        }
                    }
                    return false;
                }
            }
            #region Handling Transforms

            /// <summary>
            /// Transforms the changed.
            /// </summary>
            /// <param name="totalTransform">The total transform.</param>
            protected virtual void OnTransformChanged(ref Matrix totalTransform)
            {
            }

            /// <summary>
            /// Occurs when [on transform changed].
            /// </summary>
            public event EventHandler<TransformArgs> TransformChanged;

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

            private object tag = null;
            /// <summary>
            /// Gets or sets the tag. This can be used to attach an external view model or property class object
            /// </summary>
            /// <value>
            /// The tag.
            /// </value>
            public object Tag
            {
                set => Set(ref tag, value);
                get => tag;
            }
            /// <summary>
            /// Gets or sets a value indicating whether this instance is in frustum in current frame.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is in frustum; otherwise, <c>false</c>.
            /// </value>
            public bool IsInFrustum { internal set; get; }
            #endregion Properties

            #region Events            
            public event EventHandler<StringArgs> NameChanged;
            /// <summary>
            /// Occurs when [visible changed].
            /// </summary>
            public event EventHandler<BoolArgs> VisibleChanged;
            /// <summary>
            /// Occurs when [attached].
            /// </summary>
            public event EventHandler Attached;
            /// <summary>
            /// Occurs when [detached].
            /// </summary>
            public event EventHandler Detached;
            /// <summary>
            /// Occurs when [mouse down].
            /// </summary>
            public event EventHandler<SceneNodeMouseDownArgs> MouseDown;
            /// <summary>
            /// Occurs when [mouse move].
            /// </summary>
            public event EventHandler<SceneNodeMouseMoveArgs> MouseMove;
            /// <summary>
            /// Occurs when [mouse up].
            /// </summary>
            public event EventHandler<SceneNodeMouseUpArgs> MouseUp;
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
                    core.InvalidateRender += RenderCore_OnInvalidateRenderer;
                    return core;
                }, true);
            }
            /// <summary>
            /// Initializes a new instance of the <see cref="SceneNode"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            public SceneNode(string name) : this()
            {
                Name = name;
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
                RenderHost = host;
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
                    OnAttached();
                    Attached?.Invoke(this, EventArgs.Empty);
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
            protected virtual void OnAttached() { }

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
                    renderTechnique = null;
                    Detached?.Invoke(this, EventArgs.Empty);              
                }
            }

            /// <summary>
            /// Used to override Detach
            /// </summary>
            protected virtual void OnDetach()
            {
                RenderHost = null;
            }

            protected void InvalidateRenderEvent(object sender, EventArgs arg)
            {
                RenderHost?.InvalidateRender();
            }

            /// <summary>
            /// Tries to invalidate the current render, causes re-render
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void InvalidateRender()
            {
                RenderHost?.InvalidateRender();
            }

            /// <summary>
            /// Invalidates the scene graph. Use this if scene graph has been changed.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected void InvalidateSceneGraph()
            {
                RenderHost?.InvalidateSceneGraph();
            }

            /// <summary>
            /// Invalidates the per frame renderables.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected void InvalidatePerFrameRenderables()
            {
                RenderHost?.InvalidatePerFrameRenderables();
            }
            /// <summary>
            /// Updates the element total transforms, determine renderability, etc. by the specified time span.
            /// </summary>
            /// <param name="context">The time since last update.</param>
            public virtual void Update(RenderContext context)
            {
                IsRenderable = CanRender(context) && core.CanRenderFlag;
                IsInFrustum = true;//Reset during update
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
                    TotalModelMatrixInternal = modelMatrix * parent.TotalModelMatrixInternal;
                    for (int i = 0; i < ItemsInternal.Count; ++i)
                    {
                        ItemsInternal[i].NeedMatrixUpdate = true;
                    }
                    NeedMatrixUpdate = false;
                    OnTransformChanged(ref TotalModelMatrixInternal);
                    TransformChanged?.Invoke(this, new TransformArgs(ref TotalModelMatrixInternal));               
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
                core.ModelMatrix = TotalModelMatrixInternal;
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
                core.ModelMatrix = TotalModelMatrixInternal;
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
                core.ModelMatrix = TotalModelMatrixInternal;
                core.RenderCustom(context, deviceContext);
            }
            /// <summary>
            /// Renders the custom.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="deviceContext">The device context.</param>
            /// <param name="pass"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void RenderDepth(RenderContext context, DeviceContextProxy deviceContext, Shaders.ShaderPass pass)
            {
                core.ModelMatrix = TotalModelMatrixInternal;
                core.RenderDepth(context, deviceContext, pass);
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
                    return OnHitTest(context, TotalModelMatrixInternal, ref ray, ref hits);
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
            public event EventHandler<BoundChangeArgs<BoundingBox>> BoundChanged;

            /// <summary>
            /// Occurs when [on transform bound changed].
            /// </summary>
            public event EventHandler<BoundChangeArgs<BoundingBox>> TransformBoundChanged;

            /// <summary>
            /// Occurs when [on bound sphere changed].
            /// </summary>
            public event EventHandler<BoundChangeArgs<BoundingSphere>> BoundSphereChanged;

            /// <summary>
            /// Occurs when [on transform bound sphere changed].
            /// </summary>
            public event EventHandler<BoundChangeArgs<BoundingSphere>> TransformBoundSphereChanged;

            /// <summary>
            /// Raises the on transform bound changed.
            /// </summary>
            /// <param name="args">The arguments.</param>
            protected void RaiseOnTransformBoundChanged(BoundChangeArgs<BoundingBox> args)
            {
                TransformBoundChanged?.Invoke(this, args);
            }

            /// <summary>
            /// Raises the on bound changed.
            /// </summary>
            /// <param name="args">The arguments.</param>
            protected void RaiseOnBoundChanged(BoundChangeArgs<BoundingBox> args)
            {
                BoundChanged?.Invoke(this, args);
            }

            /// <summary>
            /// Raises the on transform bound sphere changed.
            /// </summary>
            /// <param name="args">The arguments.</param>
            protected void RaiseOnTransformBoundSphereChanged(BoundChangeArgs<global::SharpDX.BoundingSphere> args)
            {
                TransformBoundSphereChanged?.Invoke(this, args);
            }

            /// <summary>
            /// Raises the on bound sphere changed.
            /// </summary>
            /// <param name="args">The arguments.</param>
            protected void RaiseOnBoundSphereChanged(BoundChangeArgs<global::SharpDX.BoundingSphere> args)
            {
                BoundSphereChanged?.Invoke(this, args);
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
                ItemsInternal.Clear();
                RenderCore.Dispose();
                VisibleChanged = null;
                TransformChanged = null;
                OnSetRenderTechnique = null;
                BoundChanged = null;
                TransformBoundChanged = null;
                BoundSphereChanged = null;
                TransformBoundSphereChanged = null;
                MouseDown = null;
                MouseMove = null;
                MouseUp = null;
                Attached = null;
                Detached = null;
                WrapperSource = null;
                NameChanged = null;
                base.OnDispose(disposeManagedResources);
            }
            /// <summary>
            /// Removes self from scene graph.
            /// </summary>
            /// <returns></returns>
            public bool RemoveSelf()
            {
                if(parent != null && parent is GroupNodeBase group)
                {
                    return group.RemoveChildNode(this);
                }
                else
                {
                    return false;
                }
            }

            public int CompareTo(SceneNode other)
            {
                if(other == null) { return 1; }
                return RenderOrderKey.CompareTo(other.RenderOrderKey);
            }

            public void RaiseMouseDownEvent(IViewport3DX viewport, Vector2 pos, HitTestResult hit, object originalInputEventArgs = null)
            {
                MouseDown?.Invoke(this, new SceneNodeMouseDownArgs(viewport, pos, this, hit, originalInputEventArgs));
            }

            public void RaiseMouseMoveEvent(IViewport3DX viewport, Vector2 pos, HitTestResult hit, object originalInputEventArgs = null)
            {
                MouseMove?.Invoke(this, new SceneNodeMouseMoveArgs(viewport, pos, this, hit, originalInputEventArgs));
            }

            public void RaiseMouseUpEvent(IViewport3DX viewport, Vector2 pos, HitTestResult hit, object originalInputEventArgs = null)
            {
                MouseUp?.Invoke(this, new SceneNodeMouseUpArgs(viewport, pos, this, hit, originalInputEventArgs));
            }


            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="backingField"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            protected bool SetAffectsRender<T>(ref T backingField, T value)
            {
                if (EqualityComparer<T>.Default.Equals(backingField, value))
                {
                    return false;
                }

                backingField = value;
                InvalidateRender();
                return true;
            }

            /// <summary>
            /// Sets the affects scene graph.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="backingField">The backing field.</param>
            /// <param name="value">The value.</param>
            /// <returns></returns>
            protected bool SetAffectsSceneGraph<T>(ref T backingField, T value)
            {
                if (EqualityComparer<T>.Default.Equals(backingField, value))
                {
                    return false;
                }

                backingField = value;
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

        #region Mouse Events Args
        public class SceneNodeMouseDownArgs : EventArgs
        {
            public HitTestResult HitResult { get; }
            public SceneNode Source { get; }
            public IViewport3DX Viewport { get; }
            public Vector2 Position { get; }
            public object OriginalInputEventArgs { get; }
            public SceneNodeMouseDownArgs(IViewport3DX viewport, Vector2 pos, SceneNode node, HitTestResult hit, object originalInputEventArgs = null)
            {
                Viewport = viewport;
                Position = pos;
                Source = node;
                HitResult = hit;
                OriginalInputEventArgs = originalInputEventArgs;
            }
        }

        public class SceneNodeMouseMoveArgs : EventArgs
        {
            public HitTestResult HitResult { get; }
            public SceneNode Source { get; }
            public IViewport3DX Viewport { get; }
            public Vector2 Position { get; }
            public object OriginalInputEventArgs { get; }
            public SceneNodeMouseMoveArgs(IViewport3DX viewport, Vector2 pos, SceneNode node, HitTestResult hit, object originalInputEventArgs = null)
            {
                Viewport = viewport;
                Position = pos;
                Source = node;
                HitResult = hit;
                OriginalInputEventArgs = originalInputEventArgs;
            }
        }

        public class SceneNodeMouseUpArgs : EventArgs
        {
            public HitTestResult HitResult { get; }
            public SceneNode Source { get; }
            public IViewport3DX Viewport { get; }
            public Vector2 Position { get; }
            public object OriginalInputEventArgs { get; }
            public SceneNodeMouseUpArgs(IViewport3DX viewport, Vector2 pos, SceneNode node, HitTestResult hit, object originalInputEventArgs = null)
            {
                Viewport = viewport;
                Position = pos;
                Source = node;
                HitResult = hit;
                OriginalInputEventArgs = originalInputEventArgs;
            }
        }
        #endregion
    }
}