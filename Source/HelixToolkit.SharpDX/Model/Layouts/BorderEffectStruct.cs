using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct BorderEffectStruct
{
    public Color4 Color;
    public Matrix Param;
    public float ViewportScale; //Used to handle using lower resolution render target for bluring. Scale = Full Resolution / Low Resolution
    private Vector3 padding;

    public const int SizeInBytes = 4 * (4 + 4 * 4 + 4);
}
