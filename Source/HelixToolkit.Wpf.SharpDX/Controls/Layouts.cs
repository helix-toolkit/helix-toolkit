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
    public struct PointsVertex
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
        Vector3 position;
        float initEnergy;
        Vector3 velocity;
        float energy;
        Color4 color;
        Vector3 initAcceleration;
        float dissipRate;
        uint texRow;
        uint texColumn;
        public const int SizeInBytes = 4 * (4 * 4 + 2);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ParticlePerFrame
    {
        public uint NumParticles;
        public Vector3 ExtraAcceleration;

        public float TimeFactors;
        public Vector3 DomainBoundsMax;

        public Vector3 DomainBoundsMin;
        public uint CumulateAtBound;

        public Vector3 ConsumerLocation;
        public float ConsumerGravity;

        public float ConsumerRadius;
        private Vector3 Pad;

        public const int SizeInBytes = 4 * (4 * 5);
        public const int NumParticlesOffset = 0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ParticleInsertParameters
    {
        public Vector3 EmitterLocation;
        public float InitialEnergy;

        public float EmitterRadius;
        private Vector2 Pad;
        public float InitialVelocity;

        public Color4 ParticleBlendColor;

        public float EnergyDissipationRate; //Energy dissipation rate per second
        public Vector3 InitialAcceleration;

        public const int SizeInBytes = 4 * (4 * 4);
        public const int NumParticlesOffset = 0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ParticleCountIndirectArgs
    {
        public uint VertexCount;
        public uint InstanceCount;
        public uint StartVertexLocation;
        public uint StartInstanceLocation;
        public const int SizeInBytes = 4 * 4;
    }
}
