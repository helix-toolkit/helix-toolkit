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
        public class ArcSegment : Segment
        {
            public readonly Vector2 Point;
            public readonly Size2F Size;
            public readonly float Rotation;
            public readonly D2D.SweepDirection SweepDirection;
            public readonly D2D.ArcSize ArcSize;

            public ArcSegment(Vector2 point, Size2F size, float rotation, D2D.SweepDirection sweepDirection, D2D.ArcSize arcSize)
            {
                Point = point;
                Size = size;
                Rotation = rotation;
                SweepDirection = sweepDirection;
                ArcSize = arcSize;
            }

            public override void Create(D2D.GeometrySink sink)
            {
                sink.AddArc(new D2D.ArcSegment() { ArcSize = ArcSize, Point = Point, RotationAngle = Rotation, Size = Size, SweepDirection = SweepDirection });
            }
        }
    }

}
