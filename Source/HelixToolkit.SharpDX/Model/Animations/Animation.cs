namespace HelixToolkit.SharpDX.Animations;

/// <summary>
/// 
/// </summary>
public class Animation
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    /// <value>
    /// The unique identifier.
    /// </value>
    public Guid GUID { set; get; } = Guid.NewGuid();
    /// <summary>
    /// Gets or sets the type of the animation.
    /// </summary>
    /// <value>
    /// The type of the animation.
    /// </value>
    public AnimationType AnimationType
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name
    {
        set; get;
    } = string.Empty;
    /// <summary>
    /// Gets or sets the start time.
    /// </summary>
    /// <value>
    /// The start time.
    /// </value>
    public float StartTime
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the end time.
    /// </summary>
    /// <value>
    /// The end time.
    /// </value>
    public float EndTime
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the keyframes.
    /// </summary>
    /// <value>
    /// The keyframes.
    /// </value>
    public List<Keyframe> Keyframes
    {
        set; get;
    } = new();
    /// <summary>
    /// Gets or sets the node animation collection.
    /// </summary>
    /// <value>
    /// The node animation collection.
    /// </value>
    public List<NodeAnimation> NodeAnimationCollection
    {
        set; get;
    } = new();
    /// <summary>
    /// Gets or sets the morph target keyframes. 
    /// </summary>
    public List<MorphTargetKeyframe> MorphTargetKeyframes
    {
        get; set;
    } = new();
    /// <summary>
    /// Gets or sets the bone skin meshes.
    /// </summary>
    /// <value>
    /// The bone skin meshes.
    /// </value>
    public List<IBoneMatricesNode> BoneSkinMeshes
    {
        set; get;
    } = new();
    /// <summary>
    /// Gets a value indicating whether this animation has bone skin meshes.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this animation has bone skin meshes; otherwise, <c>false</c>.
    /// </value>
    public bool HasBoneSkinMeshes
    {
        get => BoneSkinMeshes != null && BoneSkinMeshes.Count > 0;
    }
    /// <summary>
    /// Gets or sets the root node of this animation
    /// </summary>
    /// <value>
    /// The root node.
    /// </value>
    public Model.Scene.SceneNode? RootNode
    {
        set; get;
    }
    /// <summary>
    /// Initializes a new animation of the <see cref="Animation"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    public Animation(AnimationType type)
    {
        AnimationType = type;
    }
}
