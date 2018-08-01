/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Matrix = System.Numerics.Matrix4x4;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        public uint BoneIndex;
        public float Time;
        public Matrix Transform;
        public const int SizeInBytes = 4 * (2 + 4 * 4);
    };

    public struct Animation
    {
        public float StartTime;
        public float EndTime;
        public List<Keyframe> Keyframes;
    };
}
