namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public sealed class OctreeArgs : EventArgs
{
    /// <summary>
    /// The octree
    /// </summary>
    public readonly IOctreeBasic? Octree;
    /// <summary>
    /// Initializes a new instance of the <see cref="OctreeArgs"/> class.
    /// </summary>
    /// <param name="octree">The octree.</param>
    public OctreeArgs(IOctreeBasic? octree)
    {
        Octree = octree;
    }
}
