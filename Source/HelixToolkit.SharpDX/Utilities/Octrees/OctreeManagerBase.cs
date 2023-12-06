using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Model.Scene;
using Microsoft.Extensions.Logging;
using SharpDX;

namespace HelixToolkit.SharpDX.Utilities;

/// <summary>
/// 
/// </summary>
public abstract class OctreeManagerBase : ObservableObject, IOctreeManager
{
    private static readonly ILogger logger = Logger.LogManager.Create<OctreeManagerBase>();
    /// <summary>
    /// Occurs when [on octree created].
    /// </summary>
    public event EventHandler<OctreeArgs>? OnOctreeCreated;

    private IOctreeBasic? octree;
    /// <summary>
    /// Gets or sets the octree.
    /// </summary>
    /// <value>
    /// The octree.
    /// </value>
    public IOctreeBasic? Octree
    {
        protected set
        {
            if (Set(ref octree, value))
            {
                OnOctreeCreated?.Invoke(this, new OctreeArgs(value));
            }
        }
        get
        {
            return octree;
        }
    }
    /// <summary>
    /// The m octree
    /// </summary>
    protected BoundableNodeOctree? mOctree = null;
    /// <summary>
    /// Gets or sets the parameter.
    /// </summary>
    /// <value>
    /// The parameter.
    /// </value>
    public OctreeBuildParameter Parameter { set; get; } = new OctreeBuildParameter();

    private bool mEnabled = true;
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="OctreeManagerBase"/> is enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public bool Enabled
    {
        set
        {
            mEnabled = value;
            if (!mEnabled)
            {
                Clear();
            }
        }
        get
        {
            return mEnabled;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [request update octree].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [request update octree]; otherwise, <c>false</c>.
    /// </value>
    public bool RequestUpdateOctree
    {
        get
        {
            return mRequestUpdateOctree;
        }
        protected set
        {
            mRequestUpdateOctree = value;
        }
    }
    private volatile bool mRequestUpdateOctree = false;
    /// <summary>
    /// Adds the pending item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    public abstract bool AddPendingItem(SceneNode? item);
    /// <summary>
    /// Clears this instance.
    /// </summary>
    public abstract void Clear();
    /// <summary>
    /// Rebuilds the tree.
    /// </summary>
    /// <param name="items">The items.</param>
    public abstract void RebuildTree(IEnumerable<SceneNode> items);
    /// <summary>
    /// Removes the item.
    /// </summary>
    /// <param name="item">The item.</param>
    public abstract void RemoveItem(SceneNode? item);
    /// <summary>
    /// Requests the rebuild.
    /// </summary>
    public abstract void RequestRebuild();
    /// <summary>
    /// Processes the pending items.
    /// </summary>
    public abstract void ProcessPendingItems();

    /// <summary>
    /// Normal hit test from top to bottom
    /// </summary>
    /// <param name="context"></param>
    /// <param name="model"></param>
    /// <param name="modelMatrix"></param>
    /// <param name="hits"></param>
    /// <returns></returns>
    public virtual bool HitTest(HitTestContext? context, object? model, Matrix modelMatrix, ref List<HitTestResult> hits)
    {
        return Octree is null ? false : Octree.HitTest(context, model, null, modelMatrix, ref hits);
    }
}
