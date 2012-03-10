// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenSpaceVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: Ms-PL
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// An abstract base class for visuals that use screen space dimensions when rendering.
    /// </summary>
    public abstract class ScreenSpaceVisual3D : RenderingModelVisual3D
    {
        #region Constants and Fields

        /// <summary>
        ///   The color property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Color), typeof(ScreenSpaceVisual3D), new UIPropertyMetadata(Colors.Black, ColorChanged));

        /// <summary>
        ///   The depth offset property.
        /// </summary>
        public static readonly DependencyProperty DepthOffsetProperty = DependencyProperty.Register(
            "DepthOffset", typeof(double), typeof(ScreenSpaceVisual3D), new UIPropertyMetadata(0.0, GeometryChanged));

        /// <summary>
        ///   The points property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
            "Points",
            typeof(IList<Point3D>),
            typeof(ScreenSpaceVisual3D),
            new UIPropertyMetadata(null, GeometryChanged));

        /// <summary>
        ///   The clipping object.
        /// </summary>
        protected CohenSutherlandClipping Clipping;

        /// <summary>
        ///   The mesh.
        /// </summary>
        protected MeshGeometry3D Mesh;

        /// <summary>
        ///   The model.
        /// </summary>
        protected GeometryModel3D Model;

        /// <summary>
        ///   The is rendering flag.
        /// </summary>
        private bool isRendering;

        #endregion

        /// <summary>
        /// Called when the parent of the 3-D visual object is changed.
        /// </summary>
        /// <param name="oldParent">A value of type <see cref="T:System.Windows.DependencyObject"/> that represents the previous parent of the <see cref="T:System.Windows.Media.Media3D.Visual3D"/> object. If the <see cref="T:System.Windows.Media.Media3D.Visual3D"/> object did not have a previous parent, the value of the parameter is null.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            var parent = VisualTreeHelper.GetParent(this);
            this.IsRendering = parent != null;
        }

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ScreenSpaceVisual3D" /> class.
        /// </summary>
        protected ScreenSpaceVisual3D()
        {
            this.Mesh = new MeshGeometry3D();
            this.Model = new GeometryModel3D { Geometry = this.Mesh };
            this.Content = this.Model;
            this.Points = new List<Point3D>();
            this.OnColorChanged();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the color.
        /// </summary>
        /// <value>
        ///   The color.
        /// </value>
        public Color Color
        {
            get
            {
                return (Color)this.GetValue(ColorProperty);
            }

            set
            {
                this.SetValue(ColorProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the depth offset.
        ///   A small positive number (0.0001) will move the visual slightly in front of other objects.
        /// </summary>
        /// <value>
        ///   The depth offset.
        /// </value>
        public double DepthOffset
        {
            get
            {
                return (double)this.GetValue(DepthOffsetProperty);
            }

            set
            {
                this.SetValue(DepthOffsetProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is being rendered.
        ///   When the visual is removed from the visual tree, this property should be set to false.
        /// </summary>
        public bool IsRendering
        {
            get
            {
                return this.isRendering;
            }

            set
            {
                if (value != this.isRendering)
                {
                    this.isRendering = value;
                    if (this.isRendering)
                    {
                        this.SubscribeToRenderingEvent();
                    }
                    else
                    {
                        this.UnsubscribeRenderingEvent();
                    }
                }
            }
        }

        /// <summary>
        ///   Gets or sets the points collection.
        /// </summary>
        /// <value>
        ///   The points collection.
        /// </value>
        public IList<Point3D> Points
        {
            get
            {
                return (IList<Point3D>)this.GetValue(PointsProperty);
            }

            set
            {
                this.SetValue(PointsProperty, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The geometry changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScreenSpaceVisual3D)d).OnGeometryChanged();
        }

        /// <summary>
        /// Updates the geometry.
        /// </summary>
        protected abstract void UpdateGeometry();

        /// <summary>
        /// Updates the transforms.
        /// </summary>
        /// <returns>
        /// True if the transform is updated.
        /// </returns>
        protected abstract bool UpdateTransforms();

        /// <summary>
        /// The color changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScreenSpaceVisual3D)d).OnColorChanged();
        }

        /// <summary>
        /// The composition target_ rendering.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnCompositionTargetRendering(object sender, RenderingEventArgs e)
        {
            if (this.isRendering)
            {
                if (!Visual3DHelper.IsAttachedToViewport3D(this))
                {
                    return;
                }

                if (this.UpdateTransforms())
                {
                    this.UpdateClipping();
                    this.UpdateGeometry();
                }
            }
        }

        /// <summary>
        /// Changes the material when the color changed.
        /// </summary>
        private void OnColorChanged()
        {
            var mg = new MaterialGroup();
            mg.Children.Add(new DiffuseMaterial(Brushes.Black));
            mg.Children.Add(new EmissiveMaterial(new SolidColorBrush(this.Color)) { Color = Colors.White });
            this.Model.Material = mg;
        }

        /// <summary>
        /// Called when geometry properties have changed.
        /// </summary>
        private void OnGeometryChanged()
        {
            this.UpdateGeometry();
        }

        /// <summary>
        /// The update clipping.
        /// </summary>
        private void UpdateClipping()
        {
            Viewport3D vp = Visual3DHelper.GetViewport3D(this);
            if (vp == null)
            {
                return;
            }

            this.Clipping = new CohenSutherlandClipping(10, vp.ActualWidth - 20, 10, vp.ActualHeight - 20);
        }

        #endregion


    }
}