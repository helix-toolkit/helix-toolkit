#if NETFX_CORE
namespace HelixToolkit.UWP
#elif WINUI_NET5_0
namespace HelixToolkit.WinUI
#else
#if COREWPF
using HelixToolkit.SharpDX.Core;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
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
        IOctreeBasic Octree { get; }
        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        IOctreeManager Manager { get; }
    }
}
