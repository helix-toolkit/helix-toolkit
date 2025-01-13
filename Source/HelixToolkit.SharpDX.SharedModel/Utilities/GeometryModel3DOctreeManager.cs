using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Utilities;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
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
