/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
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
    namespace Animations
    {
        public struct Bone
        {
            public Model.Scene.SceneNode ParentNode; // Used for scene graph based node animation
            public Model.Scene.SceneNode Node; // Used for scene graph based node animation
            public int ParentIndex;// Used only for array based bones
            public Matrix InvBindPose;
            public Matrix BindPose;
            public Matrix BoneLocalTransform;
        };

        public struct Keyframe
        {
            public int BoneIndex;// Used only for array based bones
            public float Time;
            public Matrix Transform;
        };

        public struct NodeAnimation
        {
            public Model.Scene.SceneNode Node; // Used for scene graph based node animation
            public List<Keyframe1> KeyFrames;
        }

        public struct Keyframe1
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
            public float Time;
        }

        public class Animation
        {
            public Guid GUID { set; get; } = Guid.NewGuid();
            public string Name { set; get; }
            public float StartTime { set; get; }
            public float EndTime { set; get; }
            public List<Keyframe> Keyframes { set; get; }
            public List<NodeAnimation> NodeAnimationCollection { set; get; }
        };
    }

}
