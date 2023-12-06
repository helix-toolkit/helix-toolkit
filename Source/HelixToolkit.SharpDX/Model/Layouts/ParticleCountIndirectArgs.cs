using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct ParticleCountIndirectArgs
{
    public uint VertexCount;
    public uint InstanceCount;
    public uint StartVertexLocation;
    public uint StartInstanceLocation;
    public const int SizeInBytes = 4 * 4;
}
