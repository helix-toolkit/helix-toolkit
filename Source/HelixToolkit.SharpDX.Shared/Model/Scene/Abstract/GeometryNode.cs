/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using Matrix = System.Numerics.Matrix4x4;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;
    using Components;

    public abstract class GeometryNode : SceneNode, IHitable, IThrowingShadow, IInstancing, IBoundable
    {
        #region Properties

        private Geometry3D geometry;
        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        public Geometry3D Geometry
        {
            set
            {
                var old = geometry;
                if (Set(ref geometry, value))
                {
                    BoundManager.Geometry = value;
                    if (IsAttached)
                    {
                        CreateGeometryBuffer();
                    }
                    OnGeometryChanged(value, old);
                    InvalidateRender();                   
                }
            }
            get
            {
                return geometry;
            }
        }

        private IList<Matrix> instances;
        /// <summary>
        /// Gets or sets the instances.
        /// </summary>
        /// <value>
        /// The instances.
        /// </value>
        public IList<Matrix> Instances
        {
            set
            {
                if (Set(ref instances, value))
                {
                    BoundManager.Instances = value;
                    InstanceBuffer.Elements = value;
                    InstancesChanged();
                }
            }
            get { return instances; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has instances.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has instances; otherwise, <c>false</c>.
        /// </value>
        public bool HasInstances { get { return InstanceBuffer.HasElements; } }

        /// <summary>
        /// Gets the instance buffer.
        /// </summary>
        /// <value>
        /// The instance buffer.
        /// </value>
        public IElementsBufferModel<Matrix> InstanceBuffer { get; } = new MatrixInstanceBufferModel();

        /// <summary>
        /// Instanceses the changed.
        /// </summary>
        protected virtual void InstancesChanged() { }

        /// <summary>
        /// The reuse vertex array buffer
        /// </summary>
        protected bool reuseVertexArrayBuffer = false;

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public delegate RasterizerStateDescription CreateRasterStateFunc();

        /// <summary>
        /// Create raster state description delegate.
        /// <para>If <see cref="OnCreateRasterState"/> is set, then <see cref="CreateRasterState"/> will not be called.</para>
        /// </summary>
        public CreateRasterStateFunc OnCreateRasterState;
        /// <summary>
        /// Gets the buffer model internal.
        /// </summary>
        /// <value>
        /// The buffer model internal.
        /// </value>
        protected IAttachableBufferModel BufferModelInternal { get { return bufferModelInternal; } }
        private IAttachableBufferModel bufferModelInternal;

        /// <summary>
        /// Gets a value indicating whether [geometry valid].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [geometry valid]; otherwise, <c>false</c>.
        /// </value>
        public bool GeometryValid { get { return BoundManager.GeometryValid; } }

        /// <summary>
        /// Gets or sets the bound manager.
        /// </summary>
        /// <value>
        /// The bound manager.
        /// </value>
        public readonly GeometryBoundManager BoundManager;
        /// <summary>
        /// Gets the original bound from the geometry. Same as <see cref="Geometry3D.Bound"/>
        /// </summary>
        /// <value>
        /// The original bound.
        /// </value>
        public sealed override BoundingBox OriginalBounds
        {
            get { return BoundManager.OriginalBounds; }
        }
        /// <summary>
        /// Gets the original bound sphere from the geometry. Same as <see cref="Geometry3D.BoundingSphere"/> 
        /// </summary>
        /// <value>
        /// The original bound sphere.
        /// </value>
        public sealed override BoundingSphere OriginalBoundsSphere
        {
            get { return BoundManager.OriginalBoundsSphere; }
        }

        /// <summary>
        /// Gets the bounds. Usually same as <see cref="OriginalBounds"/>. If have instances, the bound will enclose all instances.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public sealed override BoundingBox Bounds
        {
            get { return BoundManager.Bounds; }
        }

        /// <summary>
        /// Gets the bounds with transform. Usually same as <see cref="Bounds"/>. If have transform, the bound is the transformed <see cref="Bounds"/>
        /// </summary>
        /// <value>
        /// The bounds with transform.
        /// </value>
        public sealed override BoundingBox BoundsWithTransform
        {
            get { return BoundManager.BoundsWithTransform; }
        }


        /// <summary>
        /// Gets the bounds sphere. Usually same as <see cref="OriginalBoundsSphere"/>. If have instances, the bound sphere will enclose all instances.
        /// </summary>
        /// <value>
        /// The bounds sphere.
        /// </value>
        public override BoundingSphere BoundsSphere
        {
            get { return BoundManager.BoundsSphere; }
        }

        /// <summary>
        /// Gets the bounds sphere with transform. If have transform, the bound is the transformed <see cref="BoundsSphere"/>
        /// </summary>
        /// <value>
        /// The bounds sphere with transform.
        /// </value>
        public override BoundingSphere BoundsSphereWithTransform
        {
            get { return BoundManager.BoundsSphereWithTransform; }
        }

        #region Rasterizer parameters

        private int depthBias = 0;
        /// <summary>
        /// Gets or sets the depth bias.
        /// </summary>
        /// <value>
        /// The depth bias.
        /// </value>
        public int DepthBias
        {
            set
            {
                if (Set(ref depthBias, value))
                {
                    OnRasterStateChanged();
                }
            }
            get { return depthBias; }
        }

        private float slopScaledDepthBias = 0;
        /// <summary>
        /// Gets or sets the slope scaled depth bias.
        /// </summary>
        /// <value>
        /// The slope scaled depth bias.
        /// </value>
        public float SlopeScaledDepthBias
        {
            get { return slopScaledDepthBias; }
            set
            {
                if (Set(ref slopScaledDepthBias, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private bool isMSAAEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether Multisampling Anti-Aliasing enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is msaa enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsMSAAEnabled
        {
            get { return isMSAAEnabled = true; }
            set
            {
                if (Set(ref isMSAAEnabled, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private bool isScissorEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is scissor enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is scissor enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsScissorEnabled
        {
            get { return isScissorEnabled; }
            set
            {
                if (Set(ref isScissorEnabled, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private FillMode fillMode = FillMode.Solid;
        /// <summary>
        /// Gets or sets the fill mode.
        /// </summary>
        /// <value>
        /// The fill mode.
        /// </value>
        public FillMode FillMode
        {
            get { return fillMode; }
            set
            {
                if (Set(ref fillMode, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private bool isDepthClipEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is depth clip enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is depth clip enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDepthClipEnabled
        {
            get { return isDepthClipEnabled; }
            set
            {
                if (Set(ref isDepthClipEnabled, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        #endregion Rasterizer parameters        
        private bool enableViewFrustumCheck = true;
        /// <summary>
        /// Gets or sets a value indicating whether [enable view frustum check].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable view frustum check]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableViewFrustumCheck
        {
            set { enableViewFrustumCheck = value; }
            get { return enableViewFrustumCheck && HasBound; }
        }

        private string postEffects;
        /// <summary>
        /// Gets or sets the post effects.
        /// </summary>
        /// <value>
        /// The post effects.
        /// </value>
        public string PostEffects
        {
            get { return postEffects; }
            set
            {
                if (Set(ref postEffects, value))
                {
                    ClearPostEffect();
                    if (value is string effects)
                    {
                        if (!string.IsNullOrEmpty(effects))
                        {
                            foreach (var effect in EffectAttributes.Parse(effects))
                            {
                                AddPostEffect(effect);
                            }
                        }
                    }
                }
            }
        }
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
        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryNode"/> class.
        /// </summary>
        public GeometryNode()
        {
            BoundManager = new GeometryBoundManager(this);
            BoundManager.OnBoundChanged += (s, e) => { RaiseOnBoundChanged(e); };
            BoundManager.OnTransformBoundChanged += (s, e) => { RaiseOnTransformBoundChanged(e); };
            BoundManager.OnBoundSphereChanged += (s, e) => { RaiseOnBoundSphereChanged(e); };
            BoundManager.OnTransformBoundSphereChanged += (s, e) => { RaiseOnTransformBoundSphereChanged(e); };
            BoundManager.OnCheckGeometry = OnCheckGeometry;
            HasBound = true;
        }

        /// <summary>
        /// Called when [check geometry].
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <returns></returns>
        protected virtual bool OnCheckGeometry(Geometry3D geometry)
        {
            return !(geometry == null || geometry.Positions == null || geometry.Positions.Count == 0);
        }

        /// <summary>
        /// Called when [create buffer model].
        /// </summary>
        /// <returns></returns>
        protected virtual IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            return EmptyGeometryBufferModel.Empty;
        }

        /// <summary>
        /// Called when [raster state changed].
        /// </summary>
        protected virtual void OnRasterStateChanged()
        {
            if (IsAttached && RenderCore is IGeometryRenderCore r)
            {
                r.RasterDescription = OnCreateRasterState != null ? OnCreateRasterState() : CreateRasterState();
            }
        }

        /// <summary>
        /// Called when [geometry changed].
        /// </summary>
        /// <param name="newGeometry">The new geometry.</param>
        /// <param name="oldGeometry">The old geometry.</param>
        protected virtual void OnGeometryChanged(Geometry3D newGeometry, Geometry3D oldGeometry)
        {
        }

        /// <summary>
        /// Create raster state description.
        /// <para>If <see cref="OnCreateRasterState"/> is set, then <see cref="OnCreateRasterState"/> instead of <see cref="CreateRasterState"/> will be called.</para>
        /// </summary>
        /// <returns></returns>
        protected abstract RasterizerStateDescription CreateRasterState();

        /// <summary>
        /// This function initialize the Geometry Buffer and Instance Buffer
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return true if attached
        /// </returns>
        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                CreateGeometryBuffer();
                BoundManager.Geometry = Geometry;
                InstanceBuffer.Initialize();
                InstanceBuffer.Elements = this.Instances;
                if (RenderCore is IGeometryRenderCore r)
                {
                    r.InstanceBuffer = InstanceBuffer;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CreateGeometryBuffer()
        {
            RemoveAndDispose(ref bufferModelInternal);
            bufferModelInternal = Collect(OnCreateBufferModel(this.GUID, geometry));
            if(RenderCore is IGeometryRenderCore core)
            {
                core.GeometryBuffer = bufferModelInternal;
            }
        }
        /// <summary>
        /// Called when [attached].
        /// </summary>
        protected override void Attached()
        {
            OnRasterStateChanged();
            base.Attached();
        }

        private void BufferModel_InvalidateRenderer(object sender, bool e)
        {
            this.InvalidateRender();
        }

        /// <summary>
        /// Used to override Detach
        /// </summary>
        protected override void OnDetach()
        {
            bufferModelInternal = null;
            InstanceBuffer.DisposeAndClear();
            BoundManager.DisposeAndClear();
            base.OnDetach();
        }

        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool CanRender(RenderContext context)
        {
            if (base.CanRender(context) && GeometryValid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Views the frustum test.
        /// </summary>
        /// <param name="viewFrustum">The view frustum.</param>
        /// <returns></returns>
        public override bool TestViewFrustum(ref BoundingFrustum viewFrustum)
        {
            if (!EnableViewFrustumCheck)
            {
                return true;
            }
            return BoundingFrustumExtensions.Intersects(ref viewFrustum, ref BoundManager.BoundsWithTransform, ref BoundManager.BoundsSphereWithTransform);
            //return viewFrustum.Intersects(ref BoundManager.BoundsWithTransform) && viewFrustum.Intersects(ref BoundManager.BoundsSphereWithTransform);
        }

        /// <summary>
        ///
        /// </summary>
        public override bool HitTest(RenderContext context, Ray rayWS, ref List<HitTestResult> hits)
        {
            if (CanHitTest(context))
            {
                if (this.InstanceBuffer.HasElements)
                {
                    bool hit = false;
                    int idx = 0;
                    foreach (var modelMatrix in InstanceBuffer.Elements)
                    {
                        var b = this.Bounds;
                        if (OnHitTest(context, modelMatrix * TotalModelMatrix, ref rayWS, ref hits))
                        {
                            hit = true;
                            var lastHit = hits[hits.Count - 1];
                            lastHit.Tag = idx;
                            hits[hits.Count - 1] = lastHit;
                        }
                        ++idx;
                    }

                    return hit;
                }
                else
                {
                    return OnHitTest(context, TotalModelMatrix, ref rayWS, ref hits);
                }
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
        protected override bool CanHitTest(RenderContext context)
        {
            return base.CanHitTest(context) && GeometryValid;
        }
        /// <summary>
        /// Updates the not render.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void UpdateNotRender(RenderContext context)
        {
            base.UpdateNotRender(context);
            if (IsHitTestVisible && context.AutoUpdateOctree && geometry != null && geometry.OctreeDirty)
            {
                geometry?.UpdateOctree();
            }
        }

        protected override void OnDispose(bool disposeManagedResources)
        {
            BoundManager.Dispose();
            base.OnDispose(disposeManagedResources);
        }
    }
}