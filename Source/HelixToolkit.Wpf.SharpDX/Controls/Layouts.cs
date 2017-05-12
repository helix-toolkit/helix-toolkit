using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.Wpf.SharpDX
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DefaultVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 BiTangent;

        public const int SizeInBytes = 4 * (4 + 4 + 2 + 3 + 3 + 3);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct LinesVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public const int SizeInBytes = 4 * (4 + 4);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CubeVertex
    {
        public Vector4 Position;
        public const int SizeInBytes = 4 * 4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BillboardVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public Vector4 TexCoord;
        //public Vector2 Offset;
        public const int SizeInBytes = 4 * (4 + 4 + 4);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BillboardInstanceParameter
    {
        public Color4 DiffuseColor;
        public Vector2 TexCoordScale;
        public Vector2 TexCoordOffset;
        public const int SizeInBytes = 4 * (4 + 2 + 2);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct InstanceParameter
    {
        public Color4 DiffuseColor;       
        public Color4 AmbientColor;
        public Color4 EmissiveColor;
        public Vector2 TexCoordOffset;
        public const int SizeInBytes = 4 * (4 * 3 + 2);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BoneIds
    {
        public int Bone1;
        public int Bone2;
        public int Bone3;
        public int Bone4;
        public Vector4 Weights;

        public const int SizeInBytes = 4 * (4 + 4);
    }

   
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BoneMatricesStruct
    {
        public const int NumberOfBones = 128;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NumberOfBones)]
        public Matrix[] Bones;
        public const int SizeInBytes = 4 * (4 * 4 * NumberOfBones);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Particle
    {
        public Vector3 Position;
        public float Pad0;
        public Vector3 Direction;
        public float Pad1;
        public Vector3 Velocity;
        public float Time;
        public const int SizeInBytes = 4 * (4 * 3);
    }
}
