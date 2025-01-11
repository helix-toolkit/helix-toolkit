using HelixToolkit.SharpDX;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public class ThreePointLight3D : GroupElement3D, ILight3D
{
    public ThreePointLight3D()
    {
    }

    public LightType LightType
    {
        get
        {
            return LightType.ThreePoint;
        }
    }
}
