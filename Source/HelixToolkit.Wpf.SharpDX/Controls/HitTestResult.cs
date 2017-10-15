// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HitTestResult.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a hit test result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Provides a hit test result.
    /// </summary>
    public class HitTestResult
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

        /// <summary>
        /// The hitted triangle vertex indices.
        /// </summary>
        public System.Tuple<int, int, int> TriangleIndices { set; get; }
    }

    /// <summary>
    /// A specialized line hit test result.
    /// </summary>
    public class LineHitTestResult : HitTestResult
    {
        /// <summary>
        /// Gets or sets the index of the line segment that was hit.
        /// </summary>
        public int LineIndex { get; set; }

        /// <summary>
        /// Gets or sets the shortest distance between the hit test ray and the line that was hit.
        /// </summary>
        public double RayToLineDistance { get; set; }

        /// <summary>
        /// Gets or sets the scalar of the closest point on the hit test ray.
        /// </summary>
        public double RayHitPointScalar { get; set; }

        /// <summary>
        /// Gets or sets the scalar of the closest point on the line that was hit.
        /// </summary>
        public double LineHitPointScalar { get; set; }
    }
}