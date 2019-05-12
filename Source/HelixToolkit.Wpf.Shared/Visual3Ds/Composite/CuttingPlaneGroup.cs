// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CuttingPlaneGroup.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that applies cutting planes to all children.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Defines the cutting operation.
    /// </summary>
    public enum CuttingOperation
    {
        /// <summary>
        /// The intersect operation.
        /// </summary>
        Intersect,

        /// <summary>
        /// The subtract operation.
        /// </summary>
        Subtract,
    }

    /// <summary>
    /// A visual element that applies the intersection of all the specified cutting planes to all children.
    /// </summary>
    public class CuttingPlaneGroup : RenderingModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            "IsEnabled", typeof(bool), typeof(CuttingPlaneGroup), new UIPropertyMetadata(false, IsEnabledChanged));

        /// <summary>
        /// Identifies the <see cref="Operation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OperationProperty =
            DependencyProperty.Register("Operation", typeof(CuttingOperation), typeof(CuttingPlaneGroup), new PropertyMetadata(CuttingOperation.Intersect, OperationChanged));

        /// <summary>
        /// The cut geometries.
        /// </summary>
        private Dictionary<Model3D, Geometry3D> cutGeometries = new Dictionary<Model3D, Geometry3D>();

        /// <summary>
        /// The new cut geometries.
        /// </summary>
        private Dictionary<Model3D, Geometry3D> newCutGeometries;

        /// <summary>
        /// The new original geometries.
        /// </summary>
        private Dictionary<Model3D, Geometry3D> newOriginalGeometries;

        /// <summary>
        /// The original geometries.
        /// </summary>
        private Dictionary<Model3D, Geometry3D> originalGeometries = new Dictionary<Model3D, Geometry3D>();

        /// <summary>
        /// Initializes a new instance of the <see cref = "CuttingPlaneGroup" /> class.
        /// </summary>
        public CuttingPlaneGroup()
        {
            this.IsEnabled = true;
            this.CuttingPlanes = new List<Plane3D>();
        }

        /// <summary>
        /// Gets or sets the cutting planes.
        /// </summary>
        /// <value>
        /// The cutting planes.
        /// </value>
        /// <remarks>
        /// The the intersection of all the cutting planes will be used to
        /// intersect/subtract (defined in <see cref="Operation" /> all child visuals of the <see cref="CuttingPlaneGroup" />.
        /// </remarks>
        public List<Plane3D> CuttingPlanes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether cutting is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return (bool)this.GetValue(IsEnabledProperty);
            }

            set
            {
                this.SetValue(IsEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the cutting operation.
        /// </summary>
        /// <value>The operation.</value>
        public CuttingOperation Operation
        {
            get { return (CuttingOperation)this.GetValue(OperationProperty); }
            set { this.SetValue(OperationProperty, value); }
        }

        /// <summary>
        /// Called when the composition target rendering event is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RenderingEventArgs"/> instance containing the event data.</param>
        protected override void OnCompositionTargetRendering(object sender, RenderingEventArgs e)
        {
            // TODO: Find a better way to handle this...
            if (this.IsEnabled)
            {
                this.ApplyCuttingGeometries();
            }
        }

        /// <summary>
        /// Handles changes to the <see cref="IsEnabled" /> property.
        /// </summary>
        /// <param name="d">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var g = (CuttingPlaneGroup)d;

            if (g.IsEnabled)
            {
                g.SubscribeToRenderingEvent();
            }
            else
            {
                g.UnsubscribeRenderingEvent();
            }

            g.ApplyCuttingGeometries(true);
        }

        /// <summary>
        /// Handles changes to the <see cref="Operation" /> property.
        /// </summary>
        /// <param name="d">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OperationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CuttingPlaneGroup)d).ApplyCuttingGeometries(true);
        }

        /// <summary>
        /// Applies the cutting planes.
        /// </summary>
        /// <param name="forceUpdate">Force the geometries to be updated if set to <c>true</c>.</param>
        private void ApplyCuttingGeometries(bool forceUpdate = false)
        {
            lock (this)
            {
                this.newCutGeometries = new Dictionary<Model3D, Geometry3D>();
                this.newOriginalGeometries = new Dictionary<Model3D, Geometry3D>();
                this.Children.Traverse<GeometryModel3D>((m, t) => this.ApplyCuttingPlanesToModel(m, t, forceUpdate));
                this.cutGeometries = this.newCutGeometries;
                this.originalGeometries = this.newOriginalGeometries;
            }
        }

        /// <summary>
        /// Applies the cutting planes to the model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="updateRequired">An update is required if set to <c>true</c>.</param>
        /// <exception cref="System.InvalidOperationException">No inverse transform.</exception>
        /// <exception cref="System.NotImplementedException"></exception>
        private void ApplyCuttingPlanesToModel(GeometryModel3D model, Transform3D transform, bool updateRequired)
        {
            if (model.Geometry == null)
            {
                return;
            }

            if (!this.IsEnabled)
            {
                updateRequired = true;
            }

            Geometry3D cutGeometry;
            if (this.cutGeometries.TryGetValue(model, out cutGeometry))
            {
                // ReSharper disable once RedundantNameQualifier
                if (object.ReferenceEquals(cutGeometry, model.Geometry))
                {
                    updateRequired = true;
                }
            }

            Geometry3D originalGeometry;
            if (!this.originalGeometries.TryGetValue(model, out originalGeometry))
            {
                originalGeometry = model.Geometry;
                updateRequired = true;
            }

            this.newOriginalGeometries.Add(model, originalGeometry);

            if (!updateRequired)
            {
                return;
            }

            var newGeometry = originalGeometry;
            var originalMeshGeometry = originalGeometry as MeshGeometry3D;

            if (this.IsEnabled && originalMeshGeometry != null)
            {
                var inverseTransform = transform.Inverse;
                if (inverseTransform == null)
                {
                    throw new InvalidOperationException("No inverse transform.");
                }

                switch (this.Operation)
                {
                    case CuttingOperation.Intersect:

                        var intersectedGeometry = originalMeshGeometry;

                        // Calculate the intersection of all the intersections
                        foreach (var cp in this.CuttingPlanes)
                        {
                            intersectedGeometry = this.Intersect(intersectedGeometry, inverseTransform, cp, false);
                        }

                        newGeometry = intersectedGeometry;
                        break;
                    case CuttingOperation.Subtract:
                        var builder = new MeshBuilder(originalMeshGeometry.Normals.Any(), originalMeshGeometry.TextureCoordinates.Any());

                        // Calculate the union of all complement intersections
                        foreach (var cp in this.CuttingPlanes)
                        {
                            var cg = this.Intersect(originalMeshGeometry, inverseTransform, cp, true);
                            builder.Append(cg);
                        }

                        newGeometry = builder.ToMesh(true);
                        break;
                }
            }

            model.Geometry = newGeometry;
            this.newCutGeometries.Add(model, originalMeshGeometry);
        }

        /// <summary>
        /// Intersects the specified source mesh geometry with the specified plane.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="inverseTransform">The inverse transform of the source.</param>
        /// <param name="plane">The plane.</param>
        /// <param name="complement">Cut with the complement set if set to <c>true</c>.</param>
        /// <returns>The intersected geometry.</returns>
        private MeshGeometry3D Intersect(MeshGeometry3D source, GeneralTransform3D inverseTransform, Plane3D plane, bool complement)
        {
            var p = inverseTransform.Transform(plane.Position);
            var p2 = inverseTransform.Transform(plane.Position + plane.Normal);
            var n = p2 - p;

            if (complement)
            {
                n *= -1;
            }

            return MeshGeometryHelper.Cut(source, p, n);
        }
    }
}