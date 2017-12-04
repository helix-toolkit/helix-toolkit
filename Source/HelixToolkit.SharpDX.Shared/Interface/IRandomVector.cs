using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Utility
#else
namespace HelixToolkit.Wpf.SharpDX.Utility
#endif
{
    public interface IRandomSeed
    {
        uint Seed { get; }
    }
    public interface IRandomVector : IRandomSeed
    {
        Vector3 RandomVector3 { get; }
    }
}
