// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CombinedManipulator.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a visual element that shows translation and rotation manipulators.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Represents a visual element that shows translation and rotation manipulators.
    /// </summary>
    public class CombinedManipulator : ModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="CanRotateX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanRotateXProperty = DependencyProperty.Register(
            "CanRotateX", typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// Identifies the <see cref="CanRotateY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanRotateYProperty = DependencyProperty.Register(
            "CanRotateY", typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// Identifies the <see cref="CanRotateZ"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanRotateZProperty = DependencyProperty.Register(
            "CanRotateZ", typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// Identifies the <see cref="CanTranslateX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateXProperty = DependencyProperty.Register(
            "CanTranslateX", typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// Identifies the <see cref="CanTranslateY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateYProperty = DependencyProperty.Register(
            "CanTranslateY", typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// Identifies the <see cref="CanTranslateZ"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateZProperty = DependencyProperty.Register(
            "CanTranslateZ", typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// Identifies the <see cref="Diameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(double), typeof(CombinedManipulator), new UIPropertyMetadata(2.0));

        /// <summary>
        /// Identifies the <see cref="TargetTransform"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetTransformProperty =
            DependencyProperty.Register(
                "TargetTransform",
                typeof(Transform3D),
                typeof(CombinedManipulator),
                new FrameworkPropertyMetadata(
                    Transform3D.Identity, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// The rotate x manipulator.
        /// </summary>
        private readonly RotateManipulator rotateXManipulator;

        /// <summary>
        /// The rotate y manipulator.
        /// </summary>
        private readonly RotateManipulator rotateYManipulator;

        /// <summary>
        /// The rotate z manipulator.
        /// </summary>
        private readonly RotateManipulator rotateZManipulator;

        /// <summary>
        /// The translate x manipulator.
        /// </summary>
        private readonly TranslateManipulator translateXManipulator;

        /// <summary>
        /// The translate y manipulator.
        /// </summary>
        private readonly TranslateManipulator translateYManipulator;

        /// <summary>
        /// The translate z manipulator.
        /// </summary>
        private readonly TranslateManipulator translateZManipulator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedManipulator" /> class.
        /// </summary>
        public CombinedManipulator()
        {
            this.translateXManipulator = new TranslateManipulator
                {
                   Direction = new Vector3D(1, 0, 0), Color = Colors.Red
                };
            this.translateYManipulator = new TranslateManipulator
                {
                   Direction = new Vector3D(0, 1, 0), Color = Colors.Green
                };
            this.translateZManipulator = new TranslateManipulator
                {
                   Direction = new Vector3D(0, 0, 1), Color = Colors.Blue
                };
            this.rotateXManipulator = new RotateManipulator { Axis = new Vector3D(1, 0, 0), Color = Colors.Red };
            this.rotateYManipulator = new RotateManipulator { Axis = new Vector3D(0, 1, 0), Color = Colors.Green };
            this.rotateZManipulator = new RotateManipulator { Axis = new Vector3D(0, 0, 1), Color = Colors.Blue };

            BindingOperations.SetBinding(this, TransformProperty, new Binding("TargetTransform") { Source = this });

            BindingOperations.SetBinding(
                this.translateXManipulator,
                Manipulator.TargetTransformProperty,
                new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(
                this.translateYManipulator,
                Manipulator.TargetTransformProperty,
                new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(
                this.translateZManipulator,
                Manipulator.TargetTransformProperty,
                new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(
                this.rotateXManipulator, RotateManipulator.DiameterProperty, new Binding("Diameter") { Source = this });
            BindingOperations.SetBinding(
                this.rotateYManipulator, RotateManipulator.DiameterProperty, new Binding("Diameter") { Source = this });
            BindingOperations.SetBinding(
                this.rotateZManipulator, RotateManipulator.DiameterProperty, new Binding("Diameter") { Source = this });
            BindingOperations.SetBinding(
                this.rotateXManipulator,
                Manipulator.TargetTransformProperty,
                new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(
                this.rotateYManipulator,
                Manipulator.TargetTransformProperty,
                new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(
                this.rotateZManipulator,
                Manipulator.TargetTransformProperty,
                new Binding("TargetTransform") { Source = this });

            this.UpdateChildren();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can rotate X.
        /// </summary>
        /// <value> <c>true</c> if this instance can rotate X; otherwise, <c>false</c> . </value>
        public bool CanRotateX
        {
            get
            {
                return (bool)this.GetValue(CanRotateXProperty);
            }

            set
            {
                this.SetValue(CanRotateXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can rotate Y.
        /// </summary>
        /// <value> <c>true</c> if this instance can rotate Y; otherwise, <c>false</c> . </value>
        public bool CanRotateY
        {
            get
            {
                return (bool)this.GetValue(CanRotateYProperty);
            }

            set
            {
                this.SetValue(CanRotateYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can rotate Z.
        /// </summary>
        /// <value> <c>true</c> if this instance can rotate Z; otherwise, <c>false</c> . </value>
        public bool CanRotateZ
        {
            get
            {
                return (bool)this.GetValue(CanRotateZProperty);
            }

            set
            {
                this.SetValue(CanRotateZProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can translate X.
        /// </summary>
        /// <value> <c>true</c> if this instance can translate X; otherwise, <c>false</c> . </value>
        public bool CanTranslateX
        {
            get
            {
                return (bool)this.GetValue(CanTranslateXProperty);
            }

            set
            {
                this.SetValue(CanTranslateXProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can translate Y.
        /// </summary>
        /// <value> <c>true</c> if this instance can translate Y; otherwise, <c>false</c> . </value>
        public bool CanTranslateY
        {
            get
            {
                return (bool)this.GetValue(CanTranslateYProperty);
            }

            set
            {
                this.SetValue(CanTranslateYProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can translate Z.
        /// </summary>
        /// <value> <c>true</c> if this instance can translate Z; otherwise, <c>false</c> . </value>
        public bool CanTranslateZ
        {
            get
            {
                return (bool)this.GetValue(CanTranslateZProperty);
            }

            set
            {
                this.SetValue(CanTranslateZProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the diameter.
        /// </summary>
        /// <value> The diameter. </value>
        public double Diameter
        {
            get
            {
                return (double)this.GetValue(DiameterProperty);
            }

            set
            {
                this.SetValue(DiameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the offset of the visual (this vector is added to the Position point).
        /// </summary>
        /// <value> The offset. </value>
        public Vector3D Offset
        {
            get
            {
                return this.translateXManipulator.Offset;
            }

            set
            {
                this.translateXManipulator.Offset = value;
                this.translateYManipulator.Offset = value;
                this.translateZManipulator.Offset = value;
                this.rotateXManipulator.Offset = value;
                this.rotateYManipulator.Offset = value;
                this.rotateZManipulator.Offset = value;
            }
        }

        /// <summary>
        /// Gets or sets the pivot point of the manipulator.
        /// </summary>
        /// <value> The position. </value>
        public Point3D Pivot
        {
            get
            {
                return this.rotateXManipulator.Pivot;
            }

            set
            {
                this.rotateXManipulator.Pivot = value;
                this.rotateYManipulator.Pivot = value;
                this.rotateZManipulator.Pivot = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the manipulator.
        /// </summary>
        /// <value> The position. </value>
        public Point3D Position
        {
            get
            {
                return this.translateXManipulator.Position;
            }

            set
            {
                this.translateXManipulator.Position = value;
                this.translateYManipulator.Position = value;
                this.translateZManipulator.Position = value;
                this.rotateXManipulator.Position = value;
                this.rotateYManipulator.Position = value;
                this.rotateZManipulator.Position = value;
            }
        }

        /// <summary>
        /// Gets or sets the target transform.
        /// </summary>
        /// <value> The target transform. </value>
        public Transform3D TargetTransform
        {
            get
            {
                return (Transform3D)this.GetValue(TargetTransformProperty);
            }

            set
            {
                this.SetValue(TargetTransformProperty, value);
            }
        }

        /// <summary>
        /// Binds this manipulator to a given Visual3D.
        /// </summary>
        /// <param name="source">
        /// Source Visual3D which receives the manipulator transforms.
        /// </param>
        public virtual void Bind(ModelVisual3D source)
        {
            BindingOperations.SetBinding(this, TargetTransformProperty, new Binding("Transform") { Source = source });
            BindingOperations.SetBinding(this, TransformProperty, new Binding("Transform") { Source = source });
        }

        /// <summary>
        /// Releases the binding of this manipulator.
        /// </summary>
        public virtual void UnBind()
        {
            BindingOperations.ClearBinding(this, TargetTransformProperty);
            BindingOperations.ClearBinding(this, TransformProperty);
        }

        /// <summary>
        /// Updates the child visuals.
        /// </summary>
        protected void UpdateChildren()
        {
            this.Children.Clear();
            if (this.CanTranslateX)
            {
                this.Children.Add(this.translateXManipulator);
            }

            if (this.CanTranslateY)
            {
                this.Children.Add(this.translateYManipulator);
            }

            if (this.CanTranslateZ)
            {
                this.Children.Add(this.translateZManipulator);
            }

            if (this.CanRotateX)
            {
                this.Children.Add(this.rotateXManipulator);
            }

            if (this.CanRotateY)
            {
                this.Children.Add(this.rotateYManipulator);
            }

            if (this.CanRotateZ)
            {
                this.Children.Add(this.rotateZManipulator);
            }
        }

        /// <summary>
        /// Handles changes in properties related to the child visuals.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void ChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CombinedManipulator)d).UpdateChildren();
        }
    }
}