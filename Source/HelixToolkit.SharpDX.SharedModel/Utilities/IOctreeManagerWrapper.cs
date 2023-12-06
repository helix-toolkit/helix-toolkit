using HelixToolkit.SharpDX;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
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
