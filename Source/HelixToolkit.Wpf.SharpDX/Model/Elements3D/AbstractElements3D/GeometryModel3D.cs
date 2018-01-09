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
    using Core;

    /// <summary>
    /// Provides a base class for a scene model which contains geometry
    /// </summary>
    public abstract class GeometryModel3D : Element3D, IHitable, IThrowingShadow, ISelectable, IMouse3D
    {
        #region DependencyProperties
        public static readonly DependencyProperty ReuseVertexArrayBufferProperty =
            DependencyProperty.Register("ReuseVertexArrayBuffer", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(false));

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry3D), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(null, GeometryChanged));

        public static readonly DependencyProperty DepthBiasProperty =
            DependencyProperty.Register("DepthBias", typeof(int), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(0, RasterStateChanged));

        public static readonly DependencyProperty SlopeScaledDepthBiasProperty =
            DependencyProperty.Register("SlopeScaledDepthBias", typeof(double), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(0.0, RasterStateChanged));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(false));

        public static readonly DependencyProperty IsThrowingShadowProperty =
            DependencyProperty.Register("IsThrowingShadow", typeof(bool), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(false, (d, e) => 
            {
                if ((d as Element3D).RenderCore is IThrowingShadow)
                {
                    ((d as Element3D).RenderCore as IThrowingShadow).IsThrowingShadow = (bool)e.NewValue;
                }
            }));

        public static readonly DependencyProperty IsMultisampleEnabledProperty =
            DependencyProperty.Register("IsMultisampleEnabled", typeof(bool), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(true, RasterStateChanged));

        public static readonly DependencyProperty FillModeProperty = DependencyProperty.Register("FillMode", typeof(FillMode), typeof(GeometryModel3D),
            new AffectsRenderPropertyMetadata(FillMode.Solid, RasterStateChanged));

        public static readonly DependencyProperty IsScissorEnabledProperty =
            DependencyProperty.Register("IsScissorEnabled", typeof(bool), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(true, RasterStateChanged));

        public static readonly DependencyProperty IsDepthClipEnabledProperty = DependencyProperty.Register("IsDepthClipEnabled", typeof(bool), typeof(GeometryModel3D),
            new AffectsRenderPropertyMetadata(true, RasterStateChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsHitTestVisibleProperty =
            DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(true, (d, e) =>
            {
                (d as GeometryModel3D).geometryModelCore.IsHitTestVisible = (bool)e.NewValue;
            }));

        /// <summary>
        /// List of instance matrix.
        /// </summary>
        public static readonly DependencyProperty InstancesProperty =
            DependencyProperty.Register("Instances", typeof(IList<Matrix>), typeof(GeometryModel3D), new AffectsRenderPropertyMetadata(null, InstancesChanged));
        /// <summary>
        /// 
        /// </summary>
        public bool IsHitTestVisible
        {
            set
            {
                SetValue(IsHitTestVisibleProperty, value);
            }
            get
            {
                return (bool)GetValue(IsHitTestVisibleProperty);
            }
        }

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

        public bool IsThrowingShadow
        {
            set
            {
                SetValue(IsThrowingShadowProperty, value);
            }
            get
            {
                return (bool)GetValue(IsThrowingShadowProperty);
            }
        }
        /// <summary>
        /// List of instance matrix. 
        /// </summary>
        public IList<Matrix> Instances
        {
            get { return (IList<Matrix>)this.GetValue(InstancesProperty); }
            set { this.SetValue(InstancesProperty, value); }
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
            model.GeometryInternal = e.NewValue == null ? null : e.NewValue as Geometry3D;
            model.InvalidateRender();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void InstancesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = (GeometryModel3D)d;
            var instance = e.NewValue == null ? null : e.NewValue as IList<Matrix>;
            if(model.InstanceBuffer != null)
            {
                model.InstanceBuffer.Elements = instance;
            }
            model.geometryModelCore.Instances = instance;

        }
        #endregion
        public delegate RasterizerStateDescription CreateRasterStateFunc();
        public bool HasInstances { get { return InstanceBuffer.HasElements; } }
        protected IElementsBufferModel<Matrix> InstanceBuffer;
        /// <summary>
        /// Create raster state description delegate.
        /// <para>If <see cref="OnCreateRasterState"/> is set, then <see cref="CreateRasterState"/> will not be called.</para>
        /// </summary>
        public CreateRasterStateFunc OnCreateRasterState;

        #region Properties
        private GeometryModel3DCore geometryModelCore;

        protected IGeometryBufferModel bufferModelInternal;

        protected Geometry3D GeometryInternal
        {
            set
            {
                geometryModelCore.Geometry = value;
                if (bufferModelInternal != null)
                {
                    bufferModelInternal.Geometry = value;                   
                }
            }
            get
            {
                return geometryModelCore.Geometry;
            }
        }

        public bool GeometryValid { get { return geometryModelCore.GeometryValid; } }
        #endregion

        #region Events
        public static readonly RoutedEvent MouseDown3DEvent =
            EventManager.RegisterRoutedEvent("MouseDown3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GeometryModel3D));

        public static readonly RoutedEvent MouseUp3DEvent =
            EventManager.RegisterRoutedEvent("MouseUp3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GeometryModel3D));

        public static readonly RoutedEvent MouseMove3DEvent =
            EventManager.RegisterRoutedEvent("MouseMove3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GeometryModel3D));

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
        public GeometryModel3D(GeometryModel3DCore core) 
            : base(core)
        {
            geometryModelCore = core;
            this.MouseDown3D += OnMouse3DDown;
            this.MouseUp3D += OnMouse3DUp;
            this.MouseMove3D += OnMouse3DMove;
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

        /// <summary>
        /// Overriding OnAttach, use <see cref="CheckGeometry"/> to check if it can be attached.
        /// </summary>
        /// <param name="host"></param>
        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            { 
                bufferModelInternal = OnCreateBufferModel();
                bufferModelInternal.Geometry = GeometryInternal;
                bufferModelInternal.InvalidateRenderer += BufferModel_InvalidateRenderer;
                InstanceBuffer = new MatrixInstanceBufferModel();               
                InstanceBuffer.Initialize();
                InstanceBuffer.Elements = Instances;
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
            bufferModelInternal.InvalidateRenderer -= BufferModel_InvalidateRenderer;
            Disposer.RemoveAndDispose(ref bufferModelInternal);
            Disposer.RemoveAndDispose(ref InstanceBuffer);
            base.OnDetach();
        }

        /// <summary>
        /// <para>base.CanRender(context) &amp;&amp; <see cref="CheckGeometry"/> </para>
        /// <para></para>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool CanRender(IRenderContext context)
        {
            return geometryModelCore.IsVisible;
        }

        public virtual void OnMouse3DDown(object sender, RoutedEventArgs e) { }

        public virtual void OnMouse3DUp(object sender, RoutedEventArgs e) { }

        public virtual void OnMouse3DMove(object sender, RoutedEventArgs e) { }

        /// <summary>
        /// Checks if the ray hits the geometry of the model.
        /// If there a more than one hit, result returns the hit which is nearest to the ray origin.
        /// </summary>
        /// <param name="context">Render context from viewport</param>
        /// <param name="rayWS">Hitring ray from the camera.</param>
        /// <param name="hits">results of the hit.</param>
        /// <returns>True if the ray hits one or more times.</returns>
        public virtual bool HitTest(IRenderContext context, Ray rayWS, ref List<HitTestResult> hits, IRenderable originalSource)
        {
            return geometryModelCore.HitTest(context, rayWS, ref hits, this);
        }

        public static implicit operator GeometryModel3DCore(GeometryModel3D element)
        {
            return element.geometryModelCore;
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
