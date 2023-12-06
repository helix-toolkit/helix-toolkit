using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Interface for dynamic octree
/// </summary>
public interface IDynamicOctree : IOctreeBasic
{
    /// <summary>
    /// Gets the self as array.
    /// </summary>
    /// <value>
    /// The self array.
    /// </value>
    IDynamicOctree?[] SelfArray
    {
        get;
    }
    /// <summary>
    /// This is a bitmask indicating which child nodes are actively being used.
    /// It adds slightly more complexity, but is faster for performance since there is only one comparison instead of 8.
    /// </summary>
    byte ActiveNodes
    {
        set; get;
    }
    /// <summary>
    /// Has child octants
    /// </summary>
    bool HasChildren
    {
        get;
    }
    /// <summary>
    /// If this node is root node
    /// </summary>
    bool IsRoot
    {
        get;
    }
    /// <summary>
    /// Parent node
    /// </summary>
    IDynamicOctree? Parent
    {
        get; set;
    }
    /// <summary>
    /// Child octants
    /// </summary>
    IDynamicOctree?[] ChildNodes
    {
        get;
    }
    /// <summary>
    /// Octant bounds
    /// </summary>
    BoundingBox[]? Octants
    {
        get;
    }

    /// <summary>
    /// Delete self if is empty;
    /// </summary>
    bool AutoDeleteIfEmpty
    {
        set; get;
    }

    /// <summary>
    /// Returns true if this node tree and all children have no content
    /// </summary>
    bool IsEmpty
    {
        get;
    }

    /// <summary>
    /// Hit test for only this node, not its child node
    /// </summary>
    /// <param name="context"></param>
    /// <param name="model"></param>
    /// <param name="geometry"></param>
    /// <param name="modelMatrix"></param>
    /// <param name="hits"></param>
    /// <param name="isIntersect"></param>
    /// <param name="hitThickness">Only used for point/line hit test</param>
    /// <param name="rayModel"></param>
    /// <returns></returns>
    bool HitTestCurrentNodeExcludeChild(HitTestContext? context, object? model, Geometry3D? geometry, Matrix modelMatrix, ref Ray rayModel,
        ref List<HitTestResult> hits, ref bool isIntersect, float hitThickness);

    /// <summary>
    /// Search nearest point by a search sphere at this node only
    /// </summary>
    /// <param name="context"></param>
    /// <param name="sphere"></param>
    /// <param name="result"></param>
    /// <param name="isIntersect"></param>
    /// <returns></returns>
    bool FindNearestPointBySphereExcludeChild(HitTestContext? context, ref BoundingSphere sphere, ref List<HitTestResult> result, ref bool isIntersect);

    /// <summary>
    /// Build current node level only, this will only build current node and create children, but not build its children. 
    /// To build from top to bottom, call BuildTree
    /// </summary>
    void BuildCurretNodeOnly();
    /// <summary>
    /// Clear the tree
    /// </summary>
    void Clear();
    /// <summary>
    /// Remove self from parent node
    /// </summary>
    void RemoveSelf();
    /// <summary>
    /// Remove child from ChildNodes
    /// </summary>
    /// <param name="child"></param>
    void RemoveChild(IDynamicOctree child);
}
