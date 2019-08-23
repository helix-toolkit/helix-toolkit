// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Expander3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that translates all children relative to the specified origin.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that translates all children relative to the specified origin.
    /// </summary>
    public class Expander3D : ModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="ExpandOrigin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandOriginProperty = DependencyProperty.Register(
            "ExpandOrigin", typeof(Point3D?), typeof(Expander3D), new UIPropertyMetadata(null, ExpansionChanged));

        /// <summary>
        /// Identifies the <see cref="Expansion"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpansionProperty = DependencyProperty.Register(
            "Expansion", typeof(double), typeof(Expander3D), new UIPropertyMetadata(2.0, ExpansionChanged));

        /// <summary>
        /// The original transforms.
        /// </summary>
        private readonly Dictionary<Model3D, Transform3D> originalTransforms = new Dictionary<Model3D, Transform3D>();

        /// <summary>
        /// The actual expand origin.
        /// </summary>
        private Point3D actualExpandOrigin;

        /// <summary>
        /// Gets or sets the origin of the expansion.
        /// </summary>
        /// <value>The expand origin.</value>
        public Point3D? ExpandOrigin
        {
            get
            {
                return (Point3D?)this.GetValue(ExpandOriginProperty);
            }

            set
            {
                this.SetValue(ExpandOriginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the expansion factor.
        /// </summary>
        /// <value>The expansion.</value>
        public double Expansion
        {
            get
            {
                return (double)this.GetValue(ExpansionProperty);
            }

            set
            {
                this.SetValue(ExpansionProperty, value);
            }
        }

        /// <summary>
        /// Expands to the specified value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void ExpandTo(double value, double animationTime)
        {
            var a = new DoubleAnimation(value, new Duration(TimeSpan.FromMilliseconds(animationTime)))
                {
                   AccelerationRatio = 0.3, DecelerationRatio = 0.5
                };
            this.BeginAnimation(ExpansionProperty, a);
        }

        /// <summary>
        /// The expansion changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void ExpansionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Expander3D)d).OnExpansionChanged();
        }

        /// <summary>
        /// The expand.
        /// </summary>
        protected virtual void OnExpansionChanged()
        {
            if (!this.ExpandOrigin.HasValue)
            {
                if (this.Content != null)
                {
                    this.actualExpandOrigin = this.Content.Bounds.Location;
                }
            }
            else
            {
                this.actualExpandOrigin = this.ExpandOrigin.Value;
            }

            Model3DHelper.Traverse<GeometryModel3D>(this.Content, this.Expand);
        }

        /// <summary>
        /// The expand.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="transformation">
        /// The transformation.
        /// </param>
        private void Expand(GeometryModel3D model, Transform3D transformation)
        {
            Transform3D ot;
            if (this.originalTransforms.ContainsKey(model))
            {
                ot = this.originalTransforms[model];
            }
            else
            {
                ot = model.Transform;
                this.originalTransforms.Add(model, ot);
            }

            Transform3D totalTransform = Transform3DHelper.CombineTransform(transformation, ot);

            var mesh = model.Geometry as MeshGeometry3D;
            if (mesh == null)
            {
                return;
            }

            var bounds = new Rect3D();
            foreach (int i in mesh.TriangleIndices)
            {
                bounds.Union(totalTransform.Transform(mesh.Positions[i]));
            }

            Point3D p = bounds.Location;
            Vector3D d = p - this.actualExpandOrigin;
            d *= this.Expansion;
            Point3D p2 = this.actualExpandOrigin + d;
            var t = new TranslateTransform3D(p2 - p);

            model.Transform = Transform3DHelper.CombineTransform(ot, t);
        }

    }
}