using HelixToolkit.SharpDX.Model.Scene;
using SharpDX;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// 
/// </summary>
public sealed class InstancingRenderableOctreeManager : OctreeManagerBase
{
    /// <summary>
    /// Adds the pending item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override bool AddPendingItem(SceneNode? item)
    {
        return false;
    }
    /// <summary>
    /// Clears this instance.
    /// </summary>
    public override void Clear()
    {
        Octree = null;
    }
    /// <summary>
    /// Processes the pending items.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public override void ProcessPendingItems()
    {

    }
    /// <summary>
    /// Rebuilds the tree.
    /// </summary>
    /// <param name="items">The items.</param>
    public override void RebuildTree(IEnumerable<SceneNode> items)
    {
        Clear();
        if (items == null)
        {
            return;
        }
        if (items.FirstOrDefault() is IInstancing inst)
        {
            var instMatrix = inst.InstanceBuffer.Elements;
            var octree = instMatrix is null ? null : new StaticInstancingModelOctree(instMatrix, (inst as SceneNode)?.OriginalBounds ?? new BoundingBox(), this.Parameter);
            //new InstancingModel3DOctree(instMatrix, (inst as SceneNode).OriginalBounds, this.Parameter, new Stack<KeyValuePair<int, IOctree[]>>(10));
            octree?.BuildTree();
            Octree = octree;
        }
    }
    /// <summary>
    /// Removes the item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <exception cref="NotImplementedException"></exception>
    public override void RemoveItem(SceneNode? item)
    {
    }
    /// <summary>
    /// Requests the rebuild.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public override void RequestRebuild()
    {
    }
}
