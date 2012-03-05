// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CombinedManipulator.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows translation and rotation manipulators.
    /// </summary>
    public class CombinedManipulator : ModelVisual3D
    {
        #region Constants and Fields

        /// <summary>
        /// The can rotate x property.
        /// </summary>
        public static readonly DependencyProperty CanRotateXProperty = DependencyProperty.Register(
            "CanRotateX", typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The can rotate y property.
        /// </summary>
        public static readonly DependencyProperty CanRotateYProperty = DependencyProperty.Register(
            "CanRotateY", typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The can rotate z property.
        /// </summary>
        public static readonly DependencyProperty CanRotateZProperty = DependencyProperty.Register(
            "CanRotateZ", typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The can translate x property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateXProperty = DependencyProperty.Register(
            "CanTranslateX", typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The can translate y property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateYProperty = DependencyProperty.Register(
            "CanTranslateY", typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The can translate z property.
        /// </summary>
        public static readonly DependencyProperty CanTranslateZProperty = DependencyProperty.Register(
            "CanTranslateZ", typeof(bool), typeof(CombinedManipulator), new UIPropertyMetadata(true, ChildrenChanged));

        /// <summary>
        /// The diameter property.
        /// </summary>
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(double), typeof(CombinedManipulator), new UIPropertyMetadata(2.0, DiameterChanged));

        /// <summary>
        /// The target transform property.
        /// </summary>
        public static readonly DependencyProperty TargetTransformProperty =
            DependencyProperty.Register(
                "TargetTransform",
                typeof(Transform3D),
                typeof(CombinedManipulator),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// The rotate x manipulator.
        /// </summary>
        private readonly RotateManipulator RotateXManipulator;

        /// <summary>
        /// The rotate y manipulator.
        /// </summary>
        private readonly RotateManipulator RotateYManipulator;

        /// <summary>
        /// The rotate z manipulator.
        /// </summary>
        private readonly RotateManipulator RotateZManipulator;

        /// <summary>
        /// The translate x manipulator.
        /// </summary>
        private readonly TranslateManipulator TranslateXManipulator;

        /// <summary>
        /// The translate y manipulator.
        /// </summary>
        private readonly TranslateManipulator TranslateYManipulator;

        /// <summary>
        /// The translate z manipulator.
        /// </summary>
        private readonly TranslateManipulator TranslateZManipulator;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "CombinedManipulator" /> class.
        /// </summary>
        public CombinedManipulator()
        {
            this.TranslateXManipulator = new TranslateManipulator { Direction = new Vector3D(1, 0, 0), Color = Colors.Red };
            this.TranslateYManipulator = new TranslateManipulator { Direction = new Vector3D(0, 1, 0), Color = Colors.Green };
            this.TranslateZManipulator = new TranslateManipulator { Direction = new Vector3D(0, 0, 1), Color = Colors.Blue };
            this.RotateXManipulator = new RotateManipulator { Axis = new Vector3D(1, 0, 0), Color = Colors.Red };
            this.RotateYManipulator = new RotateManipulator { Axis = new Vector3D(0, 1, 0), Color = Colors.Green };
            this.RotateZManipulator = new RotateManipulator { Axis = new Vector3D(0, 0, 1), Color = Colors.Blue };

            BindingOperations.SetBinding(
                this,
                TransformProperty,
                new Binding("TargetTransform") { Source = this });

            BindingOperations.SetBinding(
                this.TranslateXManipulator,
                Manipulator.TargetTransformProperty,
                new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(
                this.TranslateYManipulator,
                Manipulator.TargetTransformProperty,
                new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(
                this.TranslateZManipulator,
                Manipulator.TargetTransformProperty,
                new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(
                this.RotateXManipulator, RotateManipulator.DiameterProperty, new Binding("Diameter") { Source = this });
            BindingOperations.SetBinding(
                this.RotateYManipulator, RotateManipulator.DiameterProperty, new Binding("Diameter") { Source = this });
            BindingOperations.SetBinding(
                this.RotateZManipulator, RotateManipulator.DiameterProperty, new Binding("Diameter") { Source = this });
            BindingOperations.SetBinding(
                this.RotateXManipulator,
                Manipulator.TargetTransformProperty,
                new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(
                this.RotateYManipulator,
                Manipulator.TargetTransformProperty,
                new Binding("TargetTransform") { Source = this });
            BindingOperations.SetBinding(
                this.RotateZManipulator,
                Manipulator.TargetTransformProperty,
                new Binding("TargetTransform") { Source = this });

            this.OnChildrenChanged();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets a value indicating whether this instance can rotate X.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can rotate X; otherwise, <c>false</c>.
        /// </value>
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
        ///   Gets or sets a value indicating whether this instance can rotate Y.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can rotate Y; otherwise, <c>false</c>.
        /// </value>
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
        ///   Gets or sets a value indicating whether this instance can rotate Z.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can rotate Z; otherwise, <c>false</c>.
        /// </value>
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
        ///   Gets or sets a value indicating whether this instance can translate X.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can translate X; otherwise, <c>false</c>.
        /// </value>
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
        ///   Gets or sets a value indicating whether this instance can translate Y.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can translate Y; otherwise, <c>false</c>.
        /// </value>
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
        ///   Gets or sets a value indicating whether this instance can translate Z.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can translate Z; otherwise, <c>false</c>.
        /// </value>
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
        ///   Gets or sets the diameter.
        /// </summary>
        /// <value>The diameter.</value>
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
        ///   Gets or sets the target transform.
        /// </summary>
        /// <value>The target transform.</value>
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
        ///   Gets or sets the offset of the visual (this vector is added to the Position point).
        /// </summary>
        /// <value>The offset.</value>
        public Vector3D Offset
        {
            get { return TranslateXManipulator.Offset; }
            set
            {
                TranslateXManipulator.Offset = value;
                TranslateYManipulator.Offset = value;
                TranslateZManipulator.Offset = value;
                RotateXManipulator.Offset = value;
                RotateYManipulator.Offset = value;
                RotateZManipulator.Offset = value;
            }
        }

        /// <summary>
        ///   Gets or sets the position of the manipulator.
        /// </summary>
        /// <value>The position.</value>
        public Point3D Position
        {
            get { return TranslateXManipulator.Position; }
            set
            {
                TranslateXManipulator.Position = value;
                TranslateYManipulator.Position = value;
                TranslateZManipulator.Position = value;
                RotateXManipulator.Position = value;
                RotateYManipulator.Position = value;
                RotateZManipulator.Position = value;
            }
        }

        /// <summary>
        ///   Gets or sets the pivot point of the manipulator.
        /// </summary>
        /// <value>The position.</value>
        public Point3D Pivot
        {
            get { return TranslateXManipulator.Pivot; }
            set
            {
                TranslateXManipulator.Pivot = value;
                TranslateYManipulator.Pivot = value;
                TranslateZManipulator.Pivot = value;
                RotateXManipulator.Pivot = value;
                RotateYManipulator.Pivot = value;
                RotateZManipulator.Pivot = value;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// The on children changed.
        /// </summary>
        protected virtual void OnChildrenChanged()
        {
            this.Children.Clear();
            if (this.CanTranslateX)
            {
                this.Children.Add(this.TranslateXManipulator);
            }

            if (this.CanTranslateY)
            {
                this.Children.Add(this.TranslateYManipulator);
            }

            if (this.CanTranslateZ)
            {
                this.Children.Add(this.TranslateZManipulator);
            }

            if (this.CanRotateX)
            {
                this.Children.Add(this.RotateXManipulator);
            }

            if (this.CanRotateY)
            {
                this.Children.Add(this.RotateYManipulator);
            }

            if (this.CanRotateZ)
            {
                this.Children.Add(this.RotateZManipulator);
            }
        }

        /// <summary>
        /// The on diameter changed.
        /// </summary>
        protected virtual void OnDiameterChanged()
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
            ((CombinedManipulator)d).OnChildrenChanged();
        }

        /// <summary>
        /// The diameter changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void DiameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CombinedManipulator)d).OnDiameterChanged();
        }

        #endregion

        #region Public Bindig Methods

        /// <summary>
        /// Binds this manipulator to a given Visual3D.
        /// </summary>
        /// <param name="source">Source Visual3D which receives the manipulator transforms.</param>
        public virtual void Bind(ModelVisual3D source)
        {
            BindingOperations.SetBinding(this, CombinedManipulator.TargetTransformProperty, new Binding("Transform") { Source = source });
            BindingOperations.SetBinding(this, CombinedManipulator.TransformProperty, new Binding("Transform") { Source = source });
        }

        /// <summary>
        /// Releases the binding of this manipulator.
        /// </summary>
        public virtual void UnBind()
        {
            BindingOperations.ClearBinding(this, CombinedManipulator.TargetTransformProperty);
            BindingOperations.ClearBinding(this, CombinedManipulator.TransformProperty);
        }

        #endregion
    }
}