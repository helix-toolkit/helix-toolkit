namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides a hit test result.
    /// </summary>
    public struct HitTestResult
    {
        /// <summary>
        /// Gets the distance between the hit intersection and the inner coordinate space
        /// of the System.Windows.Media.Media3D.Visual3D which initiated the hit test.
        /// 
        /// Double that indicates the distance between the hit intersection and the inner
        /// coordinate space of the System.Windows.Media.Media3D.Visual3D which initiated
        /// the hit test.
        /// </summary>
        public double Distance { get; set; }

        /// <summary>
        /// Gets the Model3D intersected by the ray along which the hit test was performed.
        /// Model3D intersected by the ray.
        /// </summary>        
        public GeometryModel3D ModelHit { get; set; }

        /// <summary>
        /// Gets the Point3D at the intersection between the ray along which the hit
        /// test was performed and the hit object.
        /// Point3D at which the hit object was intersected by the ray.
        /// </summary>
        public Point3D PointHit { get; set; }

        /// <summary>
        /// The normal vector of the triangle hit.
        /// </summary>
        public Vector3D NormalAtHit { get; set; }

        /// <summary>
        /// Indicates if this Result has data from a valid hit.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// This is a tag to add additional data.
        /// </summary>
        public object Tag { get; set; }
    }
}