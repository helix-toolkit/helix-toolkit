#if NETFX_CORE
namespace HelixToolkit.UWP
#else
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
        IOctree Octree { get; }
        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>
        /// The manager.
        /// </value>
        IOctreeManager Manager { get; }
    }
}
