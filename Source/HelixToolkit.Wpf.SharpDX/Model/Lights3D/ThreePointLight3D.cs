using HelixToolkit.SharpDX;

namespace HelixToolkit.Wpf.SharpDX;

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
