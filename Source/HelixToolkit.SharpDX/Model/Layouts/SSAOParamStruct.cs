using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
internal struct SSAOParamStruct
{
    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    //public Vector4[] Kernels;
    public Vector2 NoiseScale;
    public int TextureScale;
    public float Radius;
    public Matrix InvProjection;
    public const int SizeInBytes = 4 * (4 * 32 + 4 + 4 * 4);
}
