using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct BatchedMeshVertex
{
    public Vector4 Position;
    public Vector3 Normal;
    public Vector3 Tangent;
    public Vector3 BiTangent;
    public Vector2 TexCoord;
    public Vector4 Color;//Diffuse, Emissive, Specular, Reflect
    public Vector4 Color2;//Ambient, sMaterialShininess, diffuseAlpha
    public const int SizeInBytes = 4 * (4 + 3 + 3 + 3 + 2 + 4 + 4);
}
