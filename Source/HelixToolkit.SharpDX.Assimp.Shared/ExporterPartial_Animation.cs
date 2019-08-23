/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Assimp;
using System.Linq;
using Animation = Assimp.Animation;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Assimp
    {
        public partial class Exporter
        {
            public static double DefaultTicksPerSecond = 30;
            private ErrorCode AddAnimationsToScene(Scene scene)
            {
                if(animations == null || animations.Count == 0)
                {
                    return ErrorCode.Succeed;
                }

                for(int i = 0; i < animations.Count; ++i)
                {
                    var ani = new Animation
                    {
                        Name = string.IsNullOrEmpty(animations[i].Name) ? $"Animation_{i}" : animations[i].Name,
                        TicksPerSecond = DefaultTicksPerSecond,
                        DurationInTicks = (animations[i].EndTime - animations[i].StartTime) * DefaultTicksPerSecond
                    };
                    foreach (var f in animations[i].NodeAnimationCollection)
                    {
                        if(f.Node == null || string.IsNullOrEmpty(f.Node.Name))
                        {
                            Log(HelixToolkit.Logger.LogLevel.Warning, $"Node Animation NodeName is empty. AnimationName:{ani.Name}");
                            continue;
                        }
                        var ch = new NodeAnimationChannel
                        {
                            NodeName = f.Node.Name
                        };
                        foreach(var kf in f.KeyFrames)
                        {
                            var t = kf.Time * DefaultTicksPerSecond;
                            ch.PositionKeys.Add(new VectorKey(t, kf.Translation.ToAssimpVector3D()));
                            ch.ScalingKeys.Add(new VectorKey(t, kf.Scale.ToAssimpVector3D()));
                            ch.RotationKeys.Add(new QuaternionKey(t, kf.Rotation.ToAssimpQuaternion()));
                        }

                        ani.NodeAnimationChannels.Add(ch);
                    }

                    scene.Animations.Add(ani);
                }
                return ErrorCode.Succeed;
            }
        }
    }
}
