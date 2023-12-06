using D2D = SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Core2D;

/// <summary>
/// <see href="https://jeremiahmorrill.wordpress.com/2013/02/06/direct2d-gui-librarygraphucks/"/>
/// </summary>
public abstract class Segment : ISegment
{
    public bool IsDirty { private set; get; } = false;

    protected void Invalidate()
    {
        IsDirty = true;
    }

    public abstract void Create(D2D.GeometrySink sink);
}
