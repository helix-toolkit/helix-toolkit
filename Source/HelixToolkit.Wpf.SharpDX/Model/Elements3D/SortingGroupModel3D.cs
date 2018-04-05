namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Media3D;
    using global::SharpDX;
    using HelixToolkit.Wpf.SharpDX.Cameras;
    using HelixToolkit.Wpf.SharpDX.Render;

    /// <summary>
    /// Specifies the sorting method for the SortingVisual3D.
    /// </summary>
    public enum SortingMethod
    {
        /// <summary>
        /// Sort on the distance from camera to bounding box center.
        /// </summary>
        BoundingBoxCenter,

        /// <summary>
        /// Sort on the minimum distance from camera to bounding box corners.
        /// </summary>
        BoundingBoxCorners,

        /// <summary>
        /// Sort on the minimum distance from camera to bounding sphere surface.
        /// </summary>
        BoundingSphereSurface
    }


    /// <summary>
    /// Implements a GroupModel3D that sorts it's transparent items 
    /// by their distance from the camera so they can be properly rendered.
    /// </summary>
    /// <remarks>
    /// The children are sorted by the distance to the camera position. This will not always work when you have overlapping objects.
    /// </remarks>
    public class SortingGroupModel3D : GroupModel3D, ITransformable, IHitable, IVisible
    {
        private List<IRenderable> SortedItems;
        private object Locker = new object();
        private TimeSpan LastSortTime = TimeSpan.MinValue;

        /// <summary>
        /// Identifies the <see cref="Method"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MethodProperty = DependencyProperty.Register(
            "Method",
            typeof(SortingMethod),
            typeof(SortingGroupModel3D),
            new UIPropertyMetadata(SortingMethod.BoundingBoxCorners));

        /// <summary>
        /// Identifies the <see cref="SkipOpaque"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SkipOpaqueProperty =
            DependencyProperty.Register(
                nameof(SkipOpaque), typeof(bool), typeof(SortingGroupModel3D), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="SortingFrequency"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SortingIntervalProperty =
            DependencyProperty.Register(
                "SortingInterval", typeof(int), typeof(SortingGroupModel3D), new UIPropertyMetadata(100));

        /// <summary>
        /// Sorting interval [ms].
        /// </summary> 
        public int SortingInterval
        {
            get => (int)this.GetValue(SortingIntervalProperty);
            set => this.SetValue(SortingIntervalProperty, value);
        }


        /// <summary>
        /// Gets or sets a value indicating whether to sort only transparent items 
        /// (and render opaques first)
        /// </summary>
        public bool SkipOpaque
        {
            get => (bool)this.GetValue(SkipOpaqueProperty);
            set => this.SetValue(SkipOpaqueProperty, value);
        }

        /// <summary>
        /// Gets or sets the sorting method.
        /// </summary>
        /// <value> The method. </value>
        public SortingMethod Method
        {
            get => (SortingMethod)this.GetValue(MethodProperty);
            set => this.SetValue(MethodProperty, value);
        }




        public override void Update(IRenderContext renderContext)
        {
            //sort children 
            if (SortedItems?.Count != Items.Count()
                || (renderContext.TimeStamp - LastSortTime).TotalMilliseconds > SortingInterval)
            {
                var items = SortItems(renderContext.Camera);
                Children.Clear();
                foreach (var it in items)
                    Children.Add(it);

                lock (Locker)
                    SortedItems = items;
                LastSortTime = renderContext.TimeStamp;
                //Children.Clear();
                //ItemsSource.Clear();
                //foreach (var it in sortedChildren)
                //    ItemsSource.Add(it);
            }

            base.Update(renderContext);
        }
        protected override void OnRender(IRenderContext renderContext, DeviceContextProxy deviceContext)
        {
            lock (Locker)
                foreach (var c in SortedItems)
                {
                    c.Render(renderContext, deviceContext);
                }

        }

        /// <summary>
        /// The sort children.
        /// </summary>
        private List<IRenderable> SortItems(CameraCore camera)
        {
            var cam = camera;
            if (cam == null)
            {
                return new List<IRenderable>(0);
            }

            var cameraPos = cam.Position;
            var transform = this.Transform;

            List<GeometryModel3D> transparentChildren = new List<GeometryModel3D>();
            List<IRenderable> opaqueChildren = new List<IRenderable>();

            foreach (var child in this.Items)
            {
                if (child is MeshGeometryModel3D mgm3)
                {
                    if (IsTransparent(mgm3.Material) || !SkipOpaque)
                        transparentChildren.Add(mgm3);
                    else
                        opaqueChildren.Add(mgm3);
                }
                else
                {
                    opaqueChildren.Add(child);
                }
            }
            // sort the children by distance from camera (note that OrderBy is a stable sort algorithm)
            var sortedTransparentChildren =
                transparentChildren.OrderBy(item => -this.GetCameraDistance(item, cameraPos, transform)).ToList();

            opaqueChildren.AddRange(sortedTransparentChildren);
            return opaqueChildren;
        }

        private static bool IsTransparent(Material material)
        {
            if (material is PhongMaterial pm)
            {
                if (pm.DiffuseColor.Alpha < 1 || pm.ReflectiveColor.Alpha < 1 || pm.SpecularColor.Alpha < 1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the distance from the camera for the specified visual.
        /// </summary>
        /// <param name="c">
        /// The visual.
        /// </param>
        /// <param name="cameraPos">
        /// The camera position.
        /// </param>
        /// <param name="transform">
        /// The total transform of the visual.
        /// </param>
        /// <returns>
        /// The camera distance.
        /// </returns>
        private double GetCameraDistance(GeometryModel3D c, Vector3 cameraPos, Transform3D transform)
        {
            double distFromCamera(float x, float y, float z)
            {
                var diff = new Vector3(cameraPos.X - (float)x, cameraPos.Y - (float)y, cameraPos.Z - (float)z);
                return diff.LengthSquared();
            }
            switch (this.Method)
            {
                case SortingMethod.BoundingBoxCenter:
                    var bounds = c.BoundsWithTransform;
                    var mid = (bounds.Maximum + bounds.Minimum) / 2;
                    return (mid - cameraPos).LengthSquared();


                case SortingMethod.BoundingBoxCorners:
                    var bmin = c.BoundsWithTransform.Minimum;
                    var bmax = c.BoundsWithTransform.Maximum;
                    double d = double.MaxValue;
                    d = Math.Min(d, distFromCamera(bmin.X, bmin.Y, bmin.Z));
                    d = Math.Min(d, distFromCamera(bmin.X, bmin.Y, bmax.Z));
                    d = Math.Min(d, distFromCamera(bmin.X, bmax.Y, bmin.Z));
                    d = Math.Min(d, distFromCamera(bmin.X, bmax.Y, bmax.Z));
                    d = Math.Min(d, distFromCamera(bmax.X, bmin.Y, bmin.Z));
                    d = Math.Min(d, distFromCamera(bmax.X, bmin.Y, bmax.Z));
                    d = Math.Min(d, distFromCamera(bmax.X, bmax.Y, bmin.Z));
                    d = Math.Min(d, distFromCamera(bmax.X, bmax.Y, bmax.Z));
                    return d;
                default:
                    var boundingSphere = c.BoundsSphereWithTransform;//new BoundingSphere(bounds.c BoundingSphere.CreateFromRect3D(bounds);
                    return (boundingSphere.Center - cameraPos).LengthSquared();
            }

        }
    }
}
