using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// Used combine with <see cref="PointLineMaterialStruct"/>
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct ParticleModelStruct
{
    public Matrix World;
    public int HasInstances;
    public int HasInstanceParams;
    public int HasTexture;
    private int padding;
    public const int SizeInBytes = 4 * (4 * 4 + 4);
}
