// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResizeManipulator3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   The can translate x property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MouseDragDemo
{
    using System.Collections.Generic;
    using System.Windows;
    using HelixToolkit.Wpf.SharpDX;

    using SharpDX;

    public class ResizeManipulator3D : GroupElement3D //, IHitable, INotifyPropertyChanged
    {
        private UITranslateManipulator3D translateXL, translateYL, translateZL, translateXR, translateYR, translateZR;
        private LineGeometryModel3D selectionBounds;


        /// <summary>
        /// The can translate x property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateXProperty = DependencyProperty.Register(
            "CanTranslateX", typeof(bool), typeof(ResizeManipulator3D), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The can translate y property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateYProperty = DependencyProperty.Register(
            "CanTranslateY", typeof(bool), typeof(ResizeManipulator3D), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The can translate z property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateZProperty = DependencyProperty.Register(
            "CanTranslateZ", typeof(bool), typeof(ResizeManipulator3D), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(MeshGeometryModel3D), typeof(ResizeManipulator3D), new UIPropertyMetadata(null, ContentChanged));

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
        /// 
        /// </summary>
        public ResizeManipulator3D()
        {
            var red = PhongMaterials.Red;
            red.ReflectiveColor = Color.Black;
            //red.SpecularShininess = 0f;
            this.translateXR = new UITranslateManipulator3D { Direction = new Vector3(+1, 0, 0), IsThrowingShadow = false, Material = red, };
            this.translateYR = new UITranslateManipulator3D { Direction = new Vector3(0, +1, 0), IsThrowingShadow = false, Material = PhongMaterials.Green };
            this.translateZR = new UITranslateManipulator3D { Direction = new Vector3(0, 0, +1), IsThrowingShadow = false, Material = PhongMaterials.Blue };
            this.translateXL = new UITranslateManipulator3D { Direction = new Vector3(-1, 0, 0), IsThrowingShadow = false, Material = red };
            this.translateYL = new UITranslateManipulator3D { Direction = new Vector3(0, -1, 0), IsThrowingShadow = false, Material = PhongMaterials.Green };
            this.translateZL = new UITranslateManipulator3D { Direction = new Vector3(0, 0, -1), IsThrowingShadow = false, Material = PhongMaterials.Blue };
            //this.rotateZ = new UIRotateManipulator3D { Axis = Vector3.UnitZ, InnerDiameter = 2, OuterDiameter = 2.15, Length = 0.05 };

            this.CanTranslateX = true;
            this.CanTranslateY = false;
            this.CanTranslateZ = false;
            this.IsRendering = false;

            this.OnChildrenChanged();
           // this.OnContentChanged();                       
        }

        ~ResizeManipulator3D()
        {            
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
            ((ResizeManipulator3D)d).OnChildrenChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ContentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            //((ResizeManipulator3D)obj).OnContentChanged();
        }


        /// <summary>
        /// The on children changed.
        /// </summary>
        protected virtual void OnChildrenChanged()
        {
            this.translateXL.Length = 0.5;
            this.translateYL.Length = 0.5;
            this.translateZL.Length = 0.5;
            this.translateXR.Length = 0.5;
            this.translateYR.Length = 0.5;
            this.translateZR.Length = 0.5;

            this.Children.Clear();

            if (this.CanTranslateX)
            {
                this.Children.Add(this.translateXL);
                this.Children.Add(this.translateXR);
            }

            if (this.CanTranslateY)
            {
                this.Children.Add(this.translateYL);
                this.Children.Add(this.translateYR);
            }

            if (this.CanTranslateZ)
            {
                this.Children.Add(this.translateZL);
                this.Children.Add(this.translateZR);
            }


            {
                var g = new LineBuilder();
                g.AddLine(new Vector3(0, 0, 0), new Vector3(1, 0, 0));
                g.AddLine(new Vector3(1, 0, 0), new Vector3(1, 1, 0));
                g.AddLine(new Vector3(1, 1, 0), new Vector3(0, 1, 0));
                g.AddLine(new Vector3(0, 1, 0), new Vector3(0, 0, 0));
                this.selectionBounds = new LineGeometryModel3D()
                {
                    Thickness = 3,
                    Smoothness = 2,
                    Color = System.Windows.Media.Colors.Red,
                    IsThrowingShadow = false,
                    Geometry = g.ToLineGeometry3D(),
                };
                this.Children.Add(this.selectionBounds);
            }            
        }
    }
}