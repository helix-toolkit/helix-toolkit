using HelixToolkit.SharpDX;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
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
