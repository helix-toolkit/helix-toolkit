using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
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
    public Vector3 RandomVector;

    public uint RandomSeed;
    public uint NumTexCol;
    public uint NumTexRow;
    public int AnimateByEnergyLevel;

    public Vector2 ParticleSize;
    public float Turbulance;
    float padding;

    public const int SizeInBytes = 4 * (4 * 7);
    public const int NumParticlesOffset = 0;
}
