using HelixToolkit.SharpDX.Model;

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX;
#else
#error Unknown framework
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

#if false
#elif WINUI
#elif WPF
    protected override Freezable CreateInstanceCore()
    {
        return new PositionColorMaterial()
        {
            Name = Name
        };
    }
#elif AVALONIA
#else
#error Unknown framework
#endif
}
