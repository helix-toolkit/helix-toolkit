/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.Numerics;
using D2D = SharpDX.Direct2D1;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    using Mathematics;
    /// <summary>
    /// <see href="https://jeremiahmorrill.wordpress.com/2013/02/06/direct2d-gui-librarygraphucks/"/>
    /// </summary>
    public class LineSegment : Segment
    {
        public readonly Vector2 Point;
        public LineSegment(Vector2 point)
        {
            Point = point;
        }

        public override void Create(D2D.GeometrySink sink)
        {
            sink.AddLine(Point.ToRaw());
        }
    }
}
