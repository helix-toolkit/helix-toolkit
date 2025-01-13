using HelixToolkit.SharpDX;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// 
/// </summary>
public interface IOctreeManagerWrapper
{
    /// <summary>
    /// Gets the octree.
    /// </summary>
    /// <value>
    /// The octree.
    /// </value>
    IOctreeBasic? Octree
    {
        get;
    }

    /// <summary>
    /// Gets the manager.
    /// </summary>
    /// <value>
    /// The manager.
    /// </value>
    IOctreeManager Manager
    {
        get;
    }
}
