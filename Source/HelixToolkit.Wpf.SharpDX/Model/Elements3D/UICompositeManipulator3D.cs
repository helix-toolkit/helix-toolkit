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

    using Transform3D = System.Windows.Media.Media3D.Transform3D;

    public class UICompositeManipulator3D : CompositeModel3D
    {
        private UIRotateManipulator3D rotateX, rotateY, rotateZ;
        private UITranslateManipulator3D translateX, translateY, translateZ;


        /// <summary>
        /// The can rotate x property.
        /// </summary>
        public static readonly DependencyProperty CanRotateXProperty = DependencyProperty.Register(
            "CanRotateX", typeof(bool), typeof(UICompositeManipulator3D), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The can rotate y property.
        /// </summary>
        public static readonly DependencyProperty CanRotateYProperty = DependencyProperty.Register(
            "CanRotateY", typeof(bool), typeof(UICompositeManipulator3D), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The can rotate z property.
        /// </summary>
        public static readonly DependencyProperty CanRotateZProperty = DependencyProperty.Register(
            "CanRotateZ", typeof(bool), typeof(UICompositeManipulator3D), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The can translate x property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateXProperty = DependencyProperty.Register(
            "CanTranslateX", typeof(bool), typeof(UICompositeManipulator3D), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The can translate y property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateYProperty = DependencyProperty.Register(
            "CanTranslateY", typeof(bool), typeof(UICompositeManipulator3D), new UIPropertyMetadata(true,  ChildrenChanged));

        /// <summary>
        /// The can translate z property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateZProperty = DependencyProperty.Register(
            "CanTranslateZ", typeof(bool), typeof(UICompositeManipulator3D), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The diameter property.
        /// </summary>
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(double), typeof(UICompositeManipulator3D), new AffectsRenderPropertyMetadata(2.0, ChildrenChanged));

        /// <summary>
        ///   The target transform property.
        /// </summary>
        public static readonly DependencyProperty TargetTransformProperty = DependencyProperty.Register(
            "TargetTransform", typeof(Transform3D), typeof(UICompositeManipulator3D), 
            new AffectsRenderFrameworkPropertyMetadata(Transform3D.Identity, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender));

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
            this.translateX = new UITranslateManipulator3D { Direction = new Vector3(1, 0, 0), Material = PhongMaterials.Red };
            this.translateY = new UITranslateManipulator3D { Direction = new Vector3(0, 1, 0), Material = PhongMaterials.Green };
            this.translateZ = new UITranslateManipulator3D { Direction = new Vector3(0, 0, 1), Material = PhongMaterials.Blue };
            this.rotateX = new UIRotateManipulator3D { Axis = new Vector3(1, 0, 0), Length = 0.05, Material = PhongMaterials.Red, };
            this.rotateY = new UIRotateManipulator3D { Axis = new Vector3(0, 1, 0), Length = 0.05, Material = PhongMaterials.Green };
            this.rotateZ = new UIRotateManipulator3D { Axis = new Vector3(0, 0, 1), Length = 0.05, Material = PhongMaterials.Blue };

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

            this.OnChildrenChanged();
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
        /// 
        /// </summary>
        /// <param name="context"></param>
        protected override void OnRender(RenderContext context)
        {
            foreach (var c in this.Children)
            {
                var model = c as ITransformable;
                if (model != null)
                {
                    // apply transform
                    model.Transform = this.Transform;
                    //model.PushMatrix(this.modelMatrix);
                    // render model
                    c.Render(context);
                    //model.PopMatrix();
                }
                else
                {
                    c.Render(context);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="hits"></param>
        /// <returns></returns>
        protected override bool OnHitTest(IRenderMatrices context, Ray ray, ref List<HitTestResult> hits)
        {
            bool hit = false;
            foreach (var c in this.Children)
            {
                var hc = c as IHitable;
                if (hc != null)
                {
                    if (hc.HitTest(context, ray, ref hits))
                    {
                        hit = true;
                    }
                }
            }
            return hit;
        }

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

            this.Children.Clear();

            if (this.CanTranslateX)
            {
                this.Children.Add(this.translateX);
            }

            if (this.CanTranslateY)
            {
                this.Children.Add(this.translateY);
            }

            if (this.CanTranslateZ)
            {
                this.Children.Add(this.translateZ);
            }

            if (this.CanRotateX)
            {
                this.Children.Add(this.rotateX);
            }

            if (this.CanRotateY)
            {
                this.Children.Add(this.rotateY);
            }

            if (this.CanRotateZ)
            {
                this.Children.Add(this.rotateZ);
            }
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
            ((UICompositeManipulator3D)d).OnChildrenChanged();
        }
    }
}