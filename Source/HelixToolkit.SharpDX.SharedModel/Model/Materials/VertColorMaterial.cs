using HelixToolkit.SharpDX.Model;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// Render color by mesh vertex color
/// </summary>
public sealed class VertColorMaterial : Material
{
    protected override MaterialCore OnCreateCore()
    {
        return ColorMaterialCore.Core;
    }

    public VertColorMaterial()
    {
    }

    public VertColorMaterial(ColorMaterialCore core) : base(core)
    {
    }

#if false
#elif WINUI
#elif WPF
    protected override Freezable CreateInstanceCore()
    {
        return new VertColorMaterial()
        {
            Name = Name
        };
    }
#else
#error Unknown framework
#endif
}
