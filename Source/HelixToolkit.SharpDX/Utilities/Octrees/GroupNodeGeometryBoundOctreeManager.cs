using HelixToolkit.SharpDX.Model.Scene;
using Microsoft.Extensions.Logging;
using SharpDX;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// Use to create geometryModel3D octree for groups. Each ItemsModel3D must has its own manager, do not share between two ItemsModel3D
/// </summary>
public sealed class GroupNodeGeometryBoundOctreeManager : OctreeManagerBase
{
    private static readonly ILogger logger = Logger.LogManager.Create<GroupNodeGeometryBoundOctreeManager>();
    private object lockObj = new();

    private readonly HashSet<SceneNode> NonBoundableItems = new();

    private void UpdateOctree(BoundableNodeOctree? tree)
    {
        Octree = tree;
        mOctree = tree;
    }
    /// <summary>
    /// Rebuilds the tree.
    /// </summary>
    /// <param name="items">The items.</param>
    public override void RebuildTree(IEnumerable<SceneNode> items)
    {
        lock (lockObj)
        {
            RequestUpdateOctree = false;
            if (Enabled)
            {
                var nodes = items.Where(x => x.HasBound);
                if (nodes.Count() == 0)
                {
                    return;
                }
                UpdateOctree(RebuildOctree(nodes));
                if (Octree == null)
                {
                    RequestRebuild();
                }
                else
                {
                    foreach (var item in nodes)
                    {
                        NonBoundableItems.Add(item);
                    }
                }
            }
            else
            {
                Clear();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SubscribeBoundChangeEvent(SceneNode item)
    {
        item.TransformBoundChanged -= Item_OnBoundChanged;
        item.TransformBoundChanged += Item_OnBoundChanged;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UnsubscribeBoundChangeEvent(SceneNode item)
    {
        item.TransformBoundChanged -= Item_OnBoundChanged;
    }

    private readonly HashSet<SceneNode> pendingItems = new();

    private void Item_OnBoundChanged(object? sender, BoundChangeArgs<BoundingBox> args)
    {
        if (sender is not SceneNode item)
        {
            return;
        }
        else
        {
            pendingItems.Add(item);
            return;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public override void ProcessPendingItems()
    {
        lock (lockObj)
        {
            foreach (var item in pendingItems)
            {
                if (!item.HasBound)
                {
                    NonBoundableItems.Add(item);
                    continue;
                }
                if (mOctree == null || !item.IsAttached)
                {
                    UnsubscribeBoundChangeEvent(item);
                    continue;
                }
                var node = mOctree.FindItemByGuid(item.GUID, item, out int index);
                var rootAdd = true;
                if (node != null)
                {
                    var tree = mOctree;
                    UpdateOctree(null);
                    var geoNode = node as BoundableNodeOctree;
                    if (geoNode is not null && geoNode.Bound.Contains(item.BoundsWithTransform) == ContainmentType.Contains)
                    {
                        if (geoNode.PushExistingToChild(index))
                        {
                            tree = tree.Shrink() as BoundableNodeOctree;
                        }
                        rootAdd = false;
                    }
                    else
                    {
                        geoNode?.RemoveAt(index, tree);
                    }
                    UpdateOctree(tree);
                }
                else
                {
                    mOctree.RemoveByGuid(item.GUID, item, mOctree);
                }
                if (rootAdd)
                {
                    AddItem(item);
                }
            }
            pendingItems.Clear();
        }
    }

    private BoundableNodeOctree? RebuildOctree(IEnumerable<SceneNode>? items)
    {
        Clear();
        if (items == null)
        {
            return null;
        }
        var tree = new BoundableNodeOctree(items.ToList(), Parameter);
        tree.BuildTree();
        if (tree.TreeBuilt)
        {
            foreach (var item in items)
            {
                SubscribeBoundChangeEvent(item);
            }
        }
        return tree.TreeBuilt ? tree : null;
    }

    /// <summary>
    /// Adds the pending item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    public override bool AddPendingItem(SceneNode? item)
    {
        lock (lockObj)
        {
            if (Enabled && item != null)
            {
                if (item.HasBound)
                {
                    item.TransformBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
                    item.TransformBoundChanged += GeometryModel3DOctreeManager_OnBoundInitialized;
                    pendingItems.Add(item);
                }
                else
                {
                    NonBoundableItems.Add(item);
                }
                //if (item.Bounds != ZeroBound)
                //{
                //    AddItem(item);
                //}
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void GeometryModel3DOctreeManager_OnBoundInitialized(object? sender, BoundChangeArgs<BoundingBox> args)
    {
        var item = sender as SceneNode;
        if (item is not null)
        {
            item.TransformBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
        }
        AddItem(item);
    }

    private void AddItem(SceneNode? item)
    {
        if (Enabled && item != null)
        {
            if (item.HasBound)
            {
                var tree = mOctree;
                UpdateOctree(null);
                if (tree == null)
                {
                    RequestRebuild();
                }
                else
                {
                    var succeed = true;
                    var counter = 0;
                    while (!tree!.Add(item))
                    {
                        var direction = (item.Bounds.Minimum + item.Bounds.Maximum)
                            - (tree.Bound.Minimum + tree.Bound.Maximum);
                        tree = tree.Expand(ref direction) as BoundableNodeOctree;
                        ++counter;
                        if (counter > 10)
                        {
#if DEBUG
                            throw new Exception("Expand tree failed");
#else
                                    succeed = false;
                                    break;
#endif
                        }
                    }
                    if (succeed)
                    {
                        UpdateOctree(tree);
                        SubscribeBoundChangeEvent(item);
                    }
                    else
                    {
                        RequestRebuild();
                    }
                }
            }
            else
            {
                NonBoundableItems.Add(item);
            }
        }
    }
    /// <summary>
    /// Removes the item.
    /// </summary>
    /// <param name="item">The item.</param>
    public override void RemoveItem(SceneNode? item)
    {
        if (Enabled && Octree != null && item != null)
        {
            lock (lockObj)
            {
                if (item.HasBound)
                {
                    var tree = mOctree;
                    UpdateOctree(null);
                    item.TransformBoundChanged -= GeometryModel3DOctreeManager_OnBoundInitialized;
                    UnsubscribeBoundChangeEvent(item);
                    if (!tree!.RemoveByBound(item))
                    {
                        if (logger.IsEnabled(LogLevel.Debug))
                        {
                            logger.LogDebug("Remove failed.");
                        }
                    }
                    else
                    {
                        tree = tree.Shrink() as BoundableNodeOctree;
                    }
                    UpdateOctree(tree);
                }
                else
                {
                    NonBoundableItems.Remove(item);
                }
            }
        }
    }
    /// <summary>
    /// Clears this instance.
    /// </summary>
    public override void Clear()
    {
        lock (lockObj)
        {
            RequestUpdateOctree = false;
            UpdateOctree(null);
            NonBoundableItems.Clear();
        }
    }
    /// <summary>
    /// Requests the rebuild.
    /// </summary>
    public override void RequestRebuild()
    {
        lock (lockObj)
        {
            Clear();
            RequestUpdateOctree = true;
        }
    }
    public override bool HitTest(HitTestContext? context, object? model, Matrix modelMatrix, ref List<HitTestResult> hits)
    {
        if (Octree == null)
        {
            return false;
        }
        var hit = Octree.HitTest(context, model, null, modelMatrix, ref hits);
        foreach (var item in NonBoundableItems)
        {
            hit |= item.HitTest(context, ref hits);
        }
        return hit;
    }
}
