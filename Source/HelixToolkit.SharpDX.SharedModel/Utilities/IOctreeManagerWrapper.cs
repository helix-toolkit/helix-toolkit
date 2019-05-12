#if NETFX_CORE
namespace HelixToolkit.UWP
#else
#if COREWPF
namespace HelixToolkit.SharpDX.Core.Wpf
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
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
