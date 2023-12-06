using HelixToolkit.SharpDX.Animations;

namespace HelixToolkit.SharpDX;

public class AnimationHierarchy : IGUID
{
    public Guid GUID { get; } = Guid.NewGuid();
    public Dictionary<string, Animation> Animations = new();
    public List<Bone> Bones = new();
    public List<Object3D> Meshes = new();
}
