using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct MorphTargetVertex
{
    public Vector3 deltaPosition;
    public Vector3 deltaNormal;
    public Vector3 deltaTangent;
}
