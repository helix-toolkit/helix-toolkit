// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineBuilder.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using global::SharpDX;
using Vector3D = global::SharpDX.Vector3;
using Vector3 = global::SharpDX.Vector3;
using Point3D = global::SharpDX.Vector3;    
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    using Core;  
    public class LineBuilder
    {
        private Vector3Collection positions;
        private IntCollection lineListIndices;
        
        /// <summary>
        /// 
        /// </summary>
        public LineBuilder()
        {
            positions = new Vector3Collection();
            // textureCoordinates = new List<Point>();
            lineListIndices = new IntCollection();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isClosed"></param>
        /// <param name="points"></param>
        public void Add(bool isClosed, params Point3D[] points)
        {
            int i0 = positions.Count;
            foreach (var p in points) positions.Add(p);
            for (int i = 0; i + 1 < points.Length; i++)
            {
                this.lineListIndices.Add(i0 + i);
                this.lineListIndices.Add(i0 + i + 1);
            }

            if (isClosed)
            {
                this.lineListIndices.Add(i0 + points.Length - 1);
                this.lineListIndices.Add(i0);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="xlength"></param>
        /// <param name="ylength"></param>
        /// <param name="zlength"></param>
        public void AddBox(Point3D center, double xlength, double ylength, double zlength)
        {
            int i0 = positions.Count;
            var dx = new Vector3D((float)xlength/2f, 0, 0);
            var dy = new Vector3D(0, (float)ylength/2f, 0);
            var dz = new Vector3D(0, 0, (float)zlength/2f);
            this.Add(true, center - dx - dy - dz, center + dx - dy - dz, center + dx + dy - dz, center - dx + dy - dz);
            this.Add(true, center - dx - dy + dz, center + dx - dy + dz, center + dx + dy + dz, center - dx + dy + dz);
            lineListIndices.AddRange(new[] { i0 + 0, i0 + 4, i0 + 1, i0 + 5, i0 + 2, i0 + 6, i0 + 3, i0 + 7 });
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public void AddLine(Point3D p1, Point3D p2)
        {
            int i0 = positions.Count;
            this.positions.Add(p1);
            this.positions.Add(p2);
            this.lineListIndices.Add(i0);
            this.lineListIndices.Add(i0 + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void AddGrid(BoxFaces plane, int columns, int rows, float width, float height)
        {
            // checks
            if (columns < 2 || rows < 2)
            {
                throw new ArgumentNullException("columns or rows too small");
            }
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentNullException("width or height too small");
            }

            // step
            var stepy = height / (rows - 1);
            var stepx = width / (columns - 1);

            float minx = 0, miny = 0;
            float maxx = width;
            float maxy = height;

            for (int y = 0; y < rows; y++)
            {
                this.AddLine(new Vector3(minx, stepy * y, 0), new Vector3(maxx, stepy * y, 0));
            }
            for (int x = 0; x < columns; x++)
            {
                this.AddLine(new Vector3(stepx * x, miny, 0), new Vector3(stepx * x, maxy, 0));           
            }
        }

        /// <summary>
        /// Creates the resulting <see cref="LineGeometry3D"/>.
        /// </summary>
        /// <param name="unshareVertices">
        /// If true, the resulting <see cref="LineGeometry3D"/> has no shared vertices.
        /// </param>
        /// <returns>Returns the resulting <see cref="LineGeometry3D"/>.</returns>
        public LineGeometry3D ToLineGeometry3D(bool unshareVertices = false)
        {
            if (unshareVertices)
            {
                var count = this.lineListIndices.Count;
                var pos = new Vector3Collection(count);
                var idx = new IntCollection(count);
                for (var i = 0; i < count; i++)
                {
                    pos.Add(this.positions[this.lineListIndices[i]]);
                    idx.Add(i);
                }

                return new LineGeometry3D { Positions = pos, Indices = idx };
            }

            return new LineGeometry3D { Positions = this.positions, Indices = this.lineListIndices };
        }
               
        /// <summary>
        /// Generates a square grid with a step of 1.0 
        /// </summary>
        /// <returns></returns>
        public static LineGeometry3D GenerateGrid(int width = 10)
        {
            return GenerateGrid(Vector3.UnitY, 0, width);
        }

        /// <summary>
        /// Generates a square grid with a step of 1.0 
        /// </summary>
        /// <returns></returns>
        public static LineGeometry3D GenerateGrid(Vector3 plane, int min0 = 0, int max0 = 10, int min1 = 0, int max1 = 10)
        {
            var grid = new LineBuilder();
            //int width = max - min;
            if (plane == Vector3.UnitX)
            {
                for (int i = min0; i <= max0; i++)
                {
                    grid.AddLine(new Vector3(0, i, min1), new Vector3(0, i, max1));                    
                }
                for (int i = min1; i <= max1; i++)
                {                    
                    grid.AddLine(new Vector3(0, min0, i), new Vector3(0, max0, i));
                }
            }
            else if (plane == Vector3.UnitY)
            {
                for (int i = min0; i <= max0; i++)
                {
                    grid.AddLine(new Vector3(i, 0, min1), new Vector3(i, 0, max1));
                    
                }
                for (int i = min1; i <= max1; i++)
                {
                    grid.AddLine(new Vector3(min0, 0, i), new Vector3(max0, 0, i));
                }
            }
            else
            {
                for (int i = min0; i <= max0; i++)
                {
                    grid.AddLine(new Vector3(i, min1, 0), new Vector3(i, max1, 0));                    
                }
                for (int i = min1; i <= max1; i++)
                {
                    grid.AddLine(new Vector3(min0, i, 0), new Vector3(max0, i, 0));
                }
            }

            return grid.ToLineGeometry3D();
        }

        
        /// <summary>
        /// Generates a square grid with a step of 1.0 
        /// </summary>
        /// <returns></returns>
        public static LineGeometry3D GenerateGrid(Vector3 plane, int min = 0, int max = 10)
        {
            var grid = new LineBuilder();
            //int width = max - min;
            if (plane == Vector3.UnitX)
            {
                for (int i = min; i <= max; i++)
                {
                    grid.AddLine(new Vector3(0, i, min), new Vector3(0, i, max));
                    grid.AddLine(new Vector3(0, min, i), new Vector3(0, max, i));
                }
            }
            else if (plane == Vector3.UnitY)
            {
                for (int i = min; i <= max; i++)
                {
                    grid.AddLine(new Vector3(i, 0, min), new Vector3(i, 0, max));
                    grid.AddLine(new Vector3(min, 0, i), new Vector3(max, 0, i));
                }
            }
            else
            {
                for (int i = min; i <= max; i++)
                {
                    grid.AddLine(new Vector3(i, min, 0), new Vector3(i, max, 0));
                    grid.AddLine(new Vector3(min, i, 0), new Vector3(max, i, 0));
                }
            }

            return grid.ToLineGeometry3D();
        }

        /// <summary>
        /// Returns a line geometry of the axis-aligned bounding-box of the given mesh.
        /// </summary>
        /// <param name="mesh">Input mesh for the computation of the b-box</param>
        /// <returns></returns>
        public static LineGeometry3D GenerateBoundingBox(Geometry3D mesh)
        {
            var bb = BoundingBoxExtensions.FromPoints(mesh.Positions);
            return GenerateBoundingBox(bb);
        }

        /// <summary>
        /// Returns a line geometry of the axis-aligned bounding-box of the given mesh.
        /// </summary>
        /// <param name="points">Input points for the computation of the b-box</param>
        /// <returns></returns>
        public static LineGeometry3D GenerateBoundingBox(Vector3[] points)
        {
            var bb = global::SharpDX.BoundingBox.FromPoints(points);
            return GenerateBoundingBox(bb);
        }

        /// <summary>
        /// Returns a line geometry of the axis-aligned bounding-box.
        /// </summary>
        /// <param name="bb">The bounding-box</param>
        /// <returns></returns>
        public static LineGeometry3D GenerateBoundingBox(global::SharpDX.BoundingBox bb)
        {            
            var cc = bb.GetCorners();
            var ll = new LineBuilder();
            ll.AddLine(cc[0], cc[1]);
            ll.AddLine(cc[1], cc[2]);
            ll.AddLine(cc[2], cc[3]);
            ll.AddLine(cc[3], cc[0]);

            ll.AddLine(cc[4], cc[5]);
            ll.AddLine(cc[5], cc[6]);
            ll.AddLine(cc[6], cc[7]);
            ll.AddLine(cc[7], cc[4]);

            ll.AddLine(cc[0], cc[4]);
            ll.AddLine(cc[1], cc[5]);
            ll.AddLine(cc[2], cc[6]);
            ll.AddLine(cc[3], cc[7]);
            return ll.ToLineGeometry3D();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="radius"></param>
        /// <param name="segments"></param>
        /// <returns></returns>
        public static LineGeometry3D GenerateCircle(Vector3 plane, float radius, int segments)
        {
            if (segments < 3)
            {
                throw new ArgumentNullException("too few segments, at least 3");
            }

            var circle = new LineBuilder();

            float sectionAngle = (float)(2.0 * Math.PI / segments);

            if (plane == Vector3.UnitX)
            {
                Point3D start = new Point3D(0.0f, 0.0f, radius);
                Point3D current = new Point3D(0.0f, 0.0f, radius);
                Point3D next = new Point3D(0.0f, 0.0f, 0.0f);

                for (int i = 1; i < segments; i++)
                {
                    next.Z = radius * (float)Math.Cos(i * sectionAngle);
                    next.Y = radius * (float)Math.Sin(i * sectionAngle);

                    circle.AddLine(current, next);

                    current = next;
                }

                circle.AddLine(current, start);
            }
            else if (plane == Vector3.UnitY)
            {
                Point3D start = new Point3D(radius, 0.0f, 0.0f);
                Point3D current = new Point3D(radius, 0.0f, 0.0f);
                Point3D next = new Point3D(0.0f, 0.0f, 0.0f);

                for (int i = 1; i < segments; i++)
                {
                    next.X = radius * (float)Math.Cos(i * sectionAngle);
                    next.Z = radius * (float)Math.Sin(i * sectionAngle);

                    circle.AddLine(current, next);

                    current = next;
                }

                circle.AddLine(current, start);
            }
            else
            {
                Point3D start = new Point3D(0.0f, radius, 0.0f);
                Point3D current = new Point3D(0.0f, radius, 0.0f);
                Point3D next = new Point3D(0.0f, 0.0f, 0.0f);

                for (int i = 1; i < segments; i++)
                {
                    next.Y = radius * (float)Math.Cos(i * sectionAngle);
                    next.X = radius * (float)Math.Sin(i * sectionAngle);

                    circle.AddLine(current, next);

                    current = next;
                }

                circle.AddLine(current, start);
            }

            return circle.ToLineGeometry3D();
        }

        /// <summary>
        /// ~7874015 tests per second, 3.36 times faster than GetRayToLineDistance()
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="closest"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float GetPointToLineDistance2D(ref Vector3 pt, ref Vector3 p0, ref Vector3 p1, out Vector3 closest, out float t)
        {
            float dx = p1.X - p0.X;
            float dy = p1.Y - p0.Y;
            if (Math.Abs(dx) < float.Epsilon && Math.Abs(dy) < float.Epsilon)
            {
                // The points are too close together.
                closest = p0;
                dx = pt.X - p0.X;
                dy = pt.Y - p0.Y;
                t = 0f;
                return (float)Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate scale factor t of intersection.
            t = ((pt.X - p0.X) * dx + (pt.Y - p0.Y) * dy) / (dx * dx + dy * dy);

            // Test, if t inside line bounds.
            if (t < 0)
            {
                closest = new Vector3(p0.X, p0.Y, p0.Z);
                t = 0f;
                dx = pt.X - p0.X;
                dy = pt.Y - p0.Y;
            }
            else if (t > 1)
            {
                closest = new Vector3(p1.X, p1.Y, p1.Z);
                t = 1f;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else
            {
                closest = new Vector3(p0.X + t * dx, p0.Y + t * dy, pt.Z);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="sp"></param>
        /// <param name="tp"></param>
        /// <param name="sc"></param>
        /// <param name="tc"></param>
        /// <returns></returns>
        public static float GetRayToLineDistance(
            Ray ray, Vector3 t0, Vector3 t1, out Vector3 sp, out Vector3 tp, out float sc, out float tc)
        {
            var s0 = ray.Position;
            var s1 = ray.Position + ray.Direction;
            return GetLineToLineDistance(s0, s1, t0, t1, out sp, out tp, out sc, out tc, true);
        }

        /// <summary>
        /// Source: http://geomalgorithms.com/a07-_distance.html
        /// ~2341920 tests per second</summary>
        /// <param name="s0"></param>
        /// <param name="s1"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="sp"></param>
        /// <param name="tp"></param>
        /// <param name="sc"></param>
        /// <param name="tc"></param>
        /// <param name="sIsRay"></param>
        /// <returns></returns>
        public static float GetLineToLineDistance(
            Vector3 s0, Vector3 s1, Vector3 t0, Vector3 t1, out Vector3 sp, out Vector3 tp, out float sc, out float tc, bool sIsRay = false)
        {
            Vector3 u = s1 - s0;
            Vector3 v = t1 - t0;
            Vector3 w = s0 - t0;

            float a = Vector3.Dot(u, u); // always >= 0
            float b = Vector3.Dot(u, v);
            float c = Vector3.Dot(v, v); // always >= 0
            float d = Vector3.Dot(u, w);
            float e = Vector3.Dot(v, w);
            float D = a * c - b * b;     // always >= 0
            float sN, sD = D;            // sc = sN / sD, default sD = D >= 0
            float tN, tD = D;            // tc = tN / tD, default tD = D >= 0

            // compute the line parameters of the two closest points
            if (D < float.Epsilon)
            {
                // the lines are almost parallel
                sN = 0.0f; // force using point P0 on segment S1
                sD = 1.0f; // to prevent possible division by 0.0 later
                tN = e;
                tD = c;
            }
            else
            {
                // get the closest points on the infinite lines
                sN = (b * e - c * d);
                tN = (a * e - b * d);

                if (!sIsRay)
                {
                    if (sN < 0.0f)
                    {
                        // sc < 0 => the s=0 edge is visible
                        sN = 0.0f;
                        tN = e;
                        tD = c;
                    }
                    else if (sN > sD)
                    {
                        // sc > 1  => the s=1 edge is visible
                        sN = sD;
                        tN = e + b;
                        tD = c;
                    }                    
                }
            }

            if (tN < 0.0f)
            {
                // tc < 0 => the t=0 edge is visible
                tN = 0.0f;
                // recompute sc for this edge
                if (-d < 0.0f)
                {
                    sN = 0.0f;
                }
                else if (-d > a)
                {
                    sN = sD;
                }
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD)
            {
                // tc > 1  => the t=1 edge is visible
                tN = tD;
                // recompute sc for this edge
                if ((-d + b) < 0.0f)
                {
                    sN = 0;
                }
                else if ((-d + b) > a)
                {
                    sN = sD;
                }
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }

            // finally do the division to get sc and tc
            sc = (Math.Abs(sN) < float.Epsilon ? 0.0f : sN / sD);
            tc = (Math.Abs(tN) < float.Epsilon ? 0.0f : tN / tD);

            // get the difference of the two closest points
            sp = s0 + (sc * u);
            tp = t0 + (tc * v);
            var tv = sp - tp;

            return tv.Length(); // return the closest distance
        }
    }
}