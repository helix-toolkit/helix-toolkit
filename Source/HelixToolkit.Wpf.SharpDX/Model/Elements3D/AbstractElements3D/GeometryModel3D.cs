// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a base class for a scene model which contains geometry
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using Core;
    using global::SharpDX;
    using global::SharpDX.Direct3D11;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// Provides a base class for a scene model which contains geometry
    /// </summary>
    public abstract class GeometryModel3D : Element3D, IHitable, IBoundable, IThrowingShadow, ISelectable, IMouse3D, IInstancing
    {
        #region DependencyProperties        
        /// <summary>
        /// The geometry property
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry3D), typeof(GeometryModel3D), new PropertyMetadata(null, GeometryChanged));
        /// <summary>
        /// The depth bias property
        /// </summary>
        public static readonly DependencyProperty DepthBiasProperty =
            DependencyProperty.Register("DepthBias", typeof(int), typeof(GeometryModel3D), new PropertyMetadata(0, RasterStateChanged));
        /// <summary>
        /// The slope scaled depth bias property
        /// </summary>
        public static readonly DependencyProperty SlopeScaledDepthBiasProperty =
            DependencyProperty.Register("SlopeScaledDepthBias", typeof(double), typeof(GeometryModel3D), new PropertyMetadata(0.0, RasterStateChanged));
        /// <summary>
        /// The is selected property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(false));
        /// <summary>
        /// The is multisample enabled property
        /// </summary>
        public static readonly DependencyProperty IsMultisampleEnabledProperty =
            DependencyProperty.Register("IsMultisampleEnabled", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(true, RasterStateChanged));
        /// <summary>
        /// The fill mode property
        /// </summary>
        public static readonly DependencyProperty FillModeProperty = DependencyProperty.Register("FillMode", typeof(FillMode), typeof(GeometryModel3D),
            new PropertyMetadata(FillMode.Solid, RasterStateChanged));
        /// <summary>
        /// The is scissor enabled property
        /// </summary>
        public static readonly DependencyProperty IsScissorEnabledProperty =
            DependencyProperty.Register("IsScissorEnabled", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(true, RasterStateChanged));
        /// <summary>
        /// The enable view frustum check property
        /// </summary>
        public static readonly DependencyProperty EnableViewFrustumCheckProperty =
            DependencyProperty.Register("EnableViewFrustumCheck", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(true,
                (d,e)=> { (d as GeometryModel3D).enableViewFrustumCheck = (bool)e.NewValue; }));
        /// <summary>
        /// The is depth clip enabled property
        /// </summary>
        public static readonly DependencyProperty IsDepthClipEnabledProperty = DependencyProperty.Register("IsDepthClipEnabled", typeof(bool), typeof(GeometryModel3D),
            new PropertyMetadata(true, RasterStateChanged));

        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        public Geometry3D Geometry
        {
            get
            {
                return (Geometry3D)this.GetValue(GeometryProperty);
            }
            set
            {
                this.SetValue(GeometryProperty, value);
            }
        }

        /// <summary>
        /// List of instance matrix.
        /// </summary>
        public static readonly DependencyProperty InstancesProperty =
            DependencyProperty.Register("Instances", typeof(IList<Matrix>), typeof(GeometryModel3D), new PropertyMetadata(null, InstancesChanged));

        /// <summary>
        /// List of instance matrix. 
        /// </summary>
        public IList<Matrix> Instances
        {
            get { return (IList<Matrix>)this.GetValue(InstancesProperty); }
            set { this.SetValue(InstancesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the depth bias.
        /// </summary>
        /// <value>
        /// The depth bias.
        /// </value>
        public int DepthBias
        {
            get
            {
                return (int)this.GetValue(DepthBiasProperty);
            }
            set
            {
                this.SetValue(DepthBiasProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the slope scaled depth bias.
        /// </summary>
        /// <value>
        /// The slope scaled depth bias.
        /// </value>
        public double SlopeScaledDepthBias
        {
            get
            {
                return (double)this.GetValue(SlopeScaledDepthBiasProperty);
            }
            set
            {
                this.SetValue(SlopeScaledDepthBiasProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(IsSelectedProperty);
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Only works under FillMode = Wireframe. MSAA is determined by viewport MSAA settings for FillMode = Solid
        /// </summary>
        public bool IsMultisampleEnabled
        {
            set
            {
                SetValue(IsMultisampleEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsMultisampleEnabledProperty);
            }
        }
        /// <summary>
        /// Gets or sets the fill mode.
        /// </summary>
        /// <value>
        /// The fill mode.
        /// </value>
        public FillMode FillMode
        {
            set
            {
                SetValue(FillModeProperty, value);
            }
            get
            {
                return (FillMode)GetValue(FillModeProperty);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is scissor enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is scissor enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsScissorEnabled
        {
            set
            {
                SetValue(IsScissorEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsScissorEnabledProperty);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is depth clip enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is depth clip enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDepthClipEnabled
        {
            set
            {
                SetValue(IsDepthClipEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsDepthClipEnabledProperty);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable view frustum check].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable view frustum check]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableViewFrustumCheck
        {
            set
            {
                SetValue(EnableViewFrustumCheckProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableViewFrustumCheckProperty);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable view frustum check].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable view frustum check]; otherwise, <c>false</c>.
        /// </value>
        protected bool enableViewFrustumCheck { private set; get; } = true;
        #endregion

        #region Static Methods        
        /// <summary>
        /// Rasters the state changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void RasterStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GeometryModel3D)d).OnRasterStateChanged();
        }
        /// <summary>
        /// Geometries the changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = d as GeometryModel3D;
            model.GeometryInternal = e.NewValue == null ? null : e.NewValue as Geometry3D;
        }

        /// <summary>
        /// 
        /// </summary>
        private static void InstancesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (GeometryModel3D)d;
            model.BoundManager.Instances = model.InstanceBuffer.Elements = e.NewValue == null ? null : e.NewValue as IList<Matrix>;            
            model.InstancesChanged();
        }
        #endregion        
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

        #region Properties       

        private IGeometryBufferModel bufferModelInternal;
        /// <summary>
        /// The buffer model internal
        /// </summary>
        protected IGeometryBufferModel BufferModelInternal
        {
            set
            {
                if (bufferModelInternal == value)
                { return; }
                if (bufferModelInternal != null)
                {
                    bufferModelInternal.DetachRenderHost(RenderHost);
                }
                bufferModelInternal = value;
                if (bufferModelInternal != null)
                {
                    bufferModelInternal.AttachRenderHost(RenderHost);
                    ((IGeometryRenderCore)RenderCore).GeometryBuffer = bufferModelInternal;
                }
            }
            get
            {
                return bufferModelInternal;
            }
        }

        private Geometry3D geometryInternal;
        /// <summary>
        /// Gets or sets the geometry internal.
        /// </summary>
        /// <value>
        /// The geometry internal.
        /// </value>
        protected Geometry3D GeometryInternal
        {
            set
            {
                if(geometryInternal == value)
                {
                    return;
                }
                if (IsAttached)
                {
                    OnUnregisterBufferModel(this.GUID, geometryInternal);
                }
                geometryInternal = value;
                if (IsAttached)
                {
                    BoundManager.Geometry = value;
                    BufferModelInternal = OnCreateBufferModel(this.GUID, geometryInternal);
                }
            }
            get
            {
                return geometryInternal;
            }
        }

        public bool GeometryValid { get { return BoundManager.GeometryValid; } }
        public GeometryBoundManager BoundManager { private set; get; }

        public override BoundingBox Bounds
        {
            get { return BoundManager.Bounds; }
        }

        public override BoundingBox BoundsWithTransform
        {
            get { return BoundManager.BoundsWithTransform; }
        }

        public override BoundingSphere BoundsSphere
        {
            get
            {
                return BoundManager.BoundsSphere;
            }
        }

        public override BoundingSphere BoundsSphereWithTransform
        {
            get
            {
                return BoundManager.BoundsSphereWithTransform;
            }
        }

        #endregion        
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryModel3D"/> class.
        /// </summary>
        public GeometryModel3D()
        {
            BoundManager = new GeometryBoundManager(this);
            BoundManager.OnBoundChanged += (s, e) => { RaiseOnBoundChanged(e); };
            BoundManager.OnTransformBoundChanged += (s, e) => { RaiseOnTransformBoundChanged(e); };
            BoundManager.OnBoundSphereChanged += (s, e) => { RaiseOnBoundSphereChanged(e); };
            BoundManager.OnTransformBoundSphereChanged += (s, e) => { RaiseOnTransformBoundSphereChanged(e); };
            BoundManager.OnCheckGeometry = OnCheckGeometry;
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
        protected virtual IGeometryBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D geometry) { return new EmptyGeometryBufferModel(); }

        protected virtual void OnUnregisterBufferModel(Guid modelGuid, Geometry3D geometry) { }

        /// <summary>
        /// Called when [raster state changed].
        /// </summary>
        protected virtual void OnRasterStateChanged()
        {
            if (RenderCore is IGeometryRenderCore)
            {
                (RenderCore as IGeometryRenderCore).RasterDescription = OnCreateRasterState != null ? OnCreateRasterState() : CreateRasterState();
            }
        }

        /// <summary>
        /// Create raster state description.
        /// <para>If <see cref="OnCreateRasterState"/> is set, then <see cref="OnCreateRasterState"/> instead of <see cref="CreateRasterState"/> will be called.</para>
        /// </summary>
        /// <returns></returns>
        protected abstract RasterizerStateDescription CreateRasterState();

        /// <summary>
        /// To override Attach routine, please override this.
        /// </summary>
        /// <param name="host"></param>
        /// <returns>
        /// Return true if attached
        /// </returns>
        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                BufferModelInternal = OnCreateBufferModel(this.GUID, geometryInternal);
                BoundManager.Geometry = GeometryInternal;
                InstanceBuffer.Initialize();
                InstanceBuffer.Elements = this.Instances;
                if (RenderCore is IGeometryRenderCore)
                {                    
                    ((IGeometryRenderCore)RenderCore).InstanceBuffer = InstanceBuffer;
                }
                OnRasterStateChanged();
                return true;
            }
            else
            {
                return false;
            }
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
            OnUnregisterBufferModel(this.GUID, geometryInternal);
            BufferModelInternal = null;
            InstanceBuffer.DisposeAndClear();
            BoundManager.DisposeAndClear();
            base.OnDetach();
        }
        /// <summary>
        /// <para>Determine if this can be rendered.</para>
        /// <para>Default returns <see cref="IsAttached" /> &amp;&amp; <see cref="IsRendering" /> &amp;&amp; <see cref="Visibility" /> == <see cref="Visibility.Visible" /></para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool CanRender(IRenderContext context)
        {
            if (base.CanRender(context) && GeometryValid && (!(context.EnableBoundingFrustum && enableViewFrustumCheck)
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
    }


    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public sealed class BoundChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new bound
        /// </summary>
        public readonly BoundingBox NewBound;
        /// <summary>
        /// The old bound
        /// </summary>
        public readonly BoundingBox OldBound;
        /// <summary>
        /// Initializes a new instance of the <see cref="BoundChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newBound">The new bound.</param>
        /// <param name="oldBound">The old bound.</param>
        public BoundChangedEventArgs(BoundingBox newBound, BoundingBox oldBound)
        {
            NewBound = newBound;
            OldBound = oldBound;
        }
    }
}
