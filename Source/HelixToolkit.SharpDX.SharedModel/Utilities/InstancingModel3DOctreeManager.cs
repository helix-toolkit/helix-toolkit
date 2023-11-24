using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Utilities;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public sealed class InstancingModel3DOctreeManager : OctreeManagerBaseWrapper
{
    protected override IOctreeManager OnCreateManager()
    {
        return new InstancingRenderableOctreeManager();
    }
}
