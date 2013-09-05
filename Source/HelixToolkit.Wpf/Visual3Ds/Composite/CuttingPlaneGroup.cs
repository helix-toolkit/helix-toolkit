// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CuttingPlaneGroup.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that applies cutting planes to all children.
    /// </summary>
    public class CuttingPlaneGroup : RenderingModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            "IsEnabled", typeof(bool), typeof(CuttingPlaneGroup), new UIPropertyMetadata(false, IsEnabledChanged));

        /// <summary>
        /// The cut geometries.
        /// </summary>
        private Dictionary<Model3D, Geometry3D> CutGeometries = new Dictionary<Model3D, Geometry3D>();

        /// <summary>
        /// The new cut geometries.
        /// </summary>
        private Dictionary<Model3D, Geometry3D> NewCutGeometries;

        /// <summary>
        /// The new original geometries.
        /// </summary>
        private Dictionary<Model3D, Geometry3D> NewOriginalGeometries;

        /// <summary>
        /// The original geometries.
        /// </summary>
        private Dictionary<Model3D, Geometry3D> OriginalGeometries = new Dictionary<Model3D, Geometry3D>();

        /// <summary>
        /// The force update.
        /// </summary>
        private bool forceUpdate;

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
        /// <value>The cutting planes.</value>
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
        /// The is sorting changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CuttingPlaneGroup)d).OnIsEnabledChanged();
        }

        /// <summary>
        /// Applies the cutting planes.
        /// </summary>
        /// <param name="forceUpdate">
        /// if set to <c>true</c> [force update].
        /// </param>
        private void ApplyCuttingPlanes(bool forceUpdate = false)
        {
            lock (this)
            {
                this.NewCutGeometries = new Dictionary<Model3D, Geometry3D>();
                this.NewOriginalGeometries = new Dictionary<Model3D, Geometry3D>();
                this.forceUpdate = forceUpdate;
                Visual3DHelper.Traverse<GeometryModel3D>(this.Children, this.ApplyCuttingPlanesToModel);
                this.CutGeometries = this.NewCutGeometries;
                this.OriginalGeometries = this.NewOriginalGeometries;
            }
        }

        /// <summary>
        /// Applies the cutting planes to the model.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="transform">
        /// The transform.
        /// </param>
        private void ApplyCuttingPlanesToModel(GeometryModel3D model, Transform3D transform)
        {
            if (model.Geometry == null)
            {
                return;
            }

            bool updateRequired = this.forceUpdate;

            if (!this.IsEnabled)
            {
                updateRequired = true;
            }

            Geometry3D cutGeometry;
            if (this.CutGeometries.TryGetValue(model, out cutGeometry))
            {
                if (cutGeometry != model.Geometry)
                {
                    updateRequired = true;
                }
            }

            Geometry3D originalGeometry;
            if (!this.OriginalGeometries.TryGetValue(model, out originalGeometry))
            {
                originalGeometry = model.Geometry;
                updateRequired = true;
            }

            this.NewOriginalGeometries.Add(model, originalGeometry);

            if (!updateRequired)
            {
                return;
            }

            var g = originalGeometry as MeshGeometry3D;

            if (this.IsEnabled)
            {
                var inverseTransform = transform.Inverse;
                foreach (var cp in this.CuttingPlanes)
                {
                    var p = inverseTransform.Transform(cp.Position);
                    var p2 = inverseTransform.Transform(cp.Position + cp.Normal);
                    var n = p2 - p;

                    // var p = transform.Transform(cp.Position);
                    // var n = transform.Transform(cp.Normal);
                    g = MeshGeometryHelper.Cut(g, p, n);
                }
            }

            model.Geometry = g;
            this.NewCutGeometries.Add(model, g);
        }

        /// <summary>
        /// The compositiontarget rendering.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void OnCompositionTargetRendering(object sender, RenderingEventArgs e)
        {
            if (this.IsEnabled)
            {
                this.ApplyCuttingPlanes();
            }
        }

        /// <summary>
        /// Called when IsEnabled is changed.
        /// </summary>
        private void OnIsEnabledChanged()
        {
            if (this.IsEnabled)
            {
                this.SubscribeToRenderingEvent();
            }
            else
            {
                this.UnsubscribeRenderingEvent();
            }

            this.ApplyCuttingPlanes(true);
        }

    }
}