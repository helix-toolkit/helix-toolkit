using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Utilities;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// Use to create geometryModel3D octree for groups. Each ItemsModel3D must has its own manager, do not share between two ItemsModel3D
/// </summary>
public sealed class GeometryModel3DOctreeManager : OctreeManagerBaseWrapper
{
    protected override IOctreeManager OnCreateManager()
    {
        return new GroupNodeGeometryBoundOctreeManager();
    }
}
