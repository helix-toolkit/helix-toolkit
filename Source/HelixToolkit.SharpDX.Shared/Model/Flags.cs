using System;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    [Flags]
    public enum ShaderStage
    {
        None = 0,
        Vertex = 1,
        Hull = 2,
        Domain = 4,
        Geometry = 8,
        Pixel = 16,
        Compute = 32
    }
}
