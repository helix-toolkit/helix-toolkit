using HelixToolkit.SharpDX.Animations;

namespace HelixToolkit.SharpDX;

public static class AnimationExtensions
{
    public static Dictionary<string, IAnimationUpdater> CreateAnimationUpdaters(this IEnumerable<Animation> animations)
    {
        var dict = new Dictionary<string, IAnimationUpdater>();
        foreach (var ani in animations)
        {
            switch (ani.AnimationType)
            {
                case AnimationType.Keyframe:
                    if (ani.RootNode is IBoneMatricesNode bNode)
                    {
                        AddUpdaterToDict(dict, new KeyFrameUpdater(ani, bNode.Bones ?? Array.Empty<Bone>()));
                    }
                    else if (ani.BoneSkinMeshes != null)
                    {
                        foreach (var b in ani.BoneSkinMeshes)
                        {
                            AddUpdaterToDict(dict, new KeyFrameUpdater(ani, b.Bones ?? Array.Empty<Bone>()));
                        }
                    }
                    break;
                case AnimationType.Node:
                    AddUpdaterToDict(dict, new NodeAnimationUpdater(ani));
                    break;
                case AnimationType.MorphTarget:
                    if (ani.RootNode is IBoneMatricesNode mNode)
                    {
                        AddUpdaterToDict(dict, new MorphTargetKeyFrameUpdater(ani, mNode.MorphTargetWeights ?? Array.Empty<float>()));
                    }
                    else if (ani.BoneSkinMeshes != null)
                    {
                        foreach (var b in ani.BoneSkinMeshes)
                        {
                            AddUpdaterToDict(dict, new MorphTargetKeyFrameUpdater(ani, b.MorphTargetWeights ?? Array.Empty<float>()));
                        }
                    }
                    break;

            }
        }
        return dict;
    }

    private static void AddUpdaterToDict(Dictionary<string, IAnimationUpdater> dict, IAnimationUpdater updater)
    {
        if (dict.TryGetValue(updater.Name, out var existingUpdater))
        {
            if (existingUpdater is AnimationGroupUpdater group)
            {
                group.Children.Add(updater);
            }
            else
            {
                dict.Remove(updater.Name);
                var newGroup = new AnimationGroupUpdater(updater.Name);
                newGroup.Children.Add(existingUpdater);
                newGroup.Children.Add(updater);
                dict.Add(newGroup.Name, newGroup);
            }
        }
        else
        {
            dict.Add(updater.Name, updater);
        }
    }
}
