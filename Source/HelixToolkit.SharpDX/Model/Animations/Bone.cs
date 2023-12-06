using SharpDX;

namespace HelixToolkit.SharpDX.Animations;

public struct Bone
{
    public string Name;
    public Model.Scene.SceneNode ParentNode; // Used for scene graph based node animation
    public Model.Scene.SceneNode Node; // Used for scene graph based node animation
    public int ParentIndex;// Used only for array based bones
    public Matrix InvBindPose;
    public Matrix BindPose;
    public Matrix BoneLocalTransform;
};
