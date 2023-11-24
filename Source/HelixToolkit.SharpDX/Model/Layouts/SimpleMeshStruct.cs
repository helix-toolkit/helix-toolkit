using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Used for simple mesh rendering without materials. Such as ShadowPass
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SimpleMeshStruct
{
    public Matrix World;
    public int HasInstances;
    Vector3 padding;
    public const int SizeInBytes = 4 * (4 * 4 + 4);
}
