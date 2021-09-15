/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using D2D = SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX;
using System.Collections.Generic;

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
    namespace Core2D
    {
        /// <summary>
        /// <see href="https://jeremiahmorrill.wordpress.com/2013/02/06/direct2d-gui-librarygraphucks/"/>
        /// </summary>
        public class Figure
        {
            private List<SegmentData> Segments { get; } = new List<SegmentData>();

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="Figure"/> is closed.
            /// </summary>
            /// <value>
            ///   <c>true</c> if closed; otherwise, <c>false</c>.
            /// </value>
            public bool Closed { private set; get; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="Figure"/> is filled.
            /// </summary>
            /// <value>
            ///   <c>true</c> if filled; otherwise, <c>false</c>.
            /// </value>
            public bool Filled { private set; get; }

            /// <summary>
            /// Gets or sets the start point.
            /// </summary>
            /// <value>
            /// The start point.
            /// </value>
            public Vector2 StartPoint { private set; get; }


            /// <summary>
            /// Initializes a new instance of the <see cref="Figure"/> class.
            /// </summary>
            /// <param name="startPoint">The start point.</param>
            /// <param name="filled">if set to <c>true</c> [filled].</param>
            /// <param name="closed">if set to <c>true</c> [closed].</param>
            public Figure(Vector2 startPoint, bool filled, bool closed)
            {
                StartPoint = startPoint;
                Filled = filled;
                Closed = closed;
            }

            /// <summary>
            /// Adds the segment.
            /// </summary>
            /// <param name="segment">The segment.</param>
            /// <param name="isStroked">if set to <c>true</c> [is stroked].</param>
            /// <param name="isSmoothJoined">if set to <c>true</c> [is smooth joined].</param>
            public void AddSegment(ISegment segment, bool isStroked = true, bool isSmoothJoined = true)
            {
                Segments.Add(new SegmentData(segment, isStroked, isSmoothJoined));
            }

            /// <summary>
            /// Creates the specified sink.
            /// </summary>
            /// <param name="sink">The sink.</param>
            public void Create(D2D.GeometrySink sink)
            {
                sink.BeginFigure(StartPoint, Filled ? D2D.FigureBegin.Filled : D2D.FigureBegin.Hollow);
                for(int i = 0; i < Segments.Count; ++i)
                {
                    D2D.PathSegment flag = D2D.PathSegment.None;
                    var segment = Segments[i];
                    if (!segment.IsStroked)
                    {
                        flag |= D2D.PathSegment.ForceUnstroked;
                    }
                    if (segment.IsSmoothJoined)
                    {
                        flag |= D2D.PathSegment.ForceRoundLineJoin;
                    }
                    sink.SetSegmentFlags(flag);
                    segment.Segment.Create(sink);
                }
                sink.EndFigure(Closed ? D2D.FigureEnd.Closed : D2D.FigureEnd.Open);
            }

            /// <summary>
            /// 
            /// </summary>
            private struct SegmentData
            {
                public ISegment Segment;
                public bool IsStroked;
                public bool IsSmoothJoined;
                public SegmentData(ISegment segment, bool isStroked, bool isSmoothJoined)
                {
                    Segment = segment;
                    IsStroked = isStroked;
                    IsSmoothJoined = isSmoothJoined;
                }
            }
        }
    }

}
