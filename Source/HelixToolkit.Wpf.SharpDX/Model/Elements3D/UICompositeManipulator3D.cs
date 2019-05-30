// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UICompositeManipulator3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   The can rotate x property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Data;

    using global::SharpDX;
    using Model;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;

    public class UICompositeManipulator3D : CompositeModel3D
    {
        private UIRotateManipulator3D rotateX, rotateY, rotateZ;
        private UITranslateManipulator3D translateX, translateY, translateZ;       

        /// <summary>
        /// The can rotate x property.
        /// </summary>
        public static readonly DependencyProperty CanRotateXProperty = DependencyProperty.Register(
            "CanRotateX", typeof(bool), typeof(UICompositeManipulator3D), new PropertyMetadata(true, (d,e)=>
            {
                (d as UICompositeManipulator3D).rotateX.IsRendering = (bool)e.NewValue;
            }));

        /// <summary>
        /// The can rotate y property.
        /// </summary>
        public static readonly DependencyProperty CanRotateYProperty = DependencyProperty.Register(
            "CanRotateY", typeof(bool), typeof(UICompositeManipulator3D), new PropertyMetadata(true, (d, e) =>
            {
                (d as UICompositeManipulator3D).rotateY.IsRendering = (bool)e.NewValue;
            }));

        /// <summary>
        /// The can rotate z property.
        /// </summary>
        public static readonly DependencyProperty CanRotateZProperty = DependencyProperty.Register(
            "CanRotateZ", typeof(bool), typeof(UICompositeManipulator3D), new PropertyMetadata(true, (d, e) =>
            {
                (d as UICompositeManipulator3D).rotateZ.IsRendering = (bool)e.NewValue;
            }));

        /// <summary>
        /// The can translate x property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateXProperty = DependencyProperty.Register(
            "CanTranslateX", typeof(bool), typeof(UICompositeManipulator3D), new PropertyMetadata(true, (d, e) =>
            {
                (d as UICompositeManipulator3D).translateX.IsRendering = (bool)e.NewValue;
            }));

        /// <summary>
        /// The can translate y property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateYProperty = DependencyProperty.Register(
            "CanTranslateY", typeof(bool), typeof(UICompositeManipulator3D), new PropertyMetadata(true, (d, e) =>
            {
                (d as UICompositeManipulator3D).translateY.IsRendering = (bool)e.NewValue;
            }));

        /// <summary>
        /// The can translate z property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateZProperty = DependencyProperty.Register(
            "CanTranslateZ", typeof(bool), typeof(UICompositeManipulator3D), new PropertyMetadata(true, (d, e) =>
            {
                (d as UICompositeManipulator3D).translateZ.IsRendering = (bool)e.NewValue;
            }));

        /// <summary>
        /// The diameter property.
        /// </summary>
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(double), typeof(UICompositeManipulator3D), new PropertyMetadata(2.0, ChildrenChanged));

        /// <summary>
        ///   The target transform property.
        /// </summary>
        public static readonly DependencyProperty TargetTransformProperty = DependencyProperty.Register(
            "TargetTransform", typeof(Transform3D), typeof(UICompositeManipulator3D), 
            new FrameworkPropertyMetadata(Transform3D.Identity, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (d,e)=> { (d as Element3DCore).InvalidateRender(); }));

        /// <summary>
        ///   Gets or sets TargetTransform.
        /// </summary>
        public Transform3D TargetTransform
        {
            get { return (Transform3D)this.GetValue(TargetTransformProperty); }
            set { this.SetValue(TargetTransformProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can rotate X.
        /// </summary>
        /// <value> <c>true</c> if this instance can rotate X; otherwise, <c>false</c> . </value>
        public bool CanRotateX
        {
            get { return (bool)this.GetValue(CanRotateXProperty); }
            set { this.SetValue(CanRotateXProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can rotate Y.
        /// </summary>
        /// <value> <c>true</c> if this instance can rotate Y; otherwise, <c>false</c> . </value>
        public bool CanRotateY
        {
            get { return (bool)this.GetValue(CanRotateYProperty); }
            set { this.SetValue(CanRotateYProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can rotate Z.
        /// </summary>
        /// <value> <c>true</c> if this instance can rotate Z; otherwise, <c>false</c> . </value>
        public bool CanRotateZ
        {
            get { return (bool)this.GetValue(CanRotateZProperty); }
            set { this.SetValue(CanRotateZProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can translate X.
        /// </summary>
        /// <value> <c>true</c> if this instance can translate X; otherwise, <c>false</c> . </value>
        public bool CanTranslateX
        {
            get { return (bool)this.GetValue(CanTranslateXProperty); }
            set { this.SetValue(CanTranslateXProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can translate Y.
        /// </summary>
        /// <value> <c>true</c> if this instance can translate Y; otherwise, <c>false</c> . </value>
        public bool CanTranslateY
        {
            get { return (bool)this.GetValue(CanTranslateYProperty); }
            set { this.SetValue(CanTranslateYProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can translate Z.
        /// </summary>
        /// <value> <c>true</c> if this instance can translate Z; otherwise, <c>false</c> . </value>
        public bool CanTranslateZ
        {
            get { return (bool)this.GetValue(CanTranslateZProperty); }
            set { this.SetValue(CanTranslateZProperty, value); }
        }

        /// <summary>
        /// Gets or sets the diameter.
        /// </summary>
        /// <value> The diameter. </value>
        public double Diameter
        {
            get { return (double)this.GetValue(DiameterProperty); }
            set { this.SetValue(DiameterProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public UICompositeManipulator3D()
        {
            OnSceneNodeCreated += (s, e) =>
            {
                e.Node.Attached += SceneNode_OnAttached;
            };
            this.translateX = new UITranslateManipulator3D { Direction = new Vector3(1, 0, 0), Material = DiffuseMaterials.Red };
            this.translateY = new UITranslateManipulator3D { Direction = new Vector3(0, 1, 0), Material = DiffuseMaterials.Green };
            this.translateZ = new UITranslateManipulator3D { Direction = new Vector3(0, 0, 1), Material = DiffuseMaterials.Blue };
            this.rotateX = new UIRotateManipulator3D { Axis = new Vector3(1, 0, 0), Length = 0.05, Material = DiffuseMaterials.Red, };
            this.rotateY = new UIRotateManipulator3D { Axis = new Vector3(0, 1, 0), Length = 0.05, Material = DiffuseMaterials.Green };
            this.rotateZ = new UIRotateManipulator3D { Axis = new Vector3(0, 0, 1), Length = 0.05, Material = DiffuseMaterials.Blue };

            // bind UITranslateManipulators3D.TargetTransform to this.Transform            
            BindingOperations.SetBinding(this.translateX, UIManipulator3D.TargetTransformProperty, new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(this.translateY, UIManipulator3D.TargetTransformProperty, new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(this.translateZ, UIManipulator3D.TargetTransformProperty, new Binding("TargetTransform") { Source = this });

            BindingOperations.SetBinding(this.rotateX, UIManipulator3D.TargetTransformProperty, new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(this.rotateY, UIManipulator3D.TargetTransformProperty, new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(this.rotateZ, UIManipulator3D.TargetTransformProperty, new Binding("TargetTransform") { Source = this });

            //BindingOperations.SetBinding(this.translateX, UIManipulator3D.TransformProperty, new Binding("TargetTransform") { Source = this });
            //BindingOperations.SetBinding(this.translateY, UIManipulator3D.TransformProperty, new Binding("TargetTransform") { Source = this });
            //BindingOperations.SetBinding(this.translateZ, UIManipulator3D.TransformProperty, new Binding("TargetTransform") { Source = this });

            //BindingOperations.SetBinding(this.rotateX, UIManipulator3D.TransformProperty, new Binding("TargetTransform") { Source = this });
            //BindingOperations.SetBinding(this.rotateY, UIManipulator3D.TransformProperty, new Binding("TargetTransform") { Source = this });
            //BindingOperations.SetBinding(this.rotateZ, UIManipulator3D.TransformProperty, new Binding("TargetTransform") { Source = this });

            // bind this.Transform to this.TargetTransform (TwoWay)
            BindingOperations.SetBinding(this, TransformProperty, new Binding("TargetTransform") { Source = this, Mode = BindingMode.TwoWay, });

            this.Children.Clear();
            this.Children.Add(this.translateX);
            this.Children.Add(this.translateY);
            this.Children.Add(this.translateZ);
            this.Children.Add(this.rotateX);
            this.Children.Add(this.rotateY);
            this.Children.Add(this.rotateZ);
        }

        private void SceneNode_OnAttached(object sender, System.EventArgs e)
        {
            OnChildrenChanged();
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="totalModelMatrix"></param>
        ///// <param name="ray"></param>
        ///// <param name="hits"></param>
        ///// <returns></returns>
        //protected override bool OnHitTest(IRenderContext context, Matrix totalModelMatrix, ref Ray ray, ref List<HitTestResult> hits)
        //{
        //    bool hit = false;
        //    foreach (var c in this.Children)
        //    {
        //        var hc = c as IHitable;
        //        if (hc != null)
        //        {
        //            if (hc.HitTest(context, ray, ref hits))
        //            {
        //                hit = true;
        //            }
        //        }
        //    }
        //    return hit;
        //}

        /// <summary>
        /// The on children changed.
        /// </summary>
        protected virtual void OnChildrenChanged()
        {
            var diameter = Diameter;

            translateX.Length = diameter;
            translateY.Length = diameter;
            translateZ.Length = diameter;

            rotateX.InnerDiameter = diameter;
            rotateY.InnerDiameter = diameter;
            rotateZ.InnerDiameter = diameter;

            rotateY.InnerDiameter += 0.01;
            rotateZ.InnerDiameter += 0.02;
            rotateX.OuterDiameter = rotateX.InnerDiameter + 0.25;
            rotateY.OuterDiameter = rotateY.InnerDiameter + 0.25;
            rotateZ.OuterDiameter = rotateZ.InnerDiameter + 0.25;
        }

        /// <summary>
        /// The children changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void ChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model = d as UICompositeManipulator3D;
            if (model.SceneNode.IsAttached)
            {
                model.OnChildrenChanged();
            }
        }
    }
}