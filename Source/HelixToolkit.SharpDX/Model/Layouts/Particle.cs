using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct Particle
{
    private Vector3 position;
    private float initEnergy;
    private Vector3 velocity;
    private float energy;
    private Color4 color;
    private Vector3 initAcceleration;
    private float dissipRate;
    private uint texRow;
    private uint texColumn;

    public const int SizeInBytes = 4 * (4 * 4 + 2);
}
