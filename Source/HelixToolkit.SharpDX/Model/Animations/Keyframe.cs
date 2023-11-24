using SharpDX;

namespace HelixToolkit.SharpDX.Animations;

public struct Keyframe : IKeyFrame
{
    public Vector3 Translation;
    public Quaternion Rotation;
    public Vector3 Scale;
    public float Time { set; get; }
    public int BoneIndex;// Used only for array based bones
    public Matrix ToTransformMatrix()
    {
        return Matrix.Scaling(Scale) * Matrix.RotationQuaternion(Rotation) * Matrix.Translation(Translation);
    }
}
