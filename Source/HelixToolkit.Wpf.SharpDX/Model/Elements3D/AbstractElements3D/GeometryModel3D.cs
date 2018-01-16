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
        /// The reuse vertex array buffer property
        /// </summary>
        public static readonly DependencyProperty ReuseVertexArrayBufferProperty =
            DependencyProperty.Register("ReuseVertexArrayBuffer", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(false,
                (d,e)=> {
                    (d as GeometryModel3D).reuseVertexArrayBuffer = (bool)e.NewValue;
                }));
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
        /// Reuse previous vertext array buffer during CreateBuffer. Reduce excessive memory allocation during rapid geometry model changes. 
        /// Example: Repeatly updates textures, or geometries with close number of vertices.
        /// </summary>
        public bool ReuseVertexArrayBuffer
        {
            set
            {
                SetValue(ReuseVertexArrayBufferProperty, value);
            }
            get
            {
                return (bool)GetValue(ReuseVertexArrayBufferProperty);
            }
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
        protected static void RasterStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GeometryModel3D)d).OnRasterStateChanged();
        }

        protected static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = d as GeometryModel3D;
            if (e.OldValue != null)
            {
                (e.OldValue as INotifyPropertyChanged).PropertyChanged -= model.OnGeometryPropertyChangedPrivate;
            }
            if (e.NewValue != null)
            {
                (e.NewValue as INotifyPropertyChanged).PropertyChanged -= model.OnGeometryPropertyChangedPrivate;
                (e.NewValue as INotifyPropertyChanged).PropertyChanged += model.OnGeometryPropertyChangedPrivate;
            }
            model.GeometryInternal = e.NewValue == null ? null : e.NewValue as Geometry3D;
            model.OnGeometryChanged(e);
            model.InvalidateRender();
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

        public bool HasInstances { get { return InstanceBuffer.HasElements; } }
        public IElementsBufferModel<Matrix> InstanceBuffer { get; } = new MatrixInstanceBufferModel();
        protected virtual void InstancesChanged() { }
        protected bool reuseVertexArrayBuffer = false;
        public delegate RasterizerStateDescription CreateRasterStateFunc();

        /// <summary>
        /// Create raster state description delegate.
        /// <para>If <see cref="OnCreateRasterState"/> is set, then <see cref="CreateRasterState"/> will not be called.</para>
        /// </summary>
        public CreateRasterStateFunc OnCreateRasterState;

        #region Properties
        protected IGeometryBufferModel bufferModelInternal;

        private Geometry3D geometryInternal;
        protected Geometry3D GeometryInternal
        {
            set
            {
                geometryInternal = value;
                if (bufferModelInternal != null)
                {
                    BoundManager.Geometry = bufferModelInternal.Geometry = value;                   
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

        public GeometryModel3D()
        {
            BoundManager = new GeometryBoundManager(this);
            BoundManager.OnBoundChanged += (s, e) => { RaiseOnBoundChanged(e); };
            BoundManager.OnTransformBoundChanged += (s, e) => { RaiseOnTransformBoundChanged(e); };
            BoundManager.OnBoundSphereChanged += (s, e) => { RaiseOnBoundSphereChanged(e); };
            BoundManager.OnTransformBoundSphereChanged += (s, e) => { RaiseOnTransformBoundSphereChanged(e); };
            BoundManager.OnCheckGeometry = OnCheckGeometry;
        }

        protected virtual bool OnCheckGeometry(Geometry3D geometry)
        {
            return !(geometry == null || geometry.Positions == null || geometry.Positions.Count == 0);
        }

        protected virtual IGeometryBufferModel OnCreateBufferModel() { return new EmptyGeometryBufferModel(); }

        /// <summary>
        /// Make sure to check if <see cref="Element3D.IsAttached"/> == true
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

        protected virtual void OnGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private void OnGeometryPropertyChangedPrivate(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsAttached)
            {
                if (GeometryValid)
                {
                    OnGeometryPropertyChanged(sender, e);
                }
            }
        }

        protected virtual void OnGeometryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Overriding OnAttach, use <see cref="CheckGeometry"/> to check if it can be attached.
        /// </summary>
        /// <param name="host"></param>
        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                AttachOnGeometryPropertyChanged();
                bufferModelInternal = OnCreateBufferModel();
                BoundManager.Geometry = bufferModelInternal.Geometry = GeometryInternal;
                bufferModelInternal.InvalidateRenderer += BufferModel_InvalidateRenderer;
                InstanceBuffer.Initialize();
                InstanceBuffer.Elements = this.Instances;
                if (RenderCore is IGeometryRenderCore)
                {
                    ((IGeometryRenderCore)RenderCore).GeometryBuffer = bufferModelInternal;
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

        protected override void OnDetach()
        {
            DetachOnGeometryPropertyChanged();
            InstanceBuffer.DisposeAndClear();
            Disposer.RemoveAndDispose(ref bufferModelInternal);
            base.OnDetach();
        }

        private void AttachOnGeometryPropertyChanged()
        {
            if (GeometryInternal != null)
            {
                GeometryInternal.PropertyChanged -= OnGeometryPropertyChangedPrivate;
                GeometryInternal.PropertyChanged += OnGeometryPropertyChangedPrivate;
            }
        }

        private void DetachOnGeometryPropertyChanged()
        {
            if (GeometryInternal != null)
            {
                GeometryInternal.PropertyChanged -= OnGeometryPropertyChangedPrivate;
            }
        }

        /// <summary>
        /// <para>base.CanRender(context) &amp;&amp; <see cref="CheckGeometry"/> </para>
        /// <para></para>
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
                        // this.PushMatrix(modelMatrix);
                        if (OnHitTest(context, TotalModelMatrix * modelMatrix, ref rayWS, ref hits))
                        {
                            hit = true;
                            var lastHit = hits[hits.Count - 1];
                            lastHit.Tag = idx;
                            hits[hits.Count - 1] = lastHit;
                        }
                       // this.PopMatrix();
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

        protected override bool CanHitTest(IRenderContext context)
        {
            return base.CanHitTest(context) && GeometryValid;
        }
    }



    public sealed class BoundChangedEventArgs : EventArgs
    {
        public readonly BoundingBox NewBound;
        public readonly BoundingBox OldBound;
        public BoundChangedEventArgs(BoundingBox newBound, BoundingBox oldBound)
        {
            NewBound = newBound;
            OldBound = oldBound;
        }
    }
}
