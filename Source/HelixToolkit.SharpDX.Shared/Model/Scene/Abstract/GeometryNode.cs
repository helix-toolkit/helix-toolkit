/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using System.Linq;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene
#endif
{
    using Core;

    public abstract class GeometryNode : SceneNode, IHitable, IThrowingShadow, IInstancing
    { 
        #region Properties   
        private Geometry3D geometry;
        public Geometry3D Geometry
        {
            set
            {
                if(Set(ref geometry, value))
                {
                    if (IsAttached)
                    {
                        BoundManager.Geometry = value;
                        BufferModelInternal = OnCreateBufferModel(this.GUID, value);
                    }
                    InvalidateRender();
                    OnGeometryChanged(value);
                }
            }
            get
            {
                return geometry;
            }
        }

        private IList<Matrix> instances;
        public IList<Matrix> Instances
        {
            set
            {
                if(Set(ref instances, value))
                {
                    BoundManager.Instances = value;
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

    

        private IGeometryBufferProxy bufferModelInternal;
        /// <summary>
        /// The buffer model internal
        /// </summary>
        protected IGeometryBufferProxy BufferModelInternal
        {
            set
            {
                if (bufferModelInternal == value)
                { return; }
                Disposer.RemoveAndDispose(ref bufferModelInternal);
                bufferModelInternal = value;
                ((IGeometryRenderCore)RenderCore).GeometryBuffer = bufferModelInternal == null ? null : bufferModelInternal.BufferModel;
            }
            get
            {
                return bufferModelInternal;
            }
        }

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
        public GeometryBoundManager BoundManager { private set; get; }
        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public override BoundingBox Bounds
        {
            get { return BoundManager.Bounds; }
        }
        /// <summary>
        /// Gets the bounds with transform.
        /// </summary>
        /// <value>
        /// The bounds with transform.
        /// </value>
        public override BoundingBox BoundsWithTransform
        {
            get { return BoundManager.BoundsWithTransform; }
        }
        /// <summary>
        /// Gets the bounds sphere.
        /// </summary>
        /// <value>
        /// The bounds sphere.
        /// </value>
        public override BoundingSphere BoundsSphere
        {
            get
            {
                return BoundManager.BoundsSphere;
            }
        }
        /// <summary>
        /// Gets the bounds sphere with transform.
        /// </summary>
        /// <value>
        /// The bounds sphere with transform.
        /// </value>
        public override BoundingSphere BoundsSphereWithTransform
        {
            get
            {
                return BoundManager.BoundsSphereWithTransform;
            }
        }


        #region Rasterizer parameters
        private int depthBias = 0;
        public int DepthBias
        {
            set
            {
                if(Set(ref depthBias, value))
                {
                    OnRasterStateChanged();
                }
            }
            get { return depthBias; }
        }

        private float slopScaledDepthBias = 0;

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

        public bool IsMSAAEnabled
        {
            get { return isMSAAEnabled = true; }
            set
            {
                if(Set(ref isMSAAEnabled, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private bool isScissorEnabled = true;

        public bool IsScissorEnabled
        {
            get { return isScissorEnabled; }
            set
            {
                if(Set(ref isScissorEnabled, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private FillMode fillMode = FillMode.Solid;

        public FillMode FillMode
        {
            get { return fillMode; }
            set
            {
                if(Set(ref fillMode, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private bool isDepthClipEnabled = true;
        public bool IsDepthClipEnabled
        {
            get { return isDepthClipEnabled; }
            set
            {
                if(Set(ref isDepthClipEnabled, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        #endregion
        public bool EnableViewFrustumCheck
        {
            set; get;
        } = true;

        private string postEffects;

        public string PostEffects
        {
            get { return postEffects; }
            set
            {
                if(Set(ref postEffects, value))
                {
                    var core = RenderCore;
                    core.ClearPostEffect();
                    if (value is string effects)
                    {
                        if (!string.IsNullOrEmpty(effects))
                        {
                            foreach (var effect in EffectAttributes.Parse(effects))
                            {
                                core.AddPostEffect(effect);
                            }
                        }
                    }
                }
            }
        }

        #endregion

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
        protected virtual IGeometryBufferProxy OnCreateBufferModel(Guid modelGuid, Geometry3D geometry)
        {
            return GeometryBufferProxy<EmptyGeometryBufferModel>.Empty;
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
        /// <param name="geometry">The geometry.</param>
        protected virtual void OnGeometryChanged(Geometry3D geometry)
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
                BufferModelInternal = OnCreateBufferModel(this.GUID, geometry);
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

        protected override void OnAttached()
        {
            OnRasterStateChanged();
            base.OnAttached();
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
            BufferModelInternal = null;
            InstanceBuffer.DisposeAndClear();
            BoundManager.DisposeAndClear();
            base.OnDetach();
        }

        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool CanRender(IRenderContext context)
        {
            if (base.CanRender(context) && GeometryValid && (!(context.EnableBoundingFrustum && EnableViewFrustumCheck)
                || CheckBoundingFrustum(context.BoundingFrustum)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks the bounding frustum.
        /// </summary>
        /// <param name="viewFrustum">The view frustum.</param>
        /// <returns></returns>
        protected virtual bool CheckBoundingFrustum(BoundingFrustum viewFrustum)
        {
            if (!HasBound)
            {
                return true;
            }
            var bound = BoundsWithTransform;
            var sphere = BoundsSphereWithTransform;
            return viewFrustum.Intersects(ref bound) && viewFrustum.Intersects(ref sphere);
        }

        /// <summary>
        /// 
        /// </summary>        
        public override bool HitTest(IRenderContext context, Ray rayWS, ref List<HitTestResult> hits)
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
                        if (OnHitTest(context, TotalModelMatrix * modelMatrix, ref rayWS, ref hits))
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
        protected override bool CanHitTest(IRenderContext context)
        {
            return base.CanHitTest(context) && GeometryValid;
        }

        public override void UpdateNotRender(IRenderContext context)
        {
            base.UpdateNotRender(context);
            if (IsHitTestVisible && context.AutoUpdateOctree && geometry != null && geometry.OctreeDirty)
            {
                geometry?.UpdateOctree();
            }
        }
    }
}
