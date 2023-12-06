using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
///
/// </summary>
[DebuggerDisplay("Name={" + nameof(Name) + "}; Child Count={" + nameof(ItemsCount) + "};")]
public class GroupNode : GroupNodeBase, IHitable
{
    static readonly ILogger logger = Logger.LogManager.Create<GroupNode>();
    private IOctreeManager? octreeManager;
    public IOctreeManager? OctreeManager
    {
        set
        {
            var old = octreeManager;
            if (Set(ref octreeManager, value))
            {
                old?.Clear();
                if (octreeManager != null)
                {
                    foreach (var item in ItemsInternal)
                    {
                        octreeManager.AddPendingItem(item);
                    }
                }
            }
        }
        get
        {
            return octreeManager;
        }
    }

    /// <summary>
    /// Gets the octree in OctreeManager.
    /// </summary>
    /// <value>
    /// The octree.
    /// </value>
    public IOctreeBasic? Octree
    {
        get
        {
            return OctreeManager?.Octree;
        }
    }

    public GroupNode()
    {
        ChildNodeAdded += NodeGroup_OnAddChildNode;
        ChildNodeRemoved += NodeGroup_OnRemoveChildNode;
        Cleared += NodeGroup_OnClear;
    }

    private void NodeGroup_OnClear(object? sender, OnChildNodeChangedArgs e)
    {
        OctreeManager?.Clear();
        OctreeManager?.RequestRebuild();
    }

    private void NodeGroup_OnRemoveChildNode(object? sender, OnChildNodeChangedArgs e)
    {
        OctreeManager?.RemoveItem(e);
    }

    private void NodeGroup_OnAddChildNode(object? sender, OnChildNodeChangedArgs e)
    {
        OctreeManager?.AddPendingItem(e);
    }

    /// <summary>
    /// Updates the not render.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void UpdateNotRender(RenderContext context)
    {
        base.UpdateNotRender(context);
        if (OctreeManager != null)
        {
            OctreeManager.ProcessPendingItems();
            if (OctreeManager.RequestUpdateOctree)
            {
                OctreeManager?.RebuildTree(this.ItemsInternal);
            }
        }
    }

    /// <summary>
    /// Called when [hit test].
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="totalModelMatrix">The total model matrix.</param>
    /// <param name="hits">The hits.</param>
    /// <returns></returns>
    protected override bool OnHitTest(HitTestContext? context, global::SharpDX.Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        var isHit = false;
        if (octreeManager != null)
        {
            isHit = octreeManager.HitTest(context, this.WrapperSource, totalModelMatrix, ref hits);
            if (isHit && logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace("Octree hit test, hit at {0}", hits[0].PointHit);
            }
        }
        else
        {
            isHit = base.OnHitTest(context, totalModelMatrix, ref hits);
        }
        return isHit;
    }
}
