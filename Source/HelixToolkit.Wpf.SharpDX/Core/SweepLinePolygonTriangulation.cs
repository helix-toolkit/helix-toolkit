// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubchainSamplingPolygonTriangulation.cs" company="Helix Toolkit">
//   Copyright (c) 2016 Franz Spitaler
// </copyright>
// <summary>
//   A polygon triangulator for simple polygons with no holes. Expected runtime is O(n)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using Point = global::SharpDX.Vector2;
    using PointCollection = System.Collections.Generic.List<global::SharpDX.Vector2>;
    using Int32Collection = System.Collections.Generic.List<int>;
    using System;
    using System.Linq;
    using System.Collections;
    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Based on https://www.cs.princeton.edu/~chazelle/pubs/polygon-triang.pdf ???
    /// References
    /// http://www.cs.uu.nl/docs/vakken/ga/slides3.pdf
    /// </remarks>
    public class SweepLinePolygonTriangulation
    {
        public static Int32Collection Triangulate(IList<Point> polygon)
        {
            // Allocate and initialize List of Indices in Polygon
            var result = new Int32Collection();
            var points = polygon.ToList();

            // Make the Polygon CounterClockWise
            if (!isCCW(polygon))
                points.Reverse();

            // Create Polygon Data Structure
            var poly = new PolygonData(polygon.ToList());

            // Sort Points from highest y to lowest y
            // and if two or more Points have the same y Value from lowest x to highest x Value
            var events = poly.Points;
            events.Sort();

            // Construct Status, a List of Edges left of every Point of the Polygon
            // by shooting a Ray from the Vertex to the left.
            // The Helper Point of that Edge will be used to create Monotone Polygons
            // by adding Diagonals from/to Split- and Merge-Points
            var statusAndHelper = new List<Tuple<PolygonEdge, PolygonPoint>>();

            // Sweep through the Polygon using the sorted Polygon Points
            foreach (var ev in events)
            {

            }

            // Update the Helpers for the left Edges of the Polygon Points


            // 


            return result;
        }


        // Compute the Orientation of the Polygon.
        /// <summary>
        /// Calculates the Orientation.
        /// </summary>
        /// <param name="polygon">The Polygon.</param>
        /// <returns>True if the Polygon is present in a CCW manner.</returns>
        private static bool isCCW(IList<Point> polygon)
        {
            int n = polygon.Count;
            double area = 0.0;
            for (int p = n - 1, q = 0; q < n; p = q++)
            {
                area += polygon[p].X * polygon[q].Y - polygon[q].X * polygon[p].Y;
            }
            return area > 0.0f;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class PolygonEdge
    {
        private PolygonPoint mPointOne;

        public PolygonPoint PointOne
        {
            get { return mPointOne; }
            set { mPointOne = value; }
        }
        private PolygonPoint mPointTwo;

        public PolygonPoint PointTwo
        {
            get { return mPointTwo; }
            set { mPointTwo = value; }
        }
        public PolygonEdge Last
        {
            get
            {
                if (mPointOne != null && mPointOne.EdgeOne != null)
                    return mPointOne.EdgeOne;
                else
                    return null;
            }
        }
        public PolygonEdge Next
        {
            get
            {
                if (mPointTwo != null && mPointTwo.EdgeTwo != null)
                    return mPointTwo.EdgeTwo;
                else
                    return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        internal PolygonEdge(PolygonPoint one, PolygonPoint two)
        {
            this.mPointOne = one;
            this.mPointTwo = two;
        }

        public override string ToString()
        {
            return "From: " + mPointOne + " To: " + mPointTwo;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class PolygonPoint: IComparable<PolygonPoint>
    {
        private Point mPoint;
	    public Point Point
	    {
		    get { return mPoint;}
		    set { mPoint = value;}
	    }
        public float X { get { return this.mPoint.X; } set { this.mPoint.X = value; } }
        public float Y { get { return this.mPoint.Y; } set { this.mPoint.Y = value; } }
        private PolygonEdge mEdgeOne;

        public PolygonEdge EdgeOne
        {
            get { return mEdgeOne; }
            set { mEdgeOne = value; }
        }
        private PolygonEdge mEdgeTwo;
        public PolygonEdge EdgeTwo
        {
            get { return mEdgeTwo; }
            set { mEdgeTwo = value; }
        }
        public PolygonPoint Last {
            get
            {
                if (mEdgeOne != null && mEdgeOne.PointOne != null)
                    return mEdgeOne.PointOne;
                else
                    return null;
            }
        }
        public PolygonPoint Next
        {
            get
            {
                if (mEdgeTwo != null && mEdgeTwo.PointTwo != null)
                    return mEdgeTwo.PointTwo;
                else
                    return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        internal PolygonPoint(Point p){
            this.mPoint = p;
        }

        internal PolygonPointClass PointClass()
        {
            if (Next == null || Last == null)
                throw new HelixToolkitException("No closed Polygon");
            
            /*if (Last < this && Next < this)*/
                return PolygonPointClass.Start;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "X:" + this.X + " Y:" + this.Y;
        }

        public int CompareTo(PolygonPoint second)
        {
            if (this == null || second == null)
                return 0;
            if (this.Y > second.Y || (this.Y == second.Y && this.X < second.X)) return -1;
            else if (this.Y == second.Y && this.X == second.X) return 0;
            else return 1;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class PolygonData
    {
        private List<PolygonPoint> mPoints;

        public List<PolygonPoint> Points
        {
            get { return mPoints; }
            set { mPoints = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        internal PolygonData(List<Point> points)
        {
            // Add Points
            mPoints = new List<PolygonPoint>(points.Select(p => new PolygonPoint(p)));

            // Add Edges to Points
            var cnt = mPoints.Count;
            for (int i = 0; i < cnt; i++)
			{
                var lastIdx = (i + cnt - 1) % cnt;
                var edge = new PolygonEdge(mPoints[lastIdx], mPoints[i]);
                mPoints[lastIdx].EdgeTwo = edge;
                mPoints[i].EdgeOne = edge;
			}
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal enum PolygonPointClass : byte
    {
        Start,
        Stop,
        Split,
        Merge,
        Regular
    }
}
