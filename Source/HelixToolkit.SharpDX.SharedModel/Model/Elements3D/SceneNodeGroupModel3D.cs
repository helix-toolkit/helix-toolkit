using HelixToolkit.SharpDX.Model.Scene;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// Used to hold scene nodes without WPF/UWP dependencies.
/// Used for code behind only. Avoid performance penalty from Dependency Properties.
/// </summary>
public sealed class SceneNodeGroupModel3D : Element3D
{
    public GroupNode GroupNode { get; } = new();

    /// <summary>
    /// Adds a child node. <see cref="GroupNodeBase.AddChildNode(SceneNode)"/>
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>Success or not</returns>
    public bool AddNode(SceneNode node)
    {
        return GroupNode.AddChildNode(node);
    }

    /// <summary>
    /// Removes child node. <see cref="GroupNodeBase.RemoveChildNode(SceneNode, bool)"/>
    /// If detach = false, then developer must manage the life cycle of the removed child node manually.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <param name="detachChildren">Detach children after being removed.</param>
    /// <returns>Sucess or not</returns>
    public bool RemoveNode(SceneNode node, bool detachChildren = true)
    {
        return GroupNode.RemoveChildNode(node, detachChildren);
    }

    /// <summary>
    /// Clears this group. <see cref="GroupNodeBase.Clear"/>. If detach = false, then developer must manage the life cycle of the cleared child nodes manually.
    /// </summary>
    /// <param name="detachChildren">
    /// </param>
    public void Clear(bool detachChildren = true)
    {
        GroupNode.Clear(detachChildren);
    }

    /// <summary>
    /// Moves the child node order. <see cref="GroupNodeBase.MoveChildNode(int, int)"/>
    /// </summary>
    /// <param name="fromIndex">From index.</param>
    /// <param name="toIndex">To index.</param>
    public void MoveNode(int fromIndex, int toIndex)
    {
        GroupNode.MoveChildNode(fromIndex, toIndex);
    }

    /// <summary>
    /// Transfers the node to another group. <see cref="GroupNodeBase.TransferChildNode(SceneNode, GroupNodeBase)"/>
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="target">The target.</param>
    public void TransferNode(SceneNode item, GroupNodeBase target)
    {
        GroupNode.TransferChildNode(item, target);
    }

    /// <summary>
    /// Called when [create scene node].
    /// </summary>
    /// <returns></returns>
    protected override SceneNode OnCreateSceneNode()
    {
        return GroupNode;
    }
}
