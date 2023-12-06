namespace HelixToolkit.SharpDX.Animations;

public struct NodeAnimation
{
    public Model.Scene.SceneNode Node; // Used for scene graph based node animation
    public FastList<Keyframe> KeyFrames;
}
