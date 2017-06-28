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
    using System.Linq;
    using System.Windows;
    using System.Collections.Generic;

    using global::SharpDX;
    using global::SharpDX.Direct3D11;

    using Point = System.Windows.Point;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides a base class for a scene model which contains geometry
    /// </summary>
    public abstract class GeometryModel3D : Model3D, IHitable, IBoundable, IVisible, IThrowingShadow, ISelectable, IMouse3D
    {
        #region DependencyProperties
        public static readonly DependencyProperty ReuseVertexArrayBufferProperty =
            DependencyProperty.Register("ReuseVertexArrayBuffer", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(false));

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry3D), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(null, GeometryChanged));

        public static readonly DependencyProperty DepthBiasProperty =
            DependencyProperty.Register("DepthBias", typeof(int), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(0, RasterStateChanged));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(DraggableGeometryModel3D), new AffectsRenderPropertyMetadata(false));

        public static readonly DependencyProperty IsMultisampleEnabledProperty =
            DependencyProperty.Register("IsMultisampleEnabled", typeof(bool), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(true, RasterStateChanged));

        public static readonly DependencyProperty FillModeProperty = DependencyProperty.Register("FillMode", typeof(FillMode), typeof(GeometryModel3D),
            new AffectsRenderPropertyMetadata(FillMode.Solid, RasterStateChanged));

        public static readonly DependencyProperty IsScissorEnabledProperty =
            DependencyProperty.Register("IsScissorEnabled", typeof(bool), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(true, RasterStateChanged));

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
            model.geometryInternal = e.NewValue == null ? null : e.NewValue as Geometry3D;
            model.OnGeometryChanged(e);
        }
        #endregion

        #region Variables
        protected RasterizerState rasterState = null;
        #endregion

        #region Properties
        protected Geometry3D geometryInternal { private set; get; }
        public bool GeometryValid { private set; get; } = false;

        private BoundingBox bounds;
        public BoundingBox Bounds
        {
            get { return bounds; }
            protected set
            {
                if (bounds != value)
                {
                    var old = bounds;
                    bounds = value;
                    RaiseOnBoundChanged(value, old);
                    BoundsWithTransform = Transform == null ? bounds : bounds.Transform(this.modelMatrix);
                }
            }
        }

        private BoundingBox boundsWithTransform;
        public BoundingBox BoundsWithTransform
        {
            get { return boundsWithTransform; }
            private set
            {
                if (boundsWithTransform != value)
                {
                    var old = boundsWithTransform;
                    boundsWithTransform = value;
                    RaiseOnTransformBoundChanged(value, old);
                }
            }
        }

        private BoundingSphere boundsSphere;
        public BoundingSphere BoundsSphere
        {
            protected set
            {
                if (boundsSphere != value)
                {
                    var old = boundsSphere;
                    boundsSphere = value;
                    RaiseOnBoundSphereChanged(value, old);
                    BoundsSphereWithTransform = value.TransformBoundingSphere(this.modelMatrix);
                }
            }
            get
            {
                return boundsSphere;
            }
        }

        private BoundingSphere boundsSphereWithTransform;
        public BoundingSphere BoundsSphereWithTransform
        {
            private set
            {
                if (boundsSphereWithTransform != value)
                {
                    boundsSphereWithTransform = value;
                }
            }
            get
            {
                return boundsSphereWithTransform;
            }
        }
        #endregion

        #region Events
        public static readonly RoutedEvent MouseDown3DEvent =
            EventManager.RegisterRoutedEvent("MouseDown3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Model3D));

        public static readonly RoutedEvent MouseUp3DEvent =
            EventManager.RegisterRoutedEvent("MouseUp3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Model3D));

        public static readonly RoutedEvent MouseMove3DEvent =
            EventManager.RegisterRoutedEvent("MouseMove3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Model3D));

        public delegate void BoundChangedEventHandler(object sender, ref BoundingBox newBound, ref BoundingBox oldBound);

        public event BoundChangedEventHandler OnBoundChanged;

        public event BoundChangedEventHandler OnTransformBoundChanged;

        public delegate void BoundSphereChangedEventHandler(object sender, ref BoundingSphere newBound, ref BoundingSphere oldBound);

        public event BoundSphereChangedEventHandler OnBoundSphereChanged;

        public event BoundSphereChangedEventHandler OnTransformBoundSphereChanged;


        /// <summary>
        /// Provide CLR accessors for the event 
        /// </summary>
        public event RoutedEventHandler MouseDown3D
        {
            add { AddHandler(MouseDown3DEvent, value); }
            remove { RemoveHandler(MouseDown3DEvent, value); }
        }

        /// <summary>
        /// Provide CLR accessors for the event 
        /// </summary>
        public event RoutedEventHandler MouseUp3D
        {
            add { AddHandler(MouseUp3DEvent, value); }
            remove { RemoveHandler(MouseUp3DEvent, value); }
        }

        /// <summary>
        /// Provide CLR accessors for the event 
        /// </summary>
        public event RoutedEventHandler MouseMove3D
        {
            add { AddHandler(MouseMove3DEvent, value); }
            remove { RemoveHandler(MouseMove3DEvent, value); }
        }
        #endregion
        ///// <summary>
        ///// This method raises the MouseDown3D event 
        ///// </summary>        
        //internal void RaiseMouseDown3DEvent(MouseDown3DEventArgs args)
        //{
        //    this.RaiseEvent(args);
        //}

        ///// <summary>
        ///// This method raises the MouseUp3D event 
        ///// </summary>        
        //internal void RaiseMouseUp3DEvent(MouseUp3DEventArgs args)
        //{
        //    this.RaiseEvent(args);
        //}

        ///// <summary>
        ///// This method raises the MouseMove3D event 
        ///// </summary>        
        //internal void RaiseMouseMove3DEvent(MouseMove3DEventArgs args)
        //{
        //    this.RaiseEvent(args);
        //}
        public GeometryModel3D()
        {
            this.MouseDown3D += OnMouse3DDown;
            this.MouseUp3D += OnMouse3DUp;
            this.MouseMove3D += OnMouse3DMove;
            this.IsThrowingShadow = true;
        }

        /// <summary>
        /// Make sure to check if <see cref="Element3D.IsAttached"/> == true
        /// </summary>
        protected virtual void OnRasterStateChanged() { }

        protected virtual void OnGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            GeometryValid = CheckGeometry();
            if (GeometryValid && renderHost != null)
            {
                if (IsAttached)
                {
                    OnCreateGeometryBuffers();
                }
                else
                {
                    var host = renderHost;
                    Detach();
                    Attach(host);
                }
            }
        }

        protected abstract void OnCreateGeometryBuffers();

        private void OnGeometryPropertyChangedPrivate(object sender, PropertyChangedEventArgs e)
        {
            GeometryValid = CheckGeometry();
            if (this.IsAttached)
            {
                if (e.PropertyName.Equals(nameof(Geometry3D.Bound)))
                {
                    this.Bounds = this.geometryInternal != null ? this.geometryInternal.Bound : new BoundingBox();
                }
                else if (e.PropertyName.Equals(nameof(Geometry3D.BoundingSphere)))
                {
                    this.BoundsSphere = this.geometryInternal != null ? this.geometryInternal.BoundingSphere : new BoundingSphere();
                }
                if (GeometryValid)
                {
                    OnGeometryPropertyChanged(sender, e);
                }
            }
        }

        protected virtual void OnGeometryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        protected override void OnTransformChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnTransformChanged(e);
            if (this.geometryInternal != null)
            {
                BoundsWithTransform = Bounds.Transform(this.modelMatrix);
                BoundsSphereWithTransform = BoundsSphere.TransformBoundingSphere(this.modelMatrix);
            }
            else
            {
                BoundsWithTransform = Bounds;
                BoundsSphereWithTransform = BoundsSphere;
            }
        }

        /// <summary>
        /// <para>Check geometry validity.</para>
        /// Return false if (this.geometryInternal == null || this.geometryInternal.Positions == null || this.geometryInternal.Positions.Count == 0 || this.geometryInternal.Indices == null || this.geometryInternal.Indices.Count == 0)
        /// </summary>
        /// <returns>
        /// </returns>
        protected virtual bool CheckGeometry()
        {
            return !(this.geometryInternal == null || this.geometryInternal.Positions == null || this.geometryInternal.Positions.Count == 0
                || this.geometryInternal.Indices == null || this.geometryInternal.Indices.Count == 0);
        }

        /// <summary>
        /// Overriding OnAttach, use <see cref="CheckGeometry"/> to check if it can be attached.
        /// </summary>
        /// <param name="host"></param>
        protected override bool OnAttach(IRenderHost host)
        {
            if (CheckGeometry())
            {
                AttachOnGeometryPropertyChanged();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (geometryInternal != null)
            {
                this.Bounds = this.geometryInternal.Bound;
                this.BoundsSphere = this.geometryInternal.BoundingSphere;
            }
            OnRasterStateChanged();
        }

        protected override void OnDetach()
        {
            DetachOnGeometryPropertyChanged();
            Disposer.RemoveAndDispose(ref rasterState);
            base.OnDetach();
        }

        private void AttachOnGeometryPropertyChanged()
        {
            if (geometryInternal != null)
            {
                geometryInternal.PropertyChanged -= OnGeometryPropertyChangedPrivate;
                geometryInternal.PropertyChanged += OnGeometryPropertyChangedPrivate;
            }
        }

        private void DetachOnGeometryPropertyChanged()
        {
            if (geometryInternal != null)
            {
                geometryInternal.PropertyChanged -= OnGeometryPropertyChangedPrivate;
            }
        }

        /// <summary>
        /// <para>base.CanRender(context) &amp;&amp; <see cref="CheckGeometry"/> </para>
        /// <para>If RenderContext IsShadowPass=true, return false if <see cref="IsThrowingShadow"/> = false</para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool CanRender(RenderContext context)
        {
            if (context.EnableBoundingFrustum && !CheckBoundingFrustum(ref context.boundingFrustum))
            {
                return false;
            }
            if (base.CanRender(context) && GeometryValid)
            {
                if (context.IsShadowPass)
                    if (!IsThrowingShadow)
                        return false;
                return true;
            }
            else
            {
                return false;
            }
        }
        protected virtual bool CheckBoundingFrustum(ref BoundingFrustum viewFrustum)
        {
            return viewFrustum.Intersects(ref boundsWithTransform);
        }

        public virtual void OnMouse3DDown(object sender, RoutedEventArgs e) { }

        public virtual void OnMouse3DUp(object sender, RoutedEventArgs e) { }

        public virtual void OnMouse3DMove(object sender, RoutedEventArgs e) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RaiseOnTransformBoundChanged(BoundingBox newBound, BoundingBox oldBound)
        {
            OnTransformBoundChanged?.Invoke(this, ref newBound, ref oldBound);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RaiseOnBoundChanged(BoundingBox newBound, BoundingBox oldBound)
        {
            OnBoundChanged?.Invoke(this, ref newBound, ref oldBound);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RaiseOnTransformBoundSphereChanged(BoundingSphere newBoundSphere, BoundingSphere oldBoundSphere)
        {
            OnTransformBoundSphereChanged?.Invoke(this, ref newBoundSphere, ref oldBoundSphere);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RaiseOnBoundSphereChanged(BoundingSphere newBoundSphere, BoundingSphere oldBoundSphere)
        {
            OnBoundSphereChanged?.Invoke(this, ref newBoundSphere, ref oldBoundSphere);
        }
        /// <summary>
        /// Checks if the ray hits the geometry of the model.
        /// If there a more than one hit, result returns the hit which is nearest to the ray origin.
        /// </summary>
        /// <param name="context">Render context from viewport</param>
        /// <param name="rayWS">Hitring ray from the camera.</param>
        /// <param name="hits">results of the hit.</param>
        /// <returns>True if the ray hits one or more times.</returns>
        public virtual bool HitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits)
        {
            if (CanHitTest(context))
            {
                return OnHitTest(context, rayWS, ref hits);
            }
            else
            {
                return false;
            }
        }

        protected abstract bool OnHitTest(IRenderMatrices context, Ray rayWS, ref List<HitTestResult> hits);

        protected virtual bool CanHitTest(IRenderMatrices context)
        {
            return visibleInternal && isRenderingInternal && isHitTestVisibleInternal && GeometryValid;
        }

        public bool IsThrowingShadow
        {
            get;
            set;
        }
    }

    public abstract class Mouse3DEventArgs : RoutedEventArgs
    {
        public HitTestResult HitTestResult { get; private set; }
        public Viewport3DX Viewport { get; private set; }
        public Point Position { get; private set; }

        public Mouse3DEventArgs(RoutedEvent routedEvent, object source, HitTestResult hitTestResult, Point position, Viewport3DX viewport = null)
            : base(routedEvent, source)
        {
            this.HitTestResult = hitTestResult;
            this.Position = position;
            this.Viewport = viewport;
        }
    }

    public class MouseDown3DEventArgs : Mouse3DEventArgs
    {
        public MouseDown3DEventArgs(object source, HitTestResult hitTestResult, Point position, Viewport3DX viewport = null)
            : base(GeometryModel3D.MouseDown3DEvent, source, hitTestResult, position, viewport)
        { }
    }

    public class MouseUp3DEventArgs : Mouse3DEventArgs
    {
        public MouseUp3DEventArgs(object source, HitTestResult hitTestResult, Point position, Viewport3DX viewport = null)
            : base(GeometryModel3D.MouseUp3DEvent, source, hitTestResult, position, viewport)
        { }
    }

    public class MouseMove3DEventArgs : Mouse3DEventArgs
    {
        public MouseMove3DEventArgs(object source, HitTestResult hitTestResult, Point position, Viewport3DX viewport = null)
            : base(GeometryModel3D.MouseMove3DEvent, source, hitTestResult, position, viewport)
        { }
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
