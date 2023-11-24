using HelixToolkit.SharpDX.Model;

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

/// <summary>
/// Render color by mesh vertex position
/// </summary>
public sealed class PositionColorMaterial : Material
{
    protected override MaterialCore OnCreateCore()
    {
        return PositionMaterialCore.Core;
    }

    public PositionColorMaterial()
    {
    }

    public PositionColorMaterial(PositionMaterialCore core) : base(core)
    {
    }

#if WPF
    protected override Freezable CreateInstanceCore()
    {
        return new PositionColorMaterial()
        {
            Name = Name
        };
    }
#endif
}
