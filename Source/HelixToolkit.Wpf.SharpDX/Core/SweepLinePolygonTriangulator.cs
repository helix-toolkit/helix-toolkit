// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubchainSamplingPolygonTriangulation.cs" company="Helix Toolkit">
//   Copyright (c) 2016 Franz Spitaler
// </copyright>
// <summary>
//   A polygon triangulator for simple polygons with no holes. Expected runtime is O(n log n)
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
    /// Triangulate a simple Polygon with the Sweep-Line Algorithm
    /// </summary>
    /// <remarks>
    /// Based on http://www.cs.uu.nl/docs/vakken/ga/slides3.pdf
    /// References
    /// https://www.cs.ucsb.edu/~suri/cs235/Triangulation.pdf
    /// </remarks>
    public class SweepLinePolygonTriangulator
    {
        public static float Epsilon = 0.0000001f;

        /// <summary>
        /// Perform the Triangulation of the Input.
        /// </summary>
        /// <param name="polygon">The Input Polygon</param>
        /// <returns>List of Indices representing the Triangulation of the Polygon</returns>
        public static Int32Collection Triangulate(IList<Point> polygon)
        {
            // Allocate and initialize List of Indices in Polygon
            var result = new Int32Collection();
            
            // Point-List from Input
            var points = polygon.ToList();
            var count = points.Count;
            
            // Sort the Input and create the Datastructures
            // Make the Polygon CounterClockWise
            var didReverse = false;
            if (!isCCW(polygon))
            {
                points.Reverse();
                didReverse = true;
            }
            
            // Create Polygon Data Structure
            var poly = new PolygonData(points);
            
            // Sort Points from highest y to lowest y
            // and if two or more Points have the same y Value from lowest x to highest x Value
            var events = new List<PolygonPoint>(poly.Points);
            events.Sort();

            // Mapping from the main Polygon to the current Polygon
            var mainMapping = new Dictionary<int, int>();
            for (int i = 0; i < count; i++)
			{
                mainMapping[i] = i;
			}
            // Calculate the Diagonals in the Down Sweep
            var diagonals = CalculateDiagonals(events, mainMapping);

            // Split the Polygon
            var subPolygons = SplitPolygonWithDiagonals(poly, mainMapping, diagonals);

            var monotonePolygons = new List<PolygonData>();
            // Up Sweep for all Sub-Polygons
            foreach (var subPoly in subPolygons)
            {
                // Sort the Events from Bottom to Top
                events = new List<PolygonPoint>(subPoly.Points);
                events.Sort();
                events.Reverse();

                // Mapping from the main Polygon to the current Polygon
                var polyMapping = new Dictionary<int, int>();
                for (int i = 0; i < subPoly.Points.Count; i++)
                {
                    polyMapping[subPoly.Points[i].Index] = i;
                }
                // Calculate the Diagonals in the Up Sweep
                diagonals = CalculateDiagonals(events, polyMapping, false);

                // Split the Polygon
                var monotoneSubPolygons = SplitPolygonWithDiagonals(subPoly, polyMapping, diagonals);
                // Add to List of monotone Polygons
                monotonePolygons.AddRange(monotoneSubPolygons);
            }

            // y-Monotone Polygons
            // Triangulate
            foreach (var monoton in monotonePolygons)
            {
                result.AddRange(TriangulateMonotone(monoton));
            }

            // If we reversed the Polygon,
            // we need to reverse the result also to get a correct Triangulation
            if (didReverse)
            {
                // Transform back every calculated Index
                for (int i = 0; i < result.Count; i++)
                {
                    result[i] = count - result[i] - 1;
                }
            }

            // Return all calculated Triangleindices
            return result;
        }

        /// <summary>
        /// Triangulate the y-Monotone Polygons.
        /// </summary>
        /// <param name="monoton">The y-Monotone Polygon to triangle</param>
        /// <returns>Index-List of Polygon Points (Indices from the original Polygon)</returns>
        private static Int32Collection TriangulateMonotone(PolygonData monoton)
        {
            // Collection to return
            Int32Collection result = new Int32Collection();
            
            // Sort the Events
            var events = new List<PolygonPoint>(monoton.Points);
            events.Sort();
            
            // Stack of Events to push to and pop from
            var pointStack = new Stack<PolygonPoint>();
            
            // Push the first two Events
            pointStack.Push(events[0]);
            pointStack.Push(events[1]);

            // Left- and right Chain for Triangulation
            var left = (events[0].Next == events[1]) ? events[1] : events[0];
            var right = (events[0].Last == events[1]) ? events[1] : events[0];

            // Count of Points
            var pointCnt = monoton.Points.Count;

            // Handle the 3rd...n-th Point to triangle
            for (int i = 2; i < pointCnt; i++)
            {
                // The current Point
                var newPoint = events[i];
                var top = pointStack.Peek();
                // If the new Point is not on the same side as the last Point on the Stack
                //if (!(leftChain.Contains(top) && leftChain.Contains(newPoint) || rightChain.Contains(top) && rightChain.Contains(newPoint)))
                if (!(top.Last == newPoint || top.Next == newPoint))
                {
                    // Determine this Point's Chain (left or right)
                    if (left.Next == newPoint)
                    {
                        left = newPoint;
                    }
                    else if (right.Last == newPoint)
                    {
                        right = newPoint;
                    }

                    // Third triangle Point
                    var p2 = top;
                    // While there is a Point on the Stack
                    while (pointStack.Count != 0)
                    {
                        // Pop and set the third Point
                        top = pointStack.Pop();
                        p2 = top;
                        if (pointStack.Count != 0)
                        {
                            // Pop again
                            top = pointStack.Pop();
                            
                            // Add to the result. The Order is depending on the Side
                            if (left == newPoint)
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
                        // If more Points are on the Stack,
                        // Push the Point back again, to be able to form the Triangles
                        if (pointStack.Count != 0)
                            pointStack.Push(top);
                    }
                    // Push the last to Points on the Stack
                    pointStack.Push(events[i - 1]);
                    pointStack.Push(newPoint);
                }
                // If the newPoint is on the same Side (i.e. Chain)
                else
                {
                    // Get to Point on the Stack
                    top = pointStack.Pop();
                    var p2 = top;

                    // Determine this Point's Chain (left or right)
                    if (left.Next == newPoint && right.Last == newPoint)
                    {
                        if (top.Last == newPoint)
                            right = newPoint;
                        else if (top.Next == newPoint)
                            left = newPoint;
                        else
                            throw new HelixToolkitException("Triangulation error");
                    }
                    else if (left.Next == newPoint)
                    {
                        left = newPoint;
                    }
                    else if (right.Last == newPoint)
                    {
                        right = newPoint;
                    }
                    
                    while (pointStack.Count != 0)
                    {
                        // If the Triangle is possible, add it to the result (Point Order depends on the Side)
                        if (right == newPoint && isCCW(new List<Point> { newPoint.Point, p2.Point, pointStack.Peek().Point }))
                        {
                            top = pointStack.Pop();
                            result.Add(newPoint.Index);
                            result.Add(p2.Index);
                            result.Add(top.Index);
                            p2 = top;
                        }
                        else if (left == newPoint && !isCCW(new List<Point> { newPoint.Point, p2.Point, pointStack.Peek().Point }))
                        {
                            top = pointStack.Pop();
                            result.Add(newPoint.Index);
                            result.Add(top.Index);
                            result.Add(p2.Index);
                            p2 = top;
                        }
                        // No Triangle possible, just leave the Loop
                        else
                            break;
                    }
                    // Push the last two Points on the Stack
                    pointStack.Push(p2);
                    pointStack.Push(newPoint);
                }
            }
            // Return the Triangulation
            return result;
        }

        /// <summary>
        /// Calculate the Diagonals to add inside the Polygon.
        /// </summary>
        /// <param name="events">The Events in sorted Form</param>
        /// <param name="mapping">Mapping from main Polygon to the current Polygon</param>
        /// <param name="sweepDown">True in the first Stage (sweeping down), false in the following Stages (sweeping up)</param>
        /// <returns></returns>
        private static List<Tuple<int, int>> CalculateDiagonals(List<PolygonPoint> events, Dictionary<int, int> mapping, Boolean sweepDown = true)
        {
            // Diagonals to add to the Polygon to make it monotone after the Down- and Up-Sweeps
            var diagonals = new List<Tuple<int, int>>();
            
            // Construct Status and Helper, a List of Edges left of every Point of the Polygon
            // by shooting a Ray from the Vertex to the left.
            // The Helper Point of that Edge will be used to create Monotone Polygons
            // by adding Diagonals from/to Split- and Merge-Points
            var statusAndHelper = new StatusHelper();
            
            // Sweep through the Polygon using the sorted Polygon Points
            for(int i = 0; i < events.Count; i++)
            {
                var ev = events[i];
                // Get the Class of this event (depending on the sweeping direction)
                var evClass = ev.PointClass(!sweepDown);
                
                // Temporary StatusHelperElement
                StatusHelperElement she = null;
                
                // Handle the different Point-Classes
                switch (evClass)
                {
                    case PolygonPointClass.Start:
                        // Just add the left Edge (depending on the sweeping direction)
                        statusAndHelper.Add(new StatusHelperElement(sweepDown ? ev.EdgeTwo : ev.EdgeOne, ev));
                        break;
                    case PolygonPointClass.Stop:
                        // Just remove the left Edge (depending on the sweeping direction)
                        statusAndHelper.Remove(sweepDown ? ev.EdgeOne : ev.EdgeTwo);
                        break;
                    case PolygonPointClass.Regular:
                        // If the Polygon is positioned on the right Side of this Event
                        if (ev.Last > ev.Next)
                        {
                            // Replace the corresponding (old) StatusHelperElement with the new one
                            statusAndHelper.Remove(sweepDown ? ev.EdgeOne : ev.EdgeTwo);
                            statusAndHelper.Add(new StatusHelperElement(sweepDown ? ev.EdgeTwo : ev.EdgeOne, ev));
                        }
                        else
                        {
                            // Search Edge left of the Event and set Event as it's Helper
                            she = statusAndHelper.SearchLeft(ev);
                            she.Helper = ev;
                        }
                        break;
                    case PolygonPointClass.Merge:
                        // Just remove the left Edge (depending on the sweeping direction)
                        statusAndHelper.Remove(sweepDown ? ev.EdgeOne : ev.EdgeTwo);
                        // Search Edge left of the Event and set Event as it's Helper
                        she = statusAndHelper.SearchLeft(ev);
                        she.Helper = ev;
                        break;
                    case PolygonPointClass.Split:
                        // Search Edge left of the Event
                        she = statusAndHelper.SearchLeft(ev);

                        // Chose diagonal from Helper of Edge to Event.
                        // Add two Diagonals sorted (from lowest Index to biggest)
                        // This is helpful for the Triangulation later
                        var hIdxMapped = mapping[she.Helper.Index];
                        var evIdxMapped = mapping[ev.Index];
                        var minP = Math.Min(hIdxMapped, evIdxMapped);
                        var maxP = Math.Max(hIdxMapped, evIdxMapped);
                        diagonals.Add(new Tuple<int, int>(minP, maxP));
                        diagonals.Add(new Tuple<int, int>(maxP, minP));

                        // Replace the Helper of the StatusHelperElement by Event
                        she.Helper = ev;

                        // Insert the right Edge from Event
                        statusAndHelper.Add(new StatusHelperElement(sweepDown ? ev.EdgeTwo : ev.EdgeOne, ev));
                        break;
                }
            }
            // diagonals = diagonals.OrderBy(t => t.Item1).ThenBy(t => t.Item2).ToList();
            return diagonals;
        }

        /// <summary>
        /// Split up a Polygon with the Diagonals.
        /// For every Diagonal there exist two Entries due to simplicity Reasons.
        /// </summary>
        /// <param name="poly">The Polygon to split</param>
        /// <param name="mapping">Mapping from main Polygon to the current Polygon</param>
        /// <param name="diagonals">The Diagonals inside the Polygon, that are used to split it</param>
        /// <returns></returns>
        private static PolygonData[] SplitPolygonWithDiagonals(PolygonData poly, Dictionary<int, int> mapping, List<Tuple<int, int>> diagonals)
        {
            // Create some helping Variables
            // var startIndex = poly.Points[0].Index;
            var subPolygonCount = diagonals.Count / 2 + 1;
            var subPolygons = new PolygonData[subPolygonCount];

            // Start-Indices for the Polygon-Splitting
            var startIndices = new Queue<int>();
            startIndices.Enqueue(0/*poly.Points[0].Index*/);

            // If we don't have any Diagonals, we don't need to split the Polygon at all,
            // so just return it
            if (subPolygonCount == 1)
                return new PolygonData[] { poly };
            
            // We need to create Subpolygons, so loop through them
            // by using the startIndex
            for (int i = 0; i < subPolygonCount; i++)
            {
                // Start the Polygon at the startIndex
                var polygonStart = poly.Points[startIndices.Dequeue()].Index;
                
                // We need to query the Point, because we are dealing with Indices relevant to the original Polygon
                var currentPoint = poly.Points[mapping[polygonStart]];
                var subPolyPoints = new List<PolygonPoint>();
                
                // Indicates which Diagonal may not be used in the following Step
                // Diagonals are the preferred Connection to the next Polygonpoint, since they exist "in both direction",
                // the algorithm would always "go back" and not "move on"
                // var diagonalNotAllowed = -1;
                Tuple<int, int> diagonalNotAllowed = null;
                
                // Loop until we've reached the polygonStart again
                do
                {
                    // Add the currentPoint of the Polygon
                    subPolyPoints.Add(currentPoint);
                    
                    // Index of the current Point
                    var mappedIdx = mapping[currentPoint.Index];

                    // If a Diagonal for this Point exists (i.e. with this Key) and the Diagonal is allowed to be used
                    var possibleDiags = diagonals.Where(t => t.Item1 == mappedIdx && !t.Equals(diagonalNotAllowed)).ToList();
                    Tuple<int, int> diag = null;
                    if (possibleDiags.Count > 0 && subPolyPoints.Count > 1)
                    {
                        Point vectorLast = new Point();
                        if (subPolyPoints.Count > 1)
                            vectorLast = subPolyPoints[subPolyPoints.Count - 1].Point - subPolyPoints[subPolyPoints.Count - 2].Point;
                        else
                            vectorLast = currentPoint.Point - currentPoint.Last.Point;
                        vectorLast.Normalize();
                        Point vectorNext = currentPoint.Next.Point - currentPoint.Point;
                        vectorNext.Normalize();

                        var vectorLeft = new Point(-vectorLast.Y, vectorLast.X);
                        var otherVectors = new List<Point>();
                        foreach (var possibleDiag in possibleDiags)
                        {
                            var vec = poly.Points[possibleDiag.Item2].Point - poly.Points[possibleDiag.Item1].Point;
                            vec.Normalize();
                            otherVectors.Add(vec);
                        }
                        var bestAngle = float.NegativeInfinity;
                        // The all Diagonals
                        for (int j = 0; j < otherVectors.Count; j++)
                        {
                            var dot = Point.Dot(vectorLast, otherVectors[j]);
                            // left
                            if (Point.Dot(vectorLeft, otherVectors[j]) >= 0)
                            {
                                var angle = Math.Acos(dot);
                                if (angle > bestAngle){
                                    bestAngle = (float)angle;
                                    diag = possibleDiags[j];
                                }
                            }
                            // right
                            else if (Point.Dot(vectorLeft, otherVectors[j]) < 0)
                            {
                                var angle = -Math.Acos(dot);
                                if (angle > bestAngle)
                                {
                                    bestAngle = (float)angle;
                                    diag = possibleDiags[j];
                                }
                            }
                        }
                        // Also test the next Line-Segment
                        var nDot = Point.Dot(vectorLast, vectorNext);
                        // left Side
                        if (Point.Dot(vectorLeft, vectorNext) >= 0)
                        {
                            var angle = Math.Acos(nDot);
                            if (angle > bestAngle)
                            {
                                bestAngle = (float)angle;
                                diag = null;
                            }
                        }
                        // right Side
                        else if (Point.Dot(vectorLeft, vectorNext) < 0)
                        {
                            var angle = -Math.Acos(nDot);
                            if (angle > bestAngle)
                            {
                                bestAngle = (float)angle;
                                diag = null;
                            }
                        }

                    }
                    if (diag != null)
                    {
                        if (diagonals.Contains(new Tuple<int, int>(diag.Item2, diag.Item1)))
                        {
                            startIndices.Enqueue(diag.Item1);
                        }
                        
                        // Get the next Point
                        /*if (diag.Item1 == mappedIdx)
                        {*/
                            currentPoint = poly.Points[diag.Item2];
                        /*}
                        else
                        {
                            currentPoint = poly.Points[diag.Item1];
                        }*/
                        
                        // Remove the Diagonal and don't allow the Diagonal that would lead back
                        diagonals.Remove(diag);
                        diagonalNotAllowed = new Tuple<int,int>(diag.Item2, diag.Item1);
                    }
                    // If no Diagonal exists for this Point, just add it and re-allow all Diagonals
                    else
                    {
                        currentPoint = currentPoint.Next;
                        diagonalNotAllowed = null;
                    }
                }
                while (currentPoint.Index != polygonStart);
                
                // Create a new Polygon from the found Points
                subPolygons[i] = new PolygonData(subPolyPoints);
            }

            return subPolygons;
        }

        /// <summary>
        /// Calculates the Orientation of a Polygon by usings it's (double-) Area as an Indicator.
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
    /// Helper Class that is used in the calculation Process of the Diagonals.
    /// </summary>
    internal class StatusHelper
    {
        /// <summary>
        /// List of StatusHelperElements that are currently present at the Sweeper's Position
        /// </summary>
        public List<StatusHelperElement> EdgesHelpers { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        internal StatusHelper(){
            this.EdgesHelpers = new List<StatusHelperElement>();
        }

        /// <summary>
        /// Adds a StatusHelperElement to the List
        /// </summary>
        /// <param name="element"></param>
        internal void Add(StatusHelperElement element)
        {
            this.EdgesHelpers.Add(element);
        }

        /// <summary>
        /// Removes all StatusHelperElements with a specific Edge
        /// </summary>
        /// <param name="edge"></param>
        internal void Remove(PolygonEdge edge)
        {
            this.EdgesHelpers.RemoveAll(she => she.Edge == edge);
        }

        /// <summary>
        /// Searches the nearest StatusHelperElement from the given Point
        /// </summary>
        /// <param name="point">The Point to search a StatusHelperElement for</param>
        /// <returns>The nearest StatusHelperElement that is positioned left of the Poin</returns>
        internal StatusHelperElement SearchLeft(PolygonPoint point)
        {
            // The found StatusHelperElement and the Distance Variables
            StatusHelperElement result = null;
            var dist = Double.PositiveInfinity;

            // Search for the right StatusHelperElement
            foreach (var she in this.EdgesHelpers)
            {
                // Calculate the x-Coordinate of the Intersection between
                // a horizontal Line from the Point to the Left and the Edge of the StatusHelperElement
                Double xValue = Double.NaN;
                if (point.Y == she.Edge.PointOne.Y)
                    xValue = she.Edge.PointOne.X;
                else if (point.Y == she.Edge.PointTwo.Y)
                    xValue = she.Edge.PointTwo.X;
                else
                    xValue = she.Edge.PointOne.X + ((point.Y - she.Edge.PointOne.Y) / (she.Edge.PointTwo.Y - she.Edge.PointOne.Y)) * (she.Edge.PointTwo.X - she.Edge.PointOne.X);
                
                // If the xValue is smaller than the Point's x-Coordinate (i.e. it lies on the left Side of it)
                if (xValue <= (point.X + SweepLinePolygonTriangulator.Epsilon))
                {
                    // Calculate the Distance
                    var sheDist = point.X - xValue;
                    
                    // Update, if the Distance is smaller than a previously found Result
                    if (sheDist < dist)
                    {
                        dist = sheDist;
                        result = she;
                    }
                }
            }

            // Return the nearest found StatusHelperElement
            return result;
        }
    }

    /// <summary>
    /// Helper Class that is used in the calculation Process of the Diagonals.
    /// </summary>
    internal class StatusHelperElement
    {
        /// <summary>
        /// The Edge of the StatusHelperElement
        /// </summary>
        public PolygonEdge Edge { get; set; }

        /// <summary>
        /// The Helper of the Edge is a Polygon Point
        /// </summary>
        public PolygonPoint Helper { get; set; }

        /// <summary>
        /// Constructor taking an Edge and a Helper
        /// </summary>
        /// <param name="edge">The Edge of the StatusHelperElement</param>
        /// <param name="point">The Helper for the Edge of the StatusHelperElement</param>
        internal StatusHelperElement(PolygonEdge edge, PolygonPoint point)
        {
            this.Edge = edge;
            this.Helper = point;
        }
    }
    
    /// <summary>
    /// Enumeration of PolygonPoint - Classes
    /// </summary>
    internal enum PolygonPointClass : byte
    {
        Start,
        Stop,
        Split,
        Merge,
        Regular
    }
    
    /// <summary>
    /// Helper Class for the PolygonData Object.
    /// </summary>
    internal class PolygonPoint : IComparable<PolygonPoint>
    {
        /// <summary>
        /// The actual Point of this PolygonPoint
        /// </summary>
        private Point mPoint;
        
        /// <summary>
        /// Accessor for the Point-Data
        /// </summary>
        public Point Point
        {
            get { return mPoint; }
            set { mPoint = value; }
        }
        
        /// <summary>
        /// Accessor for the X-Coordinate of the Point
        /// </summary>
        public float X { get { return this.mPoint.X; } set { this.mPoint.X = value; } }
        
        /// <summary>
        /// Accessor for the Y-Coordinate of the Point
        /// </summary>
        public float Y { get { return this.mPoint.Y; } set { this.mPoint.Y = value; } }
        
        /// <summary>
        /// The "incoming" Edge of this PolygonPoint
        /// </summary>
        private PolygonEdge mEdgeOne;
        
        /// <summary>
        /// Accessor for the incoming Edge
        /// </summary>
        public PolygonEdge EdgeOne
        {
            get { return mEdgeOne; }
            set { mEdgeOne = value; }
        }

        /// <summary>
        /// The "outgoing" Edge of this PolygonPoint
        /// </summary>
        private PolygonEdge mEdgeTwo;

        /// <summary>
        /// Accessor for the outgoing Edge
        /// </summary>
        public PolygonEdge EdgeTwo
        {
            get { return mEdgeTwo; }
            set { mEdgeTwo = value; }
        }

        /// <summary>
        /// The Index of this Point in the original Polygon
        /// that needs to be triangulated
        /// </summary>
        private int mIndex;

        /// <summary>
        /// Accessor for the iriginal Point-Index
        /// </summary>
        public int Index
        {
            get { return mIndex; }
            set { mIndex = value; }
        }

        /// <summary>
        /// The "last" neighboring Point, which is connected throught the incoming Edge
        /// </summary>
        public PolygonPoint Last
        {
            get
            {
                if (mEdgeOne != null && mEdgeOne.PointOne != null)
                    return mEdgeOne.PointOne;
                else
                    return null;
            }
        }
        
        /// <summary>
        /// The "next" neighboring Point, which is connected throught the outgoing Edge
        /// </summary>
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
        /// Comparison Operator, that is used to determine the Class of the PolygonPoints
        /// </summary>
        /// <param name="first">The first PolygonPoint</param>
        /// <param name="second">The second PolygonPoint</param>
        /// <returns>Returns true if the first PolygonPoint is smaller, compared to the second PolygonPoint, false otherwise</returns>
        public static Boolean operator <(PolygonPoint first, PolygonPoint second)
        {
            return first.CompareTo(second) == 1;
        }
        
        /// <summary>
        /// Comparison Operator, that is used to determine the Class of the PolygonPoints
        /// </summary>
        /// <param name="first">The first PolygonPoint</param>
        /// <param name="second">The second PolygonPoint</param>
        /// <returns>Returns true if the first PolygonPoint is bigger, compared to the second PolygonPoint, false otherwise</returns>
        public static Boolean operator >(PolygonPoint first, PolygonPoint second)
        {
            return first.CompareTo(second) == -1;
        }

        /// <summary>
        /// Constructor using a Point
        /// </summary>
        /// <param name="p">The Point-Data to use</param>
        internal PolygonPoint(Point p)
        {
            // Set the Point-Data, the Index must be set later
            this.mPoint = p;
            this.mIndex = -1;
        }

        /// <summary>
        /// Detrmines the Class of the PolygonPoint, depending on the sweeping Direction
        /// </summary>
        /// <param name="reverse">The Sweeping direction, top-to-bottom if false, bottom-to-top otherwise</param>
        /// <returns>The Class of the PolygonPoint</returns>
        internal PolygonPointClass PointClass(Boolean reverse = false)
        {
            // If the Point has no Next- and Last-PolygonPoint, there's an Error
            if (Next == null || Last == null)
                throw new HelixToolkitException("No closed Polygon");

            // If we use the normal Order (top-to-bottom)
            if (!reverse)
            {
                // Both neighboring PolygonPoints are below this Point means this Point is a Start - Point
                ///if (Last < this && Next < this && Last.X > Next.X)
                if (Last < this && Next < this && this.isConcavePoint())
                    return PolygonPointClass.Start;
                // Both neighboring PolygonPoints are above this Point means this Point is a Stop - Point
                ///else if (Last > this && Next > this && Last.X < Next.X)
                else if (Last > this && Next > this && this.isConcavePoint())
                    return PolygonPointClass.Stop;
                // Both neighboring PolygonPoints are below this Point (in another Order) means this Point is a Split - Point
                else if (Last < this && Next < this)
                    return PolygonPointClass.Split;
                // Both neighboring PolygonPoints are above this Point (in another Order) means this Point is a Merge - Point
                else if (Last > this && Next > this)
                    return PolygonPointClass.Merge;
                // Regular Point in all other Cases
                else
                    return PolygonPointClass.Regular;
            }
            else
            {
                // Both neighboring PolygonPoints are below this Point means this Point is a Stop - Point
                if (Last < this && Next < this && this.isConcavePoint())
                    return PolygonPointClass.Stop;
                // Both neighboring PolygonPoints are above this Point means this Point is a Start - Point
                else if (Last > this && Next > this && this.isConcavePoint())
                    return PolygonPointClass.Start;
                // Both neighboring PolygonPoints are below this Point (in another Order) means this Point is a Merge - Point
                else if (Last < this && Next < this)
                    return PolygonPointClass.Merge;
                // Both neighboring PolygonPoints are above this Point (in another Order) means this Point is a Split - Point
                else if (Last > this && Next > this)
                    return PolygonPointClass.Split;
                // Regular Point in all other Cases
                else
                    return PolygonPointClass.Regular;
            }
        }

        /// <summary>
        /// (assumption CCW)
        /// </summary>
        /// <returns></returns>
        private bool isConcavePoint()
        {
            // If the Point has no Next- and Last-PolygonPoint, there's an Error
            if (Next == null || Last == null)
                throw new HelixToolkitException("No closed Polygon");
            var vecFromLast = this.Point - this.Last.Point;
            vecFromLast.Normalize();
            var vecLeft = new Point(-vecFromLast.Y, vecFromLast.X);
            var vecToNext = this.Next.Point - this.Point;
            vecToNext.Normalize();
            if (Point.Dot(vecLeft, vecToNext) >= 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Override the ToString (for Debugging Purposes)
        /// </summary>
        /// <returns>String representing this Point</returns>
        public override string ToString()
        {
            return ((this.Index != null) ? this.Index + ": " : "") + "X:" + this.X + " Y:" + this.Y;
        }

        /// <summary>
        /// Comparison of two Points, used to sort the Polygons from top to bottom (left to right)
        /// </summary>
        /// <param name="second">Other Point to compare to</param>
        /// <returns>-1 if this Point is bigger, 0 if the same, 1 if smaller</returns>
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
    /// Helper Class for the PolygonData Object.
    /// </summary>
    internal class PolygonEdge
    {
        /// <summary>
        /// The "starting" Point of this Edge
        /// </summary>
        private PolygonPoint mPointOne;

        /// <summary>
        /// Accessor to the Startpoint of this Edge
        /// </summary>
        public PolygonPoint PointOne
        {
            get { return mPointOne; }
            set { mPointOne = value; }
        }

        /// <summary>
        /// The "ending" Point of this Edge
        /// </summary>
        private PolygonPoint mPointTwo;

        /// <summary>
        /// Accessor to the Endpoint of this Edge
        /// </summary>
        public PolygonPoint PointTwo
        {
            get { return mPointTwo; }
            set { mPointTwo = value; }
        }
        
        /// <summary>
        /// The "last" neighboring Edge, which both share the Startpoint of this Edge
        /// </summary>
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

        /// <summary>
        /// The "next" neighboring Edge, which both share the Endpoint of this Edge
        /// </summary>
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
        /// Constructor that takes both Points of the Edge
        /// </summary>
        /// <param name="one">The Startpoint</param>
        /// <param name="two">The Endpoint</param>
        internal PolygonEdge(PolygonPoint one, PolygonPoint two)
        {
            this.mPointOne = one;
            this.mPointTwo = two;
        }

        /// <summary>
        /// Override the ToString (for Debugging Purposes)
        /// </summary>
        /// <returns>String representing this Edge</returns>
        public override string ToString()
        {
            return "From: " + mPointOne + " To: " + mPointTwo;
        }
    }

    /// <summary>
    /// Helper Class for the Polygon-Triangulation.
    /// </summary>
    internal class PolygonData
    {
        /// <summary>
        /// The List of Polygonpoints that define this Polygon
        /// </summary>
        private List<PolygonPoint> mPoints;

        /// <summary>
        /// Accessor to the List of PolygonPoints
        /// </summary>
        public List<PolygonPoint> Points
        {
            get { return mPoints; }
            set { mPoints = value; }
        }

        /// <summary>
        /// Constructor that uses a List of Points and an optional List of Point-Indices
        /// </summary>
        /// <param name="points">The Polygon-Defining Points</param>
        /// <param name="indices">Optional List of Point-Indices</param>
        internal PolygonData(List<Point> points, List<int> indices = null)
        {
            // Create and add PolygonPoints for this Polygon Object
            mPoints = new List<PolygonPoint>(points.Select(p => new PolygonPoint(p)));
            
            // If no Indices were specified, add them manually
            if (indices == null)
                for (int i = 0; i < mPoints.Count; i++)
                    mPoints[i].Index = i;
            // If there were Indices specified, use them to set the PolygonPoint's Index Property
            else
                for (int i = 0; i < mPoints.Count; i++)
                    mPoints[i].Index = indices[i];

            // Add Edges between the Points (to be able to navigate along the Polygon easily later)
            var cnt = mPoints.Count;
            for (int i = 0; i < cnt; i++)
			{
                var lastIdx = (i + cnt - 1) % cnt;
                var edge = new PolygonEdge(mPoints[lastIdx], mPoints[i]);
                mPoints[lastIdx].EdgeTwo = edge;
                mPoints[i].EdgeOne = edge;
			}
        }
        
        /// <summary>
        /// Constructor that takes a List of PolygonPoints
        /// Calls the first Constructor by splitting the Input-Information (Points and Indices)
        /// </summary>
        /// <param name="points">The PolygonPoints</param>
        internal PolygonData(List<PolygonPoint> points)
            : this(points.Select(p => p.Point).ToList(), points.Select(p => p.Index).ToList())
        { }
    }
}
