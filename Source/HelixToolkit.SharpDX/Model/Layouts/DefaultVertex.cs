using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct DefaultVertex
{
    public Vector4 Position;
    public Vector3 Normal;
    public Vector3 Tangent;
    public Vector3 BiTangent;
    public const int SizeInBytes = 4 * (4 + 3 + 3 + 3);
}
