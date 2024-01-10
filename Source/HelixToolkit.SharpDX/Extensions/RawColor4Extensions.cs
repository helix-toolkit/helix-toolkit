using SharpDX.Mathematics.Interop;

namespace HelixToolkit.SharpDX;
public static class RawColor4Extensions
{
    public static RawColor4 ToRawColor4(this Color4 color)
    {
        return color.ToStruct<Color4, RawColor4>();
    }

    public static RawColor4 ToRawColor4(this Color color)
    {
        return new RawColor4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }

}
