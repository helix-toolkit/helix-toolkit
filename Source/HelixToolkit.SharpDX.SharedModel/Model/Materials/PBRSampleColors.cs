using SharpDX;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// https://google.github.io/filament/images/material_chart.jpg
/// </summary>
public static class PBRSampleColors
{
    // Metallic
    public static readonly Color4 Silver = new(250 / 255f, 249 / 255f, 245 / 255f, 1);
    public static readonly Color4 Aluminum = new(244 / 255f, 245 / 255f, 245 / 255f, 1);
    public static readonly Color4 Platinum = new(214 / 255f, 209 / 255f, 200 / 255f, 1);
    public static readonly Color4 Iron = new(192 / 255f, 189 / 255f, 186 / 255f, 1);
    public static readonly Color4 Titanium = new(206 / 255f, 200 / 255f, 194 / 255f, 1);
    public static readonly Color4 Copper = new(251 / 255f, 216 / 255f, 184 / 255f, 1);
    public static readonly Color4 Gold = new(255 / 255f, 220 / 255f, 157 / 255f, 1);
    public static readonly Color4 Brass = new(244 / 255f, 228 / 255f, 173 / 255f, 1);

    //non metallic samples
    public static readonly Color4 Coal = new(50 / 255f, 50 / 255f, 50 / 255f, 1);
    public static readonly Color4 Rubber = new(53 / 255f, 53 / 255f, 53 / 255f, 1);
    public static readonly Color4 Mud = new(85 / 255f, 61 / 255f, 49 / 255f, 1);
    public static readonly Color4 Wood = new(135 / 255f, 92 / 255f, 60 / 255f, 1);
    public static readonly Color4 Vegetation = new(123 / 255f, 130 / 255f, 78 / 255f, 1);
    public static readonly Color4 Brick = new(148 / 255f, 125 / 255f, 117 / 255f, 1);
    public static readonly Color4 Sand = new(177 / 255f, 168 / 255f, 132 / 255f, 1);
    public static readonly Color4 Concrete = new(192 / 255f, 191 / 255f, 187 / 255f, 1);
}
