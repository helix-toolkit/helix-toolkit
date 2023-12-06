using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

public sealed class BoneGroupNode : GroupNodeBase, Animations.IBoneMatricesNode
{
    public Matrix[]? BoneMatrices
    {
        set
        {
            core.BoneMatrices = value;
        }
        get
        {
            return core.BoneMatrices;
        }
    }

    /// <summary>
    /// Gets or sets the bones.
    /// </summary>
    /// <value>
    /// The bones.
    /// </value>
    public Animations.Bone[]? Bones
    {
        set; get;
    }

    public float[]? MorphTargetWeights
    {
        set; get;
    }
    /// <summary>
    /// Always return false for bone groups
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has bone group; otherwise, <c>false</c>.
    /// </value>
    public bool HasBoneGroup { get; } = false;

    private readonly BoneUploaderCore core = new();

    public BoneGroupNode()
    {
        ChildNodeAdded += NodeGroup_OnAddChildNode;
        ChildNodeRemoved += NodeGroup_OnRemoveChildNode;
    }

    protected override RenderCore OnCreateRenderCore()
    {
        return core;
    }

    private void NodeGroup_OnRemoveChildNode(object? sender, OnChildNodeChangedArgs e)
    {
        if (e.Node is BoneSkinMeshNode b)
        {
            b.HasBoneGroup = false;

            if (b.RenderCore is BoneSkinRenderCore renderCore)
            {
                renderCore.SharedBoneBuffer = null;
            }
        }
    }

    private void NodeGroup_OnAddChildNode(object? sender, OnChildNodeChangedArgs e)
    {
        if (e.Node is BoneSkinMeshNode b)
        {
            b.HasBoneGroup = true;

            if (b.RenderCore is BoneSkinRenderCore renderCore)
            {
                renderCore.SharedBoneBuffer = core;
            }
        }
    }
}
