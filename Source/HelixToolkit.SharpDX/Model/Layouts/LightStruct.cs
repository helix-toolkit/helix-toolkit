using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct LightStruct
{
    public int LightType;
    Vector3 padding;
    public Vector4 LightDir;
    public Vector4 LightPos;
    public Vector4 LightAtt;
    public Vector4 LightSpot; //(outer angle , inner angle, falloff, free)
    public Color4 LightColor;
    public Matrix LightView;
    public Matrix LightProj;
    public const int SizeInBytes = 4 * (4 * 6 + 4 * 4 * 2);
}
