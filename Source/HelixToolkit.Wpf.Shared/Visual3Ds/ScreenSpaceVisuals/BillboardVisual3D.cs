// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BillboardVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that contains a billboard (a quadrilateral that always faces camera). The size of the billboard is defined in screen space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that contains a billboard (a quadrilateral that always faces camera). The size of the billboard is defined in screen space.
    /// </summary>
    public class BillboardVisual3D : RenderingModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="DepthOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DepthOffsetProperty = DependencyProperty.Register(
            "DepthOffset", typeof(double), typeof(BillboardVisual3D), new UIPropertyMetadata(0.0));

        /// <summary>
        /// Identifies the <see cref="Height"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height", typeof(double), typeof(BillboardVisual3D), new UIPropertyMetadata(10.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="HorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register(
                "HorizontalAlignment",
                typeof(HorizontalAlignment),
                typeof(BillboardVisual3D),
                new UIPropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Identifies the <see cref="Material"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaterialProperty = DependencyProperty.Register(
            "Material",
            typeof(Material),
            typeof(BillboardVisual3D),
            new UIPropertyMetadata(Materials.Red, MaterialChanged));

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position",
            typeof(Point3D),
            typeof(BillboardVisual3D),
            new UIPropertyMetadata(new Point3D(), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="VerticalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register(
                "VerticalAlignment",
                typeof(VerticalAlignment),
                typeof(BillboardVisual3D),
                new UIPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Identifies the <see cref="Width"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width", typeof(double), typeof(BillboardVisual3D), new UIPropertyMetadata(10.0, GeometryChanged));

        /// <summary>
        /// The builder.
        /// </summary>
        private readonly BillboardGeometryBuilder builder;

        /// <summary>
        /// The is rendering flag.
        /// </summary>
        private bool isRendering;

        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardVisual3D" /> class.
        /// </summary>
        public BillboardVisual3D()
        {
            this.builder = new BillboardGeometryBuilder(this);
            this.Mesh = new MeshGeometry3D
                            {
                                TriangleIndices = BillboardGeometryBuilder.CreateIndices(1),
                                TextureCoordinates =
                                    new PointCollection
                                        {
                                            new Point(0, 1),
                                            new Point(1, 1),
                                            new Point(1, 0),
                                            new Point(0, 0)
                                        }
                            };

            this.Model = new GeometryModel3D { Geometry = this.Mesh };
            this.Content = this.Model;
            this.OnMaterialChanged();
            this.OnGeometryChanged();
        }

        /// <summary>
        /// Gets or sets the depth offset.
        /// </summary>
        /// <value>The depth offset.</value>
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
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get
            {
                return (double)this.GetValue(HeightProperty);
            }

            set
            {
                this.SetValue(HeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        /// <value>The horizontal alignment.</value>
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(HorizontalAlignmentProperty);
            }

            set
            {
                this.SetValue(HorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is being rendered.
        /// When the visual is removed from the visual tree, this property should be set to false.
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
        /// Gets or sets the material.
        /// </summary>
        /// <value>The material.</value>
        public Material Material
        {
            get
            {
                return (Material)this.GetValue(MaterialProperty);
            }

            set
            {
                this.SetValue(MaterialProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position (center) of the billboard.
        /// </summary>
        /// <value>The position.</value>
        public Point3D Position
        {
            get
            {
                return (Point3D)this.GetValue(PositionProperty);
            }

            set
            {
                this.SetValue(PositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        /// <value>The vertical alignment.</value>
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(VerticalAlignmentProperty);
            }

            set
            {
                this.SetValue(VerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get
            {
                return (double)this.GetValue(WidthProperty);
            }

            set
            {
                this.SetValue(WidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mesh.
        /// </summary>
        protected MeshGeometry3D Mesh { get; set; }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        protected GeometryModel3D Model { get; set; }

        /// <summary>
        /// The on material changed.
        /// </summary>
        public void OnMaterialChanged()
        {
            this.Model.Material = this.Material;
        }

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
            ((BillboardVisual3D)d).OnGeometryChanged();
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
                    this.UpdateGeometry();
                }
            }
        }

        /// <summary>
        /// Called when the parent of the 3-D visual object is changed.
        /// </summary>
        /// <param name="oldParent">
        /// A value of type <see cref="T:System.Windows.DependencyObject" /> that represents the previous parent of the
        ///     <see
        ///         cref="T:System.Windows.Media.Media3D.Visual3D" />
        /// object. If the
        ///     <see
        ///         cref="T:System.Windows.Media.Media3D.Visual3D" />
        /// object did not have a previous parent, the value of the parameter is null.
        /// </param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            var parent = VisualTreeHelper.GetParent(this);
            this.IsRendering = parent != null;
        }

        /// <summary>
        /// Updates the geometry.
        /// </summary>
        protected void UpdateGeometry()
        {
            var bb = new Billboard(
                this.Position,
                this.Width,
                this.Height,
                this.HorizontalAlignment,
                this.VerticalAlignment,
                this.DepthOffset);

            this.Mesh.Positions = this.builder.GetPositions(new[] { bb },new Vector());
        }

        /// <summary>
        /// Updates the transforms.
        /// </summary>
        /// <returns>
        /// True if the transform is updated.
        /// </returns>
        protected bool UpdateTransforms()
        {
            return this.builder.UpdateTransforms();
        }

        /// <summary>
        /// The color changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void MaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BillboardVisual3D)d).OnMaterialChanged();
        }

        /// <summary>
        /// Called when geometry properties have changed.
        /// </summary>
        private void OnGeometryChanged()
        {
            this.UpdateGeometry();
        }
    }
}