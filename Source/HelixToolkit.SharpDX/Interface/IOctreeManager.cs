﻿using HelixToolkit.SharpDX.Model.Scene;
using SharpDX;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
public interface IOctreeManager
{
    /// <summary>
    /// 
    /// </summary>
    event EventHandler<OctreeArgs> OnOctreeCreated;
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
    /// Gets or sets a value indicating whether this <see cref="IOctreeManager"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    bool Enabled
    {
        get; set;
    }
    /// <summary>
    /// Gets a value indicating whether [request update octree].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [request update octree]; otherwise, <c>false</c>.
    /// </value>
    bool RequestUpdateOctree
    {
        get;
    }
    /// <summary>
    /// Gets or sets the parameter.
    /// </summary>
    /// <value>
    /// The parameter.
    /// </value>
    OctreeBuildParameter Parameter
    {
        set; get;
    }
    /// <summary>
    /// Adds the pending item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    bool AddPendingItem(SceneNode? item);
    /// <summary>
    /// Processes the pending items.
    /// </summary>
    void ProcessPendingItems();
    /// <summary>
    /// Clears this instance.
    /// </summary>
    void Clear();
    /// <summary>
    /// Rebuilds the tree.
    /// </summary>
    /// <param name="items">The items.</param>
    void RebuildTree(IEnumerable<SceneNode> items);
    /// <summary>
    /// Removes the item.
    /// </summary>
    /// <param name="item">The item.</param>
    void RemoveItem(SceneNode? item);
    /// <summary>
    /// Requests the rebuild.
    /// </summary>
    void RequestRebuild();

    /// <summary>
    /// Normal hit test from top to bottom
    /// </summary>
    /// <param name="context"></param>
    /// <param name="model"></param>
    /// <param name="modelMatrix"></param>
    /// <param name="hits"></param>
    /// <returns></returns>
    bool HitTest(HitTestContext? context, object? model, Matrix modelMatrix, ref List<HitTestResult> hits);
}
