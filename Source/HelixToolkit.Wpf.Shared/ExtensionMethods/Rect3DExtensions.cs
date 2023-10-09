/*
 * The MIT License (MIT)
 * Copyright (c) 2018 Helix Toolkit contributors
 * See the LICENSE file in the project root for more information.
 */

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Extension methods for <see cref="Rect3D"/>.
    /// </summary>
    public static class Rect3DExtensions
    {
        /// <summary>
        /// Get the center point of <see cref="Rect3D"/>
        /// </summary>
        /// <param name="rect">The given Rect3D</param>
        /// <returns>The center point of given Rect3D</returns>
        public static Point3D GetCenter(this Rect3D rect)
        {
            return new Point3D(
                rect.X + rect.SizeX / 2,
                rect.Y + rect.SizeY / 2,
                rect.Z + rect.SizeZ / 2);
        }

        /// <summary>
        /// Expands the size of <see cref="Rect3D"/>
        /// in 3 directions by the given amount
        /// </summary>
        /// <param name="rect3d">The given Rect3D</param>
        /// <param name="expand">Amount of expansion</param>
        /// <returns>A newly expanded Rect3D</returns>
        public static Rect3D Expand(this Rect3D rect3d, double expand)
        {
            if (rect3d == Rect3D.Empty
               || double.IsNaN(expand)
               || double.IsInfinity(expand)
               || double.IsNegativeInfinity(expand))
                return rect3d;
            double x = rect3d.X - expand;
            double y = rect3d.Y - expand;
            double z = rect3d.Z - expand;
            double sizeX = rect3d.SizeX + 2 * expand;
            double sizeY = rect3d.SizeY + 2 * expand;
            double sizeZ = rect3d.SizeZ + 2 * expand;
            return new Rect3D(x, y, z, sizeX, sizeY, sizeZ);
        }

        /// <summary>
        /// Merge a collection of <see cref="Rect3D"/>
        /// into one total <see cref="Rect3D"/>
        /// to be 
        /// </summary>
        /// <param name="rects">Collection Rect3D</param>
        /// <returns>A newly total Rect3D</returns>
        public static Rect3D Merge(this IEnumerable<Rect3D> rects)
        {
            Rect3D result = Rect3D.Empty;
            foreach (var rect in rects)
            {
                result.Union(rect);
            }
            return result;
        }
        /// <summary>
        /// Check whether a plane intersects with a given <see cref="Rect3D"/> box.
        /// </summary>
        /// <param name="rect">The Rect3D bounding box</param>
        /// <param name="planePosition">The position of plane</param>
        /// <param name="planeNormal">The normal vector of plan</param>
        /// <returns>
        /// Whether the two objects intersected.
        /// </returns>
        public static PlaneIntersectionType Intersects(this Rect3D rect, Point3D planePosition, Vector3D planeNormal)
        {
            /* AABB-Plane intersections 
             * https://gdbooks.gitbooks.io/3dcollisions/content/Chapter2/static_aabb_plane.html
             * http://what-when-how.com/advanced-methods-in-computer-graphics/collision-detection-advanced-methods-in-computer-graphics-part-3/
             * 
             *      _______________        ^ Normal
             *     |               |       |
             *     |               |       |
             *     |       *C------|------ + --+
             *     |               |      e|   |
             *     |               |       |   |
             *   O *--------------- -------+   |d
             *                             |   |
             *                             |   |
             * ----------------------------+---+----Plane  
             */

            planeNormal.Normalize();
            Vector3D absPlaneNormal = new Vector3D(Math.Abs(planeNormal.X), Math.Abs(planeNormal.Y), Math.Abs(planeNormal.Z));
            Point3D center = rect.GetCenter();
            Vector3D centerToCorner = center - rect.Location;
            double extents = Vector3D.DotProduct(centerToCorner, absPlaneNormal);
            double distance = center.DistanceToPlane(planePosition, planeNormal);

            if (Math.Abs(distance) <= extents)
            {
                return PlaneIntersectionType.Intersecting;
            }
            else if (distance > 0)
            {
                return PlaneIntersectionType.Front;
            }
            else
            {
                return PlaneIntersectionType.Back;
            }
        }
    }
}
