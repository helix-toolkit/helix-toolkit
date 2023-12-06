using HelixToolkit.SharpDX;

namespace HelixToolkit.WinUI.SharpDX;

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
