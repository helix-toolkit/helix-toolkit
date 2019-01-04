/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using D2D = SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX;

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
        public class BezierSegment : Segment
        {
            /// <summary>
            /// 
            /// </summary>
            public readonly Vector2 P1, P2, P3;

            /// <summary>
            /// Initializes a new instance of the <see cref="BezierSegment"/> class.
            /// </summary>
            /// <param name="p1">The p1.</param>
            /// <param name="p2">The p2.</param>
            /// <param name="p3">The p3.</param>
            public BezierSegment(Vector2 p1, Vector2 p2, Vector2 p3)
            {
                P1 = p1;
                P2 = p2;
                P3 = p3;
            }

            /// <summary>
            /// Creates the specified sink.
            /// </summary>
            /// <param name="sink">The sink.</param>
            public override void Create(D2D.GeometrySink sink)
            {
                sink.AddBezier(new D2D.BezierSegment() { Point1 = P1, Point2 = P2, Point3 = P3 });
            }
        }
    }

}
