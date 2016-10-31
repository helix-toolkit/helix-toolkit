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

            // Sort the Input and create the Datastructures
            // Make the Polygon CounterClockWise
            if (!isCCW(polygon))
                points.Reverse();
            // Create Polygon Data Structure
            var poly = new PolygonData(polygon.ToList());
            // Sort Points from highest y to lowest y
            // and if two or more Points have the same y Value from lowest x to highest x Value
            var events = new List<PolygonPoint>(poly.Points);
            events.Sort();
            // Construct Status, a List of Edges left of every Point of the Polygon
            // by shooting a Ray from the Vertex to the left.
            // The Helper Point of that Edge will be used to create Monotone Polygons
            // by adding Diagonals from/to Split- and Merge-Points
            // var statusAndHelper = new BinarySearchTree<Tuple<PolygonEdge, PolygonPoint>, Double>();
            var statusAndHelper = new StatusHelper();
            var diagonals = new SortedList<int, PolygonEdge>();
            
            // Down Sweep
            // Sweep through the Polygon using the sorted Polygon Points
            foreach (var ev in events)
            {
                var evClass = ev.PointClass();
                StatusHelperElement she = null;
                // Start
                switch (evClass)
                {
                    case PolygonPointClass.Start:
                        statusAndHelper.Add(new StatusHelperElement(ev.EdgeTwo, ev));
                        break;
                    case PolygonPointClass.Stop:
                        statusAndHelper.Remove(ev.EdgeOne);
                        break;
                    case PolygonPointClass.Regular:
                        if (ev.Last > ev.Next)
                        {
                            statusAndHelper.Remove(ev.EdgeOne);
                            statusAndHelper.Add(new StatusHelperElement(ev.EdgeTwo, ev));
                        }
                        else
                        {
                            // Search Edge left of ev and set ev as it's Helper
                            she = statusAndHelper.SearchLeft(ev);
                            she.Helper = ev;
                        }
                        break;
                    case PolygonPointClass.Merge:
                        statusAndHelper.Remove(ev.EdgeOne);
                        // Search Edge left of ev and set ev as it's Helper
                        she = statusAndHelper.SearchLeft(ev);
                        she.Helper = ev;
                        break;
                    case PolygonPointClass.Split:
                        // Search Edge left of ev and set ev as it's Helper
                        she = statusAndHelper.SearchLeft(ev);

                        // Chose diagonal from Helper of Edge to Event
                        // Add sorted (from lowest Index to biggest)
                        diagonals.Add(Math.Min(she.Helper.Index, ev.Index), new PolygonEdge(she.Helper, ev));
                        diagonals.Add(Math.Max(she.Helper.Index, ev.Index), new PolygonEdge(she.Helper, ev));

                        // Replace Helper of Edge by Event
                        she.Helper = ev;

                        // Insert the Edge Counterclockwise from ev, with ev as its Helper
                        statusAndHelper.Add(new StatusHelperElement(ev.EdgeTwo, ev));
                        break;
                }
            }
            // Split the Polygon
            var subPolygons = SplitPolygonWithDiagonals(poly, diagonals);

            var monotonePolygons = new List<PolygonData>();
            // Up Sweep for all Sub-Polygons
            foreach (var subPoly in subPolygons)
            {
                events = new List<PolygonPoint>(subPoly.Points);
                events.Sort();
                events.Reverse();
                statusAndHelper = new StatusHelper();
                diagonals = new SortedList<int, PolygonEdge>();

                // Sweep through the Polygon using the sorted Polygon Points
                foreach (var ev in events)
                {
                    var evClass = ev.PointClass(true);
                    StatusHelperElement she = null;
                    // Start
                    switch (evClass)
                    {
                        case PolygonPointClass.Start:
                            statusAndHelper.Add(new StatusHelperElement(ev.EdgeOne, ev));
                            break;
                        case PolygonPointClass.Stop:
                            statusAndHelper.Remove(ev.EdgeTwo);
                            break;
                        case PolygonPointClass.Regular:
                            if (ev.Last > ev.Next)
                            {
                                statusAndHelper.Remove(ev.EdgeTwo);
                                statusAndHelper.Add(new StatusHelperElement(ev.EdgeOne, ev));
                            }
                            else
                            {
                                // Search Edge left of ev and set ev as it's Helper
                                she = statusAndHelper.SearchLeft(ev);
                                she.Helper = ev;
                            }
                            break;
                        case PolygonPointClass.Merge:
                            statusAndHelper.Remove(ev.EdgeTwo);
                            // Search Edge left of ev and set ev as it's Helper
                            she = statusAndHelper.SearchLeft(ev);
                            she.Helper = ev;
                            break;
                        case PolygonPointClass.Split:
                            // Search Edge left of ev and set ev as it's Helper
                            she = statusAndHelper.SearchLeft(ev);

                            // Chose diagonal from Helper of Edge to Event
                            // Add sorted (from lowest Index to biggest)
                            diagonals.Add(Math.Min(she.Helper.Index, ev.Index), new PolygonEdge(she.Helper, ev));
                            diagonals.Add(Math.Max(she.Helper.Index, ev.Index), new PolygonEdge(she.Helper, ev));

                            // Replace Helper of Edge by Event
                            she.Helper = ev;

                            // Insert the Edge Counterclockwise from ev, with ev as its Helper
                            statusAndHelper.Add(new StatusHelperElement(ev.EdgeOne, ev));
                            break;
                    }
                }
                // Split the Polygon
                var monotoneSubPolygons = SplitPolygonWithDiagonals(subPoly, diagonals);
                // Add to List of monotone Polygons
                monotonePolygons.AddRange(monotoneSubPolygons);
            }

            // y-Monotone Polygons
            // Triangulate
            foreach (var monoton in monotonePolygons)
            {
                events = new List<PolygonPoint>(monoton.Points);
                events.Sort();
                var pointStack = new Stack<PolygonPoint>();
                pointStack.Push(events[0]);
                pointStack.Push(events[1]);
                
                var pointCnt = monoton.Points.Count;
                var leftChain = new HashSet<PolygonPoint>();
                var p = events[0];
                do
                {
                    leftChain.Add(p);
                    p = p.Next;
                } while (p != events.Last());
                leftChain.Add(p);
                var rightChain = new HashSet<PolygonPoint>();
                p = events[0];
                do
                {
                    rightChain.Add(p);
                    p = p.Last;
                } while (p != events.Last());
                rightChain.Add(p);
                for (int i = 2; i < pointCnt; i++)
                {
                    var newPoint = events[i];
                    var top = pointStack.Peek();
                    if (!(leftChain.Contains(top) && leftChain.Contains(newPoint) || rightChain.Contains(top) && rightChain.Contains(newPoint)))
                    {
                        var p2 = top;
                        while (pointStack.Count != 0)
                        {
                            top = pointStack.Pop();
                            p2 = top;
                            if (pointStack.Count != 0)
                            {
                                top = pointStack.Pop();
                                if (leftChain.Contains(newPoint))
                                {
                                    result.Add(newPoint.Index);
                                    result.Add(p2.Index);
                                    result.Add(top.Index);
                                }
                                else
                                {
                                    result.Add(newPoint.Index);
                                    result.Add(top.Index);
                                    result.Add(p2.Index);
                                }
                            }
                            if (pointStack.Count != 0)
                                pointStack.Push(top);
                        }
                        pointStack.Push(events[i - 1]);
                        pointStack.Push(newPoint);
                    }
                    else
                    {
                        top = pointStack.Pop();
                        var p2 = top;
                        while (pointStack.Count != 0)
                        {
                            if (rightChain.Contains(top) && isCCW(new List<Point> { newPoint.Point, p2.Point, pointStack.Peek().Point }))
                            {
                                top = pointStack.Pop();
                                result.Add(newPoint.Index);
                                result.Add(p2.Index);
                                result.Add(top.Index);
                                p2 = top;
                            }
                            else if (leftChain.Contains(top) && !isCCW(new List<Point> { newPoint.Point, p2.Point, pointStack.Peek().Point }))
                            {
                                top = pointStack.Pop();
                                result.Add(newPoint.Index);
                                result.Add(top.Index);
                                result.Add(p2.Index);
                                p2 = top;
                            }
                            else
                                break;
                        }
                        pointStack.Push(p2);
                        pointStack.Push(newPoint);
                    }
                }
            }

            return result;
        }

        private static PolygonData[] SplitPolygonWithDiagonals(PolygonData poly, SortedList<int, PolygonEdge> diagonals)
        {
            var startIndex = poly.Points[0].Index;
            var subPolygonCount = diagonals.Count / 2 + 1;
            var subPolygons = new PolygonData[subPolygonCount];
            if (subPolygonCount == 1)
                return new PolygonData[] { poly };
            for (int i = 0; i < subPolygonCount; i++)
            {
                var polygonStart = startIndex;
                var currentPoint = poly.Points.FirstOrDefault(p => p.Index == polygonStart);
                var subPolyPoints = new List<PolygonPoint>();
                var newStart = false;
                var diagonalNotAllowed = -1;
                do
                {
                    subPolyPoints.Add(currentPoint);
                    if (diagonals.ContainsKey(currentPoint.Index) &&
                        diagonals[currentPoint.Index].PointOne.Index == currentPoint.Index && currentPoint.Index != diagonalNotAllowed)
                    {
                        var idx = currentPoint.Index;
                        if (!newStart)
                        {
                            startIndex = currentPoint.Index;
                            newStart = true;
                        }
                        currentPoint = poly.Points.FirstOrDefault(p => p.Index == diagonals[currentPoint.Index].PointTwo.Index);
                        diagonals.Remove(idx);
                        diagonalNotAllowed = currentPoint.Index;
                    }
                    else if (diagonals.ContainsKey(currentPoint.Index) &&
                        diagonals[currentPoint.Index].PointTwo.Index == currentPoint.Index && currentPoint.Index != diagonalNotAllowed)
                    {
                        var idx = currentPoint.Index;
                        if (!newStart)
                        {
                            startIndex = currentPoint.Index;
                            newStart = true;
                        }
                        currentPoint = poly.Points.FirstOrDefault(p => p.Index == diagonals[currentPoint.Index].PointOne.Index);
                        diagonals.Remove(idx);
                        diagonalNotAllowed = currentPoint.Index;
                    }
                    else
                    {
                        currentPoint = currentPoint.Next;
                        diagonalNotAllowed = -1;
                    }
                }
                while (currentPoint.Index != polygonStart);
                subPolygons[i] = new PolygonData(subPolyPoints);
            }

            return subPolygons;
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

    internal class StatusHelper
    {
        public List<StatusHelperElement> EdgesHelpers { get; set; }

        internal StatusHelper(){
            this.EdgesHelpers = new List<StatusHelperElement>();
        }

        internal void Add(StatusHelperElement element)
        {
            this.EdgesHelpers.Add(element);
        }
        internal void Remove(PolygonEdge edge)
        {
            this.EdgesHelpers.RemoveAll(she => she.Edge == edge);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal StatusHelperElement SearchLeft(PolygonPoint point)
        {
            StatusHelperElement result = null;
            var dist = Double.PositiveInfinity;
            foreach (var she in this.EdgesHelpers)
            {
                Double xValue = Double.NaN;
                if (point.Y == she.Edge.PointOne.Y)
                    xValue = she.Edge.PointOne.X;
                else if (point.Y == she.Edge.PointTwo.Y)
                    xValue = she.Edge.PointTwo.X;
                else
                    xValue = she.Edge.PointOne.X + ((point.Y - she.Edge.PointOne.Y) / (she.Edge.PointTwo.Y - she.Edge.PointOne.Y)) * (she.Edge.PointTwo.X - she.Edge.PointOne.X);
                if (xValue < point.X){
                    var sheDist = point.X - xValue;
                    if (sheDist < dist)
                    {
                        dist = sheDist;
                        result = she;
                    }
                }
            }
            return result;
        }
    }

    internal class StatusHelperElement
    {
        public PolygonEdge Edge { get; set; }
        public PolygonPoint Helper { get; set; }

        internal StatusHelperElement()
        {
            this.Edge = null;
            this.Helper = null;
        }
        internal StatusHelperElement(PolygonEdge edge, PolygonPoint point)
        {
            this.Edge = edge;
            this.Helper = point;
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
        private int mIndex;

        public int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
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
        public static Boolean operator <(PolygonPoint first, PolygonPoint second)
        {
            return first.CompareTo(second) == 1;
        }
        public static Boolean operator >(PolygonPoint first, PolygonPoint second)
        {
            return first.CompareTo(second) == -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        internal PolygonPoint(Point p){
            this.mPoint = p;
            this.mIndex = -1;
        }

        internal PolygonPointClass PointClass(Boolean reverse = false)
        {
            if (Next == null || Last == null)
                throw new HelixToolkitException("No closed Polygon");

            if (reverse)
            {
                if (Last < this && Next < this && Last.X > Next.X)
                    return PolygonPointClass.Stop;
                else if (Last > this && Next > this && Last.X < Next.X)
                    return PolygonPointClass.Start;
                else if (Last < this && Next < this)
                    return PolygonPointClass.Merge;
                else if (Last > this && Next > this)
                    return PolygonPointClass.Split;
                else
                    return PolygonPointClass.Regular;
            }
            else
            {
                if (Last < this && Next < this && Last.X > Next.X)
                    return PolygonPointClass.Start;
                else if (Last > this && Next > this && Last.X < Next.X)
                    return PolygonPointClass.Stop;
                else if (Last < this && Next < this)
                    return PolygonPointClass.Split;
                else if (Last > this && Next > this)
                    return PolygonPointClass.Merge;
                else
                    return PolygonPointClass.Regular;
            }
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
        internal PolygonData(List<Point> points, List<int> indices = null)
        {
            // Add Points
            mPoints = new List<PolygonPoint>(points.Select(p => new PolygonPoint(p)));
            if (indices == null)
                for (int i = 0; i < mPoints.Count; i++)
                    mPoints[i].Index = i;
            else
                for (int i = 0; i < mPoints.Count; i++)
                    mPoints[i].Index = indices[i];

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
        internal PolygonData(List<PolygonPoint> points)
            : this(points.Select(p => p.Point).ToList(), points.Select(p => p.Index).ToList())
        { }
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
