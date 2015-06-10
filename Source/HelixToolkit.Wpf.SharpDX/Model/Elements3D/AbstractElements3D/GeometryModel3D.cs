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

    /// <summary>
    /// Provides a base class for a scene model which contains geometry
    /// </summary>
    public abstract class GeometryModel3D : Model3D, IHitable, IBoundable, IVisible, IThrowingShadow, ISelectable, IMouse3D
    {
        protected RasterizerState rasterState;

        public Geometry3D Geometry
        {
            get { return (Geometry3D)this.GetValue(GeometryProperty); }
            set
            {
                this.SetValue(GeometryProperty, value);
            }
        }

        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry3D), typeof(GeometryModel3D), new UIPropertyMetadata(GeometryChanged));

        protected static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GeometryModel3D)d).OnGeometryChanged(e);
        }

        protected virtual void OnGeometryChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Geometry == null)
            {
                this.Bounds = new BoundingBox();
                return;
            }            

            //var m = this.Transform.ToMatrix();
            //var b = BoundingBox.FromPoints(this.Geometry.Positions.Select(x => Vector3.TransformCoordinate(x, m)).ToArray());
            var b = BoundingBox.FromPoints(this.Geometry.Positions.Array);
            
            //var b = BoundingBox.FromPoints(this.Geometry.Positions);
            //b.Minimum = Vector3.TransformCoordinate(b.Minimum, m);
            //b.Maximum = Vector3.TransformCoordinate(b.Maximum, m);
            this.Bounds = b;
            //this.BoundsDiameter = (b.Maximum - b.Minimum).Length();

            if (this.IsAttached)
            {
                var host = this.renderHost;
                this.Detach();
                this.Attach(host);
            }
        }

        protected override void OnTransformChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnTransformChanged(e);            
            if (this.Geometry != null)
            {                
                //var b = BoundingBox.FromPoints(this.Geometry.Positions.Select(x => Vector3.TransformCoordinate(x, this.modelMatrix)).ToArray());
                var b = BoundingBox.FromPoints(this.Geometry.Positions.Array);
                this.Bounds = b;
                //this.BoundsDiameter = (b.Maximum - b.Minimum).Length();
            }
        }

        public BoundingBox Bounds
        {
            get { return (BoundingBox)this.GetValue(BoundsProperty); }
            protected set { this.SetValue(BoundsPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey BoundsPropertyKey =
            DependencyProperty.RegisterReadOnly("Bounds", typeof(BoundingBox), typeof(GeometryModel3D), new UIPropertyMetadata(new BoundingBox()));

        public static readonly DependencyProperty BoundsProperty = BoundsPropertyKey.DependencyProperty;

        public int DepthBias
        {
            get { return (int)this.GetValue(DepthBiasProperty); }
            set { this.SetValue(DepthBiasProperty, value); }
        }

        public static readonly DependencyProperty DepthBiasProperty =
            DependencyProperty.Register("DepthBias", typeof(int), typeof(GeometryModel3D), new UIPropertyMetadata(0, RasterStateChanged));

        private static void RasterStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GeometryModel3D)d).OnRasterStateChanged((int)e.NewValue);
        }

        protected virtual void OnRasterStateChanged(int depthBias) { }

        public static readonly RoutedEvent MouseDown3DEvent =
            EventManager.RegisterRoutedEvent("MouseDown3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Model3D));

        public static readonly RoutedEvent MouseUp3DEvent =
            EventManager.RegisterRoutedEvent("MouseUp3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Model3D));

        public static readonly RoutedEvent MouseMove3DEvent =
            EventManager.RegisterRoutedEvent("MouseMove3D", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Model3D));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(GeometryModel3D), new UIPropertyMetadata(IsSelectedChanged));

        protected static void IsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GeometryModel3D)d).OnIsSelectedChanged(e);
        }

        protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsAttached)
            {
                var host = this.renderHost;
                Detach();
                Attach(host);
            }
        }

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
            //count++;
        }

        ~GeometryModel3D()
        {
            //this.Dispose();
            //this.MouseDown3D -= OnMouse3DDown;
            //this.MouseUp3D -= OnMouse3DUp;
            //this.MouseMove3D -= OnMouse3DMove;
        }

        //static ulong count = 0;

        public virtual void OnMouse3DDown(object sender, RoutedEventArgs e) { }

        public virtual void OnMouse3DUp(object sender, RoutedEventArgs e) { }

        public virtual void OnMouse3DMove(object sender, RoutedEventArgs e) { }

        /// <summary>
        /// Checks if the ray hits the geometry of the model.
        /// If there a more than one hit, result returns the hit which is nearest to the ray origin.
        /// </summary>
        /// <param name="rayWS">Hitring ray from the camera.</param>
        /// <param name="result">results of the hit.</param>
        /// <returns>True if the ray hits one or more times.</returns>
        public virtual bool HitTest(Ray rayWS, ref List<HitTestResult> hits)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                return false;
            }
            if (this.IsHitTestVisible == false)
            {
                return false;
            }

            var g = this.Geometry as MeshGeometry3D;
            var h = false;
            var result = new HitTestResult();
            result.Distance = double.MaxValue;

            if (g != null)
            {
                var m = this.modelMatrix;
                // put bounds to world space
                var b = BoundingBox.FromPoints(this.Geometry.Positions.Select(x => Vector3.TransformCoordinate(x, m)).ToArray());
                //var b = this.Bounds;
    
                // this all happens now in world space now:
                if (rayWS.Intersects(ref b))
                {
                    foreach (var t in g.Triangles)
                    {
                        float d;
                        var p0 = Vector3.TransformCoordinate(t.P0, m);
                        var p1 = Vector3.TransformCoordinate(t.P1, m);
                        var p2 = Vector3.TransformCoordinate(t.P2, m);
                        if (Collision.RayIntersectsTriangle(ref rayWS, ref p0, ref p1, ref p2, out d))
                        {
                            if (d < result.Distance) // If d is NaN, the condition is false.
                            {
                                result.IsValid = true;
                                result.ModelHit = this;
                                // transform hit-info to world space now:
                                result.PointHit = (rayWS.Position + (rayWS.Direction * d)).ToPoint3D();
                                result.Distance = d;

                                var n = Vector3.Cross(p1 - p0, p2 - p0);
                                n.Normalize();
                                // transform hit-info to world space now:
                                result.NormalAtHit = n.ToVector3D();// Vector3.TransformNormal(n, m).ToVector3D();
                                h = true;
                            }
                        }
                    }
                }
            }
            if (h)
            {
                hits.Add(result);                
            }
            return h;
        }
        
        /*
        public virtual bool HitTestMS(Ray rayWS, ref List<HitTestResult> hits)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                return false;
            }

            var result = new HitTestResult();
            result.Distance = double.MaxValue;
            var g = this.Geometry as MeshGeometry3D;
            var h = false;

            if (g != null)
            {
                var m = this.modelMatrix;
                var mi = Matrix.Invert(m);

                // put the ray to model space
                var rayMS = new Ray(Vector3.TransformNormal(rayWS.Direction, mi), Vector3.TransformCoordinate(rayWS.Position, mi));

                // bounds are in model space
                var b = this.Bounds;

                // this all happens now in model space now:
                if (rayMS.Intersects(ref b))
                {
                    foreach (var t in g.Triangles)
                    {
                        float d;
                        var p0 = t.P0;
                        var p1 = t.P1;
                        var p2 = t.P2;
                        if (Collision.RayIntersectsTriangle(ref rayMS, ref p0, ref p1, ref p2, out d))
                        {
                            if (d < result.Distance)
                            {
                                result.IsValid = true;
                                result.ModelHit = this;
                                // transform hit-info to world space now:
                                result.PointHit = Vector3.TransformCoordinate((rayMS.Position + (rayMS.Direction * d)), m).ToPoint3D();
                                result.Distance = d;

                                var n = Vector3.Cross(p1 - p0, p2 - p0);
                                n.Normalize();
                                // transform hit-info to world space now:
                                result.NormalAtHit = Vector3.TransformNormal(n, m).ToVector3D();
                            }
                            h = true;
                        }
                    }
                }
            }

            if (h)
            {
                result.IsValid = h;
                hits.Add(result);
            }
            return h;
        }
        */

        public bool IsThrowingShadow
        {
            get;
            set;
        }

        public bool IsSelected
        {
            get { return (bool)this.GetValue(IsSelectedProperty); }
            set { this.SetValue(IsSelectedProperty, value); }
        }
    }

    public abstract class Mouse3DEventArgs : RoutedEventArgs
    {
        public HitTestResult HitTestResult { get; private set; }
        public Viewport3DX Viewport { get; private set; }
        public Point Position { get; private set; }

        public Mouse3DEventArgs(RoutedEvent routedEvent, object source, HitTestResult hitTestResult, Point position,  Viewport3DX viewport = null)
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
}