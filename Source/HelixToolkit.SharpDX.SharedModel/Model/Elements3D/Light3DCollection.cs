using HelixToolkit.SharpDX;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public class Light3DCollection : GroupElement3D, ILight3D
{
    public LightType LightType
    {
        get
        {
            return LightType.None;
        }
    }

    public override bool HitTest(HitTestContext? context, ref List<HitTestResult> hits)
    {
        return false;
    }
}
