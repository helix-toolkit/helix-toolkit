using HelixToolkit.SharpDX.Model;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public class LineArrowHeadTailMaterial : LineArrowHeadMaterial
{
    protected override MaterialCore OnCreateCore()
    {
        return new LineArrowHeadTailMaterialCore()
        {
            Name = Name,
            LineColor = Color.ToColor4(),
            Smoothness = (float)Smoothness,
            Thickness = (float)Thickness,
            EnableDistanceFading = EnableDistanceFading,
            FadingNearDistance = (float)FadingNearDistance,
            FadingFarDistance = (float)FadingFarDistance,
            Texture = Texture,
            TextureScale = (float)TextureScale,
            SamplerDescription = SamplerDescription,
            ArrowSize = (float)ArrowSize,
            FixedSize = FixedSize
        };
    }
}
