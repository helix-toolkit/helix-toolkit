namespace HelixToolkit.SharpDX;

public class BillboardHitResult : HitTestResult
{
    public int TextInfoIndex { set; get; } = -1;
    public TextInfo? TextInfo { set; get; } = null;
    public BillboardType Type
    {
        set; get;
    }
}
