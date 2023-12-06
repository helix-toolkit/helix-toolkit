using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct ShadowMapParamStruct
{
    public Vector2 ShadowMapSize;
    public int HasShadowMap;
    float paddingShadow0;
    public Vector4 ShadowMapInfo;
    public Matrix LightView;
    public Matrix LightProjection;
    public const int SizeInBytes = 4 * (4 * 2 + 4 * 4 * 2);
}
