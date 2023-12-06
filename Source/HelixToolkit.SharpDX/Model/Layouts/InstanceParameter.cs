using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct InstanceParameter
{
    public Color4 DiffuseColor;
    public Color4 EmissiveColor;
    public Vector2 TexCoordOffset;
    public const int SizeInBytes = 4 * (4 * 2 + 2);
}
