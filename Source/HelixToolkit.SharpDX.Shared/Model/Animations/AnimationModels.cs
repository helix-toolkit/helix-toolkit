/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Matrix = System.Numerics.Matrix4x4;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Animations
#else
namespace HelixToolkit.UWP.Animations
#endif
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Bone
    {
        public int ParentIndex;
        public Matrix InvBindPose;
        public Matrix BindPose;
        public Matrix BoneLocalTransform;
        public const int SizeInBytes = 4 * (1 + 4 * 4 * 3);
    };

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Keyframe
    {
        public int BoneIndex;
        public float Time;
        public Matrix Transform;
        public const int SizeInBytes = 4 * (2 + 4 * 4);
    };

    public class Animation
    {
        public Guid GUID { set; get; } = Guid.NewGuid();
        public string Name { set; get; }
        public float StartTime { set; get; }
        public float EndTime { set; get; }
        public List<Keyframe> Keyframes { set; get; } = new List<Keyframe>();
    };
}
