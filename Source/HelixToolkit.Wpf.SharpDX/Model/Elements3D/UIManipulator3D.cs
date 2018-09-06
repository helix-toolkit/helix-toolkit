// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UIManipulator3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   An abstract base class for manipulators.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;

    using global::SharpDX;

    using HelixToolkit.Wpf.SharpDX.Utilities;

    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using Model;
    /// <summary>
    ///   An abstract base class for manipulators.
    /// </summary>
    public abstract class UIManipulator3D : MeshGeometryModel3D
    {
        protected bool isMouseCaptured;
        protected Viewport3DX viewport;
        protected Vector3 lastHitPosWS, cameraNormal;       

        /// <summary>
        ///   The target transform property. 
        ///   Bind the Tranform of the Target to this Property
        /// </summary>
        public static readonly DependencyProperty TargetTransformProperty = DependencyProperty.Register(
            "TargetTransform", typeof(Transform3D), typeof(UIManipulator3D), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (d,e)=> { (d as Element3DCore).InvalidateRender(); }));

        /// <summary>
        ///   The offset property.
        /// </summary>
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
            "Offset", typeof(Vector3), typeof(UIManipulator3D), new PropertyMetadata(new Vector3(0, 0, 0), ModelChanged));

        /// <summary>
        ///   The value property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(UIManipulator3D), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValueChanged));


        //public static readonly DependencyProperty PositionProperty =
        //    DependencyProperty.Register("Position", typeof(Point3D), typeof(UIManipulator3D), new FrameworkPropertyMetadata(new Point3D()));


        //protected static readonly DependencyPropertyKey PositionPropertyKey =
        //    DependencyProperty.RegisterReadOnly("Position", typeof(Point3D), typeof(UIManipulator3D), new PropertyMetadata(new Point3D()));

        ////public static readonly DependencyProperty PositionProperty = PositionPropertyKey.DependencyProperty;


        /// <summary>
        /// Called when value has been changed.
        /// </summary>
        /// <param name="d">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data. 
        /// </param>
        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var m = d as UIManipulator3D;
            m.OnValueChanged(e);
            m.InvalidateRender();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var m = d as UIManipulator3D;
            m.OnOffetChanged(e);
            m.InvalidateRender();
        }


        ///// <summary>
        ///// The position of the model in world space.
        ///// </summary>
        //public Point3D Position
        //{
        //    get { return (Point3D)this.GetValue(PositionProperty); }
        //    set { this.SetValue(PositionProperty, value); }
        //}


        /// <summary>
        ///   Gets or sets TargetTransform.
        /// </summary>
        public Transform3D TargetTransform
        {
            get{return (Transform3D)this.GetValue(TargetTransformProperty);}
            set{this.SetValue(TargetTransformProperty, value);}
        }

        /// <summary>
        ///   Gets or sets the offset of the visual (this vector is added to the Position point).
        /// </summary>
        /// <value> The offset. </value>
        [TypeConverter(typeof(Vector3Converter))]
        public Vector3 Offset
        {
            get { return (Vector3)this.GetValue(OffsetProperty); }
            set { this.SetValue(OffsetProperty, value); }
        }

        /// <summary>
        ///   Gets or sets the manipulator value.
        /// </summary>
        /// <value> The value. </value>
        public double Value
        {
            get{return (double)this.GetValue(ValueProperty);}
            set{this.SetValue(ValueProperty, value);}
        }

        /// <summary>
        /// Binds this manipulator to a given Model3D.
        /// </summary>
        /// <param name="source">
        /// Source Visual3D which receives the manipulator transforms. 
        /// </param>
        public void Bind(GeometryModel3D source)
        {
            BindingOperations.SetBinding(this, TargetTransformProperty, new Binding("Transform") { Source = source });
            BindingOperations.SetBinding(this, TransformProperty, new Binding("Transform") { Source = source });
        }

        /// <summary>
        ///   Releases the binding of this manipulator.
        /// </summary>
        public void UnBind()
        {
            BindingOperations.ClearBinding(this, TargetTransformProperty);
            BindingOperations.ClearBinding(this, TransformProperty);
        }

        /// <summary>
        /// Called when value is changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected void OnOffetChanged(DependencyPropertyChangedEventArgs e)
        {            
            //var trafo = this.Transform.Value;

            //this.Position = new Point3D(this.Position.X + this.Offset.X, this.Position.Y + this.Offset.Y, this.Position.Z + this.Offset.Z);

            //this.modelMatrix = trafo.ToMatrix();
            //if (this.Geometry != null)
            //{
            //    var b = BoundingBox.FromPoints(this.Geometry.Positions.Select(x => x + this.Offset).ToArray());
            //    //var b = BoundingBox.FromPoints(this.Geometry.Positions);
            //    this.Bounds = b;
            //    //this.BoundsDiameter = (b.Maximum - b.Minimum).Length();
            //}

            //this.OnModelChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnMouse3DDown(object sender, RoutedEventArgs e)
        {
            base.OnMouse3DDown(sender, e);

            var args = e as Mouse3DEventArgs;
            if (args == null) return;
            if (args.Viewport == null) return;

            this.isMouseCaptured = true;
            this.viewport = args.Viewport;
            this.cameraNormal = args.Viewport.Camera.LookDirection.ToVector3();
            this.lastHitPosWS = args.HitTestResult.PointHit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnMouse3DUp(object sender, RoutedEventArgs e)
        {
            base.OnMouse3DUp(sender, e);
            if (this.isMouseCaptured)
            {
                this.isMouseCaptured = false;                
                this.viewport = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnMouse3DMove(object sender, RoutedEventArgs e)
        {
            base.OnMouse3DMove(sender, e);
            if (this.isMouseCaptured)
            {                
                UpdateManipulator(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected abstract void UpdateManipulator(RoutedEventArgs e);

        /// <summary>
        /// Called when Geometry is changed.
        /// </summary>
        /// <param name="d">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data. 
        /// </param>
        protected static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var m = d as UIManipulator3D;
            if (m.IsAttached)
            {
                m.OnModelChanged();
                m.InvalidateRender();
            }
        }

        public UIManipulator3D()
        {
            OnSceneNodeCreated += UIManipulator3D_OnSceneNodeCreated;
        }

        private void UIManipulator3D_OnSceneNodeCreated(object sender, SceneNodeCreatedEventArgs e)
        {
            e.Node.Attached += E_OnAttached;
        }

        private void E_OnAttached(object sender, System.EventArgs e)
        {
            OnModelChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void OnModelChanged();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        protected Vector3 ToWorldPos(Vector3 vec)
        {
            //var m = this.Transform.Value.ToMatrix();
            return Vector3.TransformCoordinate(vec, this.TotalModelMatrix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        protected Vector3 ToWorldVec(Vector3 vec)
        {
            //var m = this.Transform.Value.ToMatrix();
            return Vector3.TransformNormal(vec, this.TotalModelMatrix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        protected Vector3 ToModelPos(Vector3 vec)
        {
            //var m = this.Transform.Value.ToMatrix();
            return Vector3.TransformCoordinate(vec, this.TotalModelMatrix.PsudoInvert());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        protected Vector3 ToModelVec(Vector3 vec)
        {
            //var m = this.Transform.Value.ToMatrix();
            return Vector3.TransformNormal(vec, Matrix.Invert(this.TotalModelMatrix.PsudoInvert()));
        }
    }
}