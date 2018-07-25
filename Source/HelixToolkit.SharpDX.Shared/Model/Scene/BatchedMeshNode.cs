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
    using Utilities;

    /// <summary>
    /// Static mesh batching. Supports multiple <see cref="Materials"/>. All geometries are merged into single buffer for rendering. Indivisual material color infomations are encoded into vertex buffer.
    /// <para>
    /// <see cref="Material"/> is used if <see cref="Materials"/> = null. And also used for shared material texture binding.
    /// </para>
    /// </summary>
    public class BatchedMeshNode : SceneNode, IHitable, IThrowingShadow, IBoundable
    {
        #region Properties
        private BatchedMeshGeometryConfig[] geometries;
        public BatchedMeshGeometryConfig[] Geometries
        {
            set
            {
                if(SetAffectsRender(ref geometries, value))
                {
                    if (IsAttached)
                    {
                        batchingBuffer.Geometries = value;
                    }
                    UpdateBounds();
                }
            }
            get
            {
                return geometries;
            }
        }

        private PhongMaterialCore[] materials;
        public PhongMaterialCore[] Materials
        {
            set
            {
                if(SetAffectsRender(ref materials, value) && IsAttached)
                {
                    batchingBuffer.Materials = value;
                    if (value == null && Material is PhongMaterialCore p)
                    {
                        batchingBuffer.Materials = new PhongMaterialCore[] { p };
                    }
                }
            }
            get { return materials; }
        }
        #region Bound
        private BoundingBox originalBounds;
        /// <summary>
        /// Gets the original bound from the geometry. Same as <see cref="Geometry3D.Bound"/>
        /// </summary>
        /// <value>
        /// The original bound.
        /// </value>
        public override BoundingBox OriginalBounds
        {
            get { return originalBounds; }
        }

        private BoundingSphere originalBoundsSphere;
        /// <summary>
        /// Gets the original bound sphere from the geometry. Same as <see cref="Geometry3D.BoundingSphere"/> 
        /// </summary>
        /// <value>
        /// The original bound sphere.
        /// </value>
        public override BoundingSphere OriginalBoundsSphere
        {
            get { return originalBoundsSphere; }
        }

        /// <summary>
        /// Gets the bounds. Usually same as <see cref="OriginalBounds"/>. If have instances, the bound will enclose all instances.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public override BoundingBox Bounds
        {
            get { return originalBounds; }
        }

        private BoundingBox boundsWithTransform;
        /// <summary>
        /// Gets the bounds with transform. Usually same as <see cref="Bounds"/>. If have transform, the bound is the transformed <see cref="Bounds"/>
        /// </summary>
        /// <value>
        /// The bounds with transform.
        /// </value>
        public override BoundingBox BoundsWithTransform
        {
            get { return boundsWithTransform; }
        }

        /// <summary>
        /// Gets the bounds sphere. Usually same as <see cref="OriginalBoundsSphere"/>. If have instances, the bound sphere will enclose all instances.
        /// </summary>
        /// <value>
        /// The bounds sphere.
        /// </value>
        public override BoundingSphere BoundsSphere
        {
            get { return originalBoundsSphere; }
        }

        private BoundingSphere boundsSphereWithTransform;
        /// <summary>
        /// Gets the bounds sphere with transform. If have transform, the bound is the transformed <see cref="BoundsSphere"/>
        /// </summary>
        /// <value>
        /// The bounds sphere with transform.
        /// </value>
        public override BoundingSphere BoundsSphereWithTransform
        {
            get { return boundsSphereWithTransform; }
        }
        #endregion
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
        private bool frontCCW = true;
        /// <summary>
        /// Gets or sets a value indicating whether [front CCW].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [front CCW]; otherwise, <c>false</c>.
        /// </value>
        public bool FrontCCW
        {
            get { return frontCCW; }
            set
            {
                if (Set(ref frontCCW, value))
                {
                    OnRasterStateChanged();
                }
            }
        }

        private CullMode cullMode = CullMode.None;
        /// <summary>
        /// Gets or sets the cull mode.
        /// </summary>
        /// <value>
        /// The cull mode.
        /// </value>
        public CullMode CullMode
        {
            get { return cullMode; }
            set
            {
                if (Set(ref cullMode, value))
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

        /// <summary>
        /// Gets or sets a value indicating whether [invert normal].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [invert normal]; otherwise, <c>false</c>.
        /// </value>
        public bool InvertNormal
        {
            get { return (RenderCore as IMeshRenderParams).InvertNormal; }
            set
            {
                (RenderCore as IMeshRenderParams).InvertNormal = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the wireframe.
        /// </summary>
        /// <value>
        /// The color of the wireframe.
        /// </value>
        public Color4 WireframeColor
        {
            get
            {
                return (RenderCore as IMeshRenderParams).WireframeColor;
            }
            set
            {
                (RenderCore as IMeshRenderParams).WireframeColor = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [render wireframe].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render wireframe]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderWireframe
        {
            get { return (RenderCore as IMeshRenderParams).RenderWireframe; }
            set { (RenderCore as IMeshRenderParams).RenderWireframe = value; }
        }

        private bool isTransparent = false;
        /// <summary>
        /// Specifiy if model material is transparent.
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public bool IsTransparent
        {
            get { return isTransparent; }
            set
            {
                if (Set(ref isTransparent, value))
                {
                    if (RenderType == RenderType.Opaque || RenderType == RenderType.Transparent)
                    {
                        RenderType = value ? RenderType.Transparent : RenderType.Opaque;
                    }
                }
            }
        }

        private MaterialVariable materialVariable;
        private MaterialCore material;
        /// <summary>
        ///
        /// </summary>
        public MaterialCore Material
        {
            get { return material; }
            set
            {
                if (Set(ref material, value))
                {
                    if (RenderHost != null)
                    {
                        if (IsAttached)
                        {
                            AttachMaterial();
                            InvalidateRender();
                        }
                        else
                        {
                            var host = RenderHost;
                            Detach();
                            Attach(host);
                        }
                    }
                }
            }
        }

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

        protected DefaultStaticMeshBatchingBuffer batchingBuffer;

        protected StaticBatchedGeometryBoundsOctree BatchedGeometryOctree { private set; get; }
        #endregion

        public BatchedMeshNode()
        {
            HasBound = true;
            OnTransformChanged += BatchedMeshNode_OnTransformChanged;
        }

        private void BatchedMeshNode_OnTransformChanged(object sender, TransformArgs e)
        {
            UpdateBoundsWithTransform();
        }

        protected override RenderCore OnCreateRenderCore()
        {
            return new MeshRenderCore
            {
                Batched = true
            };
        }

        protected override IRenderTechnique OnCreateRenderTechnique(IRenderHost host)
        {
            return host.EffectsManager[DefaultRenderTechniqueNames.BlinnBatched];
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
        /// Create raster state description.
        /// </summary>
        /// <returns></returns>
        protected virtual RasterizerStateDescription CreateRasterState()
        {
            return new RasterizerStateDescription()
            {
                FillMode = FillMode,
                CullMode = CullMode,
                DepthBias = DepthBias,
                DepthBiasClamp = -1000,
                SlopeScaledDepthBias = (float)SlopeScaledDepthBias,
                IsDepthClipEnabled = IsDepthClipEnabled,
                IsFrontCounterClockwise = FrontCCW,
                IsMultisampleEnabled = IsMSAAEnabled,
                IsScissorEnabled = IsThrowingShadow ? false : IsScissorEnabled,
            };
        }
        /// <summary>
        ///
        /// </summary>
        protected virtual void AttachMaterial()
        {
            RemoveAndDispose(ref materialVariable);
            if (material != null && RenderCore is IMaterialRenderParams core)
            {
                core.MaterialVariables = materialVariable = Collect(EffectsManager.MaterialVariableManager.Register(material, EffectTechnique));
            }            
            if(Materials == null && Material is PhongMaterialCore p)
            {
                batchingBuffer.Materials = new PhongMaterialCore[] { p };
            }
        }

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
                batchingBuffer = Collect(new DefaultStaticMeshBatchingBuffer());
                batchingBuffer.Geometries = Geometries;
                batchingBuffer.Materials = materials;
                if (RenderCore is IGeometryRenderCore r)
                {
                    r.GeometryBuffer = batchingBuffer;                  
                }
                AttachMaterial();
                return true;
            }
            else
            {
                return false;
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

        /// <summary>
        /// Used to override Detach
        /// </summary>
        protected override void OnDetach()
        {
            batchingBuffer = null;
            materialVariable = null;
            if (RenderCore is IMaterialRenderParams core)
            {
                core.MaterialVariables = null;
            }
            base.OnDetach();
        }
        protected override OrderKey OnUpdateRenderOrderKey()
        {
            return OrderKey.Create(RenderOrder, materialVariable == null ? (ushort)0 : materialVariable.ID);
        }
        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool CanRender(RenderContext context)
        {
            if (base.CanRender(context) && Geometries != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpdateBounds()
        {
            var oldBound = originalBounds;
            var oldBoundSphere = originalBoundsSphere;
            if (geometries != null && geometries.Length > 0)
            {
                var b = geometries[0].Geometry.Bound;
                var bs = geometries[0].Geometry.BoundingSphere;
                foreach(var geo in geometries)
                {
                    b = BoundingBox.Merge(b, geo.Geometry.Bound.Transform(geo.ModelTransform));
                    bs = BoundingSphere.Merge(bs, geo.Geometry.BoundingSphere.TransformBoundingSphere(geo.ModelTransform));
                }
                originalBounds = b;
                originalBoundsSphere = bs;
                BatchedGeometryOctree = new StaticBatchedGeometryBoundsOctree(geometries, new OctreeBuildParameter());
            }
            else
            {
                originalBounds = MaxBound;
                originalBoundsSphere = MaxBoundSphere;
                BatchedGeometryOctree = null;
            }

            RaiseOnBoundChanged(new BoundChangeArgs<BoundingBox>(ref originalBounds, ref oldBound));
            RaiseOnBoundSphereChanged(new BoundChangeArgs<BoundingSphere>(ref originalBoundsSphere, ref oldBoundSphere));
            UpdateBoundsWithTransform();
        }       

        private void UpdateBoundsWithTransform()
        {
            var old = boundsWithTransform;
            if (originalBounds == MaxBound)
            {
                boundsWithTransform = MaxBound;
            }
            else
            {               
                boundsWithTransform = originalBounds.Transform(TotalModelMatrix);
            }
            var oldBS = boundsSphereWithTransform;
            if(originalBoundsSphere == MaxBoundSphere)
            {
                boundsSphereWithTransform = MaxBoundSphere;
            }
            else
            {
                boundsSphereWithTransform = originalBoundsSphere.TransformBoundingSphere(TotalModelMatrix);
            }
            RaiseOnTransformBoundChanged(new BoundChangeArgs<BoundingBox>(ref boundsWithTransform, ref old));
            RaiseOnTransformBoundSphereChanged(new BoundChangeArgs<BoundingSphere>(ref boundsSphereWithTransform, ref oldBS));
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
            return BoundingFrustumExtensions.Intersects(ref viewFrustum, ref boundsWithTransform, ref boundsSphereWithTransform);
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
            return base.CanHitTest(context) && Geometries != null && Geometries.Length > 0 && Materials != null && Materials.Length > 0;
        }

        /// <summary>
        /// Updates the not render.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void UpdateNotRender(RenderContext context)
        {
            base.UpdateNotRender(context);
            if (IsHitTestVisible && context.AutoUpdateOctree && Geometries != null)
            {
                foreach (var geometry in Geometries)
                {
                    geometry.Geometry?.UpdateOctree();
                }
            }
            if(BatchedGeometryOctree != null && !BatchedGeometryOctree.TreeBuilt)
            {
                BatchedGeometryOctree.BuildTree();
            }
        }

        protected override bool OnHitTest(RenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        {
            if(ray.Intersects(boundsWithTransform) && ray.Intersects(boundsSphereWithTransform))
            {                
                if(BatchedGeometryOctree != null && BatchedGeometryOctree.TreeBuilt)
                {
                    return BatchedGeometryOctree.HitTest(context, WrapperSource, null, totalModelMatrix, ray, ref hits);
                }
                else
                {
                    bool isHit = false;
                    foreach(var geo in Geometries)
                    {
                        if(geo.Geometry is MeshGeometry3D mesh)
                        {
                            isHit |= mesh.HitTest(context, geo.ModelTransform * totalModelMatrix, ref ray, ref hits, WrapperSource);
                        }
                    }
                    return isHit;
                }            
            }
            else
            {
                return false;
            }
        }
    }
}
