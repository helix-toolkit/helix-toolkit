using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct PlaneGridModelStruct
{
    public Matrix World;
    public float GridSpacing;
    public float GridThickenss;
    public float FadingFactor;
    public float PlaneD;
    public Vector4 PlaneColor;
    public Vector4 GridColor;
    public bool HasShadowMap;
    public int Axis;
    public int Type;
    private float pad;
    public const int SizeInBytes = 4 * (4 * 4 + 4 * 4);
}
