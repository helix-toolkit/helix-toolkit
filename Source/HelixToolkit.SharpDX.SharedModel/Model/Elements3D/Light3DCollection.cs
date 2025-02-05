using HelixToolkit.SharpDX;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
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
