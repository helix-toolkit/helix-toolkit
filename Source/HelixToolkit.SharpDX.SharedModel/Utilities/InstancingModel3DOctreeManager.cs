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

public sealed class InstancingModel3DOctreeManager : OctreeManagerBaseWrapper
{
    protected override IOctreeManager OnCreateManager()
    {
        return new InstancingRenderableOctreeManager();
    }
}
