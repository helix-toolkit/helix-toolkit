using HelixToolkit.SharpDX.Model;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

public class LineArrowHeadMaterial : LineMaterial
{
    public double ArrowSize
    {
        get
        {
            return (double)GetValue(ArrowSizeProperty);
        }
        set
        {
            SetValue(ArrowSizeProperty, value);
        }
    }

    public static readonly DependencyProperty ArrowSizeProperty =
        DependencyProperty.Register("ArrowSize", typeof(double), typeof(LineArrowHeadMaterial), new PropertyMetadata(0.1, (d, e) =>
        {
            if (d is LineMaterial { Core: LineArrowHeadMaterialCore core })
            {
                core.ArrowSize = (float)(double)e.NewValue;
            }
        }));


    protected override MaterialCore OnCreateCore()
    {
        return new LineArrowHeadMaterialCore()
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
