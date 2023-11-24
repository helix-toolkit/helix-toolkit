namespace HelixToolkit.SharpDX.Animations;

/// <summary>
///
/// </summary>
public interface IAnimationNode
{
    /// <summary>
    /// Gets or sets a value indicating whether this scene node is animation node.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this scene node is animation node; otherwise, <c>false</c>.
    /// </value>
    bool IsAnimationNode
    {
        set; get;
    }
    /// <summary>
    /// Gets a value indicating whether this scene node is animation node root.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this scene node is animation node root; otherwise, <c>false</c>.
    /// </value>
    bool IsAnimationNodeRoot
    {
        get;
    }
}
