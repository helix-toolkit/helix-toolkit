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
        public static Point3D GetCenterPoint3D(this Rect3D rect)
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
        public static Rect3D ExpandRect3D(Rect3D rect3d, double expand)
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
        public static Rect3D MergeRect3D(IEnumerable<Rect3D> rects)
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
        /// True if the plane intersects with Rect3D<br/>
        /// False if the plane does not intersect with Rect3D
        /// </returns>
        public static bool Intersects(this Rect3D rect, Point3D planePosition, Vector3D planeNormal)
        {
            /* AABB-Plane intersections https://gdbooks.gitbooks.io/3dcollisions/content/Chapter2/static_aabb_plane.html
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
            Point3D center = rect.GetCenterPoint3D();
            Vector3D centerToCorner = center - rect.Location;
            double extents = Vector3D.DotProduct(centerToCorner, planeNormal);
            double distance = center.DistanceToPlane(planePosition, planeNormal);
            return Math.Abs(distance) <= extents;
        }
    }
}
