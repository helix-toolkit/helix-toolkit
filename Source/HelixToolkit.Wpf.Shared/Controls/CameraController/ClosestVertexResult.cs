using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf
{
    /// <summary>
    /// The result of a hit test where the point with the smallest 2D distance from a given point is searched.
    /// </summary>
    internal class ClosestVertexResult
    {
        public ClosestVertexResult(GeometryModel3D model, MeshGeometry3D geometry, Point3D closestPoint, Point closestPointIn2D, double distanceToPoint2D)
        {
            Model = model;
            Geometry = geometry;
            ClosestPoint = closestPoint;
            DistanceToPoint2D = distanceToPoint2D;
            ClosestPointIn2D = closestPointIn2D;
        }

        /// <summary>
        /// The mesh vertex closest to the searched coordinate.
        /// </summary>
        public Point3D ClosestPoint { get; }

        /// <summary>
        /// The 2D coordinate matching the found mesh vertex.
        /// </summary>
        public Point ClosestPointIn2D { get; }

        /// <summary>
        /// The mesh geometry the found point belongs to.
        /// </summary>
        public MeshGeometry3D Geometry { get; }

        /// <summary>
        /// The geometry model the point belongs to.
        /// </summary>
        public GeometryModel3D Model { get; }

        /// <summary>
        /// The 2D distance from the found point to the point that was searched for.
        /// </summary>
        public double DistanceToPoint2D { get; }
    }
}