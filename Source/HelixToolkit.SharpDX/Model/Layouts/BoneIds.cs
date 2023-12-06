using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
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
