using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.Wpf.SharpDX
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DefaultVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 BiTangent;

        public const int SizeInBytes = 4 * (4 + 4 + 2 + 3 + 3 + 3);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct LinesVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public const int SizeInBytes = 4 * (4 + 4);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct CubeVertex
    {
        public Vector4 Position;
        public const int SizeInBytes = 4 * 4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BillboardVertex
    {
        public Vector4 Position;
        public Color4 Color;
        public Vector4 TexCoord;
        //public Vector2 Offset;
        public const int SizeInBytes = 4 * (4 + 4 + 4);
    }
}
