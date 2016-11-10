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
    public static class SweepLinePolygonTriangulator
    {
        /// <summary>
        /// Range Extension when searching for the Helper and Edge
        /// </summary>
        public static float Epsilon = 0.0000001f;

        /// <summary>
        /// Perform the Triangulation of the Input.
        /// </summary>
        /// <param name="polygon">The Input Polygon</param>
        /// <param name="holes">The Input Polygon</param>
        /// <returns>List of Indices representing the Triangulation of the Polygon</returns>
        public static Int32Collection Triangulate(IList<Point> polygon, List<List<Point>> holes = null)
        {
            // Allocate and initialize List of Indices in Polygon
            var result = new Int32Collection();

            // Point-List from Input
            // (we don't want the first and last Point to be present two times)
            var points = polygon.ToList();
            if (points[0] == points[points.Count - 1])
            {
                points.RemoveAt(points.Count - 1);
            }
            var count = points.Count;

            // Sort the Input and create the Datastructures
            // Make the Polygon CounterClockWise
            var didReverse = false;
            if (!IsCCW(polygon))
            {
                points.Reverse();
                didReverse = true;
            }

            // Skip Polygons that don't need Triangulation
            if (count < 3)
                return null;
            else if (count == 3)
            {
                if (!didReverse)
                {
                    return new Int32Collection { 0, 1, 2 };
                }
                else
                {
                    return new Int32Collection { 2, 1, 1 };
                }
            }

            // Create Polygon Data Structure
            var poly = new PolygonData(points);
            if (holes != null)
            {
                foreach (var hole in holes)
                {
                    poly.AddHole(hole);
                }
            }

            // Sort Points from highest y to lowest y
            // and if two or more Points have the same y Value from lowest x to highest x Value
            var events = new List<PolygonPoint>(poly.Points);
            events.Sort();

            // Mapping from the main Polygon to the current Polygon
            var mapping = new Dictionary<int, int>();
            for (int i = 0; i < count; i++)
            {
                mapping[i] = i;
            }

            var polygonsWithoutHoles = new List<PolygonData>() { poly };
            if (poly.HasHoles)
            {
                var first = CalculateDiagonals(events, mapping, true, true)[0];
                events.Reverse();
                var second = CalculateDiagonals(events, mapping, false, true)[0];

                polygonsWithoutHoles = SplitPolygonWithHole(poly, first, second).ToList();
            }

            foreach (var simplePolygon in polygonsWithoutHoles)
            {
                // Sort Points from highest y to lowest y
                // and if two or more Points have the same y Value from lowest x to highest x Value
                events = new List<PolygonPoint>(simplePolygon.Points);
                events.Sort();
                mapping = new Dictionary<int, int>();
                for (int i = 0; i < simplePolygon.Points.Count; i++)
                {
                    mapping[simplePolygon.Points[i].Index] = i;
                }

                // Calculate the Diagonals in the Down Sweep
                var diagonals = CalculateDiagonals(events, mapping);

                // Split the Polygon
                var subPolygons = SplitPolygonWithDiagonals(simplePolygon, diagonals);

                var monotonePolygons = new List<PolygonData>();
                // Up Sweep for all Sub-Polygons
                foreach (var subPoly in subPolygons)
                {
                    // Sort the Events from Bottom to Top
                    events = new List<PolygonPoint>(subPoly.Points);
                    events.Sort();
                    events.Reverse();

                    // Mapping from the main Polygon to the current Polygon
                    mapping = new Dictionary<int, int>();
                    for (int i = 0; i < subPoly.Points.Count; i++)
                    {
                        mapping[subPoly.Points[i].Index] = i;
                    }
                    // Calculate the Diagonals in the Up Sweep
                    diagonals = CalculateDiagonals(events, mapping, false);

                    // Split the Polygon
                    var monotoneSubPolygons = SplitPolygonWithDiagonals(subPoly, diagonals);
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
                        if (right == newPoint && IsCCW(new List<Point> { newPoint.Point, p2.Point, pointStack.Peek().Point }))
                        {
                            top = pointStack.Pop();
                            result.Add(newPoint.Index);
                            result.Add(p2.Index);
                            result.Add(top.Index);
                            p2 = top;
                        }
                        else if (left == newPoint && !IsCCW(new List<Point> { newPoint.Point, p2.Point, pointStack.Peek().Point }))
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
        /// <param name="holeToBoundary">True if we only want the first Diagonal from the Hole to the Boundary</param>
        /// <returns></returns>
        private static List<Tuple<int, int>> CalculateDiagonals(List<PolygonPoint> events, Dictionary<int, int> mapping, Boolean sweepDown = true, Boolean holeToBoundary = false)
        {
            // Diagonals to add to the Polygon to make it monotone after the Down- and Up-Sweeps
            var diagonals = new List<Tuple<int, int>>();

            // Construct Status and Helper, a List of Edges left of every Point of the Polygon
            // by shooting a Ray from the Vertex to the left.
            // The Helper Point of that Edge will be used to create Monotone Polygons
            // by adding Diagonals from/to Split- and Merge-Points
            var statusAndHelper = new StatusHelper();

            // Sweep through the Polygon using the sorted Polygon Points
            for (int i = 0; i < events.Count; i++)
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
                        var minP = Math.Min(she.Helper.Index, ev.Index);
                        var maxP = Math.Max(she.Helper.Index, ev.Index);
                        var diagonal = new Tuple<int, int>(minP, maxP);
                        if (holeToBoundary && !ev.IsBoundary)
                            return new List<Tuple<int, int>>() { diagonal };
                        diagonals.Add(diagonal);

                        // Replace the Helper of the StatusHelperElement by Event
                        she.Helper = ev;
                        // Insert the right Edge from Event
                        statusAndHelper.Add(new StatusHelperElement(sweepDown ? ev.EdgeTwo : ev.EdgeOne, ev));
                        break;
                }
            }
            return diagonals;
        }

        /// <summary>
        /// Split up a Polygon with the Diagonals.
        /// This function splits the Polygon into two SubPolygons using the first Diagonal
        /// and then calls itself recursively.
        /// </summary>
        /// <param name="poly">The Polygon to split</param>
        /// <param name="diagonals">The Diagonals inside the Polygon, that are used to split it</param>
        /// <returns>The SubPolygons that were created by splitting the input Polygon</returns>
        private static PolygonData[] SplitPolygonWithDiagonals(PolygonData poly, List<Tuple<int, int>> diagonals)
        {
            // Count of the SubPolygons to create
            var subPolygonCount = diagonals.Count + 1;
            if (subPolygonCount == 1)
                return new PolygonData[] { poly };
            var subPolygons = new List<PolygonData>();
            // Subdivide with with first Diagonal
            var diagonal = diagonals[0];
            // Create the first SubPolygon
            var firstSubPolygonPoints = poly.GetEdgePoints(diagonal.Item1, diagonal.Item2);
            // HashSet of the Indices of the first SubPolygon
            // Used to split the remaining Diagonals
            var firstSubPolygonIndices = new HashSet<int>(firstSubPolygonPoints.Select(p => p.Index));
            // Create the second SubPolygon
            var secondSubPolygonPoints = poly.GetEdgePoints(diagonal.Item2, diagonal.Item1);
            // We don't need this Diagonal any more
            diagonals.Remove(diagonal);
            // Split the Diagonals into two sets of Diagonals
            // Depending on in which SupPolygon it is located
            var diagsFirst = new List<Tuple<int, int>>();
            var diagsSecond = new List<Tuple<int, int>>();
            foreach (var diag in diagonals)
            {
                if (firstSubPolygonIndices.Contains(diag.Item1) && firstSubPolygonIndices.Contains(diag.Item2))
                    diagsFirst.Add(diag);
                else
                    diagsSecond.Add(diag);
            }

            // Calculate the Diagonals that are positioned in first and second Subpolygon
            subPolygons.AddRange(SplitPolygonWithDiagonals(new PolygonData(firstSubPolygonPoints), diagsFirst).ToList());

            // Recursively call Function for first Subpolygon and second Subpolygon
            subPolygons.AddRange(SplitPolygonWithDiagonals(new PolygonData(secondSubPolygonPoints), diagsSecond).ToList());

            // Return the split-up Polygons
            return subPolygons.ToArray();
        }

        /// <summary>
        /// Split the Polygon using two "Diagonals" calculated between the Hole and the Boundary
        /// </summary>
        /// <param name="poly">The Polygon to split</param>
        /// <param name="firstDiagonal">First found Diagonal (from top to bottom)</param>
        /// <param name="secondDiagonal">First found Diagonal (from bottom to top)</param>
        /// <returns></returns>
        private static PolygonData[] SplitPolygonWithHole(PolygonData poly, Tuple<int, int> firstDiagonal, Tuple<int, int> secondDiagonal)
        {
            // The two split Polygons
            var result = new PolygonData[2];

            // Create the first SubPolygon
            var firstSubPolygonPoints = poly.GetEdgePoints(secondDiagonal.Item1, firstDiagonal.Item1);
            firstSubPolygonPoints.AddRange(poly.GetEdgePoints(firstDiagonal.Item2, secondDiagonal.Item2));
            result[0] = new PolygonData(firstSubPolygonPoints);

            // Create the second SubPolygon
            var secondSubPolygonPoints = poly.GetEdgePoints(firstDiagonal.Item1, secondDiagonal.Item1);
            secondSubPolygonPoints.AddRange(poly.GetEdgePoints(secondDiagonal.Item2, firstDiagonal.Item2));
            result[1] = new PolygonData(secondSubPolygonPoints);

            return result;
        }

        /// <summary>
        /// Calculates the Orientation of a Polygon by usings it's (double-) Area as an Indicator.
        /// </summary>
        /// <param name="polygon">The Polygon.</param>
        /// <returns>True if the Polygon is present in a CCW manner.</returns>
        internal static Boolean IsCCW(IList<Point> polygon)
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
    /// Helper Class that is used in the calculation Process of the Diagonals.
    /// </summary>
    internal class StatusHelper
    {
        /// <summary>
        /// List of StatusHelperElements that are currently present at the Sweeper's Position
        /// </summary>
        internal List<StatusHelperElement> EdgesHelpers { get; set; }

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
                
                // If the xValue is smaller than or equal to the Point's x-Coordinate
                // (i.e. it lies on the left Side of it - allows a small Error)
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
        /// Is the PolygonPoint on the PolygonData's initial Boundary
        /// </summary>
        private Boolean mIsBoundary;

        /// <summary>
        /// Accessor for the IsBoundary
        /// </summary>
        public Boolean IsBoundary
        {
            get { return mIsBoundary; }
            set { mIsBoundary = value; }
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
            this.mIsBoundary = true;
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
                // Both neighboring PolygonPoints are below this Point and the Point is concave
                if (Last < this && Next < this && this.isConvexPoint())
                    return PolygonPointClass.Start;
                // Both neighboring PolygonPoints are above this Point and the Point is concave
                else if (Last > this && Next > this && this.isConvexPoint())
                    return PolygonPointClass.Stop;
                // Both neighboring PolygonPoints are below this Point and the Point is convex
                else if (Last < this && Next < this)
                    return PolygonPointClass.Split;
                // Both neighboring PolygonPoints are above this Point and the Point is convex
                else if (Last > this && Next > this)
                    return PolygonPointClass.Merge;
                // Regular Point in all other Cases
                else
                    return PolygonPointClass.Regular;
            }
            else
            {
                // Both neighboring PolygonPoints are below this Point and the Point is concave
                if (Last < this && Next < this && this.isConvexPoint())
                    return PolygonPointClass.Stop;
                // Both neighboring PolygonPoints are above this Point and the Point is concave
                else if (Last > this && Next > this && this.isConvexPoint())
                    return PolygonPointClass.Start;
                // Both neighboring PolygonPoints are below this Point and the Point is convex
                else if (Last < this && Next < this)
                    return PolygonPointClass.Merge;
                // Both neighboring PolygonPoints are above this Point and the Point is convex
                else if (Last > this && Next > this)
                    return PolygonPointClass.Split;
                // Regular Point in all other Cases
                else
                    return PolygonPointClass.Regular;
            }
        }

        /// <summary>
        /// Calculates for a Point, if it is a convex Point or not
        /// (the assumption is, that we are dealing with a CCW Polygon orientation!)
        /// </summary>
        /// <returns>Returns true, if convex, false if concave (or "reflex" Vertex)</returns>
        private Boolean isConvexPoint()
        {
            // If the Point has no Next- and Last-PolygonPoint, there's an Error
            if (Next == null || Last == null)
                throw new HelixToolkitException("No closed Polygon");
            // Calculate the necessary Vectors
            // From-last-to-this Vector
            var vecFromLast = this.Point - this.Last.Point;
            vecFromLast.Normalize();
            // "Left" Vector (pointing "inward")
            var vecLeft = new Point(-vecFromLast.Y, vecFromLast.X);
            // From-this-to-next Vector
            var vecToNext = this.Next.Point - this.Point;
            vecToNext.Normalize();
            // If the next Vector is pointing to the left Vector's direction,
            // the current Point is a convex Point (Dot-Product bigger than 0)
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
            return this.Index + " X:" + this.X + " Y:" + this.Y;
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
            return "From: {" + mPointOne + "} To: {" + mPointTwo + "}";
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
        /// Does the PolygonData Object have Holes inside or not
        /// </summary>
        private Boolean mHasHoles;

        /// <summary>
        /// Accessor for the HasHoles Indicator
        /// </summary>
        public Boolean HasHoles
        {
            get { return mHasHoles; }
        }

        /// <summary>
        /// Number of initial Points on the Polygon Boundary
        /// </summary>
        private int mNumBoundaryPoints;

        /// <summary>
        /// Constructor that uses a List of Points and an optional List of Point-Indices
        /// </summary>
        /// <param name="points">The Polygon-Defining Points</param>
        /// <param name="indices">Optional List of Point-Indices</param>
        public PolygonData(List<Point> points, List<int> indices = null)
        {
            // Create and add PolygonPoints for this Polygon Object
            mPoints = new List<PolygonPoint>(points.Select(p => new PolygonPoint(p)));
            mHasHoles = false;
            mNumBoundaryPoints = mPoints.Count;
            
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
        public PolygonData(List<PolygonPoint> points)
            : this(points.Select(p => p.Point).ToList(), points.Select(p => p.Index).ToList())
        { }

        /// <summary>
        /// Add Points of a Hole to the PolygonData
        /// </summary>
        /// <param name="points">The Points that define the Hole in the Polygon</param>
        internal void AddHole(List<Point> points)
        {
            // Set to true
            mHasHoles = true;

            // Make Hole Clockwise
            if (SweepLinePolygonTriangulator.IsCCW(points))
            {
                points.Reverse();
            }
            var cntBefore = mPoints.Count;
            var pointCount = points.Count;
            // Add the PolygonPoints for this Polygon Object
            mPoints.AddRange(points.Select(p => new PolygonPoint(p) { IsBoundary = false }));

            // Add the Indices
            for (int i = cntBefore; i < mPoints.Count; i++)
                mPoints[i].Index = i;
            
            // Add Edges between the Points (to be able to navigate along the Polygon easily later)
            var cnt = mPoints.Count;
            for (int i = 0; i < pointCount; i++)
            {
                var lastIdx = (i + pointCount  - 1) % pointCount + cntBefore;
                var edge = new PolygonEdge(mPoints[lastIdx], mPoints[i + cntBefore]);
                mPoints[lastIdx].EdgeTwo = edge;
                mPoints[i + cntBefore].EdgeOne = edge;
            }
        }

        /// <summary>
        /// Get all Points (including) between the Points with the provided Indices.
        /// </summary>
        /// <param name="from">Startpoint</param>
        /// <param name="to">Endpoint</param>
        /// <returns>List of PolygonPoints between the two Points</returns>
        internal List<PolygonPoint> GetEdgePoints(int from, int to)
        {
            var result = new List<PolygonPoint>();
            var currentPoint = mPoints.FirstOrDefault(p => p.Index == from);
            while(currentPoint.Index != to)
            {
                result.Add(currentPoint);
                currentPoint = currentPoint.Next;
            }
            result.Add(currentPoint);
            return result;
        }
    }
}
