using SharpDX;
using D2D = SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Core2D;

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
        sink.AddLine(Point);
    }
}
