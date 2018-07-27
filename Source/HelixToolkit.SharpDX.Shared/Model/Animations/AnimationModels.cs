/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
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
        // Bone name is stored in Mesh with BoneNames (indexes match between Bones and BoneNames)
        public int ParentIndex;
        public Matrix InvBindPose;
        public Matrix BindPose;
        public Matrix BoneLocalTransform;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Keyframe
    {
        public uint BoneIndex;
        public float Time;
        public Matrix Transform;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Animation
    {
        public float StartTime;
        public float EndTime;
        public List<Keyframe> Keyframes;
    };
}
