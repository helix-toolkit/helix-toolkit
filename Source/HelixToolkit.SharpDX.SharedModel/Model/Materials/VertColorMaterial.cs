using HelixToolkit.SharpDX.Model;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
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

#if WPF
    protected override Freezable CreateInstanceCore()
    {
        return new VertColorMaterial()
        {
            Name = Name
        };
    }
#endif
}
