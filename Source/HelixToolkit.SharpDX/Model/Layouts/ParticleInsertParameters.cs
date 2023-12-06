using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
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
