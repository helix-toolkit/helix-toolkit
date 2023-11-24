using SharpDX;

namespace HelixToolkit.SharpDX.Animations;

public interface IBoneMatricesNode
{
    Matrix[]? BoneMatrices
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the bones.
    /// </summary>
    /// <value>
    /// The bones.
    /// </value>
    Bone[]? Bones
    {
        set; get;
    }
    /// <summary>
    /// Gets or sets the morph target weights.
    /// </summary>
    /// <value>
    /// The morph target weights.
    /// </value>
    float[]? MorphTargetWeights
    {
        set; get;
    }
    /// <summary>
    /// Gets a value indicating whether this instance is renderable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is renderable; otherwise, <c>false</c>.
    /// </value>
    bool IsRenderable
    {
        get;
    }
    /// <summary>
    /// Gets the total model matrix.
    /// </summary>
    /// <value>
    /// The total model matrix.
    /// </value>
    Matrix TotalModelMatrix
    {
        get;
    }
    /// <summary>
    /// Gets a value indicating whether this instance has bone group.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has bone group; otherwise, <c>false</c>.
    /// </value>
    bool HasBoneGroup
    {
        get;
    }
}
