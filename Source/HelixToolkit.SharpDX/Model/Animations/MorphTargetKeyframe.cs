namespace HelixToolkit.SharpDX.Animations;

public struct MorphTargetKeyframe : IKeyFrame
{
    public float Weight;
    public float Time { set; get; }
    public int Index;
}
