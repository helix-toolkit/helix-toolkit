using D2D = SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Core2D;

/// <summary>
/// <see href="https://jeremiahmorrill.wordpress.com/2013/02/06/direct2d-gui-librarygraphucks/"/>
/// </summary>
public interface ISegment
{
    bool IsDirty
    {
        get;
    }
    void Create(D2D.GeometrySink sink);
}
