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
/// Render color by triangle normal
/// </summary>
public sealed class NormalMaterial : Material
{
    public NormalMaterial()
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="NormalMaterial"/> class.
    /// </summary>
    /// <param name="core">The core.</param>
    public NormalMaterial(NormalMaterialCore core) : base(core)
    {

    }

    /// <summary>
    /// Called when [create core].
    /// </summary>
    /// <returns></returns>
    protected override MaterialCore OnCreateCore()
    {
        return NormalMaterialCore.Core;
    }

#if false
#elif WINUI
#elif WPF
    protected override Freezable CreateInstanceCore()
    {
        return new NormalMaterial()
        {
            Name = Name
        };
    }
#else
#error Unknown framework
#endif
}
